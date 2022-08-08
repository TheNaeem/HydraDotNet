using HydraDotNet.Core.Compression;
using HydraDotNet.Core.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HydraDotNet.Core.Encoding;

public class HydraDecoder : IDisposable
{
    private HydraBufferReader _buf;

    public HydraDecoder(Stream encodedStream) => _buf = new(encodedStream);
    public HydraDecoder(byte[] encodedBuffer) => _buf = new(encodedBuffer);

    private IEnumerable<object?> ReadArray(int count)
    {
        var ret = new object?[count];

        for (int i = 0; i < count; i++)
        {
            var val = ReadValue();

            if (val is null) continue;

            ret[i] = val;
        }

        return ret;
    }

    private Dictionary<object, object?> ReadMap(int count)
    {
        Dictionary<object, object?> ret = new(count);

        for (int i = 0; i < count; i++)
        {
            var key = ReadValue();
            var value = ReadValue();

            if (key is null) continue;

            ret.Add(key, value);
        }

        return ret;
    }

    private HydraCompressedObject? ReadCompressedObject()
    {
        var compressionType = _buf.Read<HydraCompressionFormat>();

        var value = ReadValue();

        if (value is not byte[] compressedData)
            return null;

        return new(compressionType, compressedData);
    }

    private object? Test()
    {
        var localizations = ReadValue() as Dictionary<string, string>;

        var val = ReadValue();

        var val2 = ReadValue() as long?;

        var val3 = ReadValue() as long?;

        var val4 = ReadValue() as long?;

        Console.WriteLine("LOCALIZATION HIT");

        return null;

        //TODO this
    }

    /// <summary>
    /// Reads an array of Hydra objects into a List. Also used for reading decoded responses of an array.
    /// </summary>
    /// <param name="baseArray">Hydra objects array.</param>
    /// <param name="listType">The type of the list to be created.</param>
    /// <returns>Created list. Can be null.</returns>
    public static IList? ReadToList(Array baseArray, Type listType)
    {
        if (listType.GetGenericTypeDefinition() != typeof(List<>))
            return default;

        var ret = Activator.CreateInstance(listType, baseArray.Length) as IList;

        if (ret is null)
            return default;

        var elementType = listType.GetGenericArguments()[0];

        if (elementType is null)
            return default;

        foreach (var i in baseArray)
        {
            if (i is not Dictionary<object, object?> val)
            {
                ret.Add(i);
                continue;
            }

            var element = Activator.CreateInstance(elementType);

            if (element is not null)
            {
                ReadToObject(ref element, val);
                ret.Add(element);
            }
            else continue;
        }

        return ret;
    }

    /// <summary>
    /// Reads a Hydra decoded Dictionary map to an object with a matching type. Main purpose is for data models. 
    /// </summary>
    /// <param name="baseObject">Object to be read to.</param>
    /// <param name="dict">Objects map being read from.</param>
    public static void ReadToObject(ref object baseObject, Dictionary<object, object?> dict)
    {
        if (baseObject is null) return;

        var type = baseObject.GetType();

        if (type is null) return;

        foreach (var prop in type.GetProperties())
        {
            var propName = prop.Name;
            if (propName.StartsWith('_')) // for those types with numbers for names  
            {
                ReadOnlySpan<char> trimmed = propName;
                trimmed = trimmed.TrimStart('_');

                if (int.TryParse(trimmed, out var _))
                    propName = trimmed.ToString();
            }

            if (!dict.TryGetValue(propName, out var obj) || obj is null)
                continue;

            if (obj is not Dictionary<object, object?> objDict)
            {
                if (prop.PropertyType.IsArray)
                {
                    var elementType = prop.PropertyType.GetElementType();

                    if (elementType is null) continue;

                    var objArr = (Array)obj;
                    var newArray = Array.CreateInstance(elementType, objArr.Length);

                    for (int i = 0; i < objArr.Length; i++)
                    {
                        var element = objArr.GetValue(i);

                        if (element is not Dictionary<object, object?> val)
                        {
                            newArray.SetValue(element, i);
                            continue;
                        }

                        var newElement = Activator.CreateInstance(elementType);

                        if (newElement is not null)
                        {
                            ReadToObject(ref newElement, val);
                            newArray.SetValue(newElement, i);
                        }
                        else continue;
                    }

                    prop.SetValue(baseObject, newArray);
                    continue;
                }
                else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (obj is not Array objArr)
                        continue;

                    var newList = ReadToList(objArr, prop.PropertyType);

                    prop.SetValue(baseObject, newList);
                    continue;
                }

                prop.SetValue(baseObject, obj);
                continue;
            }

            var objectVal = prop.GetValue(baseObject);

            if (objectVal is null)
            {
                if (prop.PropertyType == typeof(string))
                {
                    objectVal = string.Empty;
                }
                else
                {
                    objectVal = Activator.CreateInstance(prop.PropertyType);
                }
            }

            if (objectVal is not null)
                ReadToObject(ref objectVal, objDict);

            prop.SetValue(baseObject, objectVal);
        }
    }

    /// <summary>
    /// Reads Hydra encoded data from the buffer.
    /// </summary>
    /// <returns>Decoded object. Most likely a Dictionary. Can be null.</returns>
    public object? ReadValue()
    {
        if (_buf.EndOfRead) return null;

        return _buf.Read<byte>() switch
        {
            0x2 => true,
            0x3 => false,
            0x10 => _buf.Read<char>(),
            0x11 => _buf.Read<byte>(),
            0x12 => _buf.Read<short>(),
            0x13 => _buf.ReadUShort(),
            0x14 => _buf.Read<int>(),
            0x15 => _buf.Read<uint>(),
            0x16 => _buf.Read<long>(),
            0x17 => _buf.Read<ulong>(),
            0x20 => _buf.Read<float>(),
            0x21 => _buf.Read<double>(),
            0x30 => _buf.ReadString(_buf.Read<byte>()),
            0x31 => _buf.ReadString(_buf.ReadUShort()),
            0x32 => _buf.ReadString(_buf.Read<int>()),
            0x33 => _buf.Read(_buf.Read<byte>()),
            0x34 => _buf.Read(_buf.Read<ushort>()),
            0x35 => _buf.Read(_buf.Read<int>()),
            0x40 => DateTimeOffset.FromUnixTimeSeconds(_buf.ReadUInt()).DateTime,
            0x50 => ReadArray(_buf.Read<byte>()),
            0x51 => ReadArray(_buf.ReadUShort()),
            0x52 => ReadArray(_buf.Read<int>()),
            0x60 => ReadMap(_buf.Read<byte>()),
            0x61 => ReadMap(_buf.ReadUShort()),
            0x62 => ReadMap(_buf.Read<int>()),
            0x67 => ReadCompressedObject(),
            0x68 => Test(),
            0x69 => new HydraCalendarControl(this),
            _ => null
        };
    }

    public void Dispose() => _buf.Dispose();
}

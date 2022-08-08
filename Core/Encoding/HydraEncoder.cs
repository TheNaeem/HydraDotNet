using HydraDotNet.Core.Compression;
using HydraDotNet.Core.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HydraDotNet.Core.Encoding;

public class HydraEncoder : IDisposable, IAsyncDisposable
{
    private readonly HydraBufferWriter _writer;

    public HydraEncoder() => _writer = new(new MemoryStream());

    private void WriteArray(IEnumerable array)
    {
        int len = 0;

        foreach (var _ in array)
            len++;

        if (len <= byte.MaxValue)
        {
            _writer.WriteByte(0x50);
            _writer.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _writer.WriteByte(0x51);
            _writer.Write((ushort)len);
        }
        else
        {
            _writer.WriteByte(0x52);
            _writer.Write(len);
        }

        foreach (var i in array)
            WriteValue(i);
    }

    private void WriteMap(IDictionary map)
    {
        var len = map.Count;

        if (len <= byte.MaxValue)
        {
            _writer.WriteByte(0x60);
            _writer.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _writer.WriteByte(0x61);
            _writer.Write((ushort)len);
        }
        else
        {
            _writer.WriteByte(0x62);
            _writer.Write(len);
        }

        foreach (DictionaryEntry entry in map)
        {
            WriteValue(entry.Key);
            WriteValue(entry.Value ?? new());
        }
    }

    private void WriteBytes(byte[] bytes)
    {
        var len = bytes.Length;

        if (len <= byte.MaxValue)
        {
            _writer.WriteByte(0x33);
            _writer.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _writer.WriteByte(0x34);
            _writer.Write((ushort)len);
        }
        else
        {
            _writer.WriteByte(0x35);
            _writer.Write(len);
        }

        _writer.Write(bytes, 0, len);
    }

    private void WriteString(string val)
    {
        var len = val.Length;

        if (len <= byte.MaxValue)
        {
            _writer.WriteByte(0x30);
            _writer.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _writer.WriteByte(0x31);
            _writer.Write((ushort)len);
        }
        else
        {
            _writer.WriteByte(0x32);
            _writer.Write(len);
        }

        _writer.Write(System.Text.Encoding.ASCII.GetBytes(val), 0, len);
    }
    
    /// <summary>
    /// Writes encoded data to the buffer.
    /// </summary>
    /// <param name="value">Value to be encoded.</param>
    public void WriteValue(object value)
    {
        switch (value)
        {
            case bool val:
                _writer.WriteByte((byte)(val ? 0x2 : 0x3));
                break;
            case char val:
                _writer.WriteByte(0x10);
                _writer.WriteByte((byte)val);
                break;
            case byte val:
                _writer.WriteByte(0x11);
                _writer.Write(val);
                break;
            case short val:
                _writer.WriteByte(0x12);
                _writer.Write(val);
                break;
            case ushort val:
                _writer.WriteByte(0x13);
                _writer.Write(val);
                break;
            case int val:
                _writer.WriteByte(0x14);
                _writer.Write(val);
                break;
            case uint val:
                _writer.WriteByte(0x15);
                _writer.Write(val);
                break;
            case long val:
                _writer.WriteByte(0x16);
                _writer.Write(val);
                break;
            case ulong val:
                _writer.WriteByte(0x17);
                _writer.Write(val);
                break;
            case float val:
                _writer.WriteByte(0x20);
                _writer.Write(val);
                break;
            case double val:
                _writer.WriteByte(0x21);
                _writer.Write(val);
                break;
            case string val:
                WriteString(val);
                break;
            case byte[] val:
                WriteBytes(val);
                break;
            case IDictionary val:
                WriteMap(val);
                break;
            case IEnumerable val:
                WriteArray(val);
                break;
            case HydraCompressedObject val:
                _writer.WriteByte(0x67);
                _writer.Write((byte)val.Format);
                WriteBytes(val.CompressedData);
                break;
            case HydraCalendarControl val:
                if (val.Def is not null && val.Rendered is not null)
                {
                    _writer.WriteByte(0x69);
                    WriteValue(val.Def);
                    WriteValue(val.Rendered);
                }
                break;
            default:
                WriteObjectDefault(value);
                break;
        };
    }

    /// <summary>
    /// Write an object of any type as a dictionary to the buffer. Main purpose is for user defined types, mostly models. 
    /// </summary>
    private void WriteObjectDefault(object obj)
    {
        var type = obj.GetType();
        var properties = type.GetProperties();

        var len = properties.Length;

        if (len <= byte.MaxValue)
        {
            _writer.WriteByte(0x60);
            _writer.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _writer.WriteByte(0x61);
            _writer.Write((ushort)len);
        }
        else
        {
            _writer.WriteByte(0x62);
            _writer.Write(len);
        }

        foreach (var prop in properties)
        {
            var val = prop.GetValue(obj);

            if (val is null) continue;

            WriteValue(prop.Name);

            if (val is DateTime time)
            {
                WriteValue((uint)((DateTimeOffset)time).ToUnixTimeSeconds());
            }
            else WriteValue(val);
        }
    }

    public void Dispose() => _writer.Dispose();
    public ValueTask DisposeAsync() => _writer.DisposeAsync();

    /// <summary>
    /// Creates a copy of the encoded buffer.
    /// </summary>
    /// <returns>Byte array of encoded data.</returns>
    public byte[] GetBuffer()
    {
        var pos = _writer.BaseStream.Position;

        _writer.BaseStream.Seek(0, SeekOrigin.Begin);

        using var ms = new MemoryStream();
        _writer.BaseStream.CopyTo(ms);

        _writer.BaseStream.Seek(pos, SeekOrigin.Begin);

        return ms.ToArray();
    }

    /// <summary>
    /// Asynchronously creates a copy of the encoded buffer.
    /// </summary>
    /// <returns>Byte array of encoded data.</returns>
    public async Task<byte[]> GetBufferAsync()
    {
        var pos = _writer.BaseStream.Position;

        _writer.BaseStream.Seek(0, SeekOrigin.Begin);

        await using var ms = new MemoryStream();
        await _writer.BaseStream.CopyToAsync(ms);

        _writer.BaseStream.Seek(pos, SeekOrigin.Begin);

        return ms.ToArray();
    }
}

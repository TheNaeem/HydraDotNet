using HydraDotNet.Core.Compression;
using HydraDotNet.Core.Objects;
using System;
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
        List<object?> ret = new(count);

        for (int i = 0; i < count; i++)
        {
            var val = ReadValue();

            if (val is null) continue;

            ret.Add(val);
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
    /// Reads Hydra encoded data from the buffer.
    /// </summary>
    /// <returns>Decoded object.</returns>
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
            0x13 => _buf.Read<ushort>(),
            0x14 => _buf.Read<int>(),
            0x15 => _buf.Read<uint>(),
            0x16 => _buf.Read<long>(),
            0x17 => _buf.Read<ulong>(),
            0x20 => _buf.Read<float>(),
            0x21 => _buf.Read<double>(),
            0x30 => _buf.ReadString(_buf.Read<byte>()),
            0x31 => _buf.ReadString(_buf.Read<ushort>()),
            0x32 => _buf.ReadString(_buf.Read<int>()),
            0x33 => _buf.Read(_buf.Read<byte>()),
            0x34 => _buf.Read(_buf.Read<ushort>()),
            0x35 => _buf.Read(_buf.Read<int>()),
            0x40 => DateTimeOffset.FromUnixTimeSeconds(_buf.Read<int>()).DateTime,
            0x50 => ReadArray(_buf.Read<byte>()),
            0x51 => ReadArray(_buf.Read<ushort>()),
            0x52 => ReadArray(_buf.Read<int>()),
            0x60 => ReadMap(_buf.Read<byte>()),
            0x61 => ReadMap(_buf.Read<ushort>()),
            0x62 => ReadMap(_buf.Read<int>()),
            0x67 => ReadCompressedObject(),
            0x68 => Test(),
            0x69 => new HydraCalendarControl(this),
            _ => null
        };
    }

    public void Dispose() => _buf.Dispose();
}

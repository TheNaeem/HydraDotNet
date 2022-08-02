using HydraDotNet.Core.Compression;
using HydraDotNet.Core.Objects;
using System.Collections.Generic;
using System.IO;

namespace HydraDotNet.Core.Encoding;

public class HydraEncoder
{
    private HydraBufferWriter _buf;

    public HydraEncoder(Stream buffer) => _buf = new(buffer);

    private void WriteMap(Dictionary<object, object> map)
    {
        var len = map.Count;

        if (len <= byte.MaxValue)
        {
            _buf.WriteByte(0x60);
            _buf.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _buf.WriteByte(0x61);
            _buf.Write((ushort)len);
        }
        else
        {
            _buf.WriteByte(0x62);
            _buf.Write(len);
        }

        foreach (var (k, v) in map)
        {
            WriteValue(k);
            WriteValue(v);
        }
    }

    private void WriteBytes(byte[] bytes)
    {
        var len = bytes.Length;

        if (len <= byte.MaxValue)
        {
            _buf.WriteByte(0x33);
            _buf.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _buf.WriteByte(0x34);
            _buf.Write((ushort)len);
        }
        else
        {
            _buf.WriteByte(0x35);
            _buf.Write(len);
        }

        _buf.Write(bytes, 0, len);
    }

    private void WriteString(string val)
    {
        var len = val.Length;

        if (len <= byte.MaxValue)
        {
            _buf.WriteByte(0x30);
            _buf.Write((byte)len);
        }
        else if (len <= ushort.MaxValue)
        {
            _buf.WriteByte(0x31);
            _buf.Write((ushort)len);
        }
        else
        {
            _buf.WriteByte(0x32);
            _buf.Write(len);
        }

        _buf.Write(System.Text.Encoding.ASCII.GetBytes(val), 0, len);
    }
    
    public void WriteValue(object value)
    {
        switch (value)
        {
            case bool val:
                _buf.WriteByte((byte)(val ? 0x2 : 0x3));
                break;
            case char val:
                _buf.WriteByte(0x10);
                _buf.WriteByte((byte)val);
                break;
            case byte val:
                _buf.WriteByte(0x11);
                _buf.Write(val);
                break;
            case short val:
                _buf.WriteByte(0x12);
                _buf.Write(val);
                break;
            case ushort val:
                _buf.WriteByte(0x13);
                _buf.Write(val);
                break;
            case int val:
                _buf.WriteByte(0x14);
                _buf.Write(val);
                break;
            case uint val:
                _buf.WriteByte(0x15);
                _buf.Write(val);
                break;
            case long val:
                _buf.WriteByte(0x16);
                _buf.Write(val);
                break;
            case ulong val:
                _buf.WriteByte(0x17);
                _buf.Write(val);
                break;
            case float val:
                _buf.WriteByte(0x20);
                _buf.Write(val);
                break;
            case double val:
                _buf.WriteByte(0x21);
                _buf.Write(val);
                break;
            case string val:
                WriteString(val);
                break;
            case byte[] val:
                WriteBytes(val);
                break;
            case Dictionary<object, object> val:
                WriteMap(val);
                break;
            case HydraCompressedObject val:
                _buf.WriteByte(0x67);
                _buf.Write((byte)val.Format);
                WriteBytes(val.CompressedData);
                break;
            case HydraCalendarControl val:
                if (val.Def is not null && val.Rendered is not null)
                {
                    _buf.WriteByte(0x69);
                    WriteValue(val.Def);
                    WriteValue(val.Rendered);
                }
                break;
        };
    }
}

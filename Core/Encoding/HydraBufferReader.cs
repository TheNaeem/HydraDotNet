using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HydraDotNet.Core.Encoding;

internal class HydraBufferReader : IDisposable
{
    public bool EndOfRead 
    {
        get => _reader.BaseStream.Length <= _reader.BaseStream.Position;
    }

    private BinaryReader _reader;

    public HydraBufferReader(byte[] buffer) => _reader = new(new MemoryStream(buffer));
    
    public HydraBufferReader(Stream buffer)
    {
        if (!buffer.CanRead)
        {
            throw new AccessViolationException("Stream passed into HydraBufferReader does not have read access");
        }

        _reader = new(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        var buf = _reader.ReadBytes(Marshal.SizeOf<T>());

        return MemoryMarshal.Read<T>(buf);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Read(int length) => _reader.ReadBytes(length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString() => _reader.ReadString();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString(int length) => System.Text.Encoding.ASCII.GetString(_reader.ReadBytes(length));
    

    public void Dispose() => _reader.Dispose();
}

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

    public HydraBufferReader(byte[] buffer)
    {
        _reader = new(new MemoryStream(buffer));
    }

    public HydraBufferReader(Stream buffer)
    {
        _reader = new(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T Read<T>() where T : unmanaged
    {
        var buf = _reader.ReadBytes(sizeof(T));

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

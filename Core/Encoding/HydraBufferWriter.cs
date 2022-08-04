using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace HydraDotNet.Core.Encoding;

public class HydraBufferWriter : BinaryWriter
{
    public HydraBufferWriter(byte[] buffer) : base(new MemoryStream(buffer))
    {
    }

    public HydraBufferWriter(Stream buffer) : base(buffer)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteVal<T>(T val) where T : unmanaged
    {
        ReadOnlySpan<byte> view = new(&val, sizeof(T));

        base.Write(view);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(ushort val) => base.Write((ushort)((ushort)((val & 0xff) << 8) | ((val >> 8) & 0xff)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte val) => Write(val);
}

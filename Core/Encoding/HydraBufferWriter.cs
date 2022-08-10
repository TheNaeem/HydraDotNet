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

    /// <summary>
    /// This will NOT output a datetime value that can be read by Hydras decoder. It will read it as an incorrect value. Needs to be fixed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(DateTime val)
    {
        var unix = ((DateTimeOffset)val).ToUnixTimeSeconds();
        Write((uint)unix);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(ushort val) => base.Write((ushort)((ushort)((val & 0xff) << 8) | ((val >> 8) & 0xff)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte val) => Write(val);
}

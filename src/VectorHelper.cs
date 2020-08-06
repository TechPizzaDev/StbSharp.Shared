using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace StbSharp
{
    public static class VectorHelper
    {
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Widen(Vector128<byte> source, out Vector128<ushort> low, out Vector128<ushort> high)
        {
            low = Sse2.UnpackLow(source, Vector128<byte>.Zero).AsUInt16();
            high = Sse2.UnpackHigh(source, Vector128<byte>.Zero).AsUInt16();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SignedWiden(Vector128<byte> source, out Vector128<short> low, out Vector128<short> high)
        {
            low = Sse2.UnpackLow(source, Vector128<byte>.Zero).AsInt16();
            high = Sse2.UnpackHigh(source, Vector128<byte>.Zero).AsInt16();
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<byte> Narrow(Vector128<ushort> low, Vector128<ushort> high)
        {
            return Sse2.PackUnsignedSaturate(low.AsInt16(), high.AsInt16());
        }
    }
}
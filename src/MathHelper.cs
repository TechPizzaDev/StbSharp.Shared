using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace StbSharp
{
    public static class MathHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Paeth(byte a, byte b, byte c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);

            if (pa <= pb && pa <= pc)
                return a;
            if (pb <= pc)
                return b;
            return c;
        }

        public static Vector<byte> Paeth(Vector<byte> a, Vector<byte> b, Vector<byte> c)
        {
            Vector.Widen(a, out var a1, out var a2);
            Vector.Widen(b, out var b1, out var b2);
            Vector.Widen(c, out var c1, out var c2);

            var p1 = Paeth(Vector.AsVectorInt16(a1), Vector.AsVectorInt16(b1), Vector.AsVectorInt16(c1));
            var p2 = Paeth(Vector.AsVectorInt16(a2), Vector.AsVectorInt16(b2), Vector.AsVectorInt16(c2));
            return Vector.AsVectorByte(Vector.Narrow(p1, p2));
        }

        private static Vector<short> Paeth(Vector<short> a, Vector<short> b, Vector<short> c)
        {
            var p = a + b - c;
            var pa = Vector.Abs(p - a);
            var pb = Vector.Abs(p - b);
            var pc = Vector.Abs(p - c);

            var pa_pb = Vector.LessThanOrEqual(pa, pb);
            var pa_pc = Vector.LessThanOrEqual(pa, pc);
            var pb_pc = Vector.LessThanOrEqual(pb, pc);

            return Vector.ConditionalSelect(
                condition: Vector.BitwiseAnd(pa_pb, pa_pc),
                left: a,
                right: Vector.ConditionalSelect(
                    condition: pb_pc,
                    left: b,
                    right: c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateBits(uint x, int y)
        {
            return (x << y) | (x >> (32 - y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateBits(ulong x, int y)
        {
            return (x << y) | (x >> (64 - y));
        }

        /// <summary>
        /// Decomposes given floating point value into a
        /// normalized fraction and an integral power of two.
        /// <para>
        /// This code has been borrowed from https://github.com/MachineCognitis/C.math.NET
        /// </para>
        /// </summary>
        public static double FractionExponent(double number, out int exponent)
        {
            const long DoubleExponentMask = 0x7ff0000000000000L;
            const int DoubleMantissaBits = 52;
            const long DoubleSignedMask = -1 - 0x7fffffffffffffffL;
            const long DoubleMantissaMask = 0x000fffffffffffffL;
            const long DoubleExponentCLRMask = DoubleSignedMask | DoubleMantissaMask;
            const double DoubleFactor = 0x40000000000000; // 2^54

            long bits = BitConverter.DoubleToInt64Bits(number);
            int exp = (int)((bits & DoubleExponentMask) >> DoubleMantissaBits);

            if (exp == 0x7ff || number == 0D)
            {
                number += number;
                exponent = 0;
            }
            else
            {
                // Not zero and finite.
                exponent = exp - 1022;
                if (exp == 0)
                {
                    // Subnormal, scale number so that it is in [1, 2].
                    number *= DoubleFactor;
                    bits = BitConverter.DoubleToInt64Bits(number);
                    exp = (int)((bits & DoubleExponentMask) >> DoubleMantissaBits);
                    exponent = exp - 1022 - 54;
                }

                // Set exponent to -1 so that number is in [0.5, 1).
                number = BitConverter.Int64BitsToDouble(
                    (bits & DoubleExponentCLRMask) | 0x3fe0000000000000L);
            }

            return number;
        }
    }
}
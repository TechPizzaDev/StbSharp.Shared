using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace StbSharp
{
    public static class MathHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int x)
        {
            int y = x >> 31;
            return (x ^ y) - y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Paeth(byte a, byte b, byte c)
        {
            int p = a + b - c;
            int pa = Abs(p - a);
            int pb = Abs(p - b);
            int pc = Abs(p - c);

            if (pa <= pb && pa <= pc)
                return a;
            if (pb <= pc)
                return b;
            return c;
        }

        public static Vector<byte> Paeth(Vector<byte> a, Vector<byte> b, Vector<byte> c)
        {
            Vector.Widen(a, out Vector<ushort> a1, out Vector<ushort> a2);
            Vector.Widen(b, out Vector<ushort> b1, out Vector<ushort> b2);
            Vector.Widen(c, out Vector<ushort> c1, out Vector<ushort> c2);

            Vector<short> p1 = Paeth(Vector.AsVectorInt16(a1), Vector.AsVectorInt16(b1), Vector.AsVectorInt16(c1));
            Vector<short> p2 = Paeth(Vector.AsVectorInt16(a2), Vector.AsVectorInt16(b2), Vector.AsVectorInt16(c2));
            return Vector.AsVectorByte(Vector.Narrow(p1, p2));
        }

        private static Vector<short> Paeth(Vector<short> a, Vector<short> b, Vector<short> c)
        {
            Vector<short> p = a + b - c;
            Vector<short> pa = Vector.Abs(p - a);
            Vector<short> pb = Vector.Abs(p - b);
            Vector<short> pc = Vector.Abs(p - c);

            Vector<short> pa_pb = Vector.LessThanOrEqual(pa, pb);
            Vector<short> pa_pc = Vector.LessThanOrEqual(pa, pc);
            Vector<short> pb_pc = Vector.LessThanOrEqual(pb, pc);

            return Vector.ConditionalSelect(
                condition: Vector.BitwiseAnd(pa_pb, pa_pc),
                left: a,
                right: Vector.ConditionalSelect(
                    condition: pb_pc,
                    left: b,
                    right: c));
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
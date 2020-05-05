using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public static unsafe class CRuntime
    {
        #region Memory Management

        public static void* MAlloc(IntPtr size)
        {
            try
            {
                IntPtr ptr = Marshal.AllocHGlobal(size);
                MemoryStatistics.OnAllocate();
                return ptr.ToPointer();
            }
            catch
            {
                return null;
            }
        }

        public static void* MAlloc(int size)
        {
            return MAlloc((IntPtr)size);
        }

        public static void* ReAlloc(void* a, IntPtr newSize)
        {
            if (a == null)
                return MAlloc(newSize);

            try
            {
                var ptr = new IntPtr(a);
                var result = Marshal.ReAllocHGlobal(ptr, newSize);
                return result.ToPointer();
            }
            catch
            {
                return null;
            }
        }

        public static void* ReAlloc(void* a, int newSize)
        {
            return ReAlloc(a, (IntPtr)newSize);
        }

        public static void Free(void* a)
        {
            if (a == null)
                return;

            var ptr = new IntPtr(a);
            MemoryStatistics.OnFree();
            Marshal.FreeHGlobal(ptr);
        }

        #endregion

        #region Memory Manipulation

        public static void MemCopy(void* dst, void* src, uint size)
        {
            Unsafe.CopyBlockUnaligned(dst, src, size);
        }

        public static void MemCopy(void* dst, void* src, int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));
            MemCopy(dst, src, (uint)size);
        }

        public static void MemMove(void* dst, void* src, int size)
        {
            var dstSpan = new Span<byte>(dst, size);
            var srcSpan = new ReadOnlySpan<byte>(src, size);
            srcSpan.CopyTo(dstSpan);
        }

        public static int MemCompare(void* a, void* b, long size)
        {
            // TODO: use SIMD Vector instead

            var ap = (byte*)a;
            var bp = (byte*)b;
            
            // these "vectorized" implementations should decrease 
            // comparison time for memory that's equal
            if (Environment.Is64BitProcess)
            {
                while (size >= sizeof(long))
                {
                    if (*(long*)ap != *(long*)bp)
                        break;
                    ap += sizeof(long);
                    bp += sizeof(long);
                    size -= sizeof(long);
                }
            }
            else
            {
                while (size >= sizeof(int))
                {
                    if (*(int*)ap != *(int*)bp)
                        break;
                    ap += sizeof(int);
                    bp += sizeof(int);
                    size -= sizeof(int);
                }
            }

            int result = 0;

            while (size-- > 0)
                if (*ap++ != *bp++)
                    result++;

            //// TODO: remove this after testing the vectorized version out
            //int result1 = 0;
            //var ap1 = (byte*)a1;
            //var bp1 = (byte*)b1;
            //while (size1-- > 0)
            //    if (*ap1++ != *bp1++)
            //        result1++;
            //
            //if (result != result1)
            //    throw new Exception();
            //
            //#endregion

            return result;
        }

        public static int MemCompare<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, long size)
            where T : unmanaged
        {
            if (size > a.Length * sizeof(T) ||
                size > b.Length * sizeof(T))
                throw new ArgumentOutOfRangeException(nameof(size));

            fixed (T* ap = a)
            fixed (T* bp = b)
            {
                return MemCompare(ap, bp, size);
            }
        }

        #endregion

        #region Math

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastAbs(int value)
        {
            int tmp = value >> 31;
            value ^= tmp;
            value += tmp & 1;
            return value;
        }

        public static int Paeth32(int a, int b, int c)
        {
            // original math
            //int p = a + b - c;
            //int pa = Math.Abs(p - a);
            //int pb = Math.Abs(p - b);
            //int pc = Math.Abs(p - c);

            int pa = FastAbs(b - c);
            int pb = FastAbs(a - c);
            int pc = FastAbs(a - c + b - c);

            if (pa <= pb && pa <= pc)
                return a;
            if (pb <= pc)
                return b;
            return c;
        }

        public static uint RotateBits(uint x, int y)
        {
            return (x << y) | (x >> (32 - y));
        }

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
                    // Subnormal, scale number so that it is in [1, 2).
                    number *= BitConverter.Int64BitsToDouble(0x4350000000000000L); // 2^54
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

        #endregion

        public static int StringLength(ReadOnlySpan<byte> str)
        {
            int i = 0;
            for (; i < str.Length;)
            {
                if (str[i] == '\0')
                    break;
                i++;
            }
            return i;
        }
    }
}
using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public static unsafe class CRuntime
    {
        #region Memory Management

        public static void* malloc(ulong size)
        {
            return malloc((long)size);
        }

        public static void* malloc(long size)
        {
            IntPtr ptr = Marshal.AllocHGlobal((IntPtr)size);
            return ptr.ToPointer();
        }

        public static void* realloc(void* a, long newSize)
        {
            if (a == null)
                return malloc(newSize);

            var ptr = new IntPtr(a);
            var result = Marshal.ReAllocHGlobal(ptr, new IntPtr(newSize));
            return result.ToPointer();
        }

        public static void* realloc(void* a, ulong newSize)
        {
            return realloc(a, (long)newSize);
        }

        public static void free(void* a)
        {
            var ptr = new IntPtr(a);
            MemoryStatistics.Freed();
            Marshal.FreeHGlobal(ptr);
        }

        #endregion

        #region Memory Manipulation

        public static void memcpy(void* dst, void* src, long size)
        {
            // this is fast and simple
            Buffer.MemoryCopy(src, dst, size, size);

            //var idstp = (int*)dst;
            //var isrcp = (int*)src;
            //// small optimization
            //while (size > 4)
            //{
            //    *idstp++ = *isrcp++;
            //    size -= 4;
            //}
            //
            //var dstp = (byte*)idstp;
            //var srcp = (byte*)isrcp;
            //while (size-- > 0)
            //    *dstp++ = *srcp++;
        }

        public static void memcpy(void* a, void* b, ulong size)
        {
            memcpy(a, b, (long)size);
        }

        public static void memmove(void* dst, void* src, long size)
        {
            long bufferSize = Math.Min(size, 1024);
            byte* buffer = stackalloc byte[(int)bufferSize];
            byte* bsrc = (byte*)src;
            byte* bdst = (byte*)dst;

            while (size > 0)
            {
                long toCopy = Math.Min(size, bufferSize);
                memcpy(buffer, bsrc, toCopy);
                memcpy(bdst, buffer, toCopy);

                bsrc += toCopy;
                bdst += toCopy;
                size -= toCopy;
            }
        }

        public static void memmove(void* a, void* b, ulong size)
        {
            memmove(a, b, (long)size);
        }

        public static void memset(void* ptr, byte value, long size)
        {
            byte* bptr = (byte*)ptr;

            // micro optimization
            if (value == 0)
            {
                long* lbptr = (long*)bptr;
                if (Environment.Is64BitProcess)
                {
                    while (size > sizeof(long))
                    {
                        *lbptr++ = 0;
                        size -= sizeof(long);
                    }
                }
                int* ibptr = (int*)lbptr;
                while (size > sizeof(int))
                {
                    *ibptr++ = 0;
                    size -= sizeof(int);
                }
                bptr = (byte*)ibptr;
            }

            while (size-- > 0)
                *bptr++ = value;
        }

        public static void memset(void* ptr, byte value, ulong size)
        {
            memset(ptr, value, (long)size);
        }

        public static int memcmp(void* a, void* b, long size)
        {
            int result = 0;
            var ap = (byte*)a;
            var bp = (byte*)b;

            while (size-- > 0)
                if (*ap++ != *bp++)
                    result += 1;

            return result;
        }

        public static int memcmp(void* a, void* b, ulong size)
        {
            return memcmp(a, b, (long)size);
        }

        #endregion

        #region Math

        public const long DBL_EXP_MASK = 0x7ff0000000000000L;
        public const int DBL_MANT_BITS = 52;
        public const long DBL_SGN_MASK = -1 - 0x7fffffffffffffffL;
        public const long DBL_MANT_MASK = 0x000fffffffffffffL;
        public const long DBL_EXP_CLR_MASK = DBL_SGN_MASK | DBL_MANT_MASK;

        public static uint _lrotl(uint x, int y)
        {
            return (x << y) | (x >> (32 - y));
        }

        /// <summary>
        /// This code had been borrowed from here: https://github.com/MachineCognitis/C.math.NET
        /// </summary>
        /// <param name="number"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public static double frexp(double number, int* exponent)
        {
            var bits = BitConverter.DoubleToInt64Bits(number);
            var exp = (int)((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
            *exponent = 0;

            if (exp == 0x7ff || number == 0D)
                number += number;
            else
            {
                // Not zero and finite.
                *exponent = exp - 1022;
                if (exp == 0)
                {
                    // Subnormal, scale number so that it is in [1, 2).
                    number *= BitConverter.Int64BitsToDouble(0x4350000000000000L); // 2^54
                    bits = BitConverter.DoubleToInt64Bits(number);
                    exp = (int)((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
                    *exponent = exp - 1022 - 54;
                }

                // Set exponent to -1 so that number is in [0.5, 1).
                number = BitConverter.Int64BitsToDouble((bits & DBL_EXP_CLR_MASK) | 0x3fe0000000000000L);
            }

            return number;
        }

        #endregion
    }
}
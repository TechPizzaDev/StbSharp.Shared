using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;

namespace StbSharp;

public static class V128Helper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> SubtractSaturate(Vector128<byte> left, Vector128<byte> right)
    {
        if (Sse2.IsSupported)
        {
            return Sse2.SubtractSaturate(left, right);
        }
        else if (AdvSimd.IsSupported)
        {
            return AdvSimd.SubtractSaturate(left, right);
        }
        else if (PackedSimd.IsSupported)
        {
            return PackedSimd.SubtractSaturate(left, right);
        }
        else
        {
            return SoftwareFallback(left, right);
        }

        static Vector128<byte> SoftwareFallback(Vector128<byte> left, Vector128<byte> right)
        {
            Unsafe.SkipInit(out Vector128<byte> result);
            for (int i = 0; i < Vector128<byte>.Count; i++)
            {
                int sum = left.GetElement(i) - right.GetElement(i);
                // Negative (Int32) sum will have bits set to one beyond the first 8 bits.
                int mask = ~(sum >> 8);
                byte value = (byte) (sum & mask);
                result = result.WithElement(i, value);
            }
            return result;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> ShiftLeftLogical128BitLane(Vector128<byte> value, [ConstantExpected(Max = (byte) 15)] byte numBytes)
    {
        if (numBytes == 0)
        {
            return value.AsByte();
        }
        else if (numBytes > 15)
        {
            return Vector128<byte>.Zero;
        }

        if (Sse2.IsSupported)
        {
            return Sse2.ShiftLeftLogical128BitLane(value, numBytes);
        }

        byte index = (byte) (16 - numBytes);
        if (AdvSimd.IsSupported)
        {
            return AdvSimd.ExtractVector128(Vector128<byte>.Zero, value.AsByte(), index);
        }
        else
        {
            return ExtractVector128Fallback(Vector128<byte>.Zero, value.AsByte(), index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> ShiftRightLogical128BitLane(Vector128<byte> value, [ConstantExpected(Max = (byte) 15)] byte numBytes)
    {
        if (numBytes == 0)
        {
            return value.AsByte();
        }
        else if (numBytes > 15)
        {
            return Vector128<byte>.Zero;
        }

        if (Sse2.IsSupported)
        {
            return Sse2.ShiftRightLogical128BitLane(value, numBytes);
        }
        else if (AdvSimd.IsSupported)
        {
            return AdvSimd.ExtractVector128(value.AsByte(), Vector128<byte>.Zero, numBytes);
        }
        else
        {
            return ExtractVector128Fallback(value.AsByte(), Vector128<byte>.Zero, numBytes);
        }
    }

    private static Vector128<byte> ExtractVector128Fallback(Vector128<byte> upper, Vector128<byte> lower, int index)
    {
        Unsafe.SkipInit(out Vector128<byte> result);
        for (int i = index; i < Vector128<byte>.Count; i++)
        {
            // Extract high elements into lower.
            result = result.WithElement(i - index, upper.GetElement(i));
        }
        for (int i = 0; i < index; i++)
        {
            // Extract low elements into upper.
            result = result.WithElement(i + (Vector128<byte>.Count - index), lower.GetElement(i));
        }
        return result;
    }
}

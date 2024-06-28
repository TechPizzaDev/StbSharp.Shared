using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace StbSharp;

public static class V256Helper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<byte> SubtractSaturate(Vector256<byte> left, Vector256<byte> right)
    {
        if (Avx2.IsSupported)
        {
            return Avx2.SubtractSaturate(left, right);
        }
        else
        {
            return Vector256.Create(
                V128Helper.SubtractSaturate(left.GetLower(), right.GetLower()),
                V128Helper.SubtractSaturate(left.GetUpper(), right.GetUpper()));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<byte> ShiftLeftLogical128BitLane(Vector256<byte> value, [ConstantExpected(Max = (byte)15)] byte numBytes)
    {
        if (Avx2.IsSupported)
        {
            return Avx2.ShiftLeftLogical128BitLane(value, numBytes);
        }
        else
        {
            return Vector256.Create(
                V128Helper.ShiftLeftLogical128BitLane(value.GetLower(), numBytes),
                V128Helper.ShiftLeftLogical128BitLane(value.GetUpper(), numBytes));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<byte> ShiftRightLogical128BitLane(Vector256<byte> value, [ConstantExpected(Max = (byte)15)] byte numBytes)
    {
        if (Avx2.IsSupported)
        {
            return Avx2.ShiftRightLogical128BitLane(value, numBytes);
        }
        else
        {
            return Vector256.Create(
                V128Helper.ShiftRightLogical128BitLane(value.GetLower(), numBytes),
                V128Helper.ShiftRightLogical128BitLane(value.GetUpper(), numBytes));
        }
    }
}

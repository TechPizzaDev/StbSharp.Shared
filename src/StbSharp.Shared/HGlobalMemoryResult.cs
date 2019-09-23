using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public class HGlobalMemoryResult : IMemoryResult
    {
        public bool IsAllocated => Pointer != null;
        public int Length { get; }
        public IntPtr Pointer { get; }

        public HGlobalMemoryResult(IntPtr pointer, int length)
        {
            Pointer = pointer;
            Length = length;
        }

        public void Dispose()
        {
            if (IsAllocated)
                Marshal.FreeHGlobal(Pointer);
        }
    }
}
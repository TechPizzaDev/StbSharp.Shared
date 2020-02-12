using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public class HGlobalMemoryResult : IMemoryResult
    {
        public int Length { get; private set; }
        public IntPtr Pointer { get; private set; }
        public bool IsAllocated => Pointer != default;

        public HGlobalMemoryResult(IntPtr pointer, int length)
        {
            Pointer = pointer;
            Length = length;

            if (Length != 0)
                GC.AddMemoryPressure(Length);
        }

        public unsafe HGlobalMemoryResult(void* pointer, int length) :
            this((IntPtr)pointer, length)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsAllocated)
            {
                Marshal.FreeHGlobal(Pointer);
                Pointer = default;

                GC.RemoveMemoryPressure(Length);
                Length = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HGlobalMemoryResult()
        {
            Dispose(false);
        }
    }
}
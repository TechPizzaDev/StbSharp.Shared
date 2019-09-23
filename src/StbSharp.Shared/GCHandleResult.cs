using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public class GCHandleResult : IMemoryResult
    {
        private GCHandle _handle;

        public bool IsAllocated => _handle.IsAllocated;
        public int Length { get; }
        public IntPtr Pointer => _handle.AddrOfPinnedObject();

        public GCHandleResult(GCHandle handle, int length)
        {
            _handle = handle;
            Length = length;
        }

        public void Dispose()
        {
            if (IsAllocated)
                _handle.Free();
        }
    }
}
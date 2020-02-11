using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public class GCHandleResult : IMemoryResult
    {
        private GCHandle _handle;

        public int Length { get; private set; }
        public bool IsAllocated => _handle.IsAllocated;
        public IntPtr Pointer => _handle.AddrOfPinnedObject();

        public GCHandleResult(GCHandle handle, int length)
        {
            _handle = handle;
            Length = length;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsAllocated)
            {
                _handle.Free();
                _handle = default;
                Length = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GCHandleResult()
        {
            Dispose(false);
        }
    }
}
using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public unsafe class HGlobalMemoryHolder : IMemoryHolder
    {
        private IntPtr _pointer;

        public int Length { get; private set; }
        public Span<byte> Span => new Span<byte>((void*)_pointer, Length);

        public HGlobalMemoryHolder(IntPtr pointer, int byteLength)
        {
            _pointer = pointer;
            Length = byteLength;

            if (Length != 0)
                GC.AddMemoryPressure(Length);
        }

        public HGlobalMemoryHolder(void* pointer, int byteLength) :
            this((IntPtr)pointer, byteLength)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_pointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_pointer);
                _pointer = default;

                GC.RemoveMemoryPressure(Length);
                Length = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HGlobalMemoryHolder()
        {
            Dispose(false);
        }
    }
}
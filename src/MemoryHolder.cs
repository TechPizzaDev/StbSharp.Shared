using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StbSharp
{
    public class MemoryHolder<T> : IMemoryHolder
        where T : unmanaged
    {
        public Memory<T> Memory { get; private set; }

        public int Length => Memory.Length * Unsafe.SizeOf<T>();
        public Span<byte> Span => MemoryMarshal.AsBytes(Memory.Span);

        public MemoryHolder(Memory<T> memory)
        {
            Memory = memory;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Memory = default;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MemoryHolder()
        {
            Dispose(false);
        }
    }
}
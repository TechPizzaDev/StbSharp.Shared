using System;

namespace StbSharp
{
    public class ByteMemoryHolder : IMemoryHolder
    {
        public Memory<byte> Memory { get; private set; }

        public int Length => Memory.Length;
        public Span<byte> Span => Memory.Span;

        public ByteMemoryHolder(Memory<byte> memory)
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

        ~ByteMemoryHolder()
        {
            Dispose(false);
        }
    }
}
using System;

namespace StbSharp
{
    // TODO: improve with ArrayPool

    public ref struct ScratchBuffer
    {
        private byte[] _array;
        private Span<byte> _span;

        public bool IsEmpty => _span.IsEmpty;

        public ScratchBuffer(Span<byte> existingBuffer, int minSize)
        {
            if (existingBuffer.Length >= minSize)
            {
                _array = null;
                _span = existingBuffer.Slice(0, minSize);
            }
            else // allocate if the assigned buffer is too small
            {
                _array = new byte[minSize];
                _span = _array.AsSpan();
            }
        }

        public Span<byte> AsSpan() => _span;
        public Span<byte> AsSpan(int start) => _span.Slice(start);
        public Span<byte> AsSpan(int start, int length) => _span.Slice(start, length);
    }
}
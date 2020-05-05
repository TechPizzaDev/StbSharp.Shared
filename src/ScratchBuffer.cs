using System;

namespace StbSharp
{
    // TODO: improve with ArrayPool

    public ref struct ScratchBuffer
    {
        private byte[] _array;
        private Span<byte> _span;

        public bool IsEmpty => _span.IsEmpty;

        public ScratchBuffer(int minSize)
        {
            _array = new byte[minSize];
            _span = _array.AsSpan();
        }

        public Span<byte> AsSpan() => _span;
        public Span<byte> AsSpan(int start) => _span.Slice(start);
        public Span<byte> AsSpan(int start, int length) => _span.Slice(start, length);
    }
}
using System;

namespace StbSharp
{
    public unsafe ref struct ScratchBuffer
    {
        private byte* _ptr;
        private Span<byte> _span;

        public bool IsEmpty => _span.IsEmpty;

        public ScratchBuffer(Span<byte> preferredBuffer, int minSize)
        {
            if (preferredBuffer.Length >= minSize)
            {
                _ptr = null;
                _span = preferredBuffer.Slice(0, minSize);
            }
            else // allocate if the assigned buffer is too small
            {
                _ptr = (byte*)CRuntime.MAlloc(minSize);
                _span = new Span<byte>(_ptr, minSize);
            }
        }

        public Span<byte> AsSpan() => _span;
        public Span<byte> AsSpan(int start) => _span.Slice(start);
        public Span<byte> AsSpan(int start, int length) => _span.Slice(start, length);

        public void Dispose()
        {
            _span = Span<byte>.Empty;
            if (_ptr != null)
            {
                CRuntime.Free(_ptr);
                _ptr = null;
            }
        }
    }
}
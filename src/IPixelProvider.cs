using System;

namespace StbSharp
{
    public interface IPixelProvider
    {
        int Width { get; }
        int Height { get; }
        int Components { get; }

        void Fill(Span<byte> destination, int dataOffset);
        void Fill(Span<float> destination, int dataOffset);
    }
}

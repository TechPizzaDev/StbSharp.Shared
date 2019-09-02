using System;

namespace StbSharp
{
    public interface IPixelProvider
    {
        int Width { get; }
        int Height { get; }
        int Components { get; }

        void Fill(Span<byte> buffer, int dataOffset);
        void Fill(Span<float> buffer, int dataOffset);
    }
}

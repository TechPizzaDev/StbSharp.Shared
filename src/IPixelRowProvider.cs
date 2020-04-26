using System;

namespace StbSharp
{
    public interface IPixelRowProvider
    {
        int Width { get; }
        int Height { get; }
        int Components { get; }
        int Depth { get; }

        void GetRow(int row, Span<byte> destination);
        void GetRow(int row, Span<float> destination);
    }
}

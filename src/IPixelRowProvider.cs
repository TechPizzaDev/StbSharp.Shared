using System;

namespace StbSharp
{
    public interface IPixelRowProvider
    {
        int Width { get; }
        int Height { get; }

        // TODO: Use some kind of component definition system.
        int Components { get; }
        int Depth { get; }

        void GetByteRow(int row, Span<byte> destination);
        void GetFloatRow(int row, Span<float> destination);
    }
}

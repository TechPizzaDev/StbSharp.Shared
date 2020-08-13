using System.Runtime.InteropServices;

namespace StbSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Rect
    {
        public readonly int X;
        public readonly int Y;
        public readonly int W;
        public readonly int H;

        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
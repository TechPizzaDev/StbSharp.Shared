
namespace StbSharp
{
    public enum PngChunkType : uint
    {
        CgBI = ('C' << 24) + ('g' << 16) + ('B' << 8) + 'I',
        IHDR = ('I' << 24) + ('H' << 16) + ('D' << 8) + 'R',
        PLTE = ('P' << 24) + ('L' << 16) + ('T' << 8) + 'E',
        tRNS = ('t' << 24) + ('R' << 16) + ('N' << 8) + 'S',
        IDAT = ('I' << 24) + ('D' << 16) + ('A' << 8) + 'T',
        IEND = ('I' << 24) + ('E' << 16) + ('N' << 8) + 'D'
    }
}
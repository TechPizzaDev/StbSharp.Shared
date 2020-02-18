using System;

namespace StbSharp
{
    public interface IMemoryHolder : IDisposable
    {
        int Length { get; }
        Span<byte> Span { get; }
    }
}

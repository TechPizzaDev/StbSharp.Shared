using System;

namespace StbSharp
{
    public interface IMemoryResult : IDisposable
    {
        bool IsAllocated { get; }
        int Length { get; }
        IntPtr Pointer { get; }
    }
}

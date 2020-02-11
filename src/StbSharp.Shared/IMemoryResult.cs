using System;

namespace StbSharp
{
    public interface IMemoryResult : IDisposable
    {
        int Length { get; }
        IntPtr Pointer { get; }
        bool IsAllocated { get; }
    }
}

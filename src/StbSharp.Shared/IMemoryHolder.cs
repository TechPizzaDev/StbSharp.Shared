using System;

namespace StbSharp
{
    public interface IMemoryHolder : IDisposable
    {
        int Length { get; }
        IntPtr Pointer { get; }
        bool IsAllocated { get; }
    }
}

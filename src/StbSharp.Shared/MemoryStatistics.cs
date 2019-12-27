using System.Threading;

namespace StbSharp
{
	public static class MemoryStatistics
	{
		private static int _allocations;

        /// <summary>
        /// Gets the amount of pointers allocated using <see cref="CRuntime.MAlloc(long)"/>.
        /// Use <see cref="CRuntime.Free(void*)"/> to reduce this amount.
        /// </summary>
        public static int Allocations => _allocations;

		internal static void OnAllocate()
		{
			Interlocked.Increment(ref _allocations);
		}

		internal static void OnFree()
		{
			Interlocked.Decrement(ref _allocations);
		}
	}
}

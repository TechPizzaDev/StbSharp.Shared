using System.Threading;

namespace StbSharp
{
	public static class MemoryStatistics
	{
		private static int _allocations;

        /// <summary>
        /// Gets the amount of pointers allocated using <see cref="CRuntime.malloc(long)"/>.
        /// Use <see cref="CRuntime.free(void*)"/> to reduce this amount.
        /// </summary>
        public static int Allocations => _allocations;

		internal static void Allocated()
		{
			Interlocked.Increment(ref _allocations);
		}

		internal static void Freed()
		{
			Interlocked.Decrement(ref _allocations);
		}
	}
}

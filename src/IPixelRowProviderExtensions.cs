
namespace StbSharp
{
    public static class IPixelRowProviderExtensions
    {
        public static void ThrowIfCancelled<TImage>(this TImage image)
            where TImage : IPixelRowProvider
        {
            image.CancellationToken.ThrowIfCancellationRequested();
        }
    }
}

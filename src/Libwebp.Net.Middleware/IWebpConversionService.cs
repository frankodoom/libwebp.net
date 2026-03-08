namespace Libwebp.Net.Middleware;

/// <summary>
/// Converts an image stream to WebP format.
/// </summary>
public interface IWebpConversionService
{
    /// <summary>
    /// Converts the supplied image to WebP format.
    /// </summary>
    /// <param name="input">The source image stream (position must be at 0).</param>
    /// <param name="originalFileName">The original file name (e.g. "photo.jpg").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A tuple containing the converted WebP data as a <see cref="MemoryStream"/>
    /// and the new file name with a <c>.webp</c> extension.
    /// </returns>
    Task<(MemoryStream Data, string FileName)> ConvertAsync(
        Stream input,
        string originalFileName,
        CancellationToken cancellationToken = default);
}

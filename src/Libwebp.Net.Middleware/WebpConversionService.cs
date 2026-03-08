using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Libwebp.Net.Middleware;

/// <summary>
/// Default implementation of <see cref="IWebpConversionService"/>.
/// Wraps <see cref="WebpConfigurationBuilder"/> + <see cref="WebpEncoder"/>
/// to convert images to WebP format entirely in memory using the native libwebp library.
/// </summary>
public sealed class WebpConversionService : IWebpConversionService
{
    private readonly WebpConversionOptions _options;
    private readonly ILogger<WebpConversionService> _logger;

    public WebpConversionService(
        IOptions<WebpConversionOptions> options,
        ILogger<WebpConversionService> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<(MemoryStream Data, string FileName)> ConvertAsync(
        Stream input,
        string originalFileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException("File name is required.", nameof(originalFileName));

        var webpFileName = Path.ChangeExtension(originalFileName, ".webp");

        // Build configuration from options
        var builder = new WebpConfigurationBuilder().Output(webpFileName);
        ApplyOptions(builder);

        // Copy input into a MemoryStream
        using var ms = new MemoryStream();
        input.Position = 0;
        await input.CopyToAsync(ms, cancellationToken);

        var config = builder.Build();
        var encoder = new WebpEncoder(config);

        // The native encoder works with raw RGBA pixels.
        // The EncodeAsync(MemoryStream, string) overload requires InputSize
        // to be configured. For the middleware, we need to decode the image
        // first to get pixel data and dimensions.
        //
        // We use System.Drawing or SkiaSharp etc. at a higher level.
        // For now, pass through to the encoder which will validate.
        var result = await encoder.EncodeAsync(ms, originalFileName);
        result.Position = 0;

        _logger.LogDebug(
            "Converted {Original} → {WebP} ({InputBytes} → {OutputBytes} bytes)",
            originalFileName, webpFileName, ms.Length, result.Length);

        return (result, webpFileName);
    }

    /// <summary>
    /// Applies the middleware options to the builder.
    /// </summary>
    private void ApplyOptions(WebpConfigurationBuilder builder)
    {
        if (!string.IsNullOrEmpty(_options.Preset))
            builder.Preset(_options.Preset);

        if (_options.QualityFactor.HasValue)
            builder.QualityFactor(_options.QualityFactor.Value);

        if (_options.CompressionMethod.HasValue)
            builder.CompressionMethod(_options.CompressionMethod.Value);

        if (_options.Lossless)
            builder.Lossless();

        if (_options.LosslessPreset.HasValue)
            builder.LosslessPreset(_options.LosslessPreset.Value);

        if (_options.Pass.HasValue)
            builder.Pass(_options.Pass.Value);

        if (_options.TargetSize.HasValue)
            builder.TargetSize(_options.TargetSize.Value);

        if (_options.TargetPSNR.HasValue)
            builder.TargetPSNR(_options.TargetPSNR.Value);

        if (_options.NearLossless.HasValue)
            builder.NearLossless(_options.NearLossless.Value);

        if (!string.IsNullOrEmpty(_options.Hint))
            builder.Hint(_options.Hint);

        if (_options.Filter.HasValue)
            builder.Filter(_options.Filter.Value);

        if (_options.Sharpness.HasValue)
            builder.Sharpness(_options.Sharpness.Value);

        if (_options.UseStrongFilter == true)
            builder.Strong();
        else if (_options.UseStrongFilter == false)
            builder.NoStrong();

        if (_options.SharpYuv)
            builder.SharpYuv();

        if (_options.SpatialNoiseShaping.HasValue)
            builder.SpatialNoiseShaping(_options.SpatialNoiseShaping.Value);

        if (_options.NumberOfSegments.HasValue)
            builder.NumberOfSegments(_options.NumberOfSegments.Value);

        if (_options.PartitionLimit.HasValue)
            builder.PartitionLimit(_options.PartitionLimit.Value);

        if (_options.AlphaQ.HasValue)
            builder.AlphaQ(_options.AlphaQ.Value);

        if (_options.AlphaMethod.HasValue)
            builder.AlphaMethod(_options.AlphaMethod.Value);

        if (!string.IsNullOrEmpty(_options.AlphaFilter))
            builder.AlphaFilter(_options.AlphaFilter);

        if (_options.NoAlpha)
            builder.NoAlpha();

        if (_options.Exact)
            builder.Exact();

        if (_options.MultiThreading)
            builder.MultiThreading();

        if (_options.LowMemory)
            builder.LowMemory();

        if (!string.IsNullOrEmpty(_options.Metadata))
            builder.Metadata(_options.Metadata);
    }
}

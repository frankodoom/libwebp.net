using Microsoft.AspNetCore.Http;

namespace Libwebp.Net.Middleware;

/// <summary>
/// Configuration options for the WebP upload-conversion middleware.
/// Register via <c>builder.Services.AddWebpConversion(options =&gt; { ... });</c>
/// </summary>
public class WebpConversionOptions
{
    /// <summary>
    /// Quality factor (0:small .. 100:big). Default is 75.
    /// </summary>
    public float? QualityFactor { get; set; }

    /// <summary>
    /// Encode lossless. Default is false.
    /// </summary>
    public bool Lossless { get; set; }

    /// <summary>
    /// Preset: default, photo, picture, drawing, icon, text. Applied first.
    /// </summary>
    public string? Preset { get; set; }

    /// <summary>
    /// Compression method (0=fast, 6=slowest). Default is 4.
    /// </summary>
    public int? CompressionMethod { get; set; }

    /// <summary>
    /// Lossless preset level (0:fast .. 9:slowest).
    /// </summary>
    public int? LosslessPreset { get; set; }

    /// <summary>
    /// Number of analysis passes (1..10).
    /// </summary>
    public int? Pass { get; set; }

    /// <summary>
    /// Target size in bytes for the compressed output.
    /// </summary>
    public int? TargetSize { get; set; }

    /// <summary>
    /// Target PSNR in dB for the compressed output.
    /// </summary>
    public float? TargetPSNR { get; set; }

    /// <summary>
    /// Near-lossless preprocessing level (0..100). 100 = off (default).
    /// </summary>
    public int? NearLossless { get; set; }

    /// <summary>
    /// Image characteristics hint: photo, picture, or graph.
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// Deblocking filter strength (0=off..100).
    /// </summary>
    public int? Filter { get; set; }

    /// <summary>
    /// Filter sharpness (0:most sharp..7:least sharp).
    /// </summary>
    public int? Sharpness { get; set; }

    /// <summary>
    /// true = use strong filter, false = use simple filter (nostrong), null = codec default.
    /// </summary>
    public bool? UseStrongFilter { get; set; }

    /// <summary>
    /// Use sharper (slower) RGB→YUV conversion. Default is false.
    /// </summary>
    public bool SharpYuv { get; set; }

    /// <summary>
    /// Spatial noise shaping strength (0..100). Default is 50.
    /// </summary>
    public int? SpatialNoiseShaping { get; set; }

    /// <summary>
    /// Number of segments (1..4).
    /// </summary>
    public int? NumberOfSegments { get; set; }

    /// <summary>
    /// Limit quality to fit 512k first-partition limit (0..100).
    /// </summary>
    public int? PartitionLimit { get; set; }

    /// <summary>
    /// Alpha-channel quality (0..100).
    /// </summary>
    public int? AlphaQ { get; set; }

    /// <summary>
    /// Alpha compression method (0..1).
    /// </summary>
    public int? AlphaMethod { get; set; }

    /// <summary>
    /// Alpha predictive filter: none, fast, or best.
    /// </summary>
    public string? AlphaFilter { get; set; }

    /// <summary>
    /// Discard transparency information. Default is false.
    /// </summary>
    public bool NoAlpha { get; set; }

    /// <summary>
    /// Preserve RGB values in transparent areas. Default is false.
    /// </summary>
    public bool Exact { get; set; }

    /// <summary>
    /// Use multi-threaded encoding. Default is false.
    /// </summary>
    public bool MultiThreading { get; set; }

    /// <summary>
    /// Reduce memory usage (slower). Default is false.
    /// </summary>
    public bool LowMemory { get; set; }

    /// <summary>
    /// Metadata to copy: comma-separated list of all, none, exif, icc, xmp.
    /// </summary>
    public string? Metadata { get; set; }

    // ─── Middleware behaviour ──────────────────────────────────────────

    /// <summary>
    /// Content types considered convertible. Only files whose Content-Type
    /// matches one of these are converted.
    /// Default: image/jpeg, image/png, image/gif, image/tiff, image/bmp.
    /// </summary>
    public HashSet<string> InputContentTypes { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/tiff",
        "image/bmp"
    };

    /// <summary>
    /// File extensions considered convertible (case-insensitive, include the dot).
    /// Default: .jpg, .jpeg, .png, .gif, .tiff, .tif, .bmp.
    /// </summary>
    public HashSet<string> InputExtensions { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".bmp"
    };

    /// <summary>
    /// Maximum input file size in bytes. Files larger than this are passed
    /// through without conversion. Default is null (no limit).
    /// </summary>
    public long? MaxInputSizeBytes { get; set; }

    /// <summary>
    /// Optional predicate to skip conversion for specific requests / files.
    /// Return <c>true</c> to skip conversion for a given file.
    /// </summary>
    public Func<HttpContext, IFormFile, bool>? SkipConversion { get; set; }
}

using Libwebp.Net.Interop;
using System;

namespace Libwebp.Net
{
    /// <summary>
    /// WebP encoding configuration. Stores typed values that map directly
    /// to libwebp's native WebPConfig struct fields.
    /// Use <see cref="WebpConfigurationBuilder"/> to construct an instance.
    /// </summary>
    public class WebPConfiguration
    {
        protected internal WebPConfiguration()
        {
            // Defaults matching libwebp's WebPConfigInit (preset=default, quality=75)
            NativeQuality = 75f;
            NativeMethod = 4;
            NativeSegments = 4;
            NativeSnsStrength = 50;
            NativeFilterStrength = 60;
            NativeFilterSharpness = 0;
            NativeFilterType = 1; // strong by default
            NativeAlphaCompression = 1;
            NativeAlphaFiltering = 1; // fast
            NativeAlphaQuality = 100;
            NativePass = 1;
            NativeNearLossless = 100; // off by default
            NativePreset = WebPPreset.Default;
            NativeImageHint = WebPImageHint.Default;
        }

        // ── Identity ──

        /// <summary>The output file name (e.g., "output.webp").</summary>
        internal string OutputFileName { get; set; }

        // ── Core encoding parameters (typed, map to WebPConfig fields) ──

        /// <summary>Quality factor (0=small..100=big), default 75.</summary>
        internal float NativeQuality { get; set; }

        /// <summary>Lossless mode (false=lossy, true=lossless).</summary>
        internal bool NativeLossless { get; set; }

        /// <summary>Lossless preset level (0=fast..9=slowest). -1 = not set.</summary>
        internal int NativeLosslessPreset { get; set; } = -1;

        /// <summary>Compression method (0=fast, 6=slower-better), default 4.</summary>
        internal int NativeMethod { get; set; }

        /// <summary>Encoding preset.</summary>
        internal WebPPreset NativePreset { get; set; }

        /// <summary>Image content hint.</summary>
        internal WebPImageHint NativeImageHint { get; set; }

        /// <summary>Alpha quality (0..100).</summary>
        internal int NativeAlphaQuality { get; set; }

        /// <summary>Maximum number of segments (1..4).</summary>
        internal int NativeSegments { get; set; }

        /// <summary>Target size in bytes (0=no target).</summary>
        internal int NativeTargetSize { get; set; }

        /// <summary>Target PSNR in dB (0=no target).</summary>
        internal float NativeTargetPSNR { get; set; }

        /// <summary>Spatial noise shaping strength (0..100).</summary>
        internal int NativeSnsStrength { get; set; }

        /// <summary>Deblocking filter strength (0..100).</summary>
        internal int NativeFilterStrength { get; set; }

        /// <summary>Filter sharpness (0=most sharp..7=least sharp).</summary>
        internal int NativeFilterSharpness { get; set; }

        /// <summary>Filter type (0=simple, 1=strong). Default 1.</summary>
        internal int NativeFilterType { get; set; }

        /// <summary>Use sharp RGB-to-YUV conversion.</summary>
        internal bool NativeSharpYuv { get; set; }

        /// <summary>Quality degradation limit for partition 0 (0..100).</summary>
        internal int NativePartitionLimit { get; set; }

        /// <summary>Number of entropy-analysis passes (1..10).</summary>
        internal int NativePass { get; set; }

        /// <summary>Multi-threading for encoding.</summary>
        internal bool NativeMultiThreading { get; set; }

        /// <summary>Reduce memory usage (slower encoding).</summary>
        internal bool NativeLowMemory { get; set; }

        /// <summary>Alpha compression method (0=none, 1=compressed).</summary>
        internal int NativeAlphaCompression { get; set; }

        /// <summary>Alpha filtering (0=none, 1=fast, 2=best).</summary>
        internal int NativeAlphaFiltering { get; set; }

        /// <summary>Preserve RGB values in transparent area.</summary>
        internal bool NativeExact { get; set; }

        /// <summary>Discard alpha channel entirely.</summary>
        internal bool NativeNoAlpha { get; set; }

        /// <summary>Near lossless preprocessing (0..100, 100=off).</summary>
        internal int NativeNearLossless { get; set; }

        // ── Geometry operations (applied to WebPPicture, not WebPConfig) ──

        /// <summary>Crop rectangle. Null = no crop.</summary>
        internal (int X, int Y, int Width, int Height)? CropRect { get; set; }

        /// <summary>Resize dimensions. (0,0) = no resize.</summary>
        internal (int Width, int Height) ResizeDimensions { get; set; }

        /// <summary>Input size for raw YUV. (0,0) = not used.</summary>
        internal (int Width, int Height) InputSizeDimensions { get; set; }

        /// <summary>Metadata to copy (e.g. "all", "exif,icc"). Null = none.</summary>
        internal string MetadataOption { get; set; }

        // ── Build the native WebPConfig struct from these values ──

        /// <summary>
        /// Creates a native <see cref="WebPConfig"/> struct populated with the
        /// current configuration values.
        /// </summary>
        internal WebPConfig ToNativeConfig()
        {
            var config = new WebPConfig();
            if (!LibWebPNative.WebPConfigPreset(ref config, NativePreset, NativeQuality))
            {
                throw new InvalidOperationException("Failed to initialize native WebPConfig.");
            }

            config.lossless = NativeLossless ? 1 : 0;
            config.quality = NativeQuality;
            config.method = NativeMethod;
            config.image_hint = NativeImageHint;
            config.target_size = NativeTargetSize;
            config.target_PSNR = NativeTargetPSNR;
            config.segments = NativeSegments;
            config.sns_strength = NativeSnsStrength;
            config.filter_strength = NativeFilterStrength;
            config.filter_sharpness = NativeFilterSharpness;
            config.filter_type = NativeFilterType;
            config.alpha_compression = NativeAlphaCompression;
            config.alpha_filtering = NativeAlphaFiltering;
            config.alpha_quality = NativeAlphaQuality;
            config.pass = NativePass;
            config.partition_limit = NativePartitionLimit;
            config.thread_level = NativeMultiThreading ? 1 : 0;
            config.low_memory = NativeLowMemory ? 1 : 0;
            config.near_lossless = NativeNearLossless;
            config.exact = NativeExact ? 1 : 0;
            config.use_sharp_yuv = NativeSharpYuv ? 1 : 0;

            return config;
        }
    }
}

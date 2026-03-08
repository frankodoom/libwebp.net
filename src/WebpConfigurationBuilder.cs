using Libwebp.Net.errors;
using Libwebp.Net.Interop;
using System;

namespace Libwebp.Net
{
    /// <summary>
    /// Fluent builder for creating a <see cref="WebPConfiguration"/>.
    /// All settings map directly to libwebp's native WebPConfig struct fields.
    /// </summary>
    public class WebpConfigurationBuilder
    {
        private readonly WebPConfiguration _config = new WebPConfiguration();
        public WebPConfiguration Build() => _config;

        /// <summary>
        /// Specify the name of the output WebP file.
        /// </summary>
        /// <param name="value">The name of the webp output file (e.g., "output.webp")</param>
        public WebpConfigurationBuilder Output(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new OutputFileNameNotFoundException("Specify the name of the output WebP file.");
            _config.OutputFileName = value;
            return this;
        }

        /// <summary>
        /// Quality factor (0:small..100:big), default is 75.
        /// </summary>
        public WebpConfigurationBuilder QualityFactor(float value)
        {
            _config.NativeQuality = value;
            return this;
        }

        /// <summary>
        /// Transparency-compression quality (0..100).
        /// </summary>
        public WebpConfigurationBuilder AlphaQ(int value)
        {
            _config.NativeAlphaQuality = value;
            return this;
        }

        /// <summary>
        /// Preset setting, one of: default, photo, picture, drawing, icon, text.
        /// Preset must come first, as it overwrites other parameters.
        /// </summary>
        public WebpConfigurationBuilder Preset(string value)
        {
            _config.NativePreset = ParsePreset(value);
            return this;
        }

        /// <summary>
        /// Compression method (0=fast, 6=slowest), default is 4.
        /// </summary>
        public WebpConfigurationBuilder CompressionMethod(int value)
        {
            _config.NativeMethod = value;
            return this;
        }

        /// <summary>
        /// Encode the image without any loss (lossless mode).
        /// </summary>
        public WebpConfigurationBuilder Lossless()
        {
            _config.NativeLossless = true;
            return this;
        }

        /// <summary>
        /// Activates lossless preset with given level (0:fast..9:slowest).
        /// </summary>
        public WebpConfigurationBuilder LosslessPreset(int level)
        {
            _config.NativeLossless = true;
            _config.NativeLosslessPreset = level;
            // Map lossless presets to method levels as libwebp does
            _config.NativeMethod = level switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => 3,
                5 => 4,
                6 => 4,
                7 => 4,
                8 => 5,
                9 => 6,
                _ => 4
            };
            return this;
        }

        /// <summary>
        /// Number of segments to use (1..4), default is 4.
        /// </summary>
        public WebpConfigurationBuilder NumberOfSegments(int value)
        {
            _config.NativeSegments = value;
            return this;
        }

        /// <summary>
        /// Target size (in bytes) to try and reach for the compressed output.
        /// </summary>
        public WebpConfigurationBuilder TargetSize(int value)
        {
            _config.NativeTargetSize = value;
            return this;
        }

        /// <summary>
        /// Target PSNR (in dB) to try and reach for the compressed output.
        /// </summary>
        public WebpConfigurationBuilder TargetPSNR(float value)
        {
            _config.NativeTargetPSNR = value;
            return this;
        }

        /// <summary>
        /// Input size (width x height) for YUV input.
        /// </summary>
        public WebpConfigurationBuilder InputSize(int width, int height)
        {
            _config.InputSizeDimensions = (width, height);
            return this;
        }

        /// <summary>
        /// Spatial noise shaping strength (0..100), default is 50.
        /// </summary>
        public WebpConfigurationBuilder SpatialNoiseShaping(int value)
        {
            _config.NativeSnsStrength = value;
            return this;
        }

        /// <summary>
        /// Deblocking filter strength (0=off..100).
        /// </summary>
        public WebpConfigurationBuilder Filter(int value)
        {
            _config.NativeFilterStrength = value;
            return this;
        }

        /// <summary>
        /// Filter sharpness (0:most sharp..7:least sharp), default is 0.
        /// </summary>
        public WebpConfigurationBuilder Sharpness(int value)
        {
            _config.NativeFilterSharpness = value;
            return this;
        }

        /// <summary>
        /// Use strong filter instead of simple (default is strong).
        /// </summary>
        public WebpConfigurationBuilder Strong()
        {
            _config.NativeFilterType = 1;
            return this;
        }

        /// <summary>
        /// Use simple filter instead of strong.
        /// </summary>
        public WebpConfigurationBuilder NoStrong()
        {
            _config.NativeFilterType = 0;
            return this;
        }

        /// <summary>
        /// Use sharper (and slower) RGB-to-YUV conversion.
        /// Produces sharper results especially around edges.
        /// </summary>
        public WebpConfigurationBuilder SharpYuv()
        {
            _config.NativeSharpYuv = true;
            return this;
        }

        /// <summary>
        /// Limit quality to fit the 512k limit on the first partition
        /// (0=no degradation..100=full).
        /// </summary>
        public WebpConfigurationBuilder PartitionLimit(int value)
        {
            _config.NativePartitionLimit = value;
            return this;
        }

        /// <summary>
        /// Analysis pass number (1..10). More passes give better quality
        /// but are slower.
        /// </summary>
        public WebpConfigurationBuilder Pass(int value)
        {
            _config.NativePass = value;
            return this;
        }

        /// <summary>
        /// Crop the picture with the given rectangle before encoding.
        /// </summary>
        public WebpConfigurationBuilder Crop(int x, int y, int width, int height)
        {
            _config.CropRect = (x, y, width, height);
            return this;
        }

        /// <summary>
        /// Resize the picture to the given dimensions (after any cropping).
        /// Either width or height can be 0 to preserve aspect ratio.
        /// </summary>
        public WebpConfigurationBuilder Resize(int width, int height)
        {
            _config.ResizeDimensions = (width, height);
            return this;
        }

        /// <summary>
        /// Use multi-threading for encoding if available.
        /// </summary>
        public WebpConfigurationBuilder MultiThreading()
        {
            _config.NativeMultiThreading = true;
            return this;
        }

        /// <summary>
        /// Reduce memory usage at the cost of slower encoding.
        /// </summary>
        public WebpConfigurationBuilder LowMemory()
        {
            _config.NativeLowMemory = true;
            return this;
        }

        /// <summary>
        /// Transparency-compression method (0..1), default is 1.
        /// </summary>
        public WebpConfigurationBuilder AlphaMethod(int value)
        {
            _config.NativeAlphaCompression = value;
            return this;
        }

        /// <summary>
        /// Predictive filtering for alpha plane.
        /// One of: none, fast (default), or best.
        /// </summary>
        public WebpConfigurationBuilder AlphaFilter(string value)
        {
            _config.NativeAlphaFiltering = ParseAlphaFilter(value);
            return this;
        }

        /// <summary>
        /// Preserve RGB values in transparent area. Default is off.
        /// </summary>
        public WebpConfigurationBuilder Exact()
        {
            _config.NativeExact = true;
            return this;
        }

        /// <summary>
        /// Discard any transparency information.
        /// </summary>
        public WebpConfigurationBuilder NoAlpha()
        {
            _config.NativeNoAlpha = true;
            return this;
        }

        /// <summary>
        /// Use near-lossless image preprocessing (0..100).
        /// 100 = off (default), lower values apply more preprocessing.
        /// </summary>
        public WebpConfigurationBuilder NearLossless(int value)
        {
            _config.NativeNearLossless = value;
            return this;
        }

        /// <summary>
        /// Specify image characteristics hint.
        /// One of: photo, picture, or graph.
        /// </summary>
        public WebpConfigurationBuilder Hint(string value)
        {
            _config.NativeImageHint = ParseHint(value);
            return this;
        }

        /// <summary>
        /// Copy metadata from input to output.
        /// Comma-separated list of: all, none (default), exif, icc, xmp.
        /// Note: Metadata preservation is not supported in the native in-memory
        /// encoding path. This setting is stored for future compatibility.
        /// </summary>
        public WebpConfigurationBuilder Metadata(string value)
        {
            _config.MetadataOption = value;
            return this;
        }

        // ── Private helpers ──

        private static WebPPreset ParsePreset(string value) =>
            value?.ToLowerInvariant() switch
            {
                "photo" => Interop.WebPPreset.Photo,
                "picture" => Interop.WebPPreset.Picture,
                "drawing" => Interop.WebPPreset.Drawing,
                "icon" => Interop.WebPPreset.Icon,
                "text" => Interop.WebPPreset.Text,
                "default" => Interop.WebPPreset.Default,
                _ => Interop.WebPPreset.Default
            };

        private static int ParseAlphaFilter(string value) =>
            value?.ToLowerInvariant() switch
            {
                "none" => 0,
                "fast" => 1,
                "best" => 2,
                _ => 1
            };

        private static WebPImageHint ParseHint(string value) =>
            value?.ToLowerInvariant() switch
            {
                "photo" => WebPImageHint.Photo,
                "picture" => WebPImageHint.Picture,
                "graph" => WebPImageHint.Graph,
                _ => WebPImageHint.Default
            };
    }
}

using Libwebp.Net.errors;
using Libwebp.Net.Interop;
using Libwebp.Net.utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Libwebp.Net
{
    /// <summary>
    /// Encodes images to WebP format using the native libwebp C library.
    /// Operates entirely in memory — no temp files, no process spawning.
    /// </summary>
    public class WebpEncoder
    {
        private readonly WebPConfiguration _configuration;

        /// <summary>
        /// Creates a new encoder with the specified configuration.
        /// </summary>
        /// <param name="configuration">The WebP encoding configuration</param>
        public WebpEncoder(WebPConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Encodes the input image to WebP format asynchronously.
        /// Accepts common image formats (JPEG, PNG, BMP, GIF, TIFF) which are
        /// decoded automatically using the platform's native imaging API.
        /// Also accepts raw RGBA pixel data if InputSize(width, height) is configured.
        /// </summary>
        /// <param name="memoryStream">The input image as a MemoryStream</param>
        /// <param name="fileName">The original input file name (used for output naming)</param>
        /// <returns>A MemoryStream of the encoded WebP data</returns>
        public async Task<MemoryStream> EncodeAsync(MemoryStream memoryStream, string fileName)
        {
            if (memoryStream == null)
                throw new ArgumentNullException(nameof(memoryStream));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrWhiteSpace(_configuration.OutputFileName))
                throw new OutputFileNameNotFoundException("Output file name not configured. Call Output() on WebpConfigurationBuilder.");

            // Read the raw pixel data from the stream
            memoryStream.Position = 0;
            var inputData = memoryStream.ToArray();

            // Encode using native library on a thread pool thread
            // to avoid blocking the caller
            var webpBytes = await Task.Run(() => EncodeNative(inputData));

            return new MemoryStream(webpBytes, writable: false);
        }

        /// <summary>
        /// Encodes raw RGBA pixel data to WebP format synchronously.
        /// </summary>
        /// <param name="rgba">RGBA pixel data (4 bytes per pixel).</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <returns>Encoded WebP data as a byte array.</returns>
        public byte[] Encode(byte[] rgba, int width, int height)
        {
            if (rgba == null) throw new ArgumentNullException(nameof(rgba));
            if (width <= 0) throw new ArgumentException("Width must be positive.", nameof(width));
            if (height <= 0) throw new ArgumentException("Height must be positive.", nameof(height));

            var nativeConfig = _configuration.ToNativeConfig();

            return NativeEncoder.EncodeAdvanced(
                rgba, width, height, nativeConfig,
                isBgra: false,
                cropRect: _configuration.CropRect,
                resizeWidth: _configuration.ResizeDimensions.Width,
                resizeHeight: _configuration.ResizeDimensions.Height);
        }

        /// <summary>
        /// Encodes raw RGBA pixel data to WebP format asynchronously.
        /// </summary>
        /// <param name="rgba">RGBA pixel data (4 bytes per pixel).</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <returns>Encoded WebP data as a byte array.</returns>
        public async Task<byte[]> EncodeAsync(byte[] rgba, int width, int height)
        {
            if (rgba == null) throw new ArgumentNullException(nameof(rgba));
            if (width <= 0) throw new ArgumentException("Width must be positive.", nameof(width));
            if (height <= 0) throw new ArgumentException("Height must be positive.", nameof(height));

            var nativeConfig = _configuration.ToNativeConfig();

            return await Task.Run(() => NativeEncoder.EncodeAdvanced(
                rgba, width, height, nativeConfig,
                isBgra: false,
                cropRect: _configuration.CropRect,
                resizeWidth: _configuration.ResizeDimensions.Width,
                resizeHeight: _configuration.ResizeDimensions.Height));
        }

        /// <summary>
        /// Returns the version of the underlying native libwebp encoder.
        /// </summary>
        public static string GetNativeVersion() => NativeEncoder.GetVersion();

        /// <summary>
        /// Internal: encodes input data using the native config.
        /// Automatically detects encoded image files (JPEG, PNG, BMP, GIF, TIFF)
        /// and decodes them to pixels using the platform's native imaging API
        /// (GDI+ on Windows). If the input is raw RGBA pixel data, the caller
        /// must configure InputSize(width, height).
        /// </summary>
        private byte[] EncodeNative(byte[] inputData)
        {
            // If the data starts with a known image file signature,
            // decode it to pixels automatically.
            if (PlatformImageDecoder.IsEncodedImage(inputData))
            {
                var decoded = PlatformImageDecoder.Decode(inputData);
                var nativeConfig = _configuration.ToNativeConfig();

                return NativeEncoder.EncodeAdvanced(
                    decoded.Pixels, decoded.Width, decoded.Height, nativeConfig,
                    isBgra: decoded.IsBgra,
                    cropRect: _configuration.CropRect,
                    resizeWidth: _configuration.ResizeDimensions.Width,
                    resizeHeight: _configuration.ResizeDimensions.Height);
            }

            // Raw RGBA pixel data — dimensions must be configured explicitly.
            var dims = _configuration.InputSizeDimensions;
            if (dims.Width <= 0 || dims.Height <= 0)
            {
                throw new WebPEncodingException(
                    "When using EncodeAsync(MemoryStream, string) with raw pixel data, " +
                    "you must configure InputSize(width, height) so the encoder knows " +
                    "the pixel dimensions. For encoded images (JPEG, PNG, BMP, GIF), " +
                    "dimensions are detected automatically.");
            }

            int expectedSize = dims.Width * dims.Height * 4;
            if (inputData.Length != expectedSize)
            {
                throw new WebPEncodingException(
                    $"Input data size ({inputData.Length}) does not match expected RGBA size " +
                    $"({expectedSize}) for dimensions {dims.Width}x{dims.Height}.");
            }

            var rawConfig = _configuration.ToNativeConfig();

            return NativeEncoder.EncodeAdvanced(
                inputData, dims.Width, dims.Height, rawConfig,
                cropRect: _configuration.CropRect,
                resizeWidth: _configuration.ResizeDimensions.Width,
                resizeHeight: _configuration.ResizeDimensions.Height);
        }
    }
}

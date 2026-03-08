using Libwebp.Net.errors;
using Libwebp.Net.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Libwebp.Net.utility
{
    /// <summary>
    /// Low-level WebP encoder that calls the native libwebp C library directly
    /// via P/Invoke. Encodes entirely in memory — no temp files, no process spawning.
    /// </summary>
    internal static class NativeEncoder
    {
        static NativeEncoder()
        {
            LibWebPResolver.EnsureRegistered();
        }

        /// <summary>
        /// Encodes raw RGBA pixel data to WebP using the simple API.
        /// Suitable when only quality/lossless settings are needed.
        /// </summary>
        /// <param name="rgba">RGBA pixel data (4 bytes per pixel).</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="quality">Quality factor (0..100). Ignored if lossless.</param>
        /// <param name="lossless">True for lossless encoding.</param>
        /// <returns>Encoded WebP data as a byte array.</returns>
        public static byte[] EncodeSimple(byte[] rgba, int width, int height, float quality, bool lossless)
        {
            if (rgba == null) throw new ArgumentNullException(nameof(rgba));
            if (width <= 0) throw new ArgumentException("Width must be positive.", nameof(width));
            if (height <= 0) throw new ArgumentException("Height must be positive.", nameof(height));

            int stride = width * 4;
            nint outputPtr = nint.Zero;

            try
            {
                nuint size;
                if (lossless)
                {
                    size = LibWebPNative.WebPEncodeLosslessRGBA(rgba, width, height, stride, out outputPtr);
                }
                else
                {
                    size = LibWebPNative.WebPEncodeRGBA(rgba, width, height, stride, quality, out outputPtr);
                }

                if (size == 0 || outputPtr == nint.Zero)
                {
                    throw new WebPEncodingException("WebP encoding failed (output size is 0).");
                }

                var result = new byte[(int)size];
                Marshal.Copy(outputPtr, result, 0, (int)size);
                return result;
            }
            finally
            {
                if (outputPtr != nint.Zero)
                    LibWebPNative.WebPFree(outputPtr);
            }
        }

        /// <summary>
        /// Encodes raw pixel data to WebP using the advanced API,
        /// which supports the full range of encoding options.
        /// </summary>
        /// <param name="pixels">Pixel data (4 bytes per pixel, RGBA or BGRA).</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="config">Full WebP encoding configuration.</param>
        /// <param name="isBgra">True if pixel data is in BGRA order (e.g. from GDI+), false for RGBA.</param>
        /// <param name="cropRect">Optional crop rectangle (x, y, w, h). Null to skip.</param>
        /// <param name="resizeWidth">Resize width (0 to preserve aspect ratio). 0 to skip resize.</param>
        /// <param name="resizeHeight">Resize height (0 to preserve aspect ratio). 0 to skip resize.</param>
        /// <returns>Encoded WebP data as a byte array.</returns>
        public static byte[] EncodeAdvanced(
            byte[] pixels, int width, int height,
            WebPConfig config,
            bool isBgra = false,
            (int x, int y, int width, int height)? cropRect = null,
            int resizeWidth = 0, int resizeHeight = 0)
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (width <= 0) throw new ArgumentException("Width must be positive.", nameof(width));
            if (height <= 0) throw new ArgumentException("Height must be positive.", nameof(height));

            // Validate config
            if (LibWebPNative.WebPValidateConfig(ref config) == 0)
            {
                throw new WebPEncodingException("Invalid WebP configuration. Check parameter values.");
            }

            var picture = new WebPPicture();
            var outputStream = new MemoryStream();
            GCHandle streamHandle = default;

            try
            {
                // Initialize picture
                if (!LibWebPNative.WebPPictureInit(ref picture))
                {
                    throw new WebPEncodingException(
                        "Failed to initialize WebPPicture. Possible ABI version mismatch with native library.");
                }

                picture.width = width;
                picture.height = height;
                picture.use_argb = 1;

                int stride = width * 4;

                // Import pixels into the picture (BGRA from GDI+, or RGBA)
                int importResult = isBgra
                    ? LibWebPNative.WebPPictureImportBGRA(ref picture, pixels, stride)
                    : LibWebPNative.WebPPictureImportRGBA(ref picture, pixels, stride);

                if (importResult == 0)
                {
                    string fmt = isBgra ? "BGRA" : "RGBA";
                    throw new WebPEncodingException(
                        $"Failed to import {fmt} data ({width}x{height}, {pixels.Length} bytes). " +
                        $"Error: {picture.error_code}");
                }

                // Apply crop if specified
                if (cropRect.HasValue)
                {
                    var c = cropRect.Value;
                    if (LibWebPNative.WebPPictureCrop(ref picture, c.x, c.y, c.width, c.height) == 0)
                    {
                        throw new WebPEncodingException(
                            $"Crop failed ({c.x}, {c.y}, {c.width}x{c.height}). Error: {picture.error_code}");
                    }
                }

                // Apply resize if specified
                if (resizeWidth > 0 || resizeHeight > 0)
                {
                    if (LibWebPNative.WebPPictureRescale(ref picture, resizeWidth, resizeHeight) == 0)
                    {
                        throw new WebPEncodingException(
                            $"Resize failed ({resizeWidth}x{resizeHeight}). Error: {picture.error_code}");
                    }
                }

                // Set up a managed writer callback that accumulates output
                // into a MemoryStream. We pass the stream via a GCHandle stored
                // in picture.custom_ptr (Normal handle — no pinning needed).
                streamHandle = GCHandle.Alloc(outputStream);
                picture.custom_ptr = GCHandle.ToIntPtr(streamHandle);

                var writerDelegate = new WebPWriterFunction(MemoryWriterCallback);
                picture.writer = Marshal.GetFunctionPointerForDelegate(writerDelegate);

                // Encode
                int result = LibWebPNative.WebPEncode(ref config, ref picture);

                // Keep delegate alive during encoding
                GC.KeepAlive(writerDelegate);

                if (result == 0)
                {
                    throw new WebPEncodingException(
                        $"WebP encoding failed. Error code: {picture.error_code} ({GetErrorMessage(picture.error_code)})");
                }

                return outputStream.ToArray();
            }
            finally
            {
                LibWebPNative.WebPPictureFree(ref picture);
                if (streamHandle.IsAllocated)
                    streamHandle.Free();
                outputStream.Dispose();
            }
        }

        /// <summary>
        /// Managed callback that accumulates encoder output into a MemoryStream.
        /// The MemoryStream reference is stored in picture.custom_ptr via a GCHandle.
        /// </summary>
        private static int MemoryWriterCallback(nint data, nuint data_size, ref WebPPicture picture)
        {
            try
            {
                var handle = GCHandle.FromIntPtr(picture.custom_ptr);
                var stream = (MemoryStream)handle.Target!;

                int size = (int)data_size;
                byte[] buffer = new byte[size];
                Marshal.Copy(data, buffer, 0, size);
                stream.Write(buffer, 0, size);
                return 1; // success
            }
            catch
            {
                return 0; // signal write error to the encoder
            }
        }

        /// <summary>
        /// Returns a human-readable error message for a WebP error code.
        /// </summary>
        public static string GetErrorMessage(WebPEncodingError error) => error switch
        {
            WebPEncodingError.Ok => "No error.",
            WebPEncodingError.OutOfMemory => "Memory error allocating objects.",
            WebPEncodingError.BitstreamOutOfMemory => "Memory error while flushing bits.",
            WebPEncodingError.NullParameter => "A pointer parameter is NULL.",
            WebPEncodingError.InvalidConfiguration => "Configuration is invalid.",
            WebPEncodingError.BadDimension => "Picture has invalid width/height.",
            WebPEncodingError.Partition0Overflow => "Partition #0 is too big (> 512K). Try reducing quality or size.",
            WebPEncodingError.PartitionOverflow => "Partition is too big (> 16M). Try reducing quality or size.",
            WebPEncodingError.BadWrite => "Error while flushing bytes (writer callback returned 0).",
            WebPEncodingError.FileTooBig => "File is too big (> 4GB).",
            WebPEncodingError.UserAbort => "Encoding aborted by user callback.",
            _ => $"Unknown error ({(int)error})."
        };

        /// <summary>
        /// Returns the version of the native libwebp encoder library.
        /// Format: "major.minor.revision"
        /// </summary>
        public static string GetVersion()
        {
            int v = LibWebPNative.WebPGetEncoderVersion();
            int major = (v >> 16) & 0xFF;
            int minor = (v >> 8) & 0xFF;
            int revision = v & 0xFF;
            return $"{major}.{minor}.{revision}";
        }
    }
}

using System.Runtime.InteropServices;

namespace Libwebp.Net.Interop
{
    /// <summary>
    /// P/Invoke declarations for the native libwebp encoder library.
    /// Targets libwebp 1.5.0+ C API defined in webp/encode.h.
    /// 
    /// The native library is stored in the codecs/ folder (win/, linux/, osx/)
    /// and resolved at runtime via a custom NativeLibrary resolver registered
    /// in <see cref="LibWebPResolver"/>.
    /// </summary>
    internal static class LibWebPNative
    {
        /// <summary>
        /// The library name used by DllImport. .NET resolves this to
        /// libwebp.dll (Windows), libwebp.so (Linux), or libwebp.dylib (macOS).
        /// </summary>
        private const string LibWebP = "libwebp";

        // ──────────────────────────────────────────────────────────────
        //  Version
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the encoder's version number as a packed int:
        /// (major &lt;&lt; 16) | (minor &lt;&lt; 8) | revision
        /// e.g. v1.5.0 = 0x010500 = 66816
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetEncoderVersion")]
        public static extern int WebPGetEncoderVersion();

        // ──────────────────────────────────────────────────────────────
        //  Simple encoding API
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Encodes RGBA pixels to lossy WebP format.
        /// Returns output size in bytes. Caller must free output with WebPFree().
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeRGBA")]
        public static extern nuint WebPEncodeRGBA(
            byte[] rgba, int width, int height, int stride,
            float quality_factor, out nint output);

        /// <summary>
        /// Encodes RGBA pixels to lossless WebP format.
        /// Returns output size in bytes. Caller must free output with WebPFree().
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessRGBA")]
        public static extern nuint WebPEncodeLosslessRGBA(
            byte[] rgba, int width, int height, int stride,
            out nint output);

        /// <summary>
        /// Encodes RGB pixels (no alpha) to lossy WebP format.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeRGB")]
        public static extern nuint WebPEncodeRGB(
            byte[] rgb, int width, int height, int stride,
            float quality_factor, out nint output);

        /// <summary>
        /// Encodes BGR pixels to lossy WebP format.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGR")]
        public static extern nuint WebPEncodeBGR(
            byte[] bgr, int width, int height, int stride,
            float quality_factor, out nint output);

        /// <summary>
        /// Encodes BGRA pixels to lossy WebP format.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGRA")]
        public static extern nuint WebPEncodeBGRA(
            byte[] bgra, int width, int height, int stride,
            float quality_factor, out nint output);

        /// <summary>
        /// Encodes RGB pixels to lossless WebP format.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessRGB")]
        public static extern nuint WebPEncodeLosslessRGB(
            byte[] rgb, int width, int height, int stride,
            out nint output);

        /// <summary>
        /// Encodes BGRA pixels to lossless WebP format.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGRA")]
        public static extern nuint WebPEncodeLosslessBGRA(
            byte[] bgra, int width, int height, int stride,
            out nint output);

        // ──────────────────────────────────────────────────────────────
        //  Memory
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Frees memory allocated by the WebPEncode* functions.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFree")]
        public static extern void WebPFree(nint ptr);

        // ──────────────────────────────────────────────────────────────
        //  Advanced encoding API — WebPConfig
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Initializes a WebPConfig struct with default values.
        /// Returns 0 on ABI mismatch (version check failure).
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigInitInternal")]
        public static extern int WebPConfigInitInternal(
            ref WebPConfig config, WebPPreset preset, float quality, int version);

        /// <summary>
        /// Validates a WebPConfig struct. Returns 1 if config is valid, 0 otherwise.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPValidateConfig")]
        public static extern int WebPValidateConfig(ref WebPConfig config);

        // ──────────────────────────────────────────────────────────────
        //  Advanced encoding API — WebPPicture
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Initializes a WebPPicture struct. Must be called before use.
        /// Returns 0 on version mismatch.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureInitInternal")]
        public static extern int WebPPictureInitInternal(ref WebPPicture picture, int version);

        /// <summary>
        /// Allocates internal ARGB buffer for the picture based on width/height.
        /// Returns 0 on memory error.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureAlloc")]
        public static extern int WebPPictureAlloc(ref WebPPicture picture);

        /// <summary>
        /// Frees allocated memory for the picture (does not free the struct itself).
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureFree")]
        public static extern void WebPPictureFree(ref WebPPicture picture);

        /// <summary>
        /// Imports RGBA data into the picture (use_argb is set automatically).
        /// Returns 0 on memory error.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportRGBA")]
        public static extern int WebPPictureImportRGBA(ref WebPPicture picture, byte[] rgba, int stride);

        /// <summary>
        /// Imports RGB data into the picture.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportRGB")]
        public static extern int WebPPictureImportRGB(ref WebPPicture picture, byte[] rgb, int stride);

        /// <summary>
        /// Imports BGRA data into the picture.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRA")]
        public static extern int WebPPictureImportBGRA(ref WebPPicture picture, byte[] bgra, int stride);

        /// <summary>
        /// Imports BGR data into the picture.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGR")]
        public static extern int WebPPictureImportBGR(ref WebPPicture picture, byte[] bgr, int stride);

        /// <summary>
        /// Imports RGBA data from a pointer into the picture.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportRGBA")]
        public static extern int WebPPictureImportRGBA(ref WebPPicture picture, nint rgba, int stride);

        /// <summary>
        /// Imports BGRA data from a pointer into the picture.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRA")]
        public static extern int WebPPictureImportBGRA(ref WebPPicture picture, nint bgra, int stride);

        /// <summary>
        /// Rescales the picture to new dimensions.
        /// Returns 0 on memory/parameter error.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureRescale")]
        public static extern int WebPPictureRescale(ref WebPPicture picture, int width, int height);

        /// <summary>
        /// Crops the picture to the given rectangle.
        /// Returns 0 on parameter error.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureCrop")]
        public static extern int WebPPictureCrop(
            ref WebPPicture picture, int left, int top, int width, int height);

        // ──────────────────────────────────────────────────────────────
        //  Advanced encoding API — Encode
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Encodes a WebPPicture using the given config.
        /// The picture.writer callback receives the encoded output.
        /// Returns 0 on error (check picture.error_code).
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncode")]
        public static extern int WebPEncode(ref WebPConfig config, ref WebPPicture picture);

        // ──────────────────────────────────────────────────────────────
        //  Memory Writer helpers
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Initializes a WebPMemoryWriter struct.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPMemoryWriterInit")]
        public static extern void WebPMemoryWriterInit(ref WebPMemoryWriter writer);

        // ──────────────────────────────────────────────────────────────
        //  Decoder API (for WebP → RGBA decoding)
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Decodes WebP data to RGBA pixels.
        /// Returns a pointer to the decoded buffer (caller must free with WebPFree).
        /// Sets width/height on success, returns IntPtr.Zero on failure.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeRGBA")]
        public static extern nint WebPDecodeRGBA(
            byte[] data, nuint data_size, out int width, out int height);

        /// <summary>
        /// Returns the decoder's version number as a packed int.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetDecoderVersion")]
        public static extern int WebPGetDecoderVersion();

        /// <summary>
        /// The built-in memory writer function. Can be used as the
        /// picture.writer callback via Marshal.GetFunctionPointerForDelegate.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPMemoryWrite")]
        public static extern int WebPMemoryWrite(nint data, nuint data_size, ref WebPPicture picture);

        /// <summary>
        /// Clears memory allocated by a WebPMemoryWriter.
        /// </summary>
        [DllImport(LibWebP, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPMemoryWriterClear")]
        public static extern void WebPMemoryWriterClear(ref WebPMemoryWriter writer);

        // ──────────────────────────────────────────────────────────────
        //  Helper: WEBP_ENCODER_ABI_VERSION
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// ABI version for WebP encoder API (0x020f for 1.5.x, etc.).
        /// Passed to WebPConfigInitInternal / WebPPictureInitInternal.
        /// </summary>
        public const int WEBP_ENCODER_ABI_VERSION = 0x020f;

        // ──────────────────────────────────────────────────────────────
        //  Convenience wrappers (match the C macros)
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Initializes a WebPConfig with a preset and quality, using the
        /// current ABI version. Returns false on mismatch.
        /// </summary>
        public static bool WebPConfigPreset(ref WebPConfig config, WebPPreset preset, float quality)
        {
            return WebPConfigInitInternal(ref config, preset, quality, WEBP_ENCODER_ABI_VERSION) != 0;
        }

        /// <summary>
        /// Initializes a WebPConfig with default values (preset=Default, quality=75).
        /// Returns false on mismatch.
        /// </summary>
        public static bool WebPConfigInit(ref WebPConfig config)
        {
            return WebPConfigPreset(ref config, WebPPreset.Default, 75f);
        }

        /// <summary>
        /// Initializes a WebPPicture with defaults.
        /// Returns false on version mismatch.
        /// </summary>
        public static bool WebPPictureInit(ref WebPPicture picture)
        {
            return WebPPictureInitInternal(ref picture, WEBP_ENCODER_ABI_VERSION) != 0;
        }
    }
}

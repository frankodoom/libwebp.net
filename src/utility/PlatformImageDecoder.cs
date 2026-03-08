using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Libwebp.Net.Interop;

namespace Libwebp.Net.utility
{
    /// <summary>
    /// Decodes common image formats (JPEG, PNG, BMP, GIF, TIFF) to raw BGRA
    /// pixel data using the platform's native imaging APIs — no third-party
    /// packages required. On Windows, uses GDI+ (gdiplus.dll) via P/Invoke.
    /// </summary>
    internal static class PlatformImageDecoder
    {
        // ── Public API ─────────────────────────────────────────────

        /// <summary>
        /// Result of decoding an image file.
        /// </summary>
        internal readonly struct DecodedImage
        {
            /// <summary>Pixel data (4 bytes per pixel).</summary>
            public readonly byte[] Pixels;
            /// <summary>Image width in pixels.</summary>
            public readonly int Width;
            /// <summary>Image height in pixels.</summary>
            public readonly int Height;
            /// <summary>True if pixel order is BGRA (from GDI+), false if RGBA (from libwebp).</summary>
            public readonly bool IsBgra;

            public DecodedImage(byte[] pixels, int width, int height, bool isBgra = true)
            {
                Pixels = pixels;
                Width = width;
                Height = height;
                IsBgra = isBgra;
            }
        }

        /// <summary>
        /// Returns true if the byte array begins with a recognized image
        /// file signature (JPEG, PNG, BMP, GIF, TIFF).
        /// </summary>
        internal static bool IsEncodedImage(byte[] data)
        {
            if (data == null || data.Length < 4) return false;

            // JPEG: FF D8 FF
            if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF) return true;
            // PNG: 89 50 4E 47
            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47) return true;
            // BMP: 42 4D
            if (data[0] == 0x42 && data[1] == 0x4D) return true;
            // GIF: 47 49 46
            if (data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46) return true;
            // TIFF little-endian: 49 49 2A 00
            if (data[0] == 0x49 && data[1] == 0x49 && data[2] == 0x2A && data[3] == 0x00) return true;
            // TIFF big-endian: 4D 4D 00 2A
            if (data[0] == 0x4D && data[1] == 0x4D && data[2] == 0x00 && data[3] == 0x2A) return true;
            // WebP (RIFF): 52 49 46 46 ... 57 45 42 50
            if (data.Length >= 12 &&
                data[0] == 0x52 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x46 &&
                data[8] == 0x57 && data[9] == 0x45 && data[10] == 0x42 && data[11] == 0x50) return true;

            return false;
        }

        /// <summary>
        /// Decodes an encoded image (JPEG, PNG, BMP, GIF, TIFF) to raw BGRA
        /// pixel data using the platform's native imaging API.
        /// </summary>
        /// <param name="imageData">The encoded image file bytes.</param>
        /// <returns>Decoded pixel data, width, and height.</returns>
        /// <exception cref="PlatformNotSupportedException">
        /// Thrown on non-Windows platforms. Use Encode(byte[], int, int) with
        /// pre-decoded RGBA data instead.
        /// </exception>
        internal static DecodedImage Decode(byte[] imageData)
        {
            if (imageData == null) throw new ArgumentNullException(nameof(imageData));
            if (imageData.Length == 0) throw new ArgumentException("Image data is empty.", nameof(imageData));

            // WebP input — decode using libwebp's own decoder (cross-platform).
            if (IsWebP(imageData))
            {
                return DecodeWebP(imageData);
            }

            // All other formats — use GDI+ on Windows.
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException(
                    "Automatic image decoding for non-WebP formats is currently supported on Windows only. " +
                    "On other platforms, decode the image to RGBA pixels manually " +
                    "and call Encode(byte[] rgba, int width, int height) directly.");
            }

            return DecodeWithGdiPlus(imageData);
        }

        /// <summary>
        /// Returns true if the data starts with the WebP (RIFF/WEBP) signature.
        /// </summary>
        private static bool IsWebP(byte[] data)
        {
            return data.Length >= 12 &&
                   data[0] == 0x52 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x46 &&
                   data[8] == 0x57 && data[9] == 0x45 && data[10] == 0x42 && data[11] == 0x50;
        }

        /// <summary>
        /// Decodes a WebP image to RGBA pixel data using libwebp's native decoder.
        /// Works on all platforms (same native library used for encoding).
        /// </summary>
        private static DecodedImage DecodeWebP(byte[] webpData)
        {
            LibWebPResolver.EnsureRegistered();

            nint pixelPtr = LibWebPNative.WebPDecodeRGBA(
                webpData, (nuint)webpData.Length, out int width, out int height);

            if (pixelPtr == nint.Zero)
                throw new InvalidOperationException("Failed to decode WebP image data.");

            try
            {
                int pixelCount = width * height * 4;
                var pixels = new byte[pixelCount];
                Marshal.Copy(pixelPtr, pixels, 0, pixelCount);
                // WebPDecodeRGBA returns RGBA, not BGRA — mark accordingly
                return new DecodedImage(pixels, width, height, isBgra: false);
            }
            finally
            {
                LibWebPNative.WebPFree(pixelPtr);
            }
        }

        // ── GDI+ implementation (Windows) ──────────────────────────

        /// <summary>
        /// Lazy GDI+ initialization token. Initialized once per process,
        /// never shut down (the OS reclaims on process exit).
        /// </summary>
        private static readonly Lazy<nint> _gdipToken = new(() =>
        {
            var input = new GdipStartupInput { GdiplusVersion = 1 };
            int status = GdiplusStartup(out nint token, ref input, nint.Zero);
            if (status != 0)
                throw new InvalidOperationException($"Failed to initialize GDI+. Status: {status}");
            return token;
        });

        [SupportedOSPlatform("windows")]
        private static DecodedImage DecodeWithGdiPlus(byte[] imageData)
        {
            // Ensure GDI+ is initialized
            _ = _gdipToken.Value;

            nint hGlobal = nint.Zero;
            nint comStream = nint.Zero;
            nint bitmap = nint.Zero;

            try
            {
                // Allocate moveable global memory and copy the image data into it.
                // CreateStreamOnHGlobal requires GlobalAlloc'd memory (not Marshal.AllocHGlobal
                // which uses LocalAlloc).
                hGlobal = GlobalAlloc(GMEM_MOVEABLE, (nuint)imageData.Length);
                if (hGlobal == nint.Zero)
                    throw new OutOfMemoryException("Failed to allocate global memory for image data.");

                nint pGlobal = GlobalLock(hGlobal);
                Marshal.Copy(imageData, 0, pGlobal, imageData.Length);
                GlobalUnlock(hGlobal);

                // Create a COM IStream backed by the global memory.
                // fDeleteOnRelease = true  →  the stream owns the memory and frees it.
                int hr = CreateStreamOnHGlobal(hGlobal, true, out comStream);
                if (hr != 0)
                    throw new InvalidOperationException($"CreateStreamOnHGlobal failed. HRESULT: 0x{hr:X8}");
                hGlobal = nint.Zero; // ownership transferred to the COM stream

                // Load image from the COM IStream
                int status = GdipCreateBitmapFromStream(comStream, out bitmap);
                if (status != 0)
                {
                    throw new InvalidOperationException(
                        $"GDI+ failed to decode image (status {status}). " +
                        "Ensure the file is a supported image format (JPEG, PNG, BMP, GIF, TIFF).");
                }

                // Read dimensions
                GdipGetImageWidth(bitmap, out int width);
                GdipGetImageHeight(bitmap, out int height);

                if (width <= 0 || height <= 0)
                    throw new InvalidOperationException($"Invalid image dimensions: {width}x{height}.");

                // Lock the bitmap in PixelFormat32bppARGB (0x26200A).
                // On little-endian x86/x64, this gives us BGRA byte order in memory,
                // which is exactly what WebPPictureImportBGRA expects.
                var rect = new GdipRect { X = 0, Y = 0, Width = width, Height = height };
                var bitmapData = new GdipBitmapData();

                status = GdipBitmapLockBits(bitmap, ref rect,
                    ImageLockModeRead, PixelFormat32bppARGB, ref bitmapData);
                if (status != 0)
                    throw new InvalidOperationException($"GdipBitmapLockBits failed (status {status}).");

                try
                {
                    int pixelCount = width * height * 4;
                    var pixels = new byte[pixelCount];

                    // Copy row by row because stride may be larger than width*4
                    // due to DWORD alignment.
                    int rowBytes = width * 4;
                    for (int y = 0; y < height; y++)
                    {
                        nint srcRow = bitmapData.Scan0 + y * bitmapData.Stride;
                        Marshal.Copy(srcRow, pixels, y * rowBytes, rowBytes);
                    }

                    return new DecodedImage(pixels, width, height);
                }
                finally
                {
                    GdipBitmapUnlockBits(bitmap, ref bitmapData);
                }
            }
            finally
            {
                if (bitmap != nint.Zero)
                    GdipDisposeImage(bitmap);
                if (comStream != nint.Zero)
                    Marshal.Release(comStream);
                if (hGlobal != nint.Zero)
                    GlobalFree(hGlobal);
            }
        }

        // ── Constants ──────────────────────────────────────────────

        private const uint GMEM_MOVEABLE = 0x0002;
        private const int ImageLockModeRead = 1;
        private const int PixelFormat32bppARGB = 0x26200A;

        // ── P/Invoke: GDI+ (gdiplus.dll) ───────────────────────────

        [DllImport("gdiplus.dll")]
        private static extern int GdiplusStartup(out nint token, ref GdipStartupInput input, nint output);

        [DllImport("gdiplus.dll")]
        private static extern int GdipCreateBitmapFromStream(nint stream, out nint bitmap);

        [DllImport("gdiplus.dll")]
        private static extern int GdipGetImageWidth(nint image, out int width);

        [DllImport("gdiplus.dll")]
        private static extern int GdipGetImageHeight(nint image, out int height);

        [DllImport("gdiplus.dll")]
        private static extern int GdipBitmapLockBits(
            nint bitmap, ref GdipRect rect, int flags, int format, ref GdipBitmapData data);

        [DllImport("gdiplus.dll")]
        private static extern int GdipBitmapUnlockBits(nint bitmap, ref GdipBitmapData data);

        [DllImport("gdiplus.dll")]
        private static extern int GdipDisposeImage(nint image);

        // ── P/Invoke: COM stream (ole32.dll) ───────────────────────

        [DllImport("ole32.dll")]
        private static extern int CreateStreamOnHGlobal(
            nint hGlobal, [MarshalAs(UnmanagedType.Bool)] bool fDeleteOnRelease, out nint ppstm);

        // ── P/Invoke: Global memory (kernel32.dll) ─────────────────

        [DllImport("kernel32.dll")]
        private static extern nint GlobalAlloc(uint flags, nuint bytes);

        [DllImport("kernel32.dll")]
        private static extern nint GlobalLock(nint hMem);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(nint hMem);

        [DllImport("kernel32.dll")]
        private static extern nint GlobalFree(nint hMem);

        // ── Structs ────────────────────────────────────────────────

        [StructLayout(LayoutKind.Sequential)]
        private struct GdipStartupInput
        {
            public int GdiplusVersion;
            public nint DebugEventCallback;
            public int SuppressBackgroundThread;
            public int SuppressExternalCodecs;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GdipRect
        {
            public int X, Y, Width, Height;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GdipBitmapData
        {
            public int Width;
            public int Height;
            public int Stride;
            public int PixelFormat;
            public nint Scan0;
            public nint Reserved;
        }
    }
}

using Libwebp.Net;
using Libwebp.Net.utility;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Libwebp.Net.Tests
{
    /// <summary>
    /// Integration tests that call into the native libwebp library.
    /// These tests verify the full P/Invoke chain: C# → DllImport → libwebp.dll.
    /// They require the native shared library to be present in the build output.
    /// </summary>
    public class NativeEncoderIntegrationTests
    {
        // ── Version check ──

        [Fact]
        public void GetNativeVersion_ReturnsValidVersionString()
        {
            string version = WebpEncoder.GetNativeVersion();

            Assert.False(string.IsNullOrWhiteSpace(version));
            // Should be in "major.minor.revision" format
            var parts = version.Split('.');
            Assert.Equal(3, parts.Length);
            Assert.True(int.TryParse(parts[0], out int major));
            Assert.True(int.TryParse(parts[1], out _));
            Assert.True(int.TryParse(parts[2], out _));
            // We built from v1.5.0, so major should be at least 1
            Assert.True(major >= 1, $"Expected major version >= 1, got {major}");
        }

        // ── Simple lossy encoding (2x2 red pixel image) ──

        [Fact]
        public void Encode_2x2_RedImage_ReturnsValidWebP()
        {
            // Create a 2x2 solid red RGBA image (4 pixels × 4 bytes = 16 bytes)
            byte[] rgba = CreateSolidRgba(2, 2, r: 255, g: 0, b: 0, a: 255);

            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(75f)
                .Build();
            var encoder = new WebpEncoder(config);

            byte[] webpData = encoder.Encode(rgba, 2, 2);

            Assert.NotNull(webpData);
            Assert.True(webpData.Length > 0, "WebP output should not be empty");
            // WebP files start with "RIFF" signature
            Assert.Equal((byte)'R', webpData[0]);
            Assert.Equal((byte)'I', webpData[1]);
            Assert.Equal((byte)'F', webpData[2]);
            Assert.Equal((byte)'F', webpData[3]);
            // Bytes 8-11 should be "WEBP"
            Assert.Equal((byte)'W', webpData[8]);
            Assert.Equal((byte)'E', webpData[9]);
            Assert.Equal((byte)'B', webpData[10]);
            Assert.Equal((byte)'P', webpData[11]);
        }

        // ── Simple lossless encoding ──

        [Fact]
        public void Encode_Lossless_2x2_BlueImage_ReturnsValidWebP()
        {
            byte[] rgba = CreateSolidRgba(2, 2, r: 0, g: 0, b: 255, a: 255);

            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Lossless()
                .Build();
            var encoder = new WebpEncoder(config);

            byte[] webpData = encoder.Encode(rgba, 2, 2);

            Assert.NotNull(webpData);
            Assert.True(webpData.Length > 0);
            AssertWebPSignature(webpData);
        }

        // ── Larger image encoding ──

        [Fact]
        public void Encode_64x64_GradientImage_ReturnsValidWebP()
        {
            int width = 64, height = 64;
            byte[] rgba = CreateGradientRgba(width, height);

            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(80f)
                .Build();
            var encoder = new WebpEncoder(config);

            byte[] webpData = encoder.Encode(rgba, width, height);

            Assert.NotNull(webpData);
            Assert.True(webpData.Length > 0);
            AssertWebPSignature(webpData);
            // A 64x64 lossy WebP should be significantly smaller than raw RGBA
            Assert.True(webpData.Length < rgba.Length,
                $"WebP ({webpData.Length}) should be smaller than raw RGBA ({rgba.Length})");
        }

        // ── Async encoding ──

        [Fact]
        public async Task EncodeAsync_ByteArray_ReturnsValidWebP()
        {
            int width = 16, height = 16;
            byte[] rgba = CreateSolidRgba(width, height, r: 128, g: 200, b: 50, a: 255);

            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(90f)
                .Build();
            var encoder = new WebpEncoder(config);

            byte[] webpData = await encoder.EncodeAsync(rgba, width, height);

            Assert.NotNull(webpData);
            Assert.True(webpData.Length > 0);
            AssertWebPSignature(webpData);
        }

        // ── MemoryStream overload with InputSize ──

        [Fact]
        public async Task EncodeAsync_MemoryStream_WithInputSize_ReturnsValidWebP()
        {
            int width = 8, height = 8;
            byte[] rgba = CreateSolidRgba(width, height, r: 100, g: 100, b: 100, a: 255);

            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(75f)
                .InputSize(width, height)
                .Build();
            var encoder = new WebpEncoder(config);

            using var ms = new MemoryStream(rgba);
            using var result = await encoder.EncodeAsync(ms, "test.png");

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            var webpData = result.ToArray();
            AssertWebPSignature(webpData);
        }

        // ── Quality comparison ──

        [Fact]
        public void Encode_LowerQuality_ProducesSmallerOutput()
        {
            int width = 32, height = 32;
            byte[] rgba = CreateGradientRgba(width, height);

            var highConfig = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(95f)
                .Build();
            var lowConfig = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(10f)
                .Build();

            byte[] highQuality = new WebpEncoder(highConfig).Encode(rgba, width, height);
            byte[] lowQuality = new WebpEncoder(lowConfig).Encode(rgba, width, height);

            Assert.True(lowQuality.Length <= highQuality.Length,
                $"Low quality ({lowQuality.Length}) should be <= high quality ({highQuality.Length})");
        }

        // ── Helpers ──

        private static byte[] CreateSolidRgba(int width, int height, byte r, byte g, byte b, byte a)
        {
            byte[] rgba = new byte[width * height * 4];
            for (int i = 0; i < rgba.Length; i += 4)
            {
                rgba[i] = r;
                rgba[i + 1] = g;
                rgba[i + 2] = b;
                rgba[i + 3] = a;
            }
            return rgba;
        }

        private static byte[] CreateGradientRgba(int width, int height)
        {
            byte[] rgba = new byte[width * height * 4];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = (y * width + x) * 4;
                    rgba[i] = (byte)(x * 255 / Math.Max(width - 1, 1));     // R gradient
                    rgba[i + 1] = (byte)(y * 255 / Math.Max(height - 1, 1)); // G gradient
                    rgba[i + 2] = 128;                                        // B constant
                    rgba[i + 3] = 255;                                        // A opaque
                }
            }
            return rgba;
        }

        private static void AssertWebPSignature(byte[] data)
        {
            Assert.True(data.Length >= 12, "WebP data too short for header");
            // RIFF header
            Assert.Equal((byte)'R', data[0]);
            Assert.Equal((byte)'I', data[1]);
            Assert.Equal((byte)'F', data[2]);
            Assert.Equal((byte)'F', data[3]);
            // WEBP signature
            Assert.Equal((byte)'W', data[8]);
            Assert.Equal((byte)'E', data[9]);
            Assert.Equal((byte)'B', data[10]);
            Assert.Equal((byte)'P', data[11]);
        }
    }
}

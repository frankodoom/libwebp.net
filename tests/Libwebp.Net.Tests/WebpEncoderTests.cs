using Libwebp.Net;
using Libwebp.Net.errors;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Libwebp.Net.Tests
{
    public class WebpEncoderTests
    {
        // ── EncodeAsync(MemoryStream, string) validation ──

        [Fact]
        public async Task EncodeAsync_NullMemoryStream_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => encoder.EncodeAsync((MemoryStream)null!, "test.png"));
        }

        [Fact]
        public async Task EncodeAsync_NullFileName_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => encoder.EncodeAsync(new MemoryStream(), null!));
        }

        [Fact]
        public async Task EncodeAsync_EmptyFileName_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => encoder.EncodeAsync(new MemoryStream(), ""));
        }

        [Fact]
        public async Task EncodeAsync_NoOutputConfigured_ThrowsOutputFileNameNotFoundException()
        {
            var config = new WebpConfigurationBuilder().Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<OutputFileNameNotFoundException>(
                () => encoder.EncodeAsync(new MemoryStream(new byte[] { 1, 2, 3 }), "test.png"));
        }

        [Fact]
        public async Task EncodeAsync_NoInputSize_ThrowsWebPEncodingException()
        {
            // When using the MemoryStream overload, InputSize must be configured
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<WebPEncodingException>(
                () => encoder.EncodeAsync(new MemoryStream(new byte[] { 1, 2, 3 }), "test.png"));
        }

        [Fact]
        public async Task EncodeAsync_WrongDataSize_ThrowsWebPEncodingException()
        {
            // InputSize says 2x2 (expects 16 bytes), but we only provide 3 bytes
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .InputSize(2, 2)
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<WebPEncodingException>(
                () => encoder.EncodeAsync(new MemoryStream(new byte[] { 1, 2, 3 }), "test.png"));
        }

        // ── Encode(byte[], int, int) validation ──

        [Fact]
        public void Encode_NullRgba_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            Assert.Throws<ArgumentNullException>(() => encoder.Encode(null!, 1, 1));
        }

        [Fact]
        public void Encode_ZeroWidth_ThrowsArgumentException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            Assert.Throws<ArgumentException>(() => encoder.Encode(new byte[4], 0, 1));
        }

        [Fact]
        public void Encode_ZeroHeight_ThrowsArgumentException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            Assert.Throws<ArgumentException>(() => encoder.Encode(new byte[4], 1, 0));
        }

        // ── EncodeAsync(byte[], int, int) validation ──

        [Fact]
        public async Task EncodeAsync_ByteArray_NullRgba_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => encoder.EncodeAsync(null!, 1, 1));
        }

        [Fact]
        public async Task EncodeAsync_ByteArray_ZeroWidth_ThrowsArgumentException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentException>(
                () => encoder.EncodeAsync(new byte[4], 0, 1));
        }

        // ── Constructor validation ──

        [Fact]
        public void Constructor_NullConfiguration_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WebpEncoder(null!));
        }
    }
}

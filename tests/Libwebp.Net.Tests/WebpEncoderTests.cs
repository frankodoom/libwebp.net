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
        [Fact]
        public async Task EncodeAsync_NullMemoryStream_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => encoder.EncodeAsync(null, "test.png"));
        }

        [Fact]
        public async Task EncodeAsync_NullFileName_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();
            var encoder = new WebpEncoder(config);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => encoder.EncodeAsync(new MemoryStream(), null));
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
        public void Constructor_NullConfiguration_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WebpEncoder(null));
        }
    }
}

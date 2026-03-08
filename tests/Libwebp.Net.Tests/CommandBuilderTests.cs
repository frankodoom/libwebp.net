using Libwebp.Net;
using Libwebp.Net.Interop;
using Libwebp.Net.utility;
using System;
using Xunit;

namespace Libwebp.Net.Tests
{
    /// <summary>
    /// Tests for the configuration-to-native-struct conversion and
    /// NativeEncoder argument validation. These tests do NOT require
    /// the native libwebp shared library to be present — they verify
    /// the managed wrapper logic only.
    /// </summary>
    public class NativeEncoderTests
    {
        // ── NativeEncoder.EncodeSimple argument validation ──

        [Fact]
        public void EncodeSimple_NullRgba_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NativeEncoder.EncodeSimple(null!, 1, 1, 75f, false));
        }

        [Fact]
        public void EncodeSimple_ZeroWidth_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                NativeEncoder.EncodeSimple(new byte[4], 0, 1, 75f, false));
        }

        [Fact]
        public void EncodeSimple_ZeroHeight_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                NativeEncoder.EncodeSimple(new byte[4], 1, 0, 75f, false));
        }

        [Fact]
        public void EncodeSimple_NegativeWidth_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                NativeEncoder.EncodeSimple(new byte[4], -1, 1, 75f, false));
        }

        // ── NativeEncoder.EncodeAdvanced argument validation ──

        [Fact]
        public void EncodeAdvanced_NullRgba_ThrowsArgumentNullException()
        {
            var config = new WebPConfig();
            Assert.Throws<ArgumentNullException>(() =>
                NativeEncoder.EncodeAdvanced(null!, 1, 1, config));
        }

        [Fact]
        public void EncodeAdvanced_ZeroWidth_ThrowsArgumentException()
        {
            var config = new WebPConfig();
            Assert.Throws<ArgumentException>(() =>
                NativeEncoder.EncodeAdvanced(new byte[4], 0, 1, config));
        }

        [Fact]
        public void EncodeAdvanced_NegativeHeight_ThrowsArgumentException()
        {
            var config = new WebPConfig();
            Assert.Throws<ArgumentException>(() =>
                NativeEncoder.EncodeAdvanced(new byte[4], 1, -1, config));
        }

        // ── NativeEncoder.GetErrorMessage ──

        [Fact]
        public void GetErrorMessage_Ok_ReturnsNoError()
        {
            var msg = NativeEncoder.GetErrorMessage(WebPEncodingError.Ok);
            Assert.Contains("No error", msg);
        }

        [Fact]
        public void GetErrorMessage_OutOfMemory_ReturnsMemoryMessage()
        {
            var msg = NativeEncoder.GetErrorMessage(WebPEncodingError.OutOfMemory);
            Assert.Contains("Memory", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetErrorMessage_BadDimension_ReturnsDimensionMessage()
        {
            var msg = NativeEncoder.GetErrorMessage(WebPEncodingError.BadDimension);
            Assert.Contains("width/height", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetErrorMessage_InvalidConfiguration_ReturnsConfigMessage()
        {
            var msg = NativeEncoder.GetErrorMessage(WebPEncodingError.InvalidConfiguration);
            Assert.Contains("invalid", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetErrorMessage_FileTooBig_ReturnsSizeMessage()
        {
            var msg = NativeEncoder.GetErrorMessage(WebPEncodingError.FileTooBig);
            Assert.Contains("too big", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetErrorMessage_UnknownValue_ReturnsUnknown()
        {
            var msg = NativeEncoder.GetErrorMessage((WebPEncodingError)999);
            Assert.Contains("Unknown", msg, StringComparison.OrdinalIgnoreCase);
        }

        // ── WebPConfig struct default values ──

        [Fact]
        public void WebPConfig_DefaultStruct_HasZeroValues()
        {
            var config = new WebPConfig();
            Assert.Equal(0f, config.quality);
            Assert.Equal(0, config.method);
            Assert.Equal(0, config.lossless);
        }

        // ── WebPPicture struct layout ──

        [Fact]
        public void WebPPicture_DefaultStruct_HasZeroDimensions()
        {
            var picture = new WebPPicture();
            Assert.Equal(0, picture.width);
            Assert.Equal(0, picture.height);
            Assert.Equal(0, picture.use_argb);
        }

        // ── WebPMemoryWriter struct layout ──

        [Fact]
        public void WebPMemoryWriter_DefaultStruct_IsEmpty()
        {
            var writer = new WebPMemoryWriter();
            Assert.Equal(nint.Zero, writer.mem);
            Assert.Equal((nuint)0, writer.size);
        }
    }
}

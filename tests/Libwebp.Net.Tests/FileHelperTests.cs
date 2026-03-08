using Libwebp.Net.utility;
using System;
using Xunit;

namespace Libwebp.Net.Tests
{
    public class FileHelperTests
    {
        [Fact]
        public void SanitizeFileName_ValidName_ReturnsSameName()
        {
            var result = FileHelper.SanitizeFileName("image.png");
            Assert.Equal("image.png", result);
        }

        [Fact]
        public void SanitizeFileName_WithPathComponents_ReturnsFileNameOnly()
        {
            var result = FileHelper.SanitizeFileName(@"C:\Users\test\images\photo.jpg");
            Assert.Equal("photo.jpg", result);
        }

        [Fact]
        public void SanitizeFileName_WithUnixPath_ReturnsFileNameOnly()
        {
            var result = FileHelper.SanitizeFileName("/home/user/images/photo.jpg");
            Assert.Equal("photo.jpg", result);
        }

        [Fact]
        public void SanitizeFileName_Null_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileHelper.SanitizeFileName(null));
        }

        [Fact]
        public void SanitizeFileName_Empty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileHelper.SanitizeFileName(""));
        }

        [Fact]
        public void SanitizeFileName_WhitespaceOnly_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileHelper.SanitizeFileName("   "));
        }

        [Fact]
        public void SanitizeFileName_WithSpaces_PreservesSpaces()
        {
            var result = FileHelper.SanitizeFileName("my image file.png");
            Assert.Equal("my image file.png", result);
        }

        [Fact]
        public void SanitizeFileName_WebpExtension_Works()
        {
            var result = FileHelper.SanitizeFileName("output.webp");
            Assert.Equal("output.webp", result);
        }
    }
}

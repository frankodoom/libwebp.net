using Libwebp.Net;
using Libwebp.Net.errors;
using Libwebp.Net.Interop;
using Libwebp.Net.utility;
using Xunit;

namespace Libwebp.Net.Tests
{
    public class WebpConfigurationBuilderTests
    {
        [Fact]
        public void Build_WithOutput_SetsOutputFileName()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Build();

            Assert.Equal("test.webp", config.OutputFileName);
        }

        [Fact]
        public void Output_WithNull_ThrowsOutputFileNameNotFoundException()
        {
            var builder = new WebpConfigurationBuilder();

            Assert.Throws<OutputFileNameNotFoundException>(() => builder.Output(null));
        }

        [Fact]
        public void Output_WithEmpty_ThrowsOutputFileNameNotFoundException()
        {
            var builder = new WebpConfigurationBuilder();

            Assert.Throws<OutputFileNameNotFoundException>(() => builder.Output(""));
        }

        [Fact]
        public void Output_WithWhitespace_ThrowsOutputFileNameNotFoundException()
        {
            var builder = new WebpConfigurationBuilder();

            Assert.Throws<OutputFileNameNotFoundException>(() => builder.Output("   "));
        }

        [Fact]
        public void Build_WithQualityFactor_SetsNativeQuality()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(80)
                .Build();

            Assert.Equal(80f, config.NativeQuality);
        }

        [Fact]
        public void Build_WithPreset_SetsNativePreset()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Preset(Preset.PHOTO)
                .Build();

            Assert.Equal(WebPPreset.Photo, config.NativePreset);
        }

        [Fact]
        public void Build_WithLossless_SetsNativeLossless()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Lossless()
                .Build();

            Assert.True(config.NativeLossless);
        }

        [Fact]
        public void Build_WithAlphaQ_SetsNativeAlphaQuality()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .AlphaQ(50)
                .Build();

            Assert.Equal(50, config.NativeAlphaQuality);
        }

        [Fact]
        public void Build_WithCompressionMethod_SetsNativeMethod()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .CompressionMethod(4)
                .Build();

            Assert.Equal(4, config.NativeMethod);
        }

        [Fact]
        public void Build_WithNumberOfSegments_SetsNativeSegments()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NumberOfSegments(3)
                .Build();

            Assert.Equal(3, config.NativeSegments);
        }

        [Fact]
        public void Build_WithTargetSize_SetsNativeTargetSize()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .TargetSize(50000)
                .Build();

            Assert.Equal(50000, config.NativeTargetSize);
        }

        [Fact]
        public void Build_WithTargetPSNR_SetsNativeTargetPSNR()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .TargetPSNR(42.5f)
                .Build();

            Assert.Equal(42.5f, config.NativeTargetPSNR);
        }

        [Fact]
        public void Build_WithSpatialNoiseShaping_SetsNativeSnsStrength()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .SpatialNoiseShaping(80)
                .Build();

            Assert.Equal(80, config.NativeSnsStrength);
        }

        [Fact]
        public void Build_WithFilter_SetsNativeFilterStrength()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Filter(60)
                .Build();

            Assert.Equal(60, config.NativeFilterStrength);
        }

        [Fact]
        public void Build_WithInputSize_SetsInputSizeDimensions()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .InputSize(1920, 1080)
                .Build();

            Assert.Equal((1920, 1080), config.InputSizeDimensions);
        }

        [Fact]
        public void Build_WithLosslessPreset_SetsNativeLosslessAndMethod()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .LosslessPreset(6)
                .Build();

            Assert.True(config.NativeLossless);
            Assert.Equal(6, config.NativeLosslessPreset);
        }

        [Fact]
        public void Build_WithSharpness_SetsNativeFilterSharpness()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Sharpness(3)
                .Build();

            Assert.Equal(3, config.NativeFilterSharpness);
        }

        [Fact]
        public void Build_WithStrong_SetsNativeFilterType()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Strong()
                .Build();

            Assert.Equal(1, config.NativeFilterType);
        }

        [Fact]
        public void Build_WithNoStrong_SetsNativeFilterTypeToSimple()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NoStrong()
                .Build();

            Assert.Equal(0, config.NativeFilterType);
        }

        [Fact]
        public void Build_WithSharpYuv_SetsNativeSharpYuv()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .SharpYuv()
                .Build();

            Assert.True(config.NativeSharpYuv);
        }

        [Fact]
        public void Build_WithPartitionLimit_SetsNativePartitionLimit()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .PartitionLimit(50)
                .Build();

            Assert.Equal(50, config.NativePartitionLimit);
        }

        [Fact]
        public void Build_WithPass_SetsNativePass()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Pass(6)
                .Build();

            Assert.Equal(6, config.NativePass);
        }

        [Fact]
        public void Build_WithCrop_SetsCropRect()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Crop(10, 20, 300, 400)
                .Build();

            Assert.NotNull(config.CropRect);
            Assert.Equal((10, 20, 300, 400), config.CropRect.Value);
        }

        [Fact]
        public void Build_WithResize_SetsResizeDimensions()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Resize(640, 480)
                .Build();

            Assert.Equal((640, 480), config.ResizeDimensions);
        }

        [Fact]
        public void Build_WithMultiThreading_SetsNativeMultiThreading()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .MultiThreading()
                .Build();

            Assert.True(config.NativeMultiThreading);
        }

        [Fact]
        public void Build_WithLowMemory_SetsNativeLowMemory()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .LowMemory()
                .Build();

            Assert.True(config.NativeLowMemory);
        }

        [Fact]
        public void Build_WithAlphaMethod_SetsNativeAlphaCompression()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .AlphaMethod(1)
                .Build();

            Assert.Equal(1, config.NativeAlphaCompression);
        }

        [Fact]
        public void Build_WithAlphaFilter_SetsNativeAlphaFiltering()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .AlphaFilter("best")
                .Build();

            Assert.Equal(2, config.NativeAlphaFiltering); // best = 2
        }

        [Fact]
        public void Build_WithExact_SetsNativeExact()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Exact()
                .Build();

            Assert.True(config.NativeExact);
        }

        [Fact]
        public void Build_WithNoAlpha_SetsNativeNoAlpha()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NoAlpha()
                .Build();

            Assert.True(config.NativeNoAlpha);
        }

        [Fact]
        public void Build_WithNearLossless_SetsNativeNearLossless()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NearLossless(60)
                .Build();

            Assert.Equal(60, config.NativeNearLossless);
        }

        [Fact]
        public void Build_WithHint_SetsNativeImageHint()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Hint("photo")
                .Build();

            Assert.Equal(WebPImageHint.Photo, config.NativeImageHint);
        }

        [Fact]
        public void Build_WithMetadata_SetsMetadataOption()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Metadata("all")
                .Build();

            Assert.Equal("all", config.MetadataOption);
        }

        [Fact]
        public void Build_FluentChaining_SetsMultipleOptions()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Preset(Preset.PHOTO)
                .QualityFactor(85)
                .Lossless()
                .CompressionMethod(6)
                .AlphaQ(100)
                .Build();

            Assert.Equal("output.webp", config.OutputFileName);
            Assert.Equal(WebPPreset.Photo, config.NativePreset);
            Assert.Equal(85f, config.NativeQuality);
            Assert.True(config.NativeLossless);
            Assert.Equal(6, config.NativeMethod);
            Assert.Equal(100, config.NativeAlphaQuality);
        }

        [Fact]
        public void Build_DefaultValues_MatchLibwebpDefaults()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Build();

            Assert.Equal(75f, config.NativeQuality);
            Assert.Equal(4, config.NativeMethod);
            Assert.Equal(4, config.NativeSegments);
            Assert.Equal(50, config.NativeSnsStrength);
            Assert.Equal(1, config.NativeFilterType); // strong by default
            Assert.Equal(100, config.NativeAlphaQuality);
            Assert.Equal(1, config.NativePass);
            Assert.Equal(100, config.NativeNearLossless); // off
            Assert.False(config.NativeLossless);
            Assert.False(config.NativeMultiThreading);
            Assert.False(config.NativeLowMemory);
            Assert.False(config.NativeExact);
            Assert.False(config.NativeNoAlpha);
            Assert.False(config.NativeSharpYuv);
        }
    }
}

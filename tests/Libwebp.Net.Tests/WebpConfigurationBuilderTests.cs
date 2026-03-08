using Libwebp.Net;
using Libwebp.Net.errors;
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
        public void Build_WithQualityFactor_SetsQualityFactor()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .QualityFactor(80)
                .Build();

            Assert.Contains("-q ", config.QualityFactor);
            Assert.Contains("80", config.QualityFactor);
        }

        [Fact]
        public void Build_WithPreset_SetsPreset()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Preset(Preset.PHOTO)
                .Build();

            Assert.Contains("-preset ", config.Preset);
            Assert.Contains("photo", config.Preset);
        }

        [Fact]
        public void Build_WithLossless_SetsLossless()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Lossless()
                .Build();

            Assert.Contains("-lossless", config.Lossless);
        }

        [Fact]
        public void Build_WithAlphaQ_SetsAlphaQ()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .AlphaQ(50)
                .Build();

            Assert.Contains("-alpha_q ", config.AlphaQ);
            Assert.Contains("50", config.AlphaQ);
        }

        [Fact]
        public void Build_WithCompressionMethod_SetsCompressionMethod()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .CompressionMethod(4)
                .Build();

            Assert.Contains("-m ", config.CompressionMethod);
            Assert.Contains("4", config.CompressionMethod);
        }

        [Fact]
        public void Build_WithNumberOfSegments_SetsNumberOfSegments()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NumberOfSegments(3)
                .Build();

            Assert.Contains("-segments ", config.NumberOfSegments);
            Assert.Contains("3", config.NumberOfSegments);
        }

        [Fact]
        public void Build_WithTargetSize_SetsTargetSize()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .TargetSize(50000)
                .Build();

            Assert.Contains("-size ", config.TargetSize);
            Assert.Contains("50000", config.TargetSize);
        }

        [Fact]
        public void Build_WithTargetPSNR_SetsTargetPSNR()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .TargetPSNR(42.5f)
                .Build();

            Assert.Contains("-psnr ", config.TargetPSNR);
            Assert.Contains("42.5", config.TargetPSNR);
        }

        [Fact]
        public void Build_WithSpatialNoiseShaping_SetsSNS()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .SpatialNoiseShaping(80)
                .Build();

            Assert.Contains("-sns ", config.SpatialNoiseShaping);
            Assert.Contains("80", config.SpatialNoiseShaping);
        }

        [Fact]
        public void Build_WithFilter_SetsFilter()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Filter(60)
                .Build();

            Assert.Contains("-f ", config.Filter);
            Assert.Contains("60", config.Filter);
        }

        [Fact]
        public void Build_WithInputSize_SetsInputSize()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .InputSize(1920, 1080)
                .Build();

            Assert.Contains("-s ", config.InputSize);
            Assert.Contains("1920", config.InputSize);
            Assert.Contains("1080", config.InputSize);
        }

        [Fact]
        public void Build_WithLosslessPreset_SetsLosslessPreset()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .LosslessPreset(6)
                .Build();

            Assert.Contains("-z ", config.LosslessPreset);
            Assert.Contains("6", config.LosslessPreset);
        }

        [Fact]
        public void Build_WithSharpness_SetsSharpness()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Sharpness(3)
                .Build();

            Assert.Contains("-sharpness ", config.Sharpness);
            Assert.Contains("3", config.Sharpness);
        }

        [Fact]
        public void Build_WithStrong_SetsStrong()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Strong()
                .Build();

            Assert.Contains("-strong", config.Strong);
        }

        [Fact]
        public void Build_WithNoStrong_SetsNoStrong()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NoStrong()
                .Build();

            Assert.Contains("-nostrong", config.Strong);
        }

        [Fact]
        public void Build_WithSharpYuv_SetsSharpYuv()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .SharpYuv()
                .Build();

            Assert.Contains("-sharp_yuv", config.SharpYuv);
        }

        [Fact]
        public void Build_WithPartitionLimit_SetsPartitionLimit()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .PartitionLimit(50)
                .Build();

            Assert.Contains("-partition_limit ", config.PartitionLimit);
            Assert.Contains("50", config.PartitionLimit);
        }

        [Fact]
        public void Build_WithPass_SetsPass()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Pass(6)
                .Build();

            Assert.Contains("-pass ", config.Pass);
            Assert.Contains("6", config.Pass);
        }

        [Fact]
        public void Build_WithCrop_SetsCrop()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Crop(10, 20, 300, 400)
                .Build();

            Assert.Contains("-crop ", config.Crop);
            Assert.Contains("10 20 300 400", config.Crop);
        }

        [Fact]
        public void Build_WithResize_SetsResize()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Resize(640, 480)
                .Build();

            Assert.Contains("-resize ", config.Resize);
            Assert.Contains("640 480", config.Resize);
        }

        [Fact]
        public void Build_WithMultiThreading_SetsMultiThreading()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .MultiThreading()
                .Build();

            Assert.Contains("-mt", config.MultiThreading);
        }

        [Fact]
        public void Build_WithLowMemory_SetsLowMemory()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .LowMemory()
                .Build();

            Assert.Contains("-low_memory", config.LowMemory);
        }

        [Fact]
        public void Build_WithAlphaMethod_SetsAlphaMethod()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .AlphaMethod(1)
                .Build();

            Assert.Contains("-alpha_method ", config.AlphaMethod);
            Assert.Contains("1", config.AlphaMethod);
        }

        [Fact]
        public void Build_WithAlphaFilter_SetsAlphaFilter()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .AlphaFilter("best")
                .Build();

            Assert.Contains("-alpha_filter ", config.AlphaFilter);
            Assert.Contains("best", config.AlphaFilter);
        }

        [Fact]
        public void Build_WithExact_SetsExact()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Exact()
                .Build();

            Assert.Contains("-exact", config.Exact);
        }

        [Fact]
        public void Build_WithNoAlpha_SetsNoAlpha()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NoAlpha()
                .Build();

            Assert.Contains("-noalpha", config.NoAlpha);
        }

        [Fact]
        public void Build_WithNearLossless_SetsNearLossless()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .NearLossless(60)
                .Build();

            Assert.Contains("-near_lossless ", config.NearLossless);
            Assert.Contains("60", config.NearLossless);
        }

        [Fact]
        public void Build_WithHint_SetsHint()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Hint("photo")
                .Build();

            Assert.Contains("-hint ", config.Hint);
            Assert.Contains("photo", config.Hint);
        }

        [Fact]
        public void Build_WithMetadata_SetsMetadata()
        {
            var config = new WebpConfigurationBuilder()
                .Output("test.webp")
                .Metadata("all")
                .Build();

            Assert.Contains("-metadata ", config.Metadata);
            Assert.Contains("all", config.Metadata);
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
            Assert.NotNull(config.Preset);
            Assert.NotNull(config.QualityFactor);
            Assert.NotNull(config.Lossless);
            Assert.NotNull(config.CompressionMethod);
            Assert.NotNull(config.AlphaQ);
        }
    }
}

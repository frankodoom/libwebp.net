using Libwebp.Net;
using Libwebp.Net.utility;
using System;
using Xunit;

namespace Libwebp.Net.Tests
{
    public class CommandBuilderTests
    {
        [Fact]
        public void Constructor_NullConfiguration_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CommandBuilder(null, "input.png", "output.webp"));
        }

        [Fact]
        public void Constructor_NullInputPath_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();

            Assert.Throws<ArgumentNullException>(() =>
                new CommandBuilder(config, null, "output.webp"));
        }

        [Fact]
        public void Constructor_NullOutputPath_ThrowsArgumentNullException()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();

            Assert.Throws<ArgumentNullException>(() =>
                new CommandBuilder(config, "input.png", null));
        }

        [Fact]
        public void GetCommand_BasicConfig_ContainsInputAndOutput()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            // Arguments should not contain a "cwebp" prefix — the binary path is set
            // in ProcessStartInfo.FileName, so Arguments are just the cwebp flags.
            Assert.DoesNotContain("cwebp ", command);
            Assert.Contains(@"C:\temp\input.png", command);
            Assert.Contains("-o ", command);
        }

        [Fact]
        public void GetCommand_BasicConfig_ContainsInputAndOutputPaths()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();

            var inputPath = @"C:\temp\input.png";
            var outputPath = @"C:\temp\output.webp";

            var builder = new CommandBuilder(config, inputPath, outputPath);
            var command = builder.GetCommand();

            Assert.Contains(inputPath, command);
            Assert.Contains(outputPath, command);
            Assert.Contains("-o ", command);
        }

        [Fact]
        public void GetCommand_WithPreset_ContainsPresetArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Preset(Preset.PHOTO)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-preset photo", command);
        }

        [Fact]
        public void GetCommand_WithLossless_ContainsLosslessArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Lossless()
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-lossless", command);
        }

        [Fact]
        public void GetCommand_WithQualityFactor_ContainsQualityArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .QualityFactor(85)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-q 85", command);
        }

        [Fact]
        public void GetCommand_WithAllOptions_ContainsAllArgs()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Preset(Preset.PHOTO)
                .Lossless()
                .QualityFactor(90)
                .AlphaQ(100)
                .CompressionMethod(6)
                .NumberOfSegments(4)
                .TargetSize(50000)
                .SpatialNoiseShaping(80)
                .Filter(60)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-preset photo", command);
            Assert.Contains("-lossless", command);
            Assert.Contains("-q 90", command);
            Assert.Contains("-alpha_q 100", command);
            Assert.Contains("-m 6", command);
            Assert.Contains("-segments 4", command);
            Assert.Contains("-size 50000", command);
            Assert.Contains("-sns 80", command);
            Assert.Contains("-f 60", command);
            Assert.Contains("-o ", command);
        }

        [Fact]
        public void GetCommand_WithSharpness_ContainsSharpnessArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Sharpness(5)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-sharpness 5", command);
        }

        [Fact]
        public void GetCommand_WithMultiThreading_ContainsMtArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .MultiThreading()
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-mt", command);
        }

        [Fact]
        public void GetCommand_WithCrop_ContainsCropArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Crop(10, 20, 300, 400)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-crop 10 20 300 400", command);
        }

        [Fact]
        public void GetCommand_WithResize_ContainsResizeArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Resize(800, 600)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-resize 800 600", command);
        }

        [Fact]
        public void GetCommand_WithNearLossless_ContainsNearLosslessArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .NearLossless(80)
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-near_lossless 80", command);
        }

        [Fact]
        public void GetCommand_WithMetadata_ContainsMetadataArg()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Metadata("exif,icc")
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-metadata exif,icc", command);
        }

        [Fact]
        public void GetCommand_WithNewOptions_ContainsAllNewArgs()
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .LosslessPreset(6)
                .SharpYuv()
                .Pass(10)
                .Strong()
                .PartitionLimit(50)
                .LowMemory()
                .AlphaMethod(1)
                .AlphaFilter("best")
                .Exact()
                .NoAlpha()
                .Hint("photo")
                .Build();

            var builder = new CommandBuilder(config, @"C:\temp\input.png", @"C:\temp\output.webp");
            var command = builder.GetCommand();

            Assert.Contains("-z 6", command);
            Assert.Contains("-sharp_yuv", command);
            Assert.Contains("-pass 10", command);
            Assert.Contains("-strong", command);
            Assert.Contains("-partition_limit 50", command);
            Assert.Contains("-low_memory", command);
            Assert.Contains("-alpha_method 1", command);
            Assert.Contains("-alpha_filter best", command);
            Assert.Contains("-exact", command);
            Assert.Contains("-noalpha", command);
            Assert.Contains("-hint photo", command);
        }
    }
}

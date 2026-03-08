using Libwebp.Net;
using Libwebp.Net.utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // ── Resolve input file ─────────────────────────────────────
            string inputPath;
            if (args.Length > 0 && File.Exists(args[0]))
            {
                inputPath = args[0];
            }
            else
            {
                // Default: look for logo.png next to the executable
                inputPath = Path.Combine(AppContext.BaseDirectory, "logo.png");
                if (!File.Exists(inputPath))
                {
                    Console.WriteLine("Usage: console <input-image-path>");
                    Console.WriteLine("  No input file found. Place a logo.png next to the executable or pass a path.");
                    return;
                }
            }

            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("       Libwebp.Net — Console Demo (cwebp 1.5.0)");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();

            using var file = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            Console.WriteLine($"  Input file  : {Path.GetFileName(file.Name)}");
            Console.WriteLine($"  Input size  : {file.Length:N0} bytes");
            Console.WriteLine();

            // ── 1. Basic encode (default quality) ──────────────────────
            Console.WriteLine("1) Basic encode (default quality 75) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("basic.webp")
                .Build(), file.Length);

            // ── 2. High quality lossy ──────────────────────────────────
            Console.WriteLine("2) High quality lossy (q=95, preset photo, sharp_yuv) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("hq_lossy.webp")
                .Preset(Preset.PHOTO)
                .QualityFactor(95)
                .SharpYuv()
                .MultiThreading()
                .Build(), file.Length);

            // ── 3. Lossless encode ─────────────────────────────────────
            Console.WriteLine("3) Lossless encode (lossless preset 6, method 6) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("lossless.webp")
                .Lossless()
                .LosslessPreset(6)
                .CompressionMethod(6)
                .MultiThreading()
                .Build(), file.Length);

            // ── 4. Near-lossless ───────────────────────────────────────
            Console.WriteLine("4) Near-lossless (level 60) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("near_lossless.webp")
                .NearLossless(60)
                .Build(), file.Length);

            // ── 5. Small file target ───────────────────────────────────
            Console.WriteLine("5) Target file size 20 KB (pass 10) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("target_size.webp")
                .TargetSize(20_000)
                .Pass(10)
                .Build(), file.Length);

            // ── 6. Resize + Crop ───────────────────────────────────────
            Console.WriteLine("6) Resize to 320x0 (keep aspect ratio) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("resized.webp")
                .Resize(320, 0)
                .QualityFactor(80)
                .Build(), file.Length);

            // ── 7. Alpha / transparency options ────────────────────────
            Console.WriteLine("7) Alpha options (alpha_q=50, alpha_filter=best, exact) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("alpha.webp")
                .AlphaQ(50)
                .AlphaMethod(1)
                .AlphaFilter("best")
                .Exact()
                .Build(), file.Length);

            // ── 8. Advanced filter + sharpness ─────────────────────────
            Console.WriteLine("8) Filter 80, sharpness 3, strong, SNS 90 ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("filtered.webp")
                .Filter(80)
                .Sharpness(3)
                .Strong()
                .SpatialNoiseShaping(90)
                .Build(), file.Length);

            // ── 9. Low memory + metadata preservation ──────────────────
            Console.WriteLine("9) Low memory mode, preserve all metadata ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("metadata.webp")
                .QualityFactor(80)
                .LowMemory()
                .Metadata("all")
                .Hint("photo")
                .Build(), file.Length);

            // ── 10. Kitchen-sink: everything together ──────────────────
            Console.WriteLine("10) Kitchen-sink (all options combined) ...");
            await EncodeAndReport(ms, file.Name, new WebpConfigurationBuilder()
                .Output("kitchen_sink.webp")
                .Preset(Preset.PHOTO)
                .QualityFactor(85)
                .CompressionMethod(6)
                .Pass(6)
                .AlphaQ(80)
                .AlphaMethod(1)
                .AlphaFilter("best")
                .Filter(50)
                .Sharpness(2)
                .Strong()
                .SharpYuv()
                .SpatialNoiseShaping(75)
                .NumberOfSegments(4)
                .PartitionLimit(50)
                .MultiThreading()
                .Metadata("all")
                .Hint("photo")
                .Build(), file.Length);

            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  All conversions complete!");
            Console.WriteLine("═══════════════════════════════════════════════");
        }

        private static async Task EncodeAndReport(MemoryStream ms, string fileName, WebPConfiguration config, long originalSize)
        {
            var encoder = new WebpEncoder(config);
            using var output = await encoder.EncodeAsync(ms, Path.GetFileName(fileName));

            var ratio = (1.0 - (double)output.Length / originalSize) * 100;
            Console.WriteLine($"    -> {Path.GetFileName(output.Name)}  " +
                              $"{output.Length:N0} bytes  " +
                              $"(saved {ratio:F1}%)");
            Console.WriteLine();
        }
    }
}

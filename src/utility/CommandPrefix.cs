using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{
    public static class CommandPrefix
    {
        public const string Output = "-o ";
        public const string Preset = "-preset ";
        public const string AlphaQuality = "-alpha_q ";
        public const string QualityFactor = "-q ";
        public const string CompressionMethod = "-m ";
        public const string NumberOfSegments = "-segments ";
        public const string TargetSize = "-size ";
        public const string TPSNR = "-psnr ";
        public const string Lossless = "-lossless ";
        public const string LosslessPreset = "-z ";
        public const string InputSize = "-s ";
        public const string SpatialNoiseShaping = "-sns ";
        public const string Filter = "-f ";
        public const string Sharpness = "-sharpness ";
        public const string Strong = "-strong";
        public const string NoStrong = "-nostrong";
        public const string SharpYuv = "-sharp_yuv";
        public const string PartitionLimit = "-partition_limit ";
        public const string Pass = "-pass ";
        public const string Crop = "-crop ";
        public const string Resize = "-resize ";
        public const string MultiThreading = "-mt";
        public const string LowMemory = "-low_memory";
        public const string AlphaMethod = "-alpha_method ";
        public const string AlphaFilter = "-alpha_filter ";
        public const string Exact = "-exact";
        public const string NoAlpha = "-noalpha";
        public const string NearLossless = "-near_lossless ";
        public const string Hint = "-hint ";
        public const string Metadata = "-metadata ";
    }
}

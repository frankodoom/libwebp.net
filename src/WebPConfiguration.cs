using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net
{
    /// <summary>
    /// Optional LibWebP configurations 
    /// </summary>
    public class WebPConfiguration
    {
        protected internal WebPConfiguration()
        {
        }

        /// <summary>The output file name (e.g., "output.webp")</summary>
        internal string OutputFileName { get; set; }
        internal string QualityFactor { get; set; }
        internal string AlphaQ { get; set; }
        internal string Preset { get; set; }
        internal string CompressionMethod { get; set; }
        internal string Lossless { get; set; }
        internal string LosslessPreset { get; set; }
        internal string NumberOfSegments { get; set; }
        internal string TargetSize { get; set; }
        internal string TargetPSNR { get; set; }
        internal string InputSize { get; set; }
        internal string SpatialNoiseShaping { get; set; }
        internal string Filter { get; set; }
        internal string Sharpness { get; set; }
        internal string Strong { get; set; }
        internal string SharpYuv { get; set; }
        internal string PartitionLimit { get; set; }
        internal string Pass { get; set; }
        internal string Crop { get; set; }
        internal string Resize { get; set; }
        internal string MultiThreading { get; set; }
        internal string LowMemory { get; set; }
        internal string AlphaMethod { get; set; }
        internal string AlphaFilter { get; set; }
        internal string Exact { get; set; }
        internal string NoAlpha { get; set; }
        internal string NearLossless { get; set; }
        internal string Hint { get; set; }
        internal string Metadata { get; set; }
    }
}

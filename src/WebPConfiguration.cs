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
        protected  internal WebPConfiguration()
        {
        }
        internal string Output { get; set; }
        internal string QualityFactor { get; set; }
        internal string AlphaQ { get; set; }
        internal string Preset { get; set; }
        internal string CompressionMethod { get; set; }
        internal string Lossless { get; set; }


    }
}

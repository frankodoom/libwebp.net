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
        public string FileInput { get; set; }
        public string Output { get; set; }
        public float QualityFactor { get; set; }
        public int AlphaQ { get; set; }
        public string Preset { get; set; }
        public int CompressionMethod { get; set; }




    }
}

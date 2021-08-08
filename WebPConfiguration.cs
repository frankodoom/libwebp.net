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
        /// <summary>
        /// quality factor (0:small..100:big)
        /// </summary>
        public float QualityFactor { get; set; }
        /// <summary>
        /// transparency-compression quality (0..100)
        /// </summary>
        public int AlphaQ { get; set; }
        /// <summary>
        /// preset setting, one of: default, photo, picture,drawing, icon, text -preset must come first, as it overwrites other parameters
        /// </summary>
        public string Preset { get; set; }
        

        

    }
}

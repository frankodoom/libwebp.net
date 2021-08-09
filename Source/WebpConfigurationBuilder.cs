using Libwebp.Net.errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net
{
    /// <summary>
    /// Constructs a WebP configuration which is passed libwebp process
    /// ecucution as params
    /// </summary>
    public class WebpConfigurationBuilder 
    {
        private readonly WebPConfiguration _config = new WebPConfiguration();
        public WebPConfiguration Build() => _config;

        /// <summary>
        /// Specify the name of the output WebP file. If omitted, cwebp will perform compression but only report statistics. Using "-" as output name
        /// </summary>
        /// <param name="value"> The name of the webp output file</param>
        /// <returns></returns>
        public WebpConfigurationBuilder Output(string value)
        {
            if (value == null)
                throw new OutputFileNameNotFoundException("Specify the name of the output WebP file. ");
            _config.Output = value;
            return this;
        }


        /**/
        /// <summary>
        /// quality factor (0:small..100:big)
        /// </summary>
        public WebpConfigurationBuilder QualityFactor(float value)
        {
            _config.QualityFactor = value;
            return this;
        }
        public WebpConfigurationBuilder AlphaQ(int value)
        {
            _config.AlphaQ = value;
            return this;
        }
        /// <summary>
        /// preset setting, one of: default, photo, picture,drawing, icon, text -preset must come first, as it overwrites other parameters
        /// </summary>
        public WebpConfigurationBuilder Preset(string value)
        {
            _config.Preset = value;
            return this;
        }
        /// <summary>
        /// compression method (0=fast, 6=slowest)
        /// </summary>
        /// <param name="value"> speed from 0-6</param>
        /// <returns></returns>
        public WebpConfigurationBuilder CompressionMethod(int value)
        {
            _config.CompressionMethod = value;
            return this;
        }
    }
}

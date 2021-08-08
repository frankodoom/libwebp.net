using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net
{
    public class WebpConfigurationBuilder 
    {
        private readonly WebPConfiguration _config = new WebPConfiguration();
        public WebPConfiguration Build() => _config;
        public WebpConfigurationBuilder QualityFactor(float value)
        {
            _config.QualityFactor = value;
            return this;
        }
    }
}

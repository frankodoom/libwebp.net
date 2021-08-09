using Libwebp.Net;
using Libwebp.Net.utility;
using Libwebp.Standard;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create your WebP Configuration using fluent builder 
            var configuration = new WebpConfigurationBuilder()
                 .Preset(Preset.DEFAULT)
                 .QualityFactor(200)
                 .AlphaQ(10)
                  //.... add more////
                 .Build();
            
            //pass the configuration to the codec
            var codec = new Codec(configuration);

            FileStream k = File.Create("");
            //call encode function on the codec and pass a FileStream or File Path
            // your converted webp file will be returned as a FileStream
            FileStream fs =  await codec.EncodeAsync(""); 
           
            
            //...create your file by copying or downloading..etc
        }
    }
}

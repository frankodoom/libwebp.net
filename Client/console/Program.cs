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
            var configuration = new WebpConfigurationBuilder()
                .Output("new.webp")
                .Preset(Preset.DRAWING)
                .Lossless()
                .Build();

            //pass the configuration to the Encoder
            var encoder = new WebpEncoder(configuration);

            //get stream from file stored in project
            // in a real web application you can get this from IFormfile

            var path = "image.jpg";
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read);

            // Pass FileStream or File Path to the Encoder
            var fs = FileHelper.SetInputFileStream(file);

            // your converted webp file will be returned as a FileStream
            FileStream fileStream = await encoder.EncodeAsync(fs);

            Console.WriteLine(fileStream.Name);
        }
    }
}

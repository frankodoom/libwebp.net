
using Libwebp.Net;
using Libwebp.Standard;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace console
{
    class Program
    {
        static async Task Main(string[] args)
        {

            // get file to encode
            using var file = new FileStream(@"C:\Users\fodoo\Desktop\Lab\OSP\libwebp.net\client\console\logo.png", FileMode.Open);

            // copy file to Memory
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            Console.WriteLine($"Your target file : {Path.GetFileName(file.Name)}");
            Console.WriteLine($"Lenght in bytes: {file.Length}");

            var config = new WebpConfigurationBuilder()
                        .Output("output.webp")
                        .Build();

            var encoder = new WebpEncoder(config);

            Console.WriteLine("Begin Encode.....");

            // pass the inmemory file to the encoder      
            var output = await encoder.EncodeAsync(ms, Path.GetFileName(file.Name));

           /* your converted file is returned as FileStream, do what you want download, copy to disk
             or save on cloud storage*/

             Console.WriteLine($"Your output file : {Path.GetFileName(output.Name)}");
            Console.WriteLine($"Length in bytes : {output.Length}");
            Console.WriteLine($"You saved {file.Length - output.Length} bytes after compression");

        }
    }
}

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
            var configuration = new WebPConfiguration()
            {
                Output = "image.webP",
                Preset = Preset.DRAWING
            };

            CommandBuilder cmd = new CommandBuilder(configuration);
            Console.WriteLine(cmd.GetCommand());
        }
    }
}

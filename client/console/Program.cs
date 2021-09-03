using Libwebp.Net;
using Libwebp.Standard;
using System;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new WebpConfigurationBuilder()
                .Output("output.webp")
                .Build();

            var encoder = new WebpEncoder(config);

            // create a n FS from a location

                 
        }
    }
}

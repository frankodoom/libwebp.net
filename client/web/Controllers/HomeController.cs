using Libwebp.Net;
using Libwebp.Net.utility;
using Libwebp.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using web.Models;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {

            if (file == null)
                throw new FileNotFoundException();

            //you can handle file checks ie. extensions size etc..
            var oFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.webp";

            //get file as memory stream
            var ms = new MemoryStream();
                file.CopyTo(ms);

            // create your webp configuration
            var config = new WebpConfigurationBuilder()
               .Preset(Preset.PHOTO)
               .Output(oFileName)
               .Build();

            //create an encoder and pass in your configuration
            var encoder = new WebpEncoder(config);

            //call the encoder and pass in the Memorystream and FileName
            //the encoder after encoding will return a FileStream output
            //Optional cast to Stream to return file for download
            Stream fs = await encoder.EncodeAsync(ms, file.FileName);

            /*Do whatever you want with the file....download, copy to disk or 
              save to cloud*/

            return File(fs, "application/octet-stream", oFileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

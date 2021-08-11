using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Models;
using Libwebp;
using Libwebp.Net;
using Libwebp.Standard;
using System.IO;

namespace WebClient.Controllers
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
        public async Task UploadFile(IFormFile file)
        {
            //var ms = new MemoryStream();
            //file.CopyTo(ms);
            ////convert file to a file stream
            //ms.ReadByte();
            //using var stream = file.OpenReadStream();

            //byte[] bs = ms.ToArray();
            //bs.CopyTo()
            //var config = new WebpConfigurationBuilder()
            //    .Output("image.webp")
            //    .Build();

            //var encoder = new WebpEncoder(config);
            //await encoder.EncodeAsync(fs);

            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

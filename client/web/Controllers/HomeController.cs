using Libwebp.Net;
using Libwebp.Net.utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
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
            return View(new ConvertViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Quick Convert — the middleware has already converted the upload to WebP
        /// by the time the controller sees it, so we just stream it back.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickConvertAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select an image file.";
                return RedirectToAction("Index");
            }

            // The middleware already converted the file to WebP.
            // We just pass it straight back to the browser.
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var fileName = file.FileName; // already *.webp thanks to middleware
            return File(ms, "image/webp", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertAsync(ConvertViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError(nameof(model.File), "Please select an image file.");
                return View("Index", model);
            }

            var oFileName = $"{Path.GetFileNameWithoutExtension(model.File.FileName)}.webp";

            // Copy upload to memory
            using var ms = new MemoryStream();
            await model.File.CopyToAsync(ms);

            // Build configuration from all form options
            var builder = new WebpConfigurationBuilder().Output(oFileName);

            // ── Basic ──────────────────────────────────────────────
            if (!string.IsNullOrEmpty(model.Preset))
                builder.Preset(model.Preset);

            if (model.QualityFactor.HasValue)
                builder.QualityFactor(model.QualityFactor.Value);

            if (model.CompressionMethod.HasValue)
                builder.CompressionMethod(model.CompressionMethod.Value);

            if (model.Lossless)
                builder.Lossless();

            if (model.LosslessPreset.HasValue)
                builder.LosslessPreset(model.LosslessPreset.Value);

            if (model.Pass.HasValue)
                builder.Pass(model.Pass.Value);

            // ── Quality & Size ─────────────────────────────────────
            if (model.TargetSize.HasValue)
                builder.TargetSize(model.TargetSize.Value);

            if (model.TargetPSNR.HasValue)
                builder.TargetPSNR(model.TargetPSNR.Value);

            if (model.NearLossless.HasValue)
                builder.NearLossless(model.NearLossless.Value);

            if (!string.IsNullOrEmpty(model.Hint))
                builder.Hint(model.Hint);

            // ── Filter & Sharpness ─────────────────────────────────
            if (model.Filter.HasValue)
                builder.Filter(model.Filter.Value);

            if (model.Sharpness.HasValue)
                builder.Sharpness(model.Sharpness.Value);

            if (model.FilterType == "strong")
                builder.Strong();
            else if (model.FilterType == "nostrong")
                builder.NoStrong();

            if (model.SharpYuv)
                builder.SharpYuv();

            if (model.SpatialNoiseShaping.HasValue)
                builder.SpatialNoiseShaping(model.SpatialNoiseShaping.Value);

            // ── Segments & Partitions ──────────────────────────────
            if (model.NumberOfSegments.HasValue)
                builder.NumberOfSegments(model.NumberOfSegments.Value);

            if (model.PartitionLimit.HasValue)
                builder.PartitionLimit(model.PartitionLimit.Value);

            // ── Alpha / Transparency ───────────────────────────────
            if (model.AlphaQ.HasValue)
                builder.AlphaQ(model.AlphaQ.Value);

            if (model.AlphaMethod.HasValue)
                builder.AlphaMethod(model.AlphaMethod.Value);

            if (!string.IsNullOrEmpty(model.AlphaFilter))
                builder.AlphaFilter(model.AlphaFilter);

            if (model.NoAlpha)
                builder.NoAlpha();

            if (model.Exact)
                builder.Exact();

            // ── Image Transform ────────────────────────────────────
            if (model.CropX.HasValue && model.CropY.HasValue && model.CropW.HasValue && model.CropH.HasValue)
                builder.Crop(model.CropX.Value, model.CropY.Value, model.CropW.Value, model.CropH.Value);

            if (model.ResizeW.HasValue && model.ResizeH.HasValue)
                builder.Resize(model.ResizeW.Value, model.ResizeH.Value);

            // ── Performance & Metadata ─────────────────────────────
            if (model.MultiThreading)
                builder.MultiThreading();

            if (model.LowMemory)
                builder.LowMemory();

            if (!string.IsNullOrEmpty(model.Metadata))
                builder.Metadata(model.Metadata);

            var config = builder.Build();
            var encoder = new WebpEncoder(config);

            Stream fs = await encoder.EncodeAsync(ms, model.File.FileName);

            return File(fs, "application/octet-stream", oFileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

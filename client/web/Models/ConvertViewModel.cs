using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace web.Models
{
    /// <summary>
    /// View model that carries all cwebp encoding options from the UI form
    /// to the controller.
    /// </summary>
    public class ConvertViewModel
    {
        // ── File ───────────────────────────────────────────────────
        [Required(ErrorMessage = "Please select an image file.")]
        public IFormFile File { get; set; }

        // ── Basic ──────────────────────────────────────────────────
        public string Preset { get; set; }
        public float? QualityFactor { get; set; }
        public int? CompressionMethod { get; set; }
        public bool Lossless { get; set; }
        public int? LosslessPreset { get; set; }
        public int? Pass { get; set; }

        // ── Quality & Size ─────────────────────────────────────────
        public int? TargetSize { get; set; }
        public float? TargetPSNR { get; set; }
        public int? NearLossless { get; set; }
        public string Hint { get; set; }

        // ── Filter & Sharpness ─────────────────────────────────────
        public int? Filter { get; set; }
        public int? Sharpness { get; set; }
        public string FilterType { get; set; } // "strong" / "nostrong"
        public bool SharpYuv { get; set; }
        public int? SpatialNoiseShaping { get; set; }

        // ── Segments & Partitions ──────────────────────────────────
        public int? NumberOfSegments { get; set; }
        public int? PartitionLimit { get; set; }

        // ── Alpha / Transparency ───────────────────────────────────
        public int? AlphaQ { get; set; }
        public int? AlphaMethod { get; set; }
        public string AlphaFilter { get; set; }
        public bool NoAlpha { get; set; }
        public bool Exact { get; set; }

        // ── Image Transform ────────────────────────────────────────
        public int? CropX { get; set; }
        public int? CropY { get; set; }
        public int? CropW { get; set; }
        public int? CropH { get; set; }
        public int? ResizeW { get; set; }
        public int? ResizeH { get; set; }

        // ── Performance & Metadata ─────────────────────────────────
        public bool MultiThreading { get; set; }
        public bool LowMemory { get; set; }
        public string Metadata { get; set; }
    }
}

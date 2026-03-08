using System.Runtime.InteropServices;

namespace Libwebp.Net.Interop
{
    /// <summary>
    /// Mirrors libwebp's WebPConfig struct.
    /// Controls all encoding parameters for the WebP encoder.
    /// See: https://developers.google.com/speed/webp/docs/api#webpconfig
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPConfig
    {
        /// <summary>Lossless encoding (0=lossy, 1=lossless).</summary>
        public int lossless;

        /// <summary>Quality factor (0=small..100=big), for lossy encoding only.</summary>
        public float quality;

        /// <summary>Compression method (0=fast, 6=slower-better), default=4.</summary>
        public int method;

        /// <summary>Hint for image type (lossless only).</summary>
        public WebPImageHint image_hint;

        // -- Parameters related to lossy compression only --

        /// <summary>Target size (in bytes); 0 = no target.</summary>
        public int target_size;

        /// <summary>Target PSNR (in dB); 0 = no target.</summary>
        public float target_PSNR;

        /// <summary>Maximum number of segments to use (1..4).</summary>
        public int segments;

        /// <summary>Spatial noise shaping strength (0..100).</summary>
        public int sns_strength;

        /// <summary>Deblocking filter strength (0..100).</summary>
        public int filter_strength;

        /// <summary>Filter sharpness (0=most sharp, 7=least sharp).</summary>
        public int filter_sharpness;

        /// <summary>Filtering type (0=simple, 1=strong). Default is 1.</summary>
        public int filter_type;

        /// <summary>Auto-adjust filter strength (0=off, 1=on).</summary>
        public int autofilter;

        /// <summary>Algorithm for encoding alpha plane (0=none, 1=compressed).</summary>
        public int alpha_compression;

        /// <summary>Predictive filtering method for alpha (0=none, 1=fast, 2=best).</summary>
        public int alpha_filtering;

        /// <summary>Alpha quality (0..100).</summary>
        public int alpha_quality;

        /// <summary>Number of entropy-analysis passes (1..10).</summary>
        public int pass;

        /// <summary>Show compressed area (1=on, 0=off).</summary>
        public int show_compressed;

        /// <summary>Preprocessing filter (0=none, 1=segment-smooth, 2=pseudo-random dithering).</summary>
        public int preprocessing;

        /// <summary>Log2(number of token partitions) (0..3). Default 0.</summary>
        public int partitions;

        /// <summary>Quality degradation limit for partition 0 (0..100).</summary>
        public int partition_limit;

        /// <summary>Emulate older behavior (if set).</summary>
        public int emulate_jpeg_size;

        /// <summary>If true, try multi-threaded encoding.</summary>
        public int thread_level;

        /// <summary>If true, reduce memory usage (slower encoding).</summary>
        public int low_memory;

        /// <summary>Near lossless encoding (0..100, 100=off).</summary>
        public int near_lossless;

        /// <summary>Exact: preserve RGB values in transparent area (0=off, 1=on).</summary>
        public int exact;

        /// <summary>Reserved for future lossless features (use_delta_palette).</summary>
        public int use_delta_palette;

        /// <summary>If needed, use sharp (and slower) RGB-to-YUV conversion.</summary>
        public int use_sharp_yuv;

        /// <summary>Minimum permissible quality factor.</summary>
        public int qmin;

        /// <summary>Maximum permissible quality factor.</summary>
        public int qmax;
    }

    /// <summary>
    /// Memory writer structure: accumulates the encoded output in memory.
    /// Maps to libwebp's WebPMemoryWriter struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPMemoryWriter
    {
        /// <summary>Pointer to the encoded data buffer.</summary>
        public nint mem;

        /// <summary>Size of the encoded data.</summary>
        public nuint size;

        /// <summary>Maximum capacity of the buffer.</summary>
        public nuint max_size;

        /// <summary>Padding for ABI compatibility.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public uint[] pad;
    }

    /// <summary>
    /// Delegate for the WebPPicture writer callback.
    /// Called by the encoder to emit output bytes.
    /// </summary>
    /// <param name="data">Pointer to encoded data chunk.</param>
    /// <param name="data_size">Size of the data chunk.</param>
    /// <param name="picture">Pointer to the WebPPicture struct.</param>
    /// <returns>1 on success, 0 on error.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int WebPWriterFunction(nint data, nuint data_size, ref WebPPicture picture);

    /// <summary>
    /// Delegate for encoder progress reporting callback.
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int WebPProgressHook(int percent, ref WebPPicture picture);

    /// <summary>
    /// Mirrors libwebp's WebPPicture struct.
    /// Holds input pixels, output writer, and encoding state.
    /// See: https://developers.google.com/speed/webp/docs/api#webppicture
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPPicture
    {
        // -- Input --

        /// <summary>Use ARGB input (1=ARGB, 0=YUV).</summary>
        public int use_argb;

        // -- YUV input (used when use_argb == 0) --

        /// <summary>Colorspace: should be YUV420 for now (=0).</summary>
        public int colorspace;

        /// <summary>Width of picture (less than or equal to WEBP_MAX_DIMENSION).</summary>
        public int width;

        /// <summary>Height of picture (less than or equal to WEBP_MAX_DIMENSION).</summary>
        public int height;

        /// <summary>Pointer to luma (Y) plane.</summary>
        public nint y;
        /// <summary>Pointer to chroma U plane.</summary>
        public nint u;
        /// <summary>Pointer to chroma V plane.</summary>
        public nint v;

        /// <summary>Luma stride.</summary>
        public int y_stride;
        /// <summary>Chroma stride.</summary>
        public int uv_stride;

        /// <summary>Pointer to alpha plane.</summary>
        public nint a;
        /// <summary>Alpha stride.</summary>
        public int a_stride;

        /// <summary>Padding for YUV.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] pad1;

        // -- ARGB input (used when use_argb != 0) --

        /// <summary>Pointer to ARGB (32-bit) plane.</summary>
        public nint argb;
        /// <summary>Stride in pixels (not bytes) for ARGB plane.</summary>
        public int argb_stride;

        /// <summary>Padding for ARGB.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] pad2;

        // -- Output --

        /// <summary>Writer callback for output data.</summary>
        public nint writer;

        /// <summary>Custom data passed to the writer.</summary>
        public nint custom_ptr;

        /// <summary>Map of extra information (type depends on extra_info_type).</summary>
        public int extra_info_type;

        /// <summary>Pointer to side information (updated by the encoder).</summary>
        public nint extra_info;

        // -- Stats (filled after encoding) --

        /// <summary>Pointer to WebPAuxStats structure (side statistics).</summary>
        public nint stats;

        // -- Error code --

        /// <summary>Error code for the latest error.</summary>
        public WebPEncodingError error_code;

        // -- Progress hook --

        /// <summary>Progress callback (if non-null).</summary>
        public nint progress_hook;

        /// <summary>User data for the progress hook.</summary>
        public nint user_data;

        /// <summary>Padding for future fields.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] pad3;

        /// <summary>Padding (row pointer cache, etc.).</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public nint[] pad4;

        /// <summary>Padding (more internal state).</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public nint[] pad5;

        /// <summary>Pointer to private memory manager data.</summary>
        public nint memory_;

        /// <summary>Padding for memory_argb, etc.</summary>
        public nint memory_argb_;

        /// <summary>Padding.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public nint[] pad6;
    }
}

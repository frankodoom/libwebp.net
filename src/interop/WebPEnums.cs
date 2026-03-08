namespace Libwebp.Net.Interop
{
    /// <summary>
    /// Encoding presets matching libwebp's WebPPreset enum.
    /// </summary>
    public enum WebPPreset
    {
        /// <summary>Default preset.</summary>
        Default = 0,
        /// <summary>Digital picture, like portrait, inner shot.</summary>
        Picture = 1,
        /// <summary>Outdoor photograph, with natural lighting.</summary>
        Photo = 2,
        /// <summary>Hand or line drawing, with high-contrast details.</summary>
        Drawing = 3,
        /// <summary>Small-sized colorful images.</summary>
        Icon = 4,
        /// <summary>Text-like content.</summary>
        Text = 5
    }

    /// <summary>
    /// Image content hint for the encoder.
    /// Maps to libwebp's WebPImageHint enum.
    /// </summary>
    public enum WebPImageHint
    {
        /// <summary>Default hint (no specific optimization).</summary>
        Default = 0,
        /// <summary>Digital picture, like portrait, inner shot.</summary>
        Picture = 1,
        /// <summary>Outdoor photograph.</summary>
        Photo = 2,
        /// <summary>Discrete tone image (graph, map, tile, etc.).</summary>
        Graph = 3,
        /// <summary>Sentinel value (do not use).</summary>
        Last = 4
    }

    /// <summary>
    /// Encoding error codes returned by WebPEncode().
    /// Maps to libwebp's WebPEncodingError enum.
    /// </summary>
    public enum WebPEncodingError
    {
        Ok = 0,
        OutOfMemory = 1,
        BitstreamOutOfMemory = 2,
        NullParameter = 3,
        InvalidConfiguration = 4,
        BadDimension = 5,
        Partition0Overflow = 6,
        PartitionOverflow = 7,
        BadWrite = 8,
        FileTooBig = 9,
        UserAbort = 10,
        Last = 11
    }

    /// <summary>
    /// Alpha filtering method for the encoder.
    /// </summary>
    public enum WebPAlphaFilter
    {
        None = 0,
        Fast = 1,
        Best = 2
    }
}

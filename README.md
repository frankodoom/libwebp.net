
<img src="https://github.com/frankodoom/libwebp.net/blob/main/assets/libwebp-logo-2.png"/>

An asynchronous .NET library for encoding images to Google's WebP format. Seamlessly compress images using lossy and lossless encoding to improve network performance and reduce file size on disk. WebP generally has better compression than JPEG, PNG, and GIF and is designed to supersede them.

**Powered by cwebp v1.5.0** — supports **Windows (x64)**, **Linux (x86-64)**, and **macOS (arm64)** out of the box. The appropriate native binary is bundled as an embedded resource and extracted automatically at runtime.

You can see the library in action by using the web client — your result will be downloaded into your browser.

# Using the Library

### General Usage
Below shows the basic use of the library using a console app.

```csharp
class Program
{
    static async Task Main(string[] args)
    {
        // get file to encode
        using var file = new FileStream(@"C:\Users\user\Desktop\logo.png", FileMode.Open);

        // copy file to memory
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        // setup configuration for the encoder
        var config = new WebpConfigurationBuilder()
                        .Output("output.webp")
                        .Build();

        // create an encoder and pass in the configuration
        var encoder = new WebpEncoder(config);

        // start encoding by passing your memorystream and filename
        var output = await encoder.EncodeAsync(ms, Path.GetFileName(file.Name));

        /* your converted file is returned as FileStream — download it,
           copy to disk, write to db, or save on cloud storage */

        Console.WriteLine($"Your output file : {Path.GetFileName(output.Name)}");
        Console.WriteLine($"Length in bytes : {output.Length}");
    }
}
```

### ASP.NET Core
Below demonstrates how the library is used in ASP.NET Core to convert uploaded images.

```csharp
public async Task<IActionResult> UploadAsync(IFormFile file)
{
    if (file == null)
        throw new FileNotFoundException();

    // you can handle file checks ie. extensions, file size etc.

    // creating output file name
    var oFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.webp";

    // create your webp configuration
    var config = new WebpConfigurationBuilder()
       .Preset(Preset.PHOTO)
       .Output(oFileName)
       .Build();

    // create an encoder and pass in your configuration
    var encoder = new WebpEncoder(config);

    // copy file to memory stream
    var ms = new MemoryStream();
    file.CopyTo(ms);

    // call the encoder and pass in the MemoryStream and input FileName
    // the encoder after encoding will return a FileStream output
    Stream fs = await encoder.EncodeAsync(ms, file.FileName);

    /* Do whatever you want with the file — download, copy to disk,
       or save to cloud */

    return File(fs, "application/octet-stream", oFileName);
}
```

### [Coming SOON!!!] ASP.NET Core Middleware
`Libwebp.Net.Middleware` is a separate package that depends on `Libwebp.Net` and allows you to inject image compression
in your ASP.NET pipeline. This middleware will compress your specified image types using lossy and lossless algorithms.

# Advanced Encoding
The encoder contains many advanced parameters. Libwebp.Net supports cwebp's advanced encoding API which can be used to better balance the trade-off between compression efficiency and processing time. You can access the advanced encoding parameters by adding options to your `WebpConfigurationBuilder`:

```csharp
var configuration = new WebpConfigurationBuilder()
     .Output("image.webp")
     .Preset(Preset.DEFAULT)
     .QualityFactor(85)
     .CompressionMethod(6)
     .SharpYuv()
     .MultiThreading()
     .Metadata("all")
     // ... add more advanced options
     .Build();
```

### Options
All options supported by cwebp v1.5.0 are available. Use them to fine-tune the compression algorithm for your desired output.

#### Basic Options

| Method | Description |
| --- | --- |
| `Output(string value)` | Specify the name of the output WebP file |
| `QualityFactor(float value)` | Quality factor (0:small..100:big), default 75 |
| `Preset(string value)` | Preset: `default`, `photo`, `picture`, `drawing`, `icon`, `text` |
| `Lossless()` | Encode image losslessly |
| `LosslessPreset(int level)` | Lossless preset level (0:fast..9:slowest) |
| `CompressionMethod(int value)` | Compression method (0=fast, 6=slowest), default 4 |
| `Pass(int value)` | Analysis pass number (1..10), more passes = better quality |

#### Quality & Size Control

| Method | Description |
| --- | --- |
| `TargetSize(int value)` | Target size (in bytes) for the compressed output |
| `TargetPSNR(float value)` | Target PSNR (in dB) for the compressed output |
| `NearLossless(int value)` | Near-lossless preprocessing (0..100, 100=off) |
| `Hint(string value)` | Image characteristics hint: `photo`, `picture`, `graph` |

#### Filter & Sharpness

| Method | Description |
| --- | --- |
| `Filter(int value)` | Deblocking filter strength (0=off..100) |
| `Sharpness(int value)` | Filter sharpness (0:most sharp..7:least sharp), default 0 |
| `Strong()` | Use strong filter (default) |
| `NoStrong()` | Use simple filter instead of strong |
| `SharpYuv()` | Use sharper (slower) RGB-to-YUV conversion |
| `SpatialNoiseShaping(int value)` | Spatial noise shaping strength (0..100), default 50 |

#### Segments & Partitions

| Method | Description |
| --- | --- |
| `NumberOfSegments(int value)` | Number of segments (1..4), default 4 |
| `PartitionLimit(int value)` | Limit quality for first partition (0=no degradation..100) |

#### Alpha / Transparency

| Method | Description |
| --- | --- |
| `AlphaQ(int value)` | Transparency-compression quality (0..100) |
| `AlphaMethod(int value)` | Transparency compression method (0..1), default 1 |
| `AlphaFilter(string value)` | Predictive filtering for alpha: `none`, `fast`, `best` |
| `NoAlpha()` | Discard any transparency information |
| `Exact()` | Preserve RGB values in transparent area |

#### Image Transform

| Method | Description |
| --- | --- |
| `Crop(int x, int y, int w, int h)` | Crop the picture before encoding |
| `Resize(int w, int h)` | Resize after cropping (0 for either to keep aspect ratio) |
| `InputSize(int width, int height)` | Input size (width x height) for YUV input |

#### Performance & Metadata

| Method | Description |
| --- | --- |
| `MultiThreading()` | Use multi-threading for encoding |
| `LowMemory()` | Reduce memory usage (slower encoding) |
| `Metadata(string value)` | Copy metadata: `all`, `none`, `exif`, `icc`, `xmp` (comma-separated) |

## Supported Platforms

| Platform | Architecture | Status |
| --- | --- | --- |
| Windows | x64 | ✅ Supported |
| Linux | x86-64 | ✅ Supported |
| macOS | arm64 (Apple Silicon) | ✅ Supported |

## Supported Input Formats

PNG, JPEG, TIFF, WebP, and raw Y'CbCr samples.

# Licence

````
Copyright 2021

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
````

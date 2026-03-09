
<img src="https://github.com/frankodoom/libwebp.net/blob/main/assets/libwebp-logo-2.png"/>

An asynchronous .NET library for encoding images to Google's WebP format. Seamlessly compress images using lossy and lossless encoding to improve network performance and reduce file size on disk. WebP generally has better compression than JPEG, PNG, and GIF and is designed to supersede them.

**Powered by libwebp v1.5.0** — supports **Windows (x64)**, **Linux (x86-64)**, and **macOS (arm64)** out of the box. The native library is loaded via P/Invoke at runtime.

You can see the library in action by using the web client — your result will be downloaded into your browser.

---

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
        using var output = await encoder.EncodeAsync(ms, Path.GetFileName(file.Name));

        /* your converted file is returned as a MemoryStream — download it,
           copy to disk, write to db, or save on cloud storage */

        Console.WriteLine($"Output size : {output.Length} bytes");

        // Example: save to disk
        using var outFile = new FileStream("output.webp", FileMode.Create);
        await output.CopyToAsync(outFile);
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
    await file.CopyToAsync(ms);

    // encode — returns a MemoryStream with the WebP data
    var webpStream = await encoder.EncodeAsync(ms, file.FileName);

    /* Do whatever you want with the file — download, copy to disk,
       or save to cloud */

    return File(webpStream, "image/webp", oFileName);
}
```

### ASP.NET Core Middleware
`Libwebp.Net.Middleware` is a separate NuGet package that plugs into your ASP.NET Core pipeline and **automatically converts every uploaded image to WebP** — no changes needed in your controllers or Razor Pages.

#### 1. Register the service and configure options

```csharp
using Libwebp.Net.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Register the WebP conversion service with default or custom options
builder.Services.AddWebpConversion(options =>
{
    options.QualityFactor   = 80;        // 0–100 (default 75)
    options.MultiThreading  = true;      // use all cores
    options.Lossless        = false;     // lossy by default
    options.MaxInputSizeBytes = 10_000_000; // skip files > 10 MB
    // All encoding options are available here
});

var app = builder.Build();
```

#### 2. Add the middleware to the pipeline

Place `UseWebpUploadConversion()` **before** `MapControllerRoute` / `MapRazorPages` so controllers receive the converted files.

```csharp
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Every multipart/form-data upload with an image is auto-converted to WebP
app.UseWebpUploadConversion();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

#### 3. Your controller receives WebP — zero conversion code

The middleware replaces each eligible `IFormFile` in the request with the converted WebP data before the controller runs:

```csharp
[HttpPost]
public async Task<IActionResult> UploadAsync(IFormFile file)
{
    // file.FileName is already "photo.webp"
    // file.ContentType is already "image/webp"
    var ms = new MemoryStream();
    await file.CopyToAsync(ms);
    ms.Position = 0;

    return File(ms, "image/webp", file.FileName);
}
```

#### Middleware Behaviour Options

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `InputContentTypes` | `HashSet<string>` | jpeg, png, gif, tiff, bmp | Content types eligible for conversion |
| `InputExtensions` | `HashSet<string>` | .jpg, .jpeg, .png, .gif, .tiff, .tif, .bmp | File extensions eligible for conversion |
| `MaxInputSizeBytes` | `long?` | `null` (no limit) | Files larger than this are passed through |
| `SkipConversion` | `Func<HttpContext, IFormFile, bool>?` | `null` | Return `true` to skip a specific file |

All 29 encoding options from `WebpConfigurationBuilder` (quality, preset, lossless, sharpness, alpha, etc.) are also available on `WebpConversionOptions` and apply to every file the middleware converts.

#### Using `IWebpConversionService` Directly

If you need to convert images outside the middleware pipeline (e.g. background jobs), inject `IWebpConversionService`:

```csharp
public class ImageProcessor
{
    private readonly IWebpConversionService _converter;

    public ImageProcessor(IWebpConversionService converter)
        => _converter = converter;

    public async Task ProcessAsync(Stream input, string fileName)
    {
        var (webpData, webpFileName) = await _converter.ConvertAsync(input, fileName);
        // webpData is a MemoryStream with the WebP bytes
        // webpFileName is e.g. "photo.webp"
    }
}
```

# Advanced Encoding
The encoder contains many advanced parameters. Libwebp.Net supports libwebp's advanced encoding API which can be used to better balance the trade-off between compression efficiency and processing time. You can access the advanced encoding parameters by adding options to your `WebpConfigurationBuilder`:

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
All options supported by libwebp v1.5.0 are available. Use them to fine-tune the compression algorithm for your desired output.

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
| `InputSize(int width, int height)` | Input size (width x height) for raw pixel data |

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

| Format | Windows | Linux / macOS |
| --- | --- | --- |
| JPEG | ✅ Auto-decoded via GDI+ | ❌ Pass raw RGBA pixels instead |
| PNG  | ✅ Auto-decoded via GDI+ | ❌ Pass raw RGBA pixels instead |
| BMP  | ✅ Auto-decoded via GDI+ | ❌ Pass raw RGBA pixels instead |
| GIF  | ✅ Auto-decoded via GDI+ | ❌ Pass raw RGBA pixels instead |
| TIFF | ✅ Auto-decoded via GDI+ | ❌ Pass raw RGBA pixels instead |
| WebP | ✅ Auto-decoded via libwebp | ✅ Auto-decoded via libwebp |
| Raw RGBA pixels | ✅ `Encode(byte[], int, int)` | ✅ `Encode(byte[], int, int)` |

On non-Windows platforms, decode your images to raw RGBA pixels using any imaging library of your choice, then call `Encode(byte[] rgba, int width, int height)` or configure `InputSize(w, h)` and use `EncodeAsync(MemoryStream, string)`.

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

# libwebp.net
 An asynchronuous  crossplatform utility for encoding images to Google's .webp format for faster image rendering in Asp.Net Wep Apps. Seemlessly convert uploaded images to lossless formats to improve speed in your web applications. 
 
 # Using the Library
 
 ### 
 
 ```csharp
   // create your WebP Configuration using fluent builder 
            var configuration = new WebpConfigurationBuilder()
                 .Output("image.webp")
                 .Build();
            
   //pass the configuration to the codec
            var codec = new Codec(configuration);
          
    // Pass FileStream or File Path to the Encoder
    // your converted webp file will be returned as a FileStream
            FileStream fileStream =  await codec.EncodeAsync("image.png"); 
            
            //...create your file by copying or downloading..etc   
```

### Asp.Net Core Controller
 
 ```csharp
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
```

# Advanced Encoding
The encoder contains a lot advanced parameters. LibWebP.Net supports libWebp's advanced encoding API which can be used to better balance the trade-off between compression efficiency and processing time. You can get access to the advanced encode  parameters by adding the various options below to your ```WebpConfigurationBuilder```


 ```csharp
   // create your WebP Configuration using fluent builder 
            var configuration = new WebpConfigurationBuilder()
                 .Output("image.webp")
                 .Preset(Preset.DEFAULT)
                 .QualityFactor(200)
                 .AlphaQ(10)
                  //.... add more advanced options//
                 .Build();
               
```

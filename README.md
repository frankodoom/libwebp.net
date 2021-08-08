# libwebp.net
 An asynchronuous  crossplatform utility for converting images to Google's .webp format for faster image rendering in Asp.Net Wep Apps. If you are in need of a seemless way to convert uploaded images to lossless formats to improve site speed this library is for you!
 
 # Using the Library
 
 ```csharp
   // create your WebP Configuration using fluent builder 
            var configuration = new WebpConfigurationBuilder()
                 .Preset(Preset.DEFAULT)
                 .QualityFactor(200)
                 .AlphaQ(10)
                  //.... add more advanced options//
                 .Build();
            
   //pass the configuration to the codec
            var codec = new Codec(configuration);
          
    //call encode function on the codec and pass a FileStream or File Path
    // your converted webp file will be returned as a FileStream
            FileStream fileStream =  await codec.EncodeAsync("THis will be a FileStream or FilePath"); 
            
            //...create your file by copying or downloading..etc   
```

# Advanced Encoding
The encoder contains a lot advanced parameters. LibWebP.Net supports libWebp's advanced API which provides the ability to have on-the-fly cropping and rescaling, something of great usefulness on memory-constrained environments like mobile phones. They can be useful to better balance the trade-off between compression efficiency and processing time. You can get access to the advanced encode or decode parameters by adding the various options below to your ```WebpConfigurationBuilder```

# libwebp.net
An asynchronuous utility for encoding images to Google's [.webp](https://developers.google.com/speed/webp) format. Seemlessly compress images to lossy and lossless formats in your .NET projectsto improve network performance and reduce file size on disk. WebP generally has better compression than JPEG, PNG and GIF and is designed to supersede them. You can see the library in action by using the [webclient](http://libwebp.azurewebsites.net/) , your result will be downloaded into your browser.
 
 # Using the Library
 
### General Usage
Below shows the basic use of the library using a console app.
 
 ```csharp
  class Program
   {
     static async Task Main(string[] args)
      {

       // get file to encode
       using var file = new FileStream(@"C:\Users\fodoo\Desktop\logo.png", FileMode.Open);

       // copy file to Memory
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
           
        //setup configuration for the encoder
        var config = new WebpConfigurationBuilder()
                        .Output("output.webp")
                        .Build();
                        
        // create an encoder and pass in the configuration
         var encoder = new WebpEncoder(config);

        // start encoding by passing your memorystream and filename      
        var output = await encoder.EncodeAsync(ms, Path.GetFileName(file.Name));

        /* your converted file is returned as FileStream, do what you want download,
           copy to disk, write to db or save on cloud storage,*/  
              
         Console.WriteLine($"Your output file : {Path.GetFileName(output.Name)}");
         Console.WriteLine($"Length in bytes : {output.Length}");
       }
    }
```

### Asp.Net Core 
Below demonstrates how the library is used in Asp.Net Core to convert uploaded images. This library is currently supported only in Windows Environments.
 
 ```csharp
  public async Task<IActionResult> UploadAsync(IFormFile file)
        {

            if (file == null)
                throw new FileNotFoundException();

            //you can handle file checks ie. extensions, file size etc..
            
            //creating output file name
            // your name can be a unique Guid or any name of your choice with .webp extension..eg output.webp
            //in my case i am removing the extension from the uploaded file and appending
            // the .webp extension
            var oFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.webp";

            // create your webp configuration
            var config = new WebpConfigurationBuilder()
               .Preset(Preset.PHOTO)
               .Output(oFileName)
               .Build();
            
            //create an encoder and pass in your configuration
            var encoder = new WebpEncoder(config);
            
           //copy file to memory stream
            var ms = new MemoryStream();
                     file.CopyTo(ms);
            
            //call the encoder and pass in the Memorystream and input FileName
            //the encoder after encoding will return a FileStream output
            
            //Optional cast to Stream to return file for download
            Stream fs = await encoder.EncodeAsync(ms, file.FileName);

            /*Do whatever you want with the file....download, copy to disk or 
              save to cloud*/

            return File(fs, "application/octet-stream", oFileName);
        }   
```

# Compression
  Below shows the results of a basic compression done with libwebp.net, download the files with the link below and compare the file sizes.
  
 <img src="https://github.com/frankodoom/libwebp.net/blob/main/src/docs/eg.PNG">

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

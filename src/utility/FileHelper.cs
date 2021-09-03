using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{
    /// <summary>
    ///  This class is an abstraction of some 
    ///  file manipulation tasks
    /// </summary>
    public class FileHelper
    {
        private static FileStream InputFileStream { get; set; }
        private static FileStream OutputFileStream { get; set; }

        //static work around for holding file output
        public static string FileOutput { get; set; }

        //create FileStream  from Stream
        public static Task<FileStream> ReadFileFromStream(Stream stream)
        {
            return Task.FromResult(stream as FileStream);
        }

     
        public static FileStream SetInputFileStream(FileStream fileStream)
        {
            InputFileStream = fileStream;
            return InputFileStream;
        }
        public static FileStream GetInputFileStream()
        {
            return InputFileStream;
        }

         //Todo

    }
}

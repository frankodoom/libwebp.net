using Libwebp.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Libwebp.Standard
{
    /// <summary>
    /// Encodes and Decodes .WebP
    /// </summary>
    public class Codec
    {
        private WebPConfiguration _configuration { get; set; }
        /// <summary>
        /// public constructor that accepts the configuration
        /// and uses it through out the encode and decode process
        /// </summary>
        /// <param name="configuration"></param>
        public Codec(WebPConfiguration configuration)
        {
            _configuration = configuration;
        }

        //

        /// <summary>
        /// Recieve a file stream convert to file and encode, Handle encoding inmemory
        /// and write output from memory. No Persisting!
        /// </summary>
        public async Task<FileStream> EncodeAsync(FileStream File)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "codecs/cwebp.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "cwebp -q 80 image.jpg -o image.webp";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    await exeProcess.WaitForExitAsync();

                    //check if process exited if not kill process
                    if (!exeProcess.HasExited)
                        exeProcess.Kill();
                }
            }
            catch
            {
                // Log error.
            }
        }
        public async Task<FileStream> EncodeAsync(string FilePath)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "codecs/cwebp.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //TODO dynamically construct arguments based on configuration
            startInfo.Arguments = "cwebp -q 80 image.jpg -o image.webp";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    await exeProcess.WaitForExitAsync();

                    //check if process exited if not kill process
                    if (!exeProcess.HasExited)
                        exeProcess.Kill();
                }
            }
            catch
            {
                // Log error.
            }
        }

    }
}

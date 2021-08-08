using Libwebp.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Libwebp.Standard
{
    public class Converter
    {
        //

        /// <summary>
        /// Recieve a file stream convert to file and encode, Handle encoding inmemory
        /// and write output from memory. No Persisting!
        /// </summary>
        public async void ConvertToWebPAsync(FileStream File, WebPConfiguration config)
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
        public async void ConvertToWebPAsync(string FilePath)
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

    }
}

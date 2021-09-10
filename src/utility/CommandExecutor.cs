using Libwebp.Net.errors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{
    /// <summary>
    ///  Executes the libewebp codec to encode a file and returns
    ///  the encoded file as a FileStream
    /// </summary>
  public static class CommandExecutor
    {
        /// <summary>
        /// Executes command and returnd the temp filepath if successful
        /// </summary>
        /// <param name="Command"></param>
        /// <returns>Filepath to convertedf ile</returns>
       public static async Task<FileStream> Execute(string Command)
        {

            //fetch codec
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Libwebp.Net.codecs.windows.cwebp.exe");

            //copy codec to memory
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            // Get user temp directory
            var path0 = Path.GetTempPath();
            
            //write codec to temp location on server if file does not exist when FIleMode is set to OpenOrCreate
            using var fs = new FileStream(path0 + "cwebp.exe", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            ms.WriteTo(fs);
            fs.Close();

            //use codec from temp

            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = fs.Name,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = Command
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process cwebp = Process.Start(startInfo))
                {
                    await cwebp.WaitForExitAsync();

                    //check if process exited if not kill process
                    if (!cwebp.HasExited)
                        cwebp.Kill();                
                }
            }
            catch(Exception ex)
            {
                throw new CommandExecutionException("Command Executor Failed: " + ex.Message, ex.InnerException);
            }

            //get output stream from converted .webp tempfile 
            var path = Path.GetTempPath()+FileHelper.FileOutput;
            var file = new FileStream(path, FileMode.Open, FileAccess.Read);
      
            return await Task.FromResult(file);
        }


  
    }
}

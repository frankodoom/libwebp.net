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

            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePathRelativeToAssembly = Path.Combine(assemblyPath,"codecs/cwebp.exe");
            string normalizedPath = Path.GetFullPath(filePathRelativeToAssembly);

            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = normalizedPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = Command
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process webpProcess = Process.Start(startInfo))
                {
                    await webpProcess.WaitForExitAsync();
 
                    //check if process exited if not kill process
                    if (!webpProcess.HasExited)
                        webpProcess.Kill();
                }
            }
            catch(Exception ex)
            {
                throw new CommandExecutionException("Something went wrong executing the command: " + ex.Message, ex.InnerException);
            }

            //get output stream from converted .webp tempfile 
            var path = Path.GetTempPath()+FileHelper.FileOutput;

            using var file = new FileStream(path, FileMode.Open, FileAccess.Read);

            return await Task.FromResult(file);
        }
    }
}

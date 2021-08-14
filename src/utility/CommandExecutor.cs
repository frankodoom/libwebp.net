using Libwebp.Net.errors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "codecs/cwebp.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Command;

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
            catch(Exception ex)
            {
                throw new CommandExecutionException("Something went wrong executing the command: " + ex.Message, ex.InnerException);
            }

            //get output stream from converted .webp tempfile 

            var path = Path.GetTempPath()+ "output.webp" ;

            using var file = new FileStream(path, FileMode.Open, FileAccess.Read);

            return await Task.FromResult(file);
        }
    }
}

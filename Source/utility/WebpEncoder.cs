using Libwebp.Net;
using Libwebp.Net.utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Libwebp.Standard
{
    /// <summary>
    /// This class Recieves a FileStream,Encodes to.Webp and 
    /// returns a FileStream of the results 
    /// which can be Saved To disk or Downloaded by any .NET
    /// Client Application
    /// </summary>
    public class WebpEncoder
    {
        private WebPConfiguration _configuration { get; set; }
        /// <summary>
        /// public constructor that accepts the configuration
        /// and uses it through out the encode and decode process
        /// </summary>
        /// <param name="configuration"></param>
        public WebpEncoder(WebPConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<FileStream> EncodeAsync(FileStream file)
        {
            //null check
            if (file.Length == 0)
                throw new FileNotFoundException();

            //pass the filestream to the filehelper for file operations
            FileHelper.SetInputFileStream(file);

            //Construct the command from users configuration
            CommandBuilder command = new CommandBuilder(_configuration);

            //get the users command to be executed
            string usercommand = command.GetCommand();

            //Execute the command

            return await CommandExecutor.Execute(usercommand); 
        }

        

    }
}

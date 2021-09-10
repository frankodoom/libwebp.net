using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{

    /* LibWebP Command Builder
       © 2021 Frank Arkhurst Odoom
       This class is responsible for recieving the WebPConfiguration options 
       from the WebPConfigirationBuilder and dynamically inserts
       the data as arguements to construct the command
       needed by the subprocess to execute the encoder cwebp.exe when the
       Command Executor is called.
     */
    public class CommandBuilder
    {
        private StringBuilder Command;
        public CommandBuilder(WebPConfiguration configuration)
        {
            //initialize and construct  arguments in order of execution
            string[] args = new string[20];
            args[0] = string.Format("\"{0}\"", FileHelper.GetInputFileStream().Name);
            args[1] = configuration.Preset;
            args[2] = configuration.Lossless;
            args[3] = configuration.Output;

            /*Add other command arguements in order of execution*/

            Build(args);
        }

        private void Build(string[] args)
        {
            //initialize the base command
            Command = new StringBuilder($"cwebp ");

            List<string> userargs = new List<string>();

            foreach (var param in args)
            {
                if (param == null)
                {
                    continue;
                }
                userargs.Add(param);
            }
            Command.AppendJoin(" ", userargs);
        }

        /// <summary>
        /// Ger the comand built by the Builder
        /// </summary>
        /// <returns>string user command to be executed</returns>
        public string GetCommand()
        {
            return Command.ToString();
        }

    }
}

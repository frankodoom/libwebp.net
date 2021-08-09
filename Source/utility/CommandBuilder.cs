using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{
    /// <summary>
    /// 2021 Frank Arkhurst Odoom
    /// This class is responsible for recieving the WebPConfiguration options 
    /// from the WebPConfigirationBuilder and dynamically inserts
    /// the data as arguements to construct the command
    /// needed by the subprocess to execute the encoder cwebp.exe when the
    /// Command Executor is called.
    /// </summary>
    public class CommandBuilder 
    {

        private StringBuilder Command;
      
        public CommandBuilder(WebPConfiguration configuration)
        {

            //initialize all arguments in order of execution
            string[] args = new string[20];
            args[0] = configuration.FileInput;
            args[1] = configuration.Preset;
            args[2] = configuration.Output;

            //todo add all possible command arguments

            //call construct comand to build the dynamic comand
            ConstructCommand(args);
        }
       
        private void ConstructCommand(string[] args)
        {
            //initialize the base command
            Command = new StringBuilder($"cwebp ");
 
            var userargs = new List<string>();
            foreach (var param in args)
            {
                if(param == null)
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
        /// <returns>string command to be executed</returns>
        public string GetCommand()
        {
            return Command.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.errors
{
    public class CommandExecutionException: Exception
    {
        public CommandExecutionException()
        {

        }
        public CommandExecutionException(string message)
            : base(message)
        {
            
        }


        public CommandExecutionException(string message, Exception inner)
            : base(message, inner)
        {

        }

    }
}

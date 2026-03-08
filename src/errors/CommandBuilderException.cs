using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.errors
{
    public class CommandBuilderException : Exception
    {
        public CommandBuilderException() { }

        public CommandBuilderException(string message)
            : base(message) { }

        public CommandBuilderException(string message, Exception inner)
            : base(message, inner) { }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.errors
{
    public class OutputFileNameNotFoundException : Exception
    {
        public OutputFileNameNotFoundException() { }

        public OutputFileNameNotFoundException(string message)
       : base(message)
        {

        }
        public OutputFileNameNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

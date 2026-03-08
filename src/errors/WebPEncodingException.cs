using System;

namespace Libwebp.Net.errors
{
    /// <summary>
    /// Thrown when the native WebP encoder fails during encoding.
    /// </summary>
    public class WebPEncodingException : Exception
    {
        public WebPEncodingException() { }

        public WebPEncodingException(string message)
            : base(message) { }

        public WebPEncodingException(string message, Exception inner)
            : base(message, inner) { }
    }
}

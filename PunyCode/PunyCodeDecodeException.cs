using System;

namespace PunyCode
{
    public class PunyCodeDecodeException : Exception
    {
        public PunyCodeDecodeException()
        {
        }

        public PunyCodeDecodeException(string message)
            : base(message)
        {
        }

        public PunyCodeDecodeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

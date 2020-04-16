using System;
using System.Runtime.Serialization;

namespace StbSharp
{
    [Serializable]
    public class StbException : Exception
    {
        public StbException()
        {
        }

        public StbException(string message) : base(message)
        {
        }

        public StbException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StbException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
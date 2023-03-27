using System.Runtime.Serialization;

namespace WebMagic
{
    [Serializable]
    internal class KDLParserException : Exception
    {
        public KDLParserException()
        {
        }

        public KDLParserException(string? message) : base(message)
        {
        }

        public KDLParserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected KDLParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
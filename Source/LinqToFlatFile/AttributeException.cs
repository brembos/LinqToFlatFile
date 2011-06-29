using System;
using System.Runtime.Serialization;

namespace LinqToFlatFile
{
    [Serializable]
    public class AttributeException : Exception
    {
        public AttributeException()
            : base()
        {
        }

        public AttributeException(string message)
            : base(message)
        {
        }

        public AttributeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AttributeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

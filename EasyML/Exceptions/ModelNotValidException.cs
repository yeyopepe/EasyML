using System;
using System.Runtime.Serialization;

namespace EasyML.Exceptions
{
    [Serializable]
    public class ModelNotValidException : Exception
    {
        public ModelNotValidException() : base()
        { }

        public ModelNotValidException(string message) : base(message)
        { }

        public ModelNotValidException(string message, Exception innerException) : base(message, innerException)
        { }

        protected ModelNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}

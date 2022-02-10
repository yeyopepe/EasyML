using System;
using System.Runtime.Serialization;

namespace EasyML.Exceptions
{
    [Serializable]
    public class MLEngineException : Exception
    {
        public MLEngineException() : base()
        { }

        public MLEngineException(string message) : base(message)
        { }

        public MLEngineException(string message, Exception innerException) : base(message, innerException)
        { }

        protected MLEngineException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}

using System;
using System.Runtime.Serialization;

namespace EasyML.Exceptions
{
    [Serializable]
    public class SystemNotTrainedException : Exception
    {
        public SystemNotTrainedException() : base()
        { }

        public SystemNotTrainedException(string message) : base(message)
        { }

        public SystemNotTrainedException(string message, Exception innerException) : base(message, innerException)
        { }

        protected SystemNotTrainedException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}

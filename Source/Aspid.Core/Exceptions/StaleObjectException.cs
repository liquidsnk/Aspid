using System;
using System.Runtime.Serialization;

namespace Aspid.Core.Exceptions
{
    public class StaleObjectException : Exception
    {
        public StaleObjectException(string message)
            : base(message)
        {
        }

        public StaleObjectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected StaleObjectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

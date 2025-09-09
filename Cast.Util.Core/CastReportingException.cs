using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cast.Util
{
    [Serializable]
    public class CastReportingException : Exception
    {
        public CastReportingException(string message) : base(message) { }
        public CastReportingException(string message, Exception innerException) : base(message, innerException) { }

        // protected CastReportingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override bool Equals(object obj)
        {
            return obj is CastReportingException exception &&
                   EqualityComparer<Exception>.Default.Equals(InnerException, exception.InnerException) &&
                   Message == exception.Message &&
                   Source == exception.Source;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InnerException, Message, Source);
        }
    }
}

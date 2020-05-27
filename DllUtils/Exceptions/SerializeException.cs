using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class SerializeException : InjectionException
    {
        public SerializeException()
        {
        }

        public SerializeException(string message) : base(message)
        {
        }

        public SerializeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SerializeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

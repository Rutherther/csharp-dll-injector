using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class OpenHandleException : InjectionException
    {
        public OpenHandleException()
        {
        }

        public OpenHandleException(string message) : base(message)
        {
        }

        public OpenHandleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OpenHandleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

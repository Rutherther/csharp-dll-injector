using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class ThreadCreationException : InjectionException
    {
        public ThreadCreationException()
        {
        }

        public ThreadCreationException(string message) : base(message)
        {
        }

        public ThreadCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ThreadCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

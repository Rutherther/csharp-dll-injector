using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class MemoryWriteException : InjectionException
    {
        public MemoryWriteException()
        {
        }

        public MemoryWriteException(string message) : base(message)
        {
        }

        public MemoryWriteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MemoryWriteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

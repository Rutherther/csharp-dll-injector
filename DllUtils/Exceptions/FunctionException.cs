using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class FunctionException : InjectionException
    {
        public FunctionException()
        {
        }

        public FunctionException(string message) : base(message)
        {
        }

        public FunctionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FunctionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class ModuleException : InjectionException
    {
        public ModuleException()
        {
        }

        public ModuleException(string message) : base(message)
        {
        }

        public ModuleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModuleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

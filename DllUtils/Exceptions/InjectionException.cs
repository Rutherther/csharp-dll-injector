using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Exceptions
{
    public class InjectionException : Exception
    {
        public InjectionException()
        {
        }

        public InjectionException(string message) : base(message)
        {
        }

        public InjectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InjectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int LatestError => Marshal.GetLastWin32Error();
    }
}

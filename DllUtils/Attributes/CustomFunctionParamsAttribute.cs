using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Attributes
{
    /// <summary>
    /// Use this attribute to specify custom serialization to remote process.
    /// This is needed for example for arrays that should be given by ref.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CustomFunctionParamsAttribute : Attribute
    {
        public CustomFunctionParamsAttribute(EncodingType stringEncoding = EncodingType.Unicode)
        {
            StringEncoding = stringEncoding;
        }

        public EncodingType StringEncoding { get; set; }
    }

    public enum EncodingType
    {
        ASCII,
        Unicode,
        UTF8,
        UTF32
    }
}

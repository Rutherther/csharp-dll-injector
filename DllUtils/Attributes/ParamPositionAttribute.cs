using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllUtils.Attributes
{
    public enum ParamType
    {
        NotSpecified,
        Reference,
        Value
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ParamPositionAttribute : Attribute
    {
        public ParamPositionAttribute(int index, ParamType paramType = ParamType.NotSpecified)
        {
            Index = index;
            ParamType = paramType;
        }

        public ParamType ParamType { get; set; }

        public int Index { get; set; }
    }
}

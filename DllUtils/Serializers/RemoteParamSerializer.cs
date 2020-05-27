using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Attributes;
using DllUtils.Exceptions;
using DllUtils.Memory;
using DllUtils.Process;

namespace DllUtils.Serializers
{
    public class RemoteParamSerializer
    {
        public RemoteParamSerializer(ProcessHandle process)
        {
            Process = process;
        }

        public ProcessHandle Process { get; }

        public AllocatedMemory Serialize<T>(T param)
        {
            AllocatedMemory memory;

            if (param is int)
            {
                return new AllocatedMemory(Process, (IntPtr)Convert.ToInt32(param), null, sizeof(int));
            }

            if (param is Array array)
            {
                memory = SerializeArray(array);
            }
            else if (param.GetType().GetCustomAttribute<CustomFunctionParamsAttribute>() is CustomFunctionParamsAttribute attribute)
            {
                memory = SerializeCustom(param, attribute);
            }
            else
            {
                memory = SerializeUsingMarshal(param);
            }
            

            if (!memory.Alloc())
            {
                throw new MemoryAllocationException("Failed to allocate memory.");
            }

            if (!memory.Write())
            {
                throw new MemoryWriteException("Failed to write to allocated memory.");
            }

            return memory;
        }

        public AllocatedMemory SerializeArray(Array array)
        {
            byte[] bytes = SerializeArrayToBytes(array, out int size);
            return new AllocatedMemory(Process, IntPtr.Zero, bytes, size);
        }

        public AllocatedMemory SerializeCustom<T>(T param, CustomFunctionParamsAttribute customOptions)
        {
            System.Type type = param.GetType();
            IEnumerable<FieldInfo> fields = type.GetFields();
            List<ParamData> dataList = new List<ParamData>();

            foreach (FieldInfo field in fields)
            {
                ParamPositionAttribute positionAttribute = field.GetCustomAttribute<ParamPositionAttribute>();

                if (positionAttribute == null)
                {
                    continue;
                }

                ParamData currentData = new ParamData
                {
                    Position = positionAttribute.Index,
                    ParamType = positionAttribute.ParamType,
                    Value = field.GetValue(param)
                };
                int size;

                if (currentData.ParamType == ParamType.NotSpecified)
                {
                    currentData.ParamType = type.IsValueType ? ParamType.Reference : ParamType.Value;
                }

                if (currentData.Value is string stringVal)
                {
                    switch (customOptions.StringEncoding)
                    {
                        case EncodingType.ASCII:
                            currentData.Value = Encoding.ASCII.GetBytes(stringVal);
                            break;
                        case EncodingType.UTF8:
                            currentData.Value = Encoding.UTF8.GetBytes(stringVal);
                            break;
                        case EncodingType.UTF32:
                            currentData.Value = Encoding.UTF32.GetBytes(stringVal);
                            break;
                        case EncodingType.Unicode:
                            currentData.Value = Encoding.Unicode.GetBytes(stringVal);
                            break;
                    }
                }

                if (currentData.ParamType == ParamType.Reference)
                {

                    currentData.Memory = Serialize(currentData.Value);
                    currentData.Size = Marshal.SizeOf(typeof(IntPtr));
                    currentData.Data = SerializeToBytesUsingMarshal(currentData.Memory.Address, out _);
                }
                else
                {
                    if (currentData.Value is Array array)
                    {
                        currentData.Data = SerializeArrayToBytes(array, out size);
                        currentData.Size = size;
                    }
                    else
                    {
                        currentData.Data = SerializeToBytesUsingMarshal(currentData.Value, out size);
                        currentData.Size = size;
                    }
                }

                dataList.Add(currentData);
            }

            int totalSize = dataList.Sum(x => x.Size);
            int currentPosition = 0;
            byte[] bytes = new byte[totalSize];
            foreach (ParamData data in dataList.OrderBy(x => x.Position))
            {
                Array.Copy(data.Data, 0, bytes, currentPosition, data.Size);
                currentPosition += data.Size;
            }

            return new AllocatedMemory(Process, IntPtr.Zero, bytes, totalSize);
        }

        public AllocatedMemory SerializeUsingMarshal<T>(T param)
        {
            byte[] bytes = SerializeToBytesUsingMarshal(param, out int size);
            AllocatedMemory memory = new AllocatedMemory(Process, IntPtr.Zero, bytes, size);

            return memory;
        }

        protected byte[] SerializeArrayToBytes(Array array, out int size)
        {
            int elementSize;

            try
            {
                elementSize = Marshal.SizeOf(array.GetType().GetElementType());
            }
            catch (Exception exception)
            {
                throw new SerializationException("Only arrays of simple structures allowed.", exception);
            }

            size = elementSize * array.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            IntPtr current = ptr;

            foreach (object value in array)
            {
                Marshal.StructureToPtr(value, current, false);
                current += elementSize;
            }

            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);

            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        protected byte[] SerializeToBytesUsingMarshal<T>(T param, out int size)
        {
            size = Marshal.SizeOf(param);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(param, ptr, false);

            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);

            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        private class ParamData
        {
            public int Position { get; set; }

            public ParamType ParamType { get; set; }

            public int Size { get; set; }

            public object Value { get; set; }

            public AllocatedMemory Memory { get; set; }

            public byte[] Data { get; set; }
        }
    }
}

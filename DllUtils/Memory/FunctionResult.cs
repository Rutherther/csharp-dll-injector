using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Exceptions;
using DllUtils.Interop;
using DllUtils.Process;

namespace DllUtils.Memory
{
    public class FunctionResult
    {
        public FunctionResult(ProcessHandle process, IntPtr address)
        {
            Process = process;
            Address = address;
        }

        public ProcessHandle Process { get; }

        public IntPtr Address { get; }

        public T To<T>(bool reference = true)
        {
            if (typeof(T).IsPrimitive)
            {
                return (T) Convert.ChangeType((int)Address, typeof(T));
            }

            int size = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            Kernel32.ReadProcessMemory(Process.Handle, Address, bytes, (uint)size, out int bytesRead);

            if (bytesRead != size)
            {
                throw new FunctionException("Whole function result could not be read.");
            }

            GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T obj = Marshal.PtrToStructure<T>(gcHandle.AddrOfPinnedObject());
            gcHandle.Free();

            return obj;
        }
    }
}

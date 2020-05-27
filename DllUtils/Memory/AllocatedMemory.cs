using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Interop;
using DllUtils.Process;

namespace DllUtils.Memory
{
    public class AllocatedMemory
    {
        public AllocatedMemory(ProcessHandle process, IntPtr address, byte[] data, int length)
        {
            Process = process;
            Address = address;
            Data = data;
            Length = length;
        }

        public ProcessHandle Process { get; }

        public bool IsAllocated { get; private set; }
        public bool IsFreed { get; private set; }

        public byte[] Data { get; }

        public int Length { get; }

        public IntPtr Address { get; private set; }

        public bool Alloc()
        {
            if (!IsAllocated)
            {
                Address = Kernel32.VirtualAllocEx(Process.Handle, (IntPtr) null, (IntPtr) Length, 0x1000, 0x40);

                if (Address != IntPtr.Zero)
                {
                    IsAllocated = true;
                }
            }

            return Address != IntPtr.Zero;
        }

        public bool Write()
        {
            if (!IsAllocated)
            {
                return false;
            }

            if (Kernel32.WriteProcessMemory(Process.Handle, Address, Data, (uint) Length, out int bytesWritten) == 0)
            {
                return false;
            }

            return bytesWritten == Length;
        }

        public void Free()
        {
            if (!IsFreed)
            {
                IsFreed = Kernel32.VirtualFreeEx(Process.Handle, Address, UIntPtr.Zero, 0x8000);
                IsAllocated = !IsFreed;
                Address = IntPtr.Zero;
            }
        }
    }
}

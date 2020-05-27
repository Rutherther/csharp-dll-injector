using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Exceptions;
using DllUtils.Interop;

namespace DllUtils.Process
{
    public class ProcessHandle
    {
        public ProcessHandle(System.Diagnostics.Process process)
        {
            Process = process;
        }

        public IntPtr Handle { get; private set; }

        public bool IsOpened { get; private set; }

        public System.Diagnostics.Process Process { get; }

        public string ProcessName => Process.ProcessName;

        public int ProcessId => Process.Id;

        public void Close()
        {
            if (Handle != IntPtr.Zero && IsOpened)
            {
                Kernel32.CloseHandle(Handle);
                Handle = IntPtr.Zero;
            }

            IsOpened = false;
        }

        public void Open()
        {
            if (!IsOpened || Handle == IntPtr.Zero)
            {
                Handle = Kernel32.OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, (uint)ProcessId);

                if (Handle == IntPtr.Zero)
                {
                    throw new OpenHandleException($"Failed to open process handle for {ProcessName}");
                }
            }

            IsOpened = true;
        }
    }
}

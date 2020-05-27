using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Interop;
using DllUtils.Process;

namespace DllUtils.Threads
{
    public class RemoteThread
    {
        private int _waitForSingleObjectResult;
        private bool _waitedForObject;

        public RemoteThread(ProcessHandle process, IntPtr startAddress, IntPtr param, IntPtr handle)
        {
            Handle = handle;
            if (handle != IntPtr.Zero)
            {
                IsOpened = true;
            }

            StartAddress = startAddress;
            Param = param;
            Handle = handle;
            Process = process;
        }

        public bool IsOpened { get; private set; }

        public IntPtr Handle { get; private set; }

        public IntPtr StartAddress { get; }

        public IntPtr Param { get; }

        public ProcessHandle Process { get; }


        public bool Create()
        {
            if (!IsOpened)
            {
                Process.Open();

                Handle = Kernel32.CreateRemoteThread(Process.Handle, (IntPtr) null, IntPtr.Zero, StartAddress, Param, 0,
                    (IntPtr) null);

                int error = Marshal.GetLastWin32Error();

                if (Handle != IntPtr.Zero)
                {
                    IsOpened = true;
                }
            }

            return Handle != IntPtr.Zero;
        }

        public int WaitForResult(int timeout = 100000)
        {
            _waitForSingleObjectResult = Kernel32.WaitForSingleObject(Handle, timeout);
            _waitedForObject = true;

            return _waitForSingleObjectResult;
        }

        public bool CheckResult()
        {
            if (!_waitedForObject)
            {
                WaitForResult();
            }

            int result = _waitForSingleObjectResult;
            if (result == 0x00000080L || result == 0x00000102L || result == 0xFFFFFFF)
            { 
                Close();
                return false;
            }

            return true;
        }

        public IntPtr GetResult()
        {
            bool success = Kernel32.GetExitCodeThread(Handle, out IntPtr result);
            if (!success)
            {
                return IntPtr.Zero;
            }

            return result;
        }

        public void Close()
        {
            if (Handle != IntPtr.Zero)
            {
                Kernel32.CloseHandle(Handle);
                IsOpened = false;
                Handle = IntPtr.Zero;
            }
        }
    }
}

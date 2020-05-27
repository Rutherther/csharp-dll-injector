using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Exceptions;
using DllUtils.Interop;
using DllUtils.Modules;

namespace DllUtils.Process
{
    public class RemoteProcessHandle : ProcessHandle
    {
        public RemoteProcessHandle(System.Diagnostics.Process process) : base(process)
        {
        }

        public InjectedModule Inject(string dllPath)
        {
            Open();

            RemoteModule kernel32 = GetRemoteKernel32();
            string fullPath = Path.GetFullPath(dllPath);
            IntPtr loadedLibrary = kernel32.ExecuteFunction("LoadLibraryA", Encoding.ASCII.GetBytes(fullPath)).Address;

            if (loadedLibrary == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                ;
                throw new ModuleException($"Failed to inject {dllPath} library.");
            }

            IntPtr localHandle = Kernel32.LoadLibraryA(fullPath);
            if (localHandle == IntPtr.Zero)
            {
                throw new ModuleException($"Failed to load {fullPath} to local process.");
            }

            return new InjectedModule(loadedLibrary, localHandle, this, dllPath);
        }

        public RemoteModule GetRemoteKernel32()
        {
            IntPtr kernel32 = Kernel32.GetModuleHandle("kernel32.dll");
            if (kernel32 == IntPtr.Zero)
            {
                throw new ModuleException("Failed to load kernel32.dll.");
            }

            return new RemoteModule(kernel32, kernel32, this);
        }
    }
}

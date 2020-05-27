using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Interop;
using DllUtils.Memory;
using DllUtils.Process;

namespace DllUtils.Modules
{
    public class InjectedModule : RemoteModule
    {
        public InjectedModule(IntPtr remoteHandle, IntPtr localHandle, RemoteProcessHandle processHandle, string dllPath)
            : base(remoteHandle, localHandle, processHandle)
        {
            DllPath = dllPath;
        }

        public string DllPath { get; }

        public bool FreeLibrary()
        {
            RemoteModule kernel32 = Process.GetRemoteKernel32();
            FunctionResult result = kernel32.ExecuteFunction("FreeLibrary", RemoteHandle);

            return result.To<bool>();
        }
    }
}

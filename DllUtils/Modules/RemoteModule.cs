using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Exceptions;
using DllUtils.Interop;
using DllUtils.Memory;
using DllUtils.Process;
using DllUtils.Serializers;
using DllUtils.Threads;

namespace DllUtils.Modules
{
    public class RemoteModule : IModule
    {
        public RemoteModule(IntPtr remoteHandle, IntPtr localHandle, RemoteProcessHandle processHandle)
        {
            Process = processHandle;
            RemoteHandle = remoteHandle;
            LocalHandle = localHandle;
        }

        public IntPtr LocalHandle { get; }

        public IntPtr RemoteHandle { get; }

        public RemoteProcessHandle Process { get; }

        public virtual IntPtr FindFunctionHandle(string name)
        {
            Process.Open();

            IntPtr localAddress = Kernel32.GetProcAddress(LocalHandle, name);
            long functionOffset = ((long)localAddress) - ((long)LocalHandle);
            long remoteAddressLong = ((long)RemoteHandle) + functionOffset;
            IntPtr remoteAddress = new IntPtr(remoteAddressLong);

            if (remoteAddress == IntPtr.Zero)
            { 
                throw new FunctionException($"Function {name} not found.");
            }

            return remoteAddress;
        }

        public virtual FunctionResult ExecuteFunction(IntPtr function)
        {
            return ExecuteFunctionRaw(function, null);
        }

        public virtual FunctionResult ExecuteFunction<T>(IntPtr function, T param)
        {
            return ExecuteFunctionRaw(function, GetParam(param));
        }

        public virtual FunctionResult ExecuteFunction(string function)
        {
            return ExecuteFunction(FindFunctionHandle(function));
        }

        public virtual FunctionResult ExecuteFunction<T>(string function, T param)
        {
            return ExecuteFunction(FindFunctionHandle(function), param);
        }

        public Task<IntPtr> FindFunctionHandleAsync(string name)
            => new Task<IntPtr>(() => FindFunctionHandle(name));

        public Task<FunctionResult> ExecuteFunctionAsync(IntPtr function)
            => new Task<FunctionResult>(() => ExecuteFunction(function));

        public Task<FunctionResult> ExecuteFunctionAsync(string function)
            => new Task<FunctionResult>(() => ExecuteFunction(function));

        public Task<FunctionResult> ExecuteFunctionAsync<T>(string function, T param)
            => new Task<FunctionResult>(() => ExecuteFunction(function, param));

        public virtual ParamData GetParam<T>(T param)
        {
            RemoteParamSerializer serializer = new RemoteParamSerializer(Process);

            if (param.GetType().IsPrimitive)
            {
                return new ParamData
                {
                    IsRemote = false,
                    Value = Marshal.ReadIntPtr(param, 0)
                };
            }

            AllocatedMemory memory = serializer.Serialize(param);
            return new ParamData
            {
                Allocated = memory,
                IsRemote = true,
                Value = memory.Address
            };
        }

        public virtual FunctionResult ExecuteFunctionRaw(IntPtr function, ParamData param)
        {
            Process.Open();
            RemoteThread thread = new RemoteThread(Process, function, param?.Value ?? IntPtr.Zero, IntPtr.Zero);

            if (!thread.Create())
            {
                throw new ThreadCreationException("Could not create remote thread. Check if architecture of the remote process matches local process");
            }

            thread.WaitForResult(10 * 1000);
            thread.CheckResult();

            FunctionResult result = new FunctionResult(Process, thread.GetResult());

            if (param?.IsRemote ?? false)
            {
                param.Allocated.Free();
            }

            return result;
        }

        public class ParamData
        {
            public bool IsRemote { get; set; }

            public IntPtr Value { get; set; }

            public AllocatedMemory Allocated { get; set; }
        }
    }
}

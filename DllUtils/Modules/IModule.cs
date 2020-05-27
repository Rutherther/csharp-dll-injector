using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Memory;

namespace DllUtils.Modules
{
    public interface IModule
    {
        IntPtr LocalHandle { get; }

        IntPtr RemoteHandle { get; }

        IntPtr FindFunctionHandle(string name);

        FunctionResult ExecuteFunction(IntPtr function);

        FunctionResult ExecuteFunction<T>(IntPtr function, T param);

        FunctionResult ExecuteFunction(string function);

        FunctionResult ExecuteFunction<T>(string function, T param);

        Task<IntPtr> FindFunctionHandleAsync(string name);

        Task<FunctionResult> ExecuteFunctionAsync(IntPtr function);

        Task<FunctionResult> ExecuteFunctionAsync(string function);

        Task<FunctionResult> ExecuteFunctionAsync<T>(string function, T param);
    }
}

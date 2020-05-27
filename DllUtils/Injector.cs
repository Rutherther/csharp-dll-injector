using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllUtils.Modules;
using DllUtils.Process;

namespace DllUtils
{
    public static class Injector
    {
        /// <summary>
        /// Returns RemoteProcessHandle that can be used to Inject dll
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static RemoteProcessHandle GetRemoteProcess(string processName)
        {
            return GetRemoteProcess(System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault());
        }

        /// <summary>
        /// Returns RemoteProcessHandle that can be used to Inject dll
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static RemoteProcessHandle GetRemoteProcess(int processId)
        {
            return GetRemoteProcess(System.Diagnostics.Process.GetProcessById(processId));
        }

        /// <summary>
        /// Returns RemoteProcessHandle that can be used to Inject dll
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static RemoteProcessHandle GetRemoteProcess(System.Diagnostics.Process process)
        {
            if (process == null)
            {
                return null;
            }

            return new RemoteProcessHandle(process);
        }

        /// <summary>
        /// Injects dll into process using its name
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static InjectedModule Inject(string processName, string dllPath)
            => GetRemoteProcess(processName).Inject(dllPath);

        /// <summary>
        /// Injects dll into process using its id
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static InjectedModule Inject(int processId, string dllPath)
            => GetRemoteProcess(processId).Inject(dllPath);

        /// <summary>
        /// Injects dll into process using System.Diagnostics.Process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static InjectedModule Inject(System.Diagnostics.Process process, string dllPath)
            => GetRemoteProcess(process).Inject(dllPath);
    }
}

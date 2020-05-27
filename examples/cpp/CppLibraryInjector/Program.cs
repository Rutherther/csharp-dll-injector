using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DllUtils;
using DllUtils.Attributes;
using DllUtils.Memory;
using DllUtils.Modules;
using DllUtils.Process;

namespace CppLibraryInjector
{
    class Program
    {
        // To pass string in parameter CustomFunctionParams must be used
        // This will work only with cpp
        [CustomFunctionParams]
        public struct MyFunctionParams
        {
            [ParamPosition(0)]
            public string Param;

            [ParamPosition(1)]
            public int Number;
        }

        // For pritimitives StructLayout can be used
        // The struct/class will be serialized using Marshal
        // in cpp, you must specify it as a pointer Add(AddParams* params);
        // in c# it has to be class so it is given by reference, not by value

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AddParams
        {
            [MarshalAs(UnmanagedType.I4)]
            public int First;

            [MarshalAs(UnmanagedType.I4)]
            public int Second;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to CsharpInjector demo for DllExport library");
            Console.WriteLine("Just a small reminder - you cannot inject to every process using this injector");
            Console.WriteLine("Be sure to inject only to processes that match process configuration you use for this project (x86, x64)");

            Console.Write("Put in name (without .exe) or id of the process you want to inject in: ");
            string read = Console.ReadLine();
            RemoteProcessHandle processHandle;

            if (int.TryParse(read, out int processId))
            {
                processHandle = Injector.GetRemoteProcess(processId);
            }
            else
            {
                processHandle = Injector.GetRemoteProcess(read);
            }

            if (processHandle == null)
            {
                Console.WriteLine("Process not found.");
                Console.Read();
                return;
            }

            string dllPath = "CppLibrary.dll";
            if (!File.Exists(dllPath))
            {
                Console.WriteLine("Dll to inject not found.");
            }

            InjectedModule injectedModule = processHandle.Inject(dllPath);
            Console.WriteLine("Module injected");

            Console.WriteLine("Calling Main function to alloc console");
            FunctionResult result = injectedModule.ExecuteFunction("Main");
            int mainResult = result.To<int>();

            Console.WriteLine($"Result from main: {mainResult} (should be 1)");

            Console.WriteLine($"Sleeping for 1 second");
            Thread.Sleep(1000);

            // Execute addition function and get its params
            Console.WriteLine("Calling Addition with params of 10 and 5; Expected result: 15");
            result = injectedModule.ExecuteFunction("Add", new AddParams
            {
                First = 5,
                Second = 10
            });

            int additionResult = result.To<int>();
            Console.WriteLine("Result: " + additionResult);

            injectedModule.ExecuteFunction("MyFunction", new MyFunctionParams
            {
                Param = "Injected string!",
                Number = 1000
            });

            injectedModule.Process.Close();
            Console.ReadLine();
        }
    }
}

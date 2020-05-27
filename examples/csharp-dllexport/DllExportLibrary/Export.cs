using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DllExportLibrary
{
    public static class Export
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr AllocConsole();

        public unsafe struct MyFunctionParams
        {
            public byte* Param;
            public int Number;
        }
        

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class AddParams
        {
            [MarshalAs(UnmanagedType.I4)]
            public int First;

            [MarshalAs(UnmanagedType.I4)]
            public int Second;
        }

        [DllExport]
        public static void MyFunction(IntPtr pars)
        {
            Console.WriteLine("Hello from injected dll Export::MyFunction!");

            unsafe
            {
                byte[] bytes = new byte[64];
                int i = 0;

                MyFunctionParams* myParams = (MyFunctionParams*) pars;

                byte* param = myParams->Param - 1;

                while (*(++param) != 0)
                {
                    bytes[i++] = *param;
                }

                string ascii = Encoding.ASCII.GetString(bytes);

                Console.WriteLine($"The passed param is: {ascii}");
                Console.WriteLine($"Random number passed: {myParams->Number}");
            }
        }

        [DllExport]
        public static int Main()
        {
            AllocConsole();
            Console.WriteLine("Hello from injected dll Export::Main!");

            return 1;
        }

        [DllExport]
        public static int Add(AddParams pars)
        {
            Console.WriteLine($"Performing addition of numbers {pars.First} and {pars.Second}");

            return pars.First + pars.Second;
        }
    }
}

using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace nvm
{
    class Program
    {
        public static void Main(string[] args)
        {
            VirtualMachine vm = new VirtualMachine(new Class[0], new byte[5], 1, 1, 100);
            vm.DEBUG = true;

            vm.Run();

            Console.ReadKey();
        }
    }
}

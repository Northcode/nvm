using nvm.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace nvm
{
    class Program
    {
        public static void Main(string[] args)
        {
<<<<<<< HEAD
            VirtualMachine vm = new VirtualMachine(new Class[0], new byte[5], 1, 1, 100);
            vm.DEBUG = true;

            vm.Run();
=======
            VirtualMachine.InitOpCodes();
            VirtualMachine.InitInterupt();
            VirtualMachine vm = new VirtualMachine();

            Console.Write("File to load: ");
            string file = Console.ReadLine();
            byte[] fdata = File.ReadAllBytes(file);
            Buffer mem = new Buffer(2048, fdata);
            vm.memory = mem;
            MemoryManager mm = new MemoryManager(vm, 0, (uint)fdata.Length + 10, (uint)fdata.Length + 256, (uint)fdata.Length + 512);
            vm.manager = mm;

            vm.Run(true);
>>>>>>> comit

            Console.ReadKey();
        }
    }
}

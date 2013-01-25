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
            VirtualMachine.InitOpCodes();
            VirtualMachine vm = new VirtualMachine();
            Buffer mem = new Buffer(512);
            vm.memory = mem;
            MemoryManager mm = new MemoryManager(vm, 0, 1, 1, 30);
            vm.manager = mm;

            CodeBuilder cb = new CodeBuilder(1024);
            cb.WriteBytes((byte)0x0d, (int)2000);
            cb.WriteBytes((byte)0x0e, (int)660);
		    cb.WriteBytes((byte)0x1a, (byte)0xed);
            cb.WriteBytes((byte)0x11);

            mem = new Buffer(512) { data = cb.GetCode() };
            vm.memory = mem;
            mm = new MemoryManager(vm, 0, 32, 256, 512);
            vm.manager = mm;

            vm.Run(true);

            Console.ReadKey();
        }
    }
}

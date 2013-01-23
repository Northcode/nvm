using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
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

            CodeBuilder cb = new CodeBuilder(512);
            cb.WriteBytes(VirtualMachine.codes[2].GetByteCode(), (byte)2);
            cb.WriteBytes(VirtualMachine.codes[3].GetByteCode(), (byte)3);
            cb.WriteBytes(VirtualMachine.codes[0x10].GetByteCode(), (byte)0x23);
            cb.WriteBytes(VirtualMachine.codes[0x11].GetByteCode());

            mem = new Buffer(512) { data = cb.GetCode() };
            vm.memory = mem;
            mm = new MemoryManager(vm, 0, 20, 50, 100);
            vm.manager = mm;

            vm.Run();

            vm.RegDump();

            Console.ReadKey();
        }
    }
}

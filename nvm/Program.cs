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

            CodeBuilder cb = new CodeBuilder(1024);
            cb.WriteBytes(VirtualMachine.codes[2].GetByteCode(), (byte)2);
            cb.WriteBytes(VirtualMachine.codes[3].GetByteCode(), (byte)3);
            cb.WriteBytes(VirtualMachine.codes[0x10].GetByteCode(), (byte)0x23);
            cb.WriteBytes(VirtualMachine.codes[0x11].GetByteCode());

            mem = new Buffer(512) { data = cb.GetCode() };
            vm.memory = mem;
            mm = new MemoryManager(vm, 0, 32, 256, 512);
            vm.manager = mm;

            vm.Run();

            vm.RegDump();

            mm.Push(45);
            mm.Push(13);
            mm.Push(300);
            mm.Push("Test");

            mm.MemDump(mm.stackAddr,mm.staticAddr);
            vm.RegDump();

            string t = mm.PopS();
            int a = mm.Pop32();
            int b = mm.Pop32();
            int c = mm.Pop32();

            vm.RegDump();

            Console.WriteLine(t);
            Console.WriteLine(a);

            Console.WriteLine(b);
            Console.WriteLine(c);
            Console.ReadKey();
        }
    }
}

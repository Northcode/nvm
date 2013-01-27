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
            VirtualMachine.InitInterupt();
            VirtualMachine vm = new VirtualMachine();
            Buffer mem = new Buffer(512);
            vm.memory = mem;
            MemoryManager mm = new MemoryManager(vm, 0, 1, 1, 30);
            vm.manager = mm;

            CodeBuilder cb = new CodeBuilder(1024);
            cb.WriteBytes((byte)0x0f, (int)100);
            cb.WriteBytes((byte)0x24);
            cb.WriteBytes((byte)0x11);
            cb.Jump(100);
            cb.WriteBytes((byte)0x0f, (int)200);
            cb.WriteBytes((byte)0x22);
            cb.WriteBytes((byte)0x0f, 150);
            cb.WriteBytes((byte)0x23);
            cb.WriteBytes((byte)0x01, (byte)0x01);
            cb.WriteBytes((byte)0x21, (byte)0x00);
            cb.WriteBytes((byte)0x25);
            cb.Jump(200);
            cb.WriteBytes("Hello World");

            mem = new Buffer(512) { data = cb.GetCode() };
            vm.memory = mem;
            mm = new MemoryManager(vm, 0, 32, 256, 512);
            vm.manager = mm;

            vm.Run(true);

            Console.ReadKey();
        }
    }
}

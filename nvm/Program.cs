using nvm.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using nvm.Assembly;

namespace nvm
{
    class Program
    {
        public static void Main(string[] args)
        {
            VirtualMachine.InitOpCodes();
            Assembler a = new Assembler();

            a.code = File.ReadAllText("test.txt");

            byte[] code = a.Assemble();

            Console.Clear();

            VirtualMachine vm = new VirtualMachine(code, 100, 100, 1024);
            vm.DEBUG = false;

            vm.Run();
        }
    }
}

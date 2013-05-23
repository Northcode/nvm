using System;
/*
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
*/
using System.IO;
namespace nvmv2
{
    class Startup
    {
        static void Main(string[] args)
        {
            VM.InitOpcodes();

            Assembler a = new Assembler();
            a.code = File.ReadAllText("testcode.txt");
            byte[] code = a.Assemble();

            Console.Clear();

            VM v = new VM(code);
            v.Run();
            Console.ReadKey();
        }
    }
}
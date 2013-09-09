using ncc.AST;
using nvm.v2;
using nvm.v2.Assembly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncc
{
    class Program
    {
        static void Main(string[] args)
        {
            string c = "";
            if (args.Length > 0)
            {
                c = File.ReadAllText(args[0]);
            }
            else
            {
                c = File.ReadAllText("tocpp.txt");
            }

            Scanner s = new Scanner(c);
            s.Scan();
            foreach (Token t in s.tokens)
            {
                Console.WriteLine(t.type + " : " + t.val);
            }

            Parser p = new Parser(s.tokens.ToArray());
            p.Parse();

            CodeGenerator.astTree = p.stmts.ToArray();
            string ILcode = CodeGenerator.GenerateNIL();

            Console.WriteLine("----------------- ASM CODE -------------------------");
            Console.Write(ILcode);

            CompilerMeta cm = new CompilerMeta();
            cm.ProgramName = "Test";
            cm.localMeta = VarnameLocalizer.GetLocalMeta();

            Console.WriteLine("--------------- BYTES ---------------");

            VM.InitOpcodes();
            Assembler a = new Assembler();
            a.CompilerMeta = cm;
            a.code = ILcode;
            NcAssembly code = a.Assemble();

            foreach (byte b in code.code)
            {
                Console.Write("0x" + b.ToString("X").PadLeft(2,'0') + ",");
            }

            Console.ReadKey();

            Console.WriteLine("--------------- OUTPUT --------------");

            VM v = new VM(code);
            v.metadata = a.programMeta;
            v.Run();
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}

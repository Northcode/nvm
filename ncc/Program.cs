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
            string c = File.ReadAllText("testlang.txt");

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
            Console.ReadKey();

            CompilerMeta cm = new CompilerMeta();
            cm.ProgramName = "Test";
            cm.localMeta = VarnameLocalizer.GetLocalMeta();

            Console.WriteLine("--------------- OUTPUT --------------");
            
            VM.InitOpcodes();
            Assembler a = new Assembler();
            a.CompilerMeta = cm;
            a.code = ILcode;
            NcAssembly code = a.Assemble();

            VM v = new VM(code);
            v.metadata = a.programMeta;
            v.Run();
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}

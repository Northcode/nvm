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

            foreach (STMT st in p.stmts)
            {
                Console.WriteLine(st.GetType().Name);
            }

            StringBuilder sb = new StringBuilder();
            foreach (STMT st in p.stmts)
            {
                sb.Append(st.ToAsm(""));
            }

            StringBuilder sba = new StringBuilder();
            sba.AppendLine("LOCALCNT " + VarnameLocalizer.locals.Count);
            sba.AppendLine(sb.ToString());
            sba.AppendLine("END");
            Console.WriteLine("----------------- ASM CODE -------------------------");
            Console.Write(sba.ToString());
            Console.ReadKey();

            CompilerMeta cm = new CompilerMeta();
            cm.ProgramName = "Test";
            cm.localMeta = VarnameLocalizer.GetLocalMeta();

            Console.WriteLine("--------------- OUTPUT --------------");
            
            VM.InitOpcodes();
            Assembler a = new Assembler();
            a.CompilerMeta = cm;
            a.code = sba.ToString();
            byte[] code = a.Assemble();

            VM v = new VM(code);
            v.Run();
            Console.ReadKey();
        }
    }
}

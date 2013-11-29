using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nvm2.compiler;
using nvm2.compiler.ast;

namespace nvm2
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = Console.ReadLine();
            Scanner scan = new Scanner();
            scan.Code = code;
            scan.ScanAll();
            foreach (Token t in scan.Tokens)
            {
                Console.WriteLine(t.ToString());
            }
            Parser parse = new Parser();
            parse.Tokens = scan.Tokens.ToArray();
            stmt s = parse.ParseStmt();
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}

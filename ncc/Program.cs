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
            string c = File.ReadAllText("testcode.txt");
            Scanner s = new Scanner(c);
            s.Scan();
            foreach (Token t in s.tokens)
            {
                Console.WriteLine(t.type + " : " + t.val);
            }
            Console.ReadKey();
        }
    }
}

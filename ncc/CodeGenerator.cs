using ncc.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncc
{
    public static class CodeGenerator
    {
        public static STMT[] astTree { get; set; }

        public static string GenerateNIL()
        {
            StringBuilder sb = new StringBuilder();
            foreach (STMT st in astTree)
            {
                sb.Append(st.ToAsm(""));
            }

            sb.AppendLine(LambdaHolder.WriteLambdas());

            StringBuilder sba = new StringBuilder();
            
            sba.AppendLine(sb.ToString());
            sba.AppendLine("END");

            return sba.ToString();
        }
    }
}

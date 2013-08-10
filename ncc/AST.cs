using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncc
{
    namespace AST
    {
        public interface STMT
        {
            string ToAsm(string scope);
        }

        public interface EXPR : STMT
        {
        }

        class bytelit : EXPR
        {
            public byte value;

            public string ToAsm(string scope)
            {
                return "PUSH byte h" + value.ToString("X") + "\n";
            }
        }

        class intlit : EXPR
        {
            public int value;

            public string ToAsm(string scope)
            {
                return "PUSH int " + value.ToString() + "\n";
            }
        }

        class stringlit : EXPR
        {
            public string value;

            public string ToAsm(string scope)
            {
                return "PUSH string \"" + value.Replace("\"", "\\'") + "\"" + "\n";
            }
        }

        static class VarnameLocalizer
        {
            public static Dictionary<string, int> locals = new Dictionary<string,int>();

            internal static Dictionary<int, string> GetLocalMeta()
            {
                Dictionary<int, string> meta = new Dictionary<int, string>();
                foreach (KeyValuePair<string,int> item in locals)
                {
                    meta.Add(item.Value, item.Key);
                }
                return meta;
            }
        }

        class setvar : STMT
        {
            public string varname;
            public EXPR value;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(value.ToAsm(scope));
                if (VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + varname))
                {
                    sb.AppendLine("FREELOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                    sb.AppendLine("STLOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                else
                {
                    VarnameLocalizer.locals.Add((scope != "" ? scope + "." : "") + varname, VarnameLocalizer.locals.Count);
                    sb.AppendLine("STLOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                return sb.ToString();
            }
        }

        class setvarptr : STMT
        {
            public string varname;
            public EXPR value;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                if (value is fcall)
                {
                    sb.AppendLine("PUSH uint :" + (value as fcall).fname);
                }
                else
                {
                    sb.AppendLine(value.ToAsm(scope));
                }
                if (VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + varname))
                {
                    //sb.AppendLine("FREELOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                    sb.AppendLine("STPTR " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                else
                {
                    VarnameLocalizer.locals.Add((scope != "" ? scope + "." : "") + varname, VarnameLocalizer.locals.Count);
                    sb.AppendLine("STPTR " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                return sb.ToString();
            }
        }

        class getvar : EXPR
        {
            public string varname;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                if (VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + varname))
                {
                    sb.AppendLine("LDLOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                else if (VarnameLocalizer.locals.ContainsKey(varname))
                {
                    sb.AppendLine("LDLOC " + VarnameLocalizer.locals[varname]);
                }
                else
                {
                    throw new Exception("Failed to load var, Unknown variable: " + varname);
                }
                return sb.ToString();
            }
        }

        class getvarptr : EXPR
        {
            public string varname;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                if (VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + varname))
                {
                    sb.AppendLine("LDPTR " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                else if (VarnameLocalizer.locals.ContainsKey(varname))
                {
                    sb.AppendLine("LDPTR " + VarnameLocalizer.locals[varname]);
                }
                else
                {
                    throw new Exception("Failed to load pointer, Unknown variable: " + varname);
                }
                return sb.ToString();
            }
        }

        class function : STMT
        {
            public string fname;
            public string[] args;
            public STMT[] body;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(fname + ":");

                foreach (string arg in args)
                {
                    if (arg.StartsWith("@"))
                    {
                        if (!VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + fname + "." + arg.Substring(1)))
                        {
                            VarnameLocalizer.locals.Add((scope != "" ? scope + "." : "") + fname + "." + arg.Substring(1), VarnameLocalizer.locals.Count);
                        }
                        sb.AppendLine("STPTR " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + fname + "." + arg.Substring(1)]);
                    }
                    else
                    {
                        if (!VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + fname + "." + arg))
                        {
                            VarnameLocalizer.locals.Add((scope != "" ? scope + "." : "") + fname + "." + arg, VarnameLocalizer.locals.Count);
                        }
                        sb.AppendLine("STLOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + fname + "." + arg]);
                    }
                }

                foreach (STMT st in body)
                {
                    sb.Append(st.ToAsm((scope != "" ? scope + "." : "") + fname));
                }

                sb.AppendLine("RET");
                return sb.ToString();
            }
        }

        class ret : STMT
        {
            public EXPR val;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(val.ToAsm(scope));
                sb.AppendLine("RET");
                return sb.ToString();
            }
        }

        class fcall : EXPR
        {
            public string fname;
            public EXPR[] args;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = args.Length - 1; i >= 0; i--)
                {
                    sb.AppendLine(args[i].ToAsm(scope));
                }

                sb.AppendLine("CALL :" + fname);
                return sb.ToString();
            }
        }

        class end : STMT
        {
            public string ToAsm(string scope)
            {
                return "";
            }
        }

        class free : STMT
        {
            public string varname;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                if (VarnameLocalizer.locals.ContainsKey((scope != "" ? scope + "." : "") + varname))
                {
                    sb.AppendLine("FREELOC " + VarnameLocalizer.locals[(scope != "" ? scope + "." : "") + varname]);
                }
                return sb.ToString();
            }
        }

        class asm : EXPR
        {
            public string asmtxt;

            public string ToAsm(string scope)
            {
                return asmtxt + "\n";
            }
        }

        class arith_expr : EXPR
        {
            public EXPR[] exprs;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                foreach (EXPR e in exprs)
                {
                    sb.AppendLine(e.ToAsm(scope));
                }
                return sb.ToString();
            }
        }

        class add : EXPR
        {
            public string ToAsm(string scope)
            {
                return "ADD";
            }
        }

        class sub : EXPR
        {
            public string ToAsm(string scope)
            {
                return "SUB";
            }
        }

        class mul : EXPR
        {
            public string ToAsm(string scope)
            {
                return "MUL";
            }
        }

        class div : EXPR
        {
            public string ToAsm(string scope)
            {
                return "DIV";
            }
        }

        class mod : EXPR
        {
            public string ToAsm(string scope)
            {
                return "MOD";
            }
        }

        class tmp_paran : EXPR
        {
            public string ToAsm(string scope)
            {
                throw new NotImplementedException("THIS ISN'T SUPPOSED TO HAPPEN!!");
            }
        }
    }
}

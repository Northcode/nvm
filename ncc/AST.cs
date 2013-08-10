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

            public static List<string> functions = new List<string>();
            public static Stack<function> currentfunctions = new Stack<function>();

            internal static Dictionary<int, string> GetLocalMeta()
            {
                Dictionary<int, string> meta = new Dictionary<int, string>();
                foreach (KeyValuePair<string,int> item in locals)
                {
                    meta.Add(item.Value, item.Key);
                }
                return meta;
            }

            internal static bool ContainsVar(string varname, string scope)
            {
                if (scope != "")
                {
                    return currentfunctions.Peek().locals.ContainsKey(scope + "." + varname);
                }
                else
                {
                    return currentfunctions.Peek().locals.ContainsKey(varname);
                }
            }

            internal static int GetVar(string varname, string scope)
            {
                if (!ContainsVar(varname, scope))
                {
                    currentfunctions.Peek().locals.Add((scope != "" ? scope + "." : "") + varname, locals.Count);
                    locals.Add((scope != "" ? scope + "." : "") + varname, locals.Count);
                }
                return currentfunctions.Peek().locals[(scope != "" ? scope + "." : "") + varname];
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
                if (VarnameLocalizer.ContainsVar(varname,scope))
                {
                    sb.AppendLine("FREELOC " + VarnameLocalizer.GetVar(varname, scope));
                    sb.AppendLine("STLOC " + VarnameLocalizer.GetVar(varname, scope));
                }
                else
                {
                    sb.AppendLine("STLOC " + VarnameLocalizer.GetVar(varname, scope));
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
                sb.AppendLine("STPTR " + VarnameLocalizer.GetVar(varname, scope));
                return sb.ToString();
            }
        }

        class getvar : EXPR
        {
            public string varname;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                if (VarnameLocalizer.ContainsVar(varname, scope))
                {
                    sb.AppendLine("LDLOC " + VarnameLocalizer.GetVar(varname,scope));
                }
                else if (VarnameLocalizer.functions.Contains(varname))
                {
                    sb.AppendLine("PUSH uint :" + varname);
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
                sb.AppendLine("LDPTR " + VarnameLocalizer.GetVar(varname, scope));
                return sb.ToString();
            }
        }

        class function : STMT
        {
            public string fname;
            public string[] args;
            public STMT[] body;
            public Dictionary<string,int> locals = new Dictionary<string,int>();

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();

                VarnameLocalizer.currentfunctions.Push(this);
                StringBuilder sbs = new StringBuilder();
                foreach (string arg in args)
                {
                    if (arg.StartsWith("@"))
                    {
                        sbs.AppendLine("STPTR " + VarnameLocalizer.GetVar(arg.Substring(1), (scope != "" ? scope + "." : "") + fname));
                    }
                    else
                    {
                        sbs.AppendLine("STLOC " + VarnameLocalizer.GetVar(arg, (scope != "" ? scope + "." : "") + fname));
                    }
                }

                foreach (STMT st in body)
                {
                    sbs.Append(st.ToAsm((scope != "" ? scope + "." : "") + fname));
                }

                VarnameLocalizer.currentfunctions.Pop();

                sb.AppendLine(fname + ":");
                sb.AppendLine("LOCALHEAP " + locals.Count.ToString());
                sb.AppendLine(sbs.ToString());
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

                if (VarnameLocalizer.ContainsVar(fname,scope))
                {
                    sb.AppendLine("LDPTR " + VarnameLocalizer.GetVar(fname, scope));
                    sb.AppendLine("CALLS");
                }
                else
                {
                    sb.AppendLine("CALL :" + fname);
                }
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
                if (VarnameLocalizer.ContainsVar(varname,scope))
                {
                    sb.AppendLine("FREELOC " + VarnameLocalizer.GetVar(varname, scope));
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

        class ArrayInitializer : EXPR
        {
            public EXPR count;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(count.ToAsm(scope));
                sb.AppendLine("ARRAY");
                return sb.ToString();
            }
        }

        class ArrayGetElem : EXPR
        {
            public string varname;
            public EXPR index;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("LDLOC " + VarnameLocalizer.GetVar(varname, scope));

                sb.Append(index.ToAsm(scope));
                sb.AppendLine("LD_ELEM");

                if (!varname.StartsWith("@"))
                {
                    sb.AppendLine("LDADDR");
                }

                return sb.ToString();
            }
        }

        class ArrayStrElem : EXPR
        {
            public string varname;
            public EXPR index;
            public EXPR value;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                if (varname.StartsWith("@"))
                {
                    sb.AppendLine("LDLOC " + VarnameLocalizer.GetVar(varname.Substring(1), scope));
                }
                else
                {
                    sb.AppendLine("LDLOC " + VarnameLocalizer.GetVar(varname, scope));
                }

                sb.Append(index.ToAsm(scope));
                sb.Append(value.ToAsm(scope));

                if (varname.StartsWith("@"))
                {
                    sb.AppendLine("ST_ELEM_PTR");
                }
                else
                {
                    sb.AppendLine("ST_ELEM");
                }

                return sb.ToString();
            }
        }

        public class LambdaExpr : EXPR
        {
            public string lambdaRef;

            public string[] args;
            public STMT[] body;

            public string ToAsm(string scope)
            {
                StringBuilder sb = new StringBuilder();
                lambdaRef = "lmabda" + LambdaHolder.lambdas.Count;
                LambdaHolder.lambdas.Add(this);
                sb.AppendLine("PUSH uint :" + lambdaRef);
                return sb.ToString();
            }
        }

        public class LambdaHolder
        {
            public static List<LambdaExpr> lambdas = new List<LambdaExpr>();

            public static string WriteLambdas()
            {
                StringBuilder sb = new StringBuilder();
                foreach (LambdaExpr lambda in lambdas)
                {
                    sb.AppendLine(lambda.lambdaRef + ":");
                    foreach (string arg in lambda.args)
                    {
                        if (arg.StartsWith("@"))
                        {
                            sb.AppendLine("STPTR " + VarnameLocalizer.GetVar(arg, lambda.lambdaRef));
                        }
                        else
                        {
                            sb.AppendLine("STLOC " + VarnameLocalizer.GetVar(arg, lambda.lambdaRef));
                        }
                    }

                    foreach (STMT st in lambda.body)
                    {
                        sb.Append(st.ToAsm(lambda.lambdaRef));
                    }

                    sb.AppendLine("RET");
                }
                return sb.ToString();
            }
        }
    }
}

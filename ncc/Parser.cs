using ncc.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncc
{
    public class Parser
    {
        public Token[] tokens { get; set; }

        public List<STMT> stmts { get; set; }

        int i;
        bool infnc = false;

        public Parser(Token[] tokens)
        {
            this.tokens = tokens;
        }

        public void Parse()
        {
            stmts = new List<STMT>();
            i = 0;
            while (i < tokens.Length)
            {
                STMT stmt = ParseSTMT();
                if (stmt != null)
                {
                    stmts.Add(stmt);
                }
            }
        }

        STMT[] ParseBody()
        {
            List<STMT> stmtl = new List<STMT>();

            while (i < tokens.Length)
            {
                STMT stmt = ParseSTMT();
                if (stmt != null)
                {
                    if (stmt is end)
                    {
                        return stmtl.ToArray();
                    }
                    stmtl.Add(stmt);
                }
            }
            throw new Exception("End not found!");
        }

        STMT ParseSTMT()
        {
            if (i + 1 < tokens.Length && (tokens[i + 1].type == TokenType.symbol && ((char)tokens[i + 1].val) == '='))
            {
                string vname = tokens[i].val as string;
                i += 2;
                EXPR e = ParseExpr(false);
                if (vname.StartsWith("@"))
                {
                    setvarptr st = new setvarptr() { varname = vname.Substring(1), value = e };
                    return st;
                }
                else
                {
                    setvar s = new setvar() { varname = vname, value = e };
                    return s;
                }
            }
            else if (tokens[i].type == TokenType.newline)
            {
                i++;
                return null;
            }
            else if (i + 1 < tokens.Length && (tokens[i + 1].type == TokenType.symbol && (char)tokens[i + 1].val == '(' && !infnc))
            {
                string fname = tokens[i].val as string;
                i += 2;
                List<string> fargs = new List<string>();
                while (i < tokens.Length && !(tokens[i].type == TokenType.symbol && (char)tokens[i].val == ')'))
                {
                    if (tokens[i].type == TokenType.word)
                    {
                        fargs.Add(tokens[i].val as string);
                    }
                    i++;
                }
                i++;
                infnc = true;
                STMT[] body = ParseBody();
                infnc = false;

                VarnameLocalizer.functions.Add(fname);

                return new function() { fname = fname, args = fargs.ToArray(), body = body };
            }
            else if (tokens[i].type == TokenType.word && tokens[i].val as string == "end")
            {
                i++;
                return new end();
            }
            else if (tokens[i].type == TokenType.word && tokens[i].val as string == "return")
            {
                i++;
                EXPR val = ParseExpr(false);
                return new ret() { val = val };
            }
            else if (tokens[i].type == TokenType.word && tokens[i].val as string == ".free")
            {
                i++;
                string vname = tokens[i].val as string;
                i++;
                return new free() { varname = vname };
            }
            else
            {
                return ParseExpr(false);
            }
            throw new Exception();
        }

        EXPR ParseExpr(bool isArithExpr)
        {
            EXPR e = null;
            if (tokens[i].type == TokenType.byte_lit)
            {
                e = new bytelit() { value = (byte)tokens[i].val };
                i++;
            }
            else if (tokens[i].type == TokenType.int_lit)
            {
                e = new intlit() { value = (int)tokens[i].val };
                i++;
            }
            else if (tokens[i].type == TokenType.string_lit)
            {
                e = new stringlit() { value = tokens[i].val as string };
                i++;
            }
            else if (tokens[i].type == TokenType.word && tokens[i].val as string == "array" && tokens[i + 1].type == TokenType.symbol && (char)tokens[i + 1].val == '[')
            {
                i += 2;
                EXPR count = ParseExpr(isArithExpr);
                i++;
                e = new ArrayInitializer() { count = count };
            }
            else if (tokens[i].type == TokenType.word && tokens[i + 1].type == TokenType.symbol && (char)tokens[i + 1].val == '[')
            {
                string varname = tokens[i].val as string;
                i += 2;
                EXPR index = ParseExpr(isArithExpr);
                i++;
                if (tokens[i].type == TokenType.symbol && (char)tokens[i].val == '=')
                {
                    i++;
                    EXPR val = ParseExpr(isArithExpr);
                    e = new ArrayStrElem() { varname = varname, index = index, value = val };
                }
                else
                {
                    e = new ArrayGetElem() { varname = varname, index = index };
                }
            }
            else if (i + 1 < tokens.Length && tokens[i + 1].type == TokenType.symbol && (char)tokens[i + 1].val == '(')
            {
                string fname = tokens[i].val as string;
                i += 2;
                List<EXPR> args = new List<EXPR>();
                while (!(tokens[i].type == TokenType.symbol && (char)tokens[i].val == ')'))
                {
                    EXPR ex = ParseExpr(isArithExpr);
                    if (ex != null)
                    {
                        args.Add(ex);
                    }
                    else
                    {
                        i++;
                    }
                }
                i++;
                e = new fcall() { fname = fname, args = args.ToArray() };
            }
            else if (tokens[i].type == TokenType.word && tokens[i].val as string == "asm")
            {
                i++;
                string asm = tokens[i].val as string;
                i++;
                return new asm() { asmtxt = asm };
            }
            else if (tokens[i].type == TokenType.word)
            {
                string vname = tokens[i].val as string;
                if (vname.StartsWith("@"))
                {
                    e = new getvarptr() { varname = vname.Substring(1) };
                }
                else
                {
                    e = new getvar() { varname = vname };
                }
                i++;
            }

            if (tokens[i].IsArithOp() && !isArithExpr)
            {
                List<EXPR> finalexpr = new List<EXPR>();
                Stack<Token> stk = new Stack<Token>();

                finalexpr.Add(e);

                bool lastwasop = false;
                int parentCount = 0;

                while (true)
                {
                    if (tokens[i].type == TokenType.symbol && (char)tokens[i].val == ')' && parentCount > 0)
                    {
                        while (stk.Count > 0 && (char)stk.Peek().val != '(')
                        {
                            finalexpr.Add(stk.Pop().ToArith());
                        }
                        stk.Pop();
                        i++;
                        parentCount--;
                    }
                    else if (tokens[i].IsArithOp())
                    {
                        if (stk.Count > 0 && tokens[i].ArithOpPriority() < stk.Peek().ArithOpPriority())
                        {
                            if ((char)stk.Peek().val != '(')
                                finalexpr.Add(stk.Pop().ToArith());
                            else
                                parentCount++;
                        }
                        stk.Push(tokens[i]);
                        lastwasop = true;
                        i++;
                    }
                    else
                    {
                        if (lastwasop)
                        {
                            finalexpr.Add(ParseExpr(true));
                            lastwasop = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                while (stk.Count > 0)
                {
                    finalexpr.Add(stk.Pop().ToArith());
                }

                e = new arith_expr() { exprs = finalexpr.ToArray() };
            }
            return e;
        }
    }
}

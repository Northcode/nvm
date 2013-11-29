using nvm2.compiler.ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.compiler
{
    class Parser
    {
        Token[] tokens;
        int i;

        public Token[] Tokens
        {
            set
            {
                tokens = value;
            }
        }

        public Token Current
        {
            get
            {
                return GetToken(i);
            }
        }

        public Token Next
        {
            get
            {
                return GetToken(i + 1);
            }
        }

        public bool EOT
        {
            get
            {
                return InBounds(i);
            }
        }

        public bool IsNextEOT
        {
            get
            {
                return InBounds(i + 1);
            }
        }

        private bool InBounds(int i)
        {
            return i < 0 || i >= tokens.Length;
        }

        public void Step()
        {
            i++;
        }

        public Token GetToken(int i)
        {
            if (!InBounds(i))
            {
                return tokens[i];
            }
            else
            {
                throw new IndexOutOfRangeException("Unexpected end of tokens at index: " + i);
            }
        }

        public stmt ParseStmt(bool ParsingFormList = false)
        {
            if(!EOT)
            {
                stmt s = null;
                if((s = ParseWord()) != null)
                {
                    if (!ParsingFormList)
                    {
                        s = ParseStmtList(s);
                    }
                }
                else if ((s = ParseExpr()) != null)
                {
                    if (!ParsingFormList)
                    {
                        s = ParseStmtList(s);
                    }
                }
                else
                {
                    throw new Exception("Failed to parse statement at token: " + Current);
                }
                return s;
            }
            else
            {
                return null;
            }
        }

        private stmt ParseStmtList(stmt s)
        {
            if (!EOT && Next != "end")
            {
                Step(); //Step over s
                stmt_list lst = new stmt_list();
                lst.list = new List<stmt>();
                lst.list.Add(s);
                while (!IsNextEOT && Next != "end")
                {
                    lst.list.Add(ParseStmt(true));
                    Step(); //Step over stmt
                }
                Step(); //Step over stmt
                return lst;
            }
            else
            {
                return s;
            }
        }
   
        private stmt ParseWord()
        {
            if (Current == TokenType.word)
            {
                if (!IsNextEOT && Next == TokenType.symbol)
                {
                    if (Next == '=')
                    {
                        setvar stmt = new setvar();
                        stmt.name = Current;
                        Step(); Step();
                        stmt.value = ParseExpr();
                        return stmt;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (Current == "def")
                {
                    function_definition stmt = new function_definition();
                    Step(); //Step over def
                    stmt.name = Current;
                    Step(); //Step over fname
                    Step(); //Step over (
                    stmt.args = ParseArgs();
                    Step();
                    stmt.body = ParseStmt();
                    return stmt;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private string[] ParseArgs()
        {
            List<string> args = new List<string>();
            do
            {
                args.Add(Current);
                Step(); //Step over arg
                if(Current == ',')
                    Step(); //Step over comma
            } while (!EOT && Current == ',');

            return args.ToArray();
        }

        private expr ParseExpr(bool CalledFromArithmeticParsing = false)
        {
            expr e = ParseLiterals();

            if(e == null)
            {
                if (Current == TokenType.word)
                {
                    e = ParseExprWord();
                }
            }

            if(e == null)
            {
                throw new Exception("Parse error: unable to parse " + Current);
            }

            if (!IsNextEOT && !CalledFromArithmeticParsing && IsTokenArithmeticOperator(Next))
            {
                e = ParseArithExpr(e);
            }

            if (!IsNextEOT && Next == ',')
            {
                expr_list expr = new expr_list();
                expr.list = new List<expr>();
                expr.list.Add(e);
                Step(); //Step over e
                Step(); //Step over ,
                do
                {
                    Step(); //Step over ,
                    expr.list.Add(ParseExpr());
                    Step(); //Step over expr
                } while (!IsNextEOT && Next == ',');
                return expr;
            }
            return e;
        }

        private expr ParseArithExpr(expr e)
        {
            if (!IsNextEOT && IsTokenArithmeticOperator(Next))
            {
                //Initialize stacks and lists
                arithmetic_expr expr = new arithmetic_expr();
                expr.operations = new List<expr>();
                Stack<Token> operators = new Stack<Token>();
                expr.operations.Add(e);
                Step(); //Step over e

                //initialize variables
                bool LastTokenWasOperator = false;
                int parenCount = 0;

                //Start loop
                while (!EOT)
                {
                    if (Current == ')' && parenCount > 0)
                    {
                        while (operators.Count > 0 && operators.Peek() != '(')
                        {
                            expr.operations.Add(GetArithOp(operators.Peek()));
                        }
                        operators.Pop();
                        Step();
                    }
                    else if (IsTokenArithmeticOperator(Current))
                    {
                        if (operators.Count > 0 && ArithmeticOperatorPriority(Current) < ArithmeticOperatorPriority(operators.Peek()))
                        {
                            if (operators.Peek() != '(')
                                expr.operations.Add(GetArithOp(operators.Pop()));
                            else
                                parenCount++;
                        }
                        operators.Push(Current);
                        LastTokenWasOperator = true;
                        Step();
                    }
                    else
                    {
                        if (LastTokenWasOperator)
                        {
                            expr.operations.Add(ParseExpr(true));
                            Step(); //Step over expr
                            LastTokenWasOperator = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                while (operators.Count > 0)
                {
                    expr.operations.Add(GetArithOp(operators.Pop()));
                }

                i--; //this sets the current token to this expr so it will work with other methods that skip it

                return expr;
            }
            else
            {
                throw new Exception("Expression is not arithmetic");
            }
        }

        private int ArithmeticOperatorPriority(Token token)
        {
            if (token == '+' || token == '-')
            {
                return 1;
            }
            else if (token == '*' || token == '/' || token == '%')
            {
                return 2;
            }
            else if (token == '^')
            {
                return 4;
            }
            else if (token == '(')
            {
                return 5;
            }
            else
            {
                return 0;
            }
        }

        private bool IsTokenArithmeticOperator(Token token)
        {
            if (token == TokenType.symbol)
            {
                return (token == '+' || token == '-' || token == '*' || token == '/' || token == '%' || token == '^');
            }
            else
            {
                return false;
            }
        }

        private arithmetic_operator GetArithOp(Token token)
        {
            if (token == '+')
            {
                return new arithmetic_operator() { op = arith_operator.add }; 
            }
            else if (token == '-')
            {
                return new arithmetic_operator() { op = arith_operator.sub }; 
            }
            else if (token == '*')
            {
                return new arithmetic_operator() { op = arith_operator.mul }; 
            }
            else if (token == '/')
            {
                return new arithmetic_operator() { op = arith_operator.div }; 
            }
            else if (token == '%')
            {
                return new arithmetic_operator() { op = arith_operator.mod }; 
            }
            else if (token == '^')
            {
                return new arithmetic_operator() { op = arith_operator.pow }; 
            }
            else
            {
                throw new Exception("Token is not an operator: " + token);
            }
        }

        private expr ParseExprWord()
        {
            if (!IsNextEOT && Next == '(')
            {
                function_call expr = new function_call();
                expr.name = Current;
                Step(); //Step over name
                Step(); //Step over (
                expr.arg = ParseExpr();
                Step(); //Step over args
                return expr;
            }
            else
            {
                return new getvar() { name = Current };
            }
        }

        private expr ParseLiterals()
        {
            if (Current == TokenType.int_lit)
            {
                return new int_lit() { value = Current };
            }
            else if (Current == TokenType.float_lit)
            {
                return new float_lit() { value = Current };
            }
            else if (Current == TokenType.bool_lit)
            {
                return new bool_lit() { value = Current };
            }
            else if (Current == TokenType.byte_lit)
            {
                return new byte_lit() { value = Current };
            }
            else if (Current == TokenType.string_lit)
            {
                return new string_lit() { value = Current };
            }
            else
            {
                return null;
            }
        }
    }
}

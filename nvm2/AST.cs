using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.compiler.ast
{
    interface stmt
    {

    }

    interface expr : stmt
    {

    }

    class setvar : stmt
    {
        public string name;
        public expr value;

        public override string ToString()
        {
            return "{ " + name + " = " + value.ToString() + " }";
        }
    }

    class function_definition : stmt
    {
        public string name;
        public string[] args;
        public stmt body;

        public override string ToString()
        {
            return "{ function: " + name + " , args: " + string.Join(",", args) + " , body: " + body.ToString() + " }";
        }
    }

    class setpointer : stmt
    {
        public string name;
        public expr pointer;
    }

    class stmt_list : stmt
    {
        public List<stmt> list;

        public override string ToString()
        {
            return "{ " + string.Join<stmt>(",", list.ToArray()) + " }";
        }
    }

    class class_definition : stmt
    {
        public string name;
        public stmt body;
    }

    class expr_list : expr
    {
        public List<expr> list;

        public override string ToString()
        {
            return "{ expr_list: " + string.Join<expr>(",", list.ToArray()) + " }";
        }
    }

    class int_lit : expr
    {
        public int value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class string_lit : expr
    {
        public string value;

        public override string ToString()
        {
            return "\"" + value + "\"";
        }
    }

    class float_lit : expr
    {
        public float value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class bool_lit : expr
    {
        public bool value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class byte_lit : expr
    {
        public byte value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class getvar : expr
    {
        public string name;

        public override string ToString()
        {
            return "{ get: " + name + " }";
        }
    }

    class function_call : expr
    {
        public string name;
        public expr arg;

        public override string ToString()
        {
            return "{ function_call: " + name + "(" + arg.ToString() + ") }";
        }
    }

    enum arith_operator
    {
        add,sub,mul,div,mod,pow
    }

    class arithmetic_expr : expr
    {
        public List<expr> operations;

        public override string ToString()
        {
            return "{ arithmetic expr: " + string.Join<expr>(" ", operations.ToArray()) + "}";
        }
    }

    class arithmetic_operator : expr
    {
        public arith_operator op;

        public override string ToString()
        {
            return op.ToString();
        }
    }

    class lambda : expr
    {
        public string[] args;
        public stmt body;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.compiler.ast
{
    public interface stmt
    {

    }

    public interface expr : stmt
    {

    }

    public class declarevar : stmt
    {
        public string name;
        public string type;
        public expr value;

        public override string ToString()
        {
            return "{ declare " + name + " as " + type + (value != null ? " = " + value.ToString() : "") + " }";
        }
    }

    public class setvar : stmt
    {
        public string name;
        public expr value;

        public override string ToString()
        {
            return "{ " + name + " = " + value.ToString() + " }";
        }
    }

    public class function_definition : stmt
    {
        public string name;
        public arg[] args;
        public stmt body;
        public string type;

        public override string ToString()
        {
            return "{ function " + name + " with " + string.Join(",", args.Select(a => a.ToString())) + " as " + type + " doing " + body.ToString() + " }";
        }
    }

    public class setpointer : stmt
    {
        public string name;
        public expr pointer;
    }

    public class stmt_list : stmt
    {
        public List<stmt> list;

        public override string ToString()
        {
            return "{ " + string.Join<stmt>(",", list.ToArray()) + " }";
        }
    }

    public class class_definition : stmt
    {
        public string name;
        public stmt body;
        public string[] interfaces;

        public override string ToString()
        {
            return "{ class " + name + (interfaces != null ? " from " + string.Join(",", interfaces) : "") + " " + body.ToString() + " }";
        }
    }

    public class return_value : stmt
    {
        public expr returnvalue;

        public override string ToString()
        {
            return "{ return " + returnvalue.ToString() + " }";
        }
    }

    public class expr_list : expr
    {
        public List<expr> list;

        public override string ToString()
        {
            return "{ expr_list: " + string.Join<expr>(",", list.ToArray()) + " }";
        }
    }

    public class int_lit : expr
    {
        public int value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class string_lit : expr
    {
        public string value;

        public override string ToString()
        {
            return "\"" + value + "\"";
        }
    }

    public class float_lit : expr
    {
        public float value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class bool_lit : expr
    {
        public bool value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class byte_lit : expr
    {
        public byte value;

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class getvar : expr
    {
        public string name;

        public override string ToString()
        {
            return "{ get: " + name + " }";
        }
    }

    public class function_call : expr
    {
        public string name;
        public expr arg;

        public override string ToString()
        {
            return "{ function_call: " + name + "(" + (arg != null ? arg.ToString() : "") + ") }";
        }
    }

    public enum arith_operator
    {
        add,sub,mul,div,mod,pow
    }

    public class arithmetic_expr : expr
    {
        public List<expr> operations;

        public override string ToString()
        {
            return "{ arithmetic expr: " + string.Join<expr>(" ", operations.ToArray()) + "}";
        }
    }

    public class arithmetic_operator : expr
    {
        public arith_operator op;

        public override string ToString()
        {
            return op.ToString();
        }
    }

    public class nothing : stmt
    {
        public override string ToString()
        {
            return "{ nothing }";
        }
    }

    public class lambda : expr
    {
        public string[] args;
        public stmt body;
    }

    public class cast : expr
    {
        public expr value;
        public string type;

        public override string ToString()
        {
            return "{ cast " + value.ToString() + " to " + type + " }";
        }
    }

    public class newobj : expr
    {
        public string typename;
        public expr arg;

        public override string ToString()
        {
            return "{ new " + typename + " with " + (arg != null ? arg.ToString() : "") + " }";
        }
    }

    public class access_field : expr
    {
        public expr value;
        public expr field;

        public override string ToString()
        {
            return "{ get " + field + " from " + value + " }";
        }
    }

    public class function_pointer
    {
        public string name;

        public override string ToString()
        {
            return "{ function pointer " + name + " }";
        }
    }

    public class arg
    {
        public string name;
        public string type;

        public override string ToString()
        {
            return type + " " + name;
        }
    }
}

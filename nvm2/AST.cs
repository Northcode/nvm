using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.compiler.ast
{
    class stmt
    {

    }

    class expr : stmt
    {

    }

    class int_lit : expr
    {
        public int value;
    }

    class string_lit : expr
    {
        public string value;
    }

    class float_lit : expr
    {
        public float value;
    }

    class bool_lit : expr
    {
        public bool value;
    }

    class byte_lit : expr
    {
        public byte value;
    }
}

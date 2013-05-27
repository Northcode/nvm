using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.v2
{
    public delegate void OPRUN(VM m);

    public class OpCode
    {
        internal string Name { get; set; }
        internal byte BYTECODE { get; set; }
        internal OPRUN Run;
    }
}

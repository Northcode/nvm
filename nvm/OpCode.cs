using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes
{
    internal interface OpCode
    {
        void Execute(VirtualMachine machine);

        byte GetByteCode();
    }
}

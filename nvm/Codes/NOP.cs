using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes
{
    class NOP : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            return;
        }

        public byte GetByteCode()
        {
            return 0x00;
        }
    }
}

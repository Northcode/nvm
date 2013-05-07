using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes
{
    class END : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.RN = false;
        }

        public byte GetByteCode()
        {
            return 0x07;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class END : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.RN = false;
        }

        public byte GetByteCode()
        {
            return (byte)0x11;
        }
    }
}

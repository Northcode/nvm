using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class JMP : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            uint addr = (uint)machine.edx;
            machine.IP = addr;
        }

        public byte GetByteCode()
        {
            return (byte)0x26;
        }
    }
}

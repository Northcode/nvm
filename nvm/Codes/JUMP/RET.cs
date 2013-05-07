using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.JUMP
{
    class RET : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            Call c = machine.callstack.Pop();
            machine.IP = c.Raddr;
        }

        public byte GetByteCode()
        {
            return 0x11;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.JUMP
{
    class JMP : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            uint addr = machine.memory.ReadUInt(machine.IP);
            machine.IP += 4;
            machine.IP = addr;
        }

        public byte GetByteCode()
        {
            return 0x0f;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Registers.LD
{
    class LDAL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte val = machine.memory.Read(machine.IP);
            machine.IP++;
            machine.al = val;
        }

        public byte GetByteCode()
        {
            return 0x01;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes._32_Bit
{
    class STEBX : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.ebx = machine.memory.ReadInt(machine.IP);
            machine.IP += 4;
        }

        public byte GetByteCode()
        {
            return (byte)0x0d;
        }
    }
}

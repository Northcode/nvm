using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes._16_Bit
{
    class STBX : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.bx = machine.memory.ReadUInt16(machine.IP);
            machine.IP += 2;
        }

        public byte GetByteCode()
        {
            return (byte)0x09;
        }
    }
}

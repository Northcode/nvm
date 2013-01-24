using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.RegisterCodes._16_Bit
{
    class STAX : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.ax = machine.memory.ReadUInt16(machine.IP);
            machine.IP += 2;
        }

        public byte GetByteCode()
        {
            return (byte)0x08;
        }
    }
}

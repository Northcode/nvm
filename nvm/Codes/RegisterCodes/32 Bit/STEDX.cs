using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.RegisterCodes._32_Bit
{
    class STEDX : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.edx = machine.memory.ReadInt(machine.IP);
            machine.IP += 4;
        }

        public byte GetByteCode()
        {
            return (byte)0x0f;
        }
    }
}

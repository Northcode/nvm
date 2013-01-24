using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.RegisterCodes
{
    class STAH : OpCode
    {

        public void Execute(VirtualMachine machine)
        {
            machine.ah = machine.memory.Read(machine.IP);
            machine.IP++;
        }

        public byte GetByteCode()
        {
            return (byte)0x00;
        }
    }
}

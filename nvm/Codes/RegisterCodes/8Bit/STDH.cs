using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.RegisterCodes
{
    class STDH : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.dh = machine.memory.Read(machine.IP);
            machine.IP++;
        }

        public byte GetByteCode()
        {
            return (byte)0x06;
        }
    }
}

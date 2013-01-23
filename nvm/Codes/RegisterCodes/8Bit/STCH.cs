using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes
{
    internal class STCH : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.ch = machine.memory.Read(machine.IP);
            machine.IP++;
        }

        public byte GetByteCode()
        {
            return (byte)0x04;
        }
    }
}

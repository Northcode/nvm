using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes
{
    internal class STCL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.cl = machine.memory.Read(machine.IP);
            machine.IP++;
        }

        public byte GetByteCode()
        {
            return (byte)0x05;
        }
    }
}

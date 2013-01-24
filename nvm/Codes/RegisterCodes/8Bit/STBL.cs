using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.RegisterCodes
{
    internal class STBL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.bl = machine.memory.Read(machine.IP);
            machine.IP++;
        }

        public byte GetByteCode()
        {
            return (byte)0x03;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes
{
    internal class STAL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.al = machine.memory.Read(machine.IP);
            machine.IP++;
        }

        public byte GetByteCode()
        {
            return (byte)0x01;
        }
    }
}

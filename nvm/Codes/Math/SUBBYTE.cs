using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Math
{
    class SUBBYTE : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            machine.al = (byte)((byte)machine.GetRegister(VirtualMachine.FirstNibble(registers)) - (byte)machine.GetRegister(VirtualMachine.LastNibble(registers)));
        }

        public byte GetByteCode()
        {
            return (byte)0x18;
        }
    }
}

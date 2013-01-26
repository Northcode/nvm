using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Math
{
    class DIVDWORD : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            machine.eax = (int)((int)machine.GetRegister(VirtualMachine.LastNibble(registers)) / (int)machine.GetRegister(VirtualMachine.FirstNibble(registers)));
        }

        public byte GetByteCode()
        {
            return (byte)0x1f;
        }
    }
}

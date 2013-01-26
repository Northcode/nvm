using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Math
{
    class MULWORD : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            machine.ax = (ushort)((ushort)machine.GetRegister(VirtualMachine.FirstNibble(registers)) * (ushort)machine.GetRegister(VirtualMachine.LastNibble(registers)));
        }

        public byte GetByteCode()
        {
            return (byte)0x1c;
        }
    }
}

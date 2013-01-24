using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.Math
{
    class ADDDWORD : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            int val = (int)((int)machine.GetRegister(VirtualMachine.FirstNibble(registers)) + (int)machine.GetRegister(VirtualMachine.LastNibble(registers)));
            machine.eax = val;
        }

        public byte GetByteCode()
        {
            return (byte)0x17;
        }
    }
}
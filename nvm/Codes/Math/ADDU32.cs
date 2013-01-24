using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.Math
{
    class ADDU32 : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
		//Console.WriteLine("Reading registers IP: " + machine.IP);
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            uint val = (uint)((uint)machine.GetRegister(VirtualMachine.FirstNibble(registers)) + (uint)machine.GetRegister(VirtualMachine.LastNibble(registers)));
            machine.eax = (int)val;
        }

        public byte GetByteCode()
        {
            return (byte)0x16;
        }
    }
}
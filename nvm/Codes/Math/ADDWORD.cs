using System;
using System.Collections.Generic;
using System.Text;

namespace nvm.Codes.Math
{
    class ADDWORD : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
		//Console.WriteLine("Reading registers IP: " + machine.IP);
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            ushort val = (ushort)((ushort)machine.GetRegister(VirtualMachine.FirstNibble(registers)) + (ushort)machine.GetRegister(VirtualMachine.LastNibble(registers)));
            machine.ax = val;
        }

        public byte GetByteCode()
        {
            return (byte)0x15;
        }
    }
}

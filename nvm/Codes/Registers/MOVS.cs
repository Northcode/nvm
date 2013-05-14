using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Registers
{
    class MOVS : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte regs = machine.memory.Read(machine.IP);
            machine.IP++;

            byte regfrom = VirtualMachine.FirstNibble(regs);
            byte regto = VirtualMachine.LastNibble(regs);

            machine.SetRegister(regto, machine.GetSpecialRegister(regfrom));
        }

        public byte GetByteCode()
        {
            return 0x12;
        }
    }
}

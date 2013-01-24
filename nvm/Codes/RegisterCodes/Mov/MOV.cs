using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes.Mov
{
    class MOV : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte registers = machine.memory.Read(machine.IP);
            machine.IP++;
            machine.SetRegister(VirtualMachine.FirstNibble(registers), machine.GetRegister(VirtualMachine.LastNibble(registers)));
        }

        public byte GetByteCode()
        {
            return 0x12;
        }
    }
}

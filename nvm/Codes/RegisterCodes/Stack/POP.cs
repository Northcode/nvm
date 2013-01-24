using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes.Stack
{
    class POP : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte register = machine.memory.Read(machine.IP);
            machine.IP++;
            if (register > 0 && register < 8)
            {
                machine.SetRegister(register, machine.manager.Pop());
            }
            else if (register > 7 && register < 0x0c)
            {
                machine.SetRegister(register, machine.manager.Pop16());
            }
            else
            {
                machine.SetRegister(register, machine.manager.Pop32());
            }
        }

        public byte GetByteCode()
        {
            return (byte)0x14;
        }
    }
}

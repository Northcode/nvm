using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes.Stack
{
    class PUSH : OpCode
    {

        public void Execute(VirtualMachine machine)
        {
            byte register = machine.memory.Read(machine.IP);
            machine.IP++;
            if (register > 0 && register < 8)
            {
                byte val = (byte)machine.GetRegister(register);
                machine.manager.Push(val);
            }
            else if (register > 7 && register < 0x0c)
            {
                ushort val = (ushort)machine.GetRegister(register);
                machine.manager.Push(val);
            }
            else
            {
                int val = (int)machine.GetRegister(register);
                machine.manager.Push(val);
            }
        }

        public byte GetByteCode()
        {
            return 0x13;
        }
    }
}

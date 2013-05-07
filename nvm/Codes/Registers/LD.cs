using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Registers
{
    class LD : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            object val = null;
            if (reg <= 0x07)
            {
                val = machine.memory.Read(machine.IP);
                machine.IP++;
            }
            else if (reg >= 0x08 && reg <= 0x0b)
            {
                val = machine.memory.ReadUInt16(machine.IP);
                machine.IP += 2;
            }
            else
            {
                val = machine.memory.ReadInt(machine.IP);
                machine.IP += 4;
            }
            machine.SetRegister(reg, val);
        }

        public byte GetByteCode()
        {
            return 0x01;
        }
    }
}

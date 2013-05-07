using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Stack
{
    class POP_B : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            byte val = machine.Mmanager.Pop();
            machine.SetRegister(reg, val);
        }

        public byte GetByteCode()
        {
            return 0x09;
        }
    }

    class POP_S : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            ushort val = machine.Mmanager.Pop16();
            machine.SetRegister(reg, val);
        }

        public byte GetByteCode()
        {
            return 0x0a;
        }
    }

    class POP_INT : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            int val = machine.Mmanager.Pop32();
            machine.SetRegister(reg, val);
        }

        public byte GetByteCode()
        {
            return 0x0b;
        }
    }

    class POP_STR : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            uint addr = (uint)machine.GetRegister(reg);
            string val = machine.Mmanager.PopS();
            machine.memory.Write(addr,val);
        }

        public byte GetByteCode()
        {
            return 0x0c;
        }
    }
}

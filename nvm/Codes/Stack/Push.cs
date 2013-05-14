using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Stack
{
    class PUSH_B : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            byte val = (byte)machine.GetRegister(reg);
            machine.Mmanager.Push(val);
        }

        public byte GetByteCode()
        {
            return 0x03;
        }
    }

    class PUSH_S : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            ushort val = (ushort)machine.GetRegister(reg);
            machine.Mmanager.Push(val);
        }

        public byte GetByteCode()
        {
            return 0x04;
        }
    }

    class PUSH_INT : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            int val = (int)machine.GetRegister(reg);
            machine.Mmanager.Push(val);
        }

        public byte GetByteCode()
        {
            return 0x05;
        }
    }

    class PUSH_STR : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte reg = machine.memory.Read(machine.IP);
            machine.IP++;
            uint addr = (uint)((int)machine.GetRegister(reg));
            string str = machine.memory.ReadString(addr);
            machine.Mmanager.Push(str);
        }

        public byte GetByteCode()
        {
            return 0x06;
        }
    }
}

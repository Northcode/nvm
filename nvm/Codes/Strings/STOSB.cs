using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Strings
{
    class STOSB : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            ushort addr = machine.memory.ReadUInt16(machine.IP);
            machine.IP += 2;
            string str = machine.memory.ReadString(machine.IP);
            machine.IP += (uint)str.Length + 1;
            machine.memory.Write(addr, str);
        }

        public byte GetByteCode()
        {
            return 0x08;
        }
    }
}

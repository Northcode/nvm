using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Strings
{
    class LODSB : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            string str = machine.memory.ReadString(machine.IP);
            machine.IP += (uint)str.Length + 1;
            machine.Mmanager.Push(str);
        }

        public byte GetByteCode()
        {
            return 0x0d;
        }
    }
}

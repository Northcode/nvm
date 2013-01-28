using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class LDLOC : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            int local = machine.edx;
            machine.manager.Push(machine.manager.LoadPtr(local));
        }

        public byte GetByteCode()
        {
            return (byte)0x28;
        }
    }
}

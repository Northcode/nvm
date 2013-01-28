using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class STLOC : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            int local = machine.edx;
            machine.manager.StorePtr(local, machine.manager.PopU32());
        }

        public byte GetByteCode()
        {
            return (byte)0x27;
        }
    }
}

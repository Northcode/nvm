using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes.Stack
{
    class POPSTR : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            string str = machine.manager.PopS();
            uint addr = (uint)machine.edx;
            machine.memory.Write(addr, str);
        }

        public byte GetByteCode()
        {
            return (byte)0x23;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.RegisterCodes.Stack
{
    class PUSHSTR : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            uint addr = (uint)machine.edx;
            string str = machine.memory.ReadString(addr);
            machine.manager.Push(str);
        }

        public byte GetByteCode()
        {
            return (byte)0x22;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class CALL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            uint addr = (uint)machine.edx;
            machine.callstack.Push(new Call() { Caddr = addr, Raddr = machine.IP, Iaddr = -1 });
            machine.IP = addr;
        }

        public byte GetByteCode()
        {
            return (byte)0x24;
        }
    }
}

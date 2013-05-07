using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.JUMP
{
    class CALL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            uint addr = machine.memory.ReadUInt(machine.IP);
            machine.IP++;
            machine.callstack.Push(new Call() { Caddr = addr, Raddr = machine.IP, Iaddr = -1 });
            machine.IP = addr;
        }

        public byte GetByteCode()
        {
            return 0x10;
        }
    }
}

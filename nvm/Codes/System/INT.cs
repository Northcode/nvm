using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class INT : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte code = machine.memory.Read(machine.IP);
            machine.IP++;
            VirtualMachine.interupts[code].Run(machine);
        }

        public byte GetByteCode()
        {
            return (byte)0x21;
        }
    }
}

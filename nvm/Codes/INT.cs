using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes
{
    class INT : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte ac = machine.memory.Read(machine.IP);
            machine.IP++;
            VirtualMachine.interups[ac].Run(machine);
        }

        public byte GetByteCode()
        {
            return 0x0e;
        }
    }
}

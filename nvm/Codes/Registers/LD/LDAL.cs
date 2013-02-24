using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.Registers.LD
{
    class LDAL : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte val = machine.memory.Read(machine.IP);
            machine.IP++;
<<<<<<< HEAD:nvm/Codes/Registers/LD/LDAL.cs
            machine.al = val;
=======
            VirtualMachine.interupts[code].Run(machine);
>>>>>>> comit:nvm/Codes/System/INT.cs
        }

        public byte GetByteCode()
        {
            return 0x01;
        }
    }
}

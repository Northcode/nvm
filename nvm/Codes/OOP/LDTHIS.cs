using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.OOP
{
    class LDTHIS : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            if (machine.callstack.Count > 0)
            {
                Call c = machine.callstack.Peek();
                if (c.Iaddr != -1)
                {
                    Instance i = new Instance() { address = machine.Mmanager.instances[c.Iaddr].address };
                }
            }
        }

        public byte GetByteCode()
        {
            return 0x18;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes
{
    class DBG : OpCode
    {

        public void Execute(VirtualMachine machine)
        {
            if (machine.al == 1)
            {
                machine.DEBUG = true;
            }
            else
            {
                machine.DEBUG = false;
            }
        }

        public byte GetByteCode()
        {
            return 0x13;
        }
    }
}

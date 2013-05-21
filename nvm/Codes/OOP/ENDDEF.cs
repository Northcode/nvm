using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.OOP
{
    public class ENDDEF : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            if (machine.currentBuildingClass != null)
            {
                machine.classes.Add(machine.currentBuildingClass);
                machine.currentBuildingClass = null;
            }
        }

        public byte GetByteCode()
        {
            return 0x17;
        }
    }
}

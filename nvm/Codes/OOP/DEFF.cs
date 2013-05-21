using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.OOP
{
    public class DEFF : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            if (machine.currentBuildingClass != null)
            {
                string funcname = machine.memory.ReadString(machine.IP);
                machine.IP += (uint)funcname.Length + 1;
                uint addr = machine.memory.ReadUInt(machine.IP);
                machine.IP += 4;
                machine.currentBuildingClass.functions.Add(new Tuple<string, uint>(funcname, addr));
            }
        }

        public byte GetByteCode()
        {
            return 0x16;
        }
    }
}

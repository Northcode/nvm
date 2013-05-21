using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.OOP
{
    public class DEF : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            machine.currentBuildingClass = new Objects.Class();
            machine.currentBuildingClass.name = machine.memory.ReadString(machine.IP);
            machine.IP += (uint)machine.currentBuildingClass.name.Length + 1;
            machine.currentBuildingClass.fields = new List<Objects.Field>();
            machine.currentBuildingClass.functions = new List<Tuple<string, uint>>();
        }

        public byte GetByteCode()
        {
            return 0x14;
        }
    }
}

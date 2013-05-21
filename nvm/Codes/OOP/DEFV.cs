using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.OOP
{
    public class DEFV : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            if (machine.currentBuildingClass != null)
            {
                Field f = new Field();
                f.Name = machine.memory.ReadString(machine.IP);
                machine.IP += (uint)f.Name.Length + 1;
                string typename = machine.memory.ReadString(machine.IP);
                machine.IP += (uint)typename.Length + 1;
                if (machine.ContainsClass(typename))
                {
                    f.Type = machine.GetClass(typename);
                }
                else if (typename == "int32")
                {
                    f.Type = Class.C_INT32;
                }
                else if (typename == "int16")
                {
                    f.Type = Class.C_INT16;
                }
                else if (typename == "byte")
                {
                    f.Type = Class.C_BYTE;
                }
                else if (typename == "string")
                {
                    f.Type = Class.C_STRING;
                }
                f.Parent = machine.currentBuildingClass;
                machine.currentBuildingClass.fields.Add(f);
            }
        }

        public byte GetByteCode()
        {
            return 0x15;
        }
    }
}

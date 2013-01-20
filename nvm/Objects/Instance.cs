using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Objects
{
    internal class Instance
    {
        internal Class parent;
        internal uint address;

        internal Tuple<byte,uint> GetField(int index)
        {
            return new Tuple<byte,uint>(parent.fields[index].type, parent.GetMachine().memory.ReadUInt((uint)(address + parent.name.Length + 2 + (index * 4))));
        }

        internal void StoreField(int index, uint address)
        {
            parent.GetMachine().memory.Write((uint)(this.address + parent.name.Length + 2 + (index * 4)), address);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Codes.System
{
    class ALLOC : OpCode
    {
        public void Execute(VirtualMachine machine)
        {
            byte type = machine.memory.Read(machine.IP);
            machine.IP++;
            if (type == MemoryManager.TYPE_BYTE)
            {
                byte val = machine.manager.Pop();
                uint addr = machine.manager.Alloc(val);
                machine.manager.Push(addr);
            }
            else if (type == MemoryManager.TYPE_2BYTE)
            {
                ushort val = machine.manager.Pop16();
                uint addr = machine.manager.Alloc(val);
                machine.manager.Push(addr);
            }
            else if (type == MemoryManager.TYPE_4BYTE)
            {
                int val = machine.manager.Pop32();
                uint addr = machine.manager.Alloc(val);
                machine.manager.Push(addr);
            }
            else if (type == MemoryManager.TYPE_STRING)
            {
                string str = machine.manager.PopS();
                uint addr = machine.manager.Alloc(str);
                machine.manager.Push(addr);
            }
        }

        public byte GetByteCode()
        {
            return (byte)0x29;
        }
    }
}

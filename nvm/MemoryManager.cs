using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    public class MemoryManager
    {
        public const byte TYPE_EMPTY = 0;
        public const byte TYPE_BYTE = 1;
        public const byte TYPE_4BYTE = 2;
        public const byte TYPE_STRING = 3;
        public const byte TYPE_INSTANCE = 4;

        Buffer memory;
        IClassContainer classcontainer;

        internal uint codeAddr;
        internal uint staticAddr;
        internal uint heapAddr;
        internal uint stackAddr;

        uint allocAddr;
        uint stackPointer;

        int maxStaticVars;

        public MemoryManager(VirtualMachine Machine, uint codeAddr, uint stackAddr, uint staticAddr, uint heapAddr)
        {
            classcontainer = Machine;
            memory = Machine.memory;
            this.codeAddr = codeAddr;
            this.stackAddr = stackAddr;
            this.staticAddr = staticAddr;
            this.heapAddr = heapAddr;
            this.allocAddr = 0;
            this.maxStaticVars = (int)((heapAddr - staticAddr) / 4);
        }

        public uint NextFreeAddr(uint size)
        {
            for (uint a = heapAddr; a < memory.data.Length; )
            {
                byte type = memory.data[a];
                a++;
                if (type == TYPE_EMPTY)
                {
                    uint sizecounter = 0;
                    uint oaddr = (uint)(a - 1);
                    while (a < memory.data.Length && (memory.data[a] == 0x00 && sizecounter < size)) { sizecounter++; a++; }
                    if (sizecounter >= size)
                    {
                        return oaddr;
                    }
                }
                else a = SkipAddr(a, type);
            }
            throw new Exception("Not enough memory to allocate size : " + size);
        }

        private uint SkipAddr(uint a, byte type)
        {
            if (type == TYPE_BYTE)
            {
                a += 1;
            }
            else if (type == TYPE_4BYTE)
            {
                a += 4;
            }
            else if (type == TYPE_STRING)
            {
                while (memory.data[a] != 0x00) { a++; } a++;
            }
            else if (type == TYPE_INSTANCE)
            {
                string classname = memory.ReadString(a);
                a += (uint)classname.Length + 1;
                Class c = classcontainer.GetClass(classname);
                int count = c.GetSize();
                a += (uint)(count * 4);
            }
            return a;
        }

        public uint Alloc(object value)
        {
            uint maddr = heapAddr + allocAddr;

            if (value is byte)
            {
                maddr = NextFreeAddr(1);
                memory.Write(maddr, TYPE_BYTE);
                memory.Write(maddr + 1, (byte)value);
                return maddr;
            }
            else if (value is int)
            {
                maddr = NextFreeAddr(4);
                memory.Write(maddr, TYPE_4BYTE);
                memory.Write(maddr + 1, (int)value);
                return maddr;
            }
            else if (value is string)
            {
                maddr = NextFreeAddr((uint)(value as string).Length);
                memory.Write(maddr, TYPE_STRING);
                memory.Write(maddr + 1, (value as string));
                return maddr;
            }
            else if (value is Instance)
            {
                Instance i = (value as Instance);
                maddr = NextFreeAddr((uint)(i.parent.name.Length + 1 + i.parent.GetSize() * 4));
                memory.Write(maddr, TYPE_INSTANCE);
                i.address = maddr;
                maddr++;
                memory.Write(maddr, i.parent.name);
                maddr += (uint)i.parent.name.Length + 1;
                int count = i.parent.GetSize();
                for (int j = 0; j < count; j++)
                {
                    memory.Write((uint)(maddr + (j * 4)), Alloc(i.parent.fields[j].New()));
                }
                return i.address;
            }
            throw new Exception("Can not allocate object of type : " + value.GetType());
        }

        public void StorePtr(int index, uint saddr)
        {
            if (index < maxStaticVars)
            {
                memory.Write(GetAddr(index), saddr);
            }
        }

        public uint LoadPtr(int index)
        {
            if (index < maxStaticVars)
            {
                return memory.ReadUInt(GetAddr(index));
            }
            throw new Exception("Variable index is out of variable range");
        }

        public uint GetAddr(int index)
        {
            return (uint)(staticAddr + (index * 4));
        }

        public void Free(int index)
        {
            uint addr = LoadPtr(index);
            byte size = memory.data[addr];
            Free(addr, size);
        }

        public void Free(uint addr, byte size)
        {
            if (size == TYPE_BYTE)
            {
                memory.Write(addr, (byte)0); //Clears type (tells allocator this is a free address)
                memory.Write(addr + 1, (byte)0); //Clear memory (write 0 to all locations the ptr used)
            }
            else if (size == TYPE_4BYTE)
            {
                memory.Write(addr, (byte)0); //Clears type
                memory.Write(addr + 1, 0); //Clear memory (write 0 to all locations the ptr used)
            }
            else if (size == TYPE_STRING)
            {
                string str = memory.ReadString(addr + 1); //Read string so we can get length
                memory.Write(addr, (byte)0); //Clears type
                for (int i = 0; i < str.Length; i++)
                {
                    memory.Write((uint)(addr + 1 + i), (byte)0); //Clears every byte string used
                }
            }
        }

        public void Push(byte val)
        {
            if (stackPointer + 1 > staticAddr)
            {
                throw new StackOverflowException("Cannot push any more items onto the stack!");
            }
            memory.Write(stackPointer, val);
            stackPointer++;
        }

        public void Push(ushort val)
        {
            if (stackPointer + 2 > staticAddr)
            {
                throw new StackOverflowException("Cannot push any more items onto the stack!");
            }
            memory.Write(stackPointer, val);
            stackPointer += 2;
        }

        public void Push(uint val)
        {
            if (stackPointer + 4 > staticAddr)
            {
                throw new StackOverflowException("Cannot push any more items onto the stack!");
            }
            memory.Write(stackPointer, val);
            stackPointer += 4;
        }

        public void Push(int val)
        {
            if (stackPointer + 4 > staticAddr)
            {
                throw new StackOverflowException("Cannot push any more items onto the stack!");
            }
            memory.Write(stackPointer, val);
            stackPointer += 4;
        }

        public void Push(float val)
        {
            if (stackPointer + 4 > staticAddr)
            {
                throw new StackOverflowException("Cannot push any more items onto the stack!");
            }
            memory.Write(stackPointer, val);
            stackPointer += 4;
        }

        public void Push(string val)
        {
            if (stackPointer + val.Length + 1 + 4 > staticAddr)
            {
                throw new StackOverflowException("Cannot push any more items onto the stack!");
            }
            memory.Write(stackPointer, val);
            stackPointer += (uint)val.Length;
            memory.Write(stackPointer, (uint)val.Length);
            stackPointer += 4;
        }

        public byte Pop()
        {
            stackPointer--;
            return memory.Read(stackPointer);
        }
    }
}

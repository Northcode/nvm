using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvmv2
{
    public class VM
    {
        internal Stack<object> stack;
        internal Stack<Tuple<uint, int>> callstack;
        internal uint IP;
        internal Buffer Memory;
        internal uint[] locals;
        internal bool RN;
        internal uint heapstart;
        internal List<MemChunk> freeList;

        internal static OpCode[] opcodes;
        internal static Interupt[] interups;

        public VM(byte[] Program)
        {
            stack = new Stack<object>();
            callstack = new Stack<Tuple<uint, int>>();
            IP = 0;
            Memory = new Buffer(Program.Length + 4 * Buffer.KB, Program);
            heapstart = (uint)Program.Length + 32;
            locals = new uint[50];
            freeList = new List<MemChunk>();
            freeList.Add(new MemChunk() { chunkstart = heapstart, size = (int)(Memory.Size - heapstart - 1) });
        }

        public void Run()
        {
            RN = true;
            while (RN)
            {
                byte op = Memory.Read(IP); IP++;
                opcodes[op].Run(this);
            }
        }

        public static void InitOpcodes()
        {
            opcodes = new OpCode[] {
                new OpCode() {
                    Name = "NOP", BYTECODE = 0x00,
                    Run = (m) => {}
                },
                new OpCode() { 
                    Name = "JMP", BYTECODE = 0x01,
                    Run = (m) => {
                        uint addr = m.Memory.ReadUInt(m.IP); m.IP += 4;
                        if(addr != 0) 
                        {
                            m.IP = addr;
                        }
                        else
                        {
                            Console.WriteLine("ERROR: CANNOT JMP TO ADDRESS 0");
                        }
                    }
                }, //-------------------
                new OpCode() {
                    Name = "CALL", BYTECODE = 0x02,
                    Run = (m) => { 
                        uint addr = m.Memory.ReadUInt(m.IP); m.IP += 4;
                        if(addr != 0)
                        {
                            m.callstack.Push(new Tuple<uint, int>(m.IP, -1));
                            m.IP = addr;
                        }
                        else
                        {
                            Console.WriteLine("ERROR: CANNOT JMP TO ADDRESS 0");
                        }
                    }
                }, //-------------------
                new OpCode() {
                    Name = "PUSH", BYTECODE = 0x03,
                    Run = (m) => {
                        byte type = m.Memory.Read(m.IP); m.IP++;
                        object val = null;
                        switch (type)
	                    {
                            case ValueTypeCodes.BYTE:
                                val = m.Memory.Read(m.IP); m.IP++;
                                break;
                            case ValueTypeCodes.SHORT:
                                val = m.Memory.ReadUInt16(m.IP); m.IP += 2;
                                break;
                            case ValueTypeCodes.INT:
                                val = m.Memory.ReadInt(m.IP); m.IP += 4;
                                break;
                            case ValueTypeCodes.UINT:
                                val = m.Memory.ReadUInt(m.IP); m.IP += 4;
                                break;
                            case ValueTypeCodes.BOOL:
                                val = (m.Memory.Read(m.IP) == 0x01); m.IP++;
                                break;
                            case ValueTypeCodes.STRING:
                                string v = m.Memory.ReadString(m.IP); m.IP += (uint)v.Length + 1;
                                val = v;
                                break;
                            default:
                                break;
	                    }
                        m.stack.Push(val);
                    }
                }, //-------------------
                new OpCode() {
                    Name = "POP", BYTECODE = 0x04,
                    Run = (m) => { m.stack.Pop(); }
                }, //-------------------
                new OpCode() {
                    Name = "LOCALCNT", BYTECODE = 0x05,
                    Run = (m) => {
                        int c = m.Memory.ReadInt(m.IP); m.IP += 4;
                        m.locals = new uint[c];
                    }
                },//-------------------
                new OpCode() {
                    Name = "END", BYTECODE = 0x06,
                    Run = (m) => { m.RN = false; }
                }, //-------------------
                new OpCode() {
                    Name = "DMPSTACK", BYTECODE = 0x07,
                    Run = (m) => {
                        while(m.stack.Count > 0) {
                            Console.WriteLine(m.stack.Pop());
                        }
                    }
                },
                new OpCode() {
                    Name = "STLOC", BYTECODE = 0x08,
                    Run = (m) => {
                        int v = m.Memory.ReadInt(m.IP); m.IP += 4;
                        object o = m.stack.Pop();
                        uint aaddr = m.Alloc(o);
                        m.locals[v] = aaddr;
                    }
                },
                new OpCode() {
                    Name = "FREELOC", BYTECODE = 0x09,
                    Run = (m) => {
                        int v = m.Memory.ReadInt(m.IP); m.IP += 4;
                        uint addr = m.locals[v];
                        byte t = m.Memory.Read(addr);
                        if(t == ValueTypeCodes.BYTE)
                        {
                            m.Free(addr, 2);
                        }
                        else if(t == ValueTypeCodes.INT)
                        {
                            m.Free(addr, 5);
                        }
                        m.locals[v] = 0;
                    }
                },
                new OpCode() {
                    Name = "LDLOC", BYTECODE = 0x0a,
                    Run = (m) => {
                        int l = m.Memory.ReadInt(m.IP); m.IP += 4;
                        uint addr = m.locals[l];
                        byte t = m.Memory.Read(addr);
                        if(t == ValueTypeCodes.BYTE)
                        {
                            byte v = m.Memory.Read(addr + 1);
                            m.stack.Push(v);
                        }
                        else if(t == ValueTypeCodes.INT)
                        {
                            int v = m.Memory.ReadInt(addr + 1);
                            m.stack.Push(v);
                        }
                        else if(t == ValueTypeCodes.UINT)
                        {
                            uint v = m.Memory.ReadUInt(addr + 1);
                            m.stack.Push(v);
                        }
                        else if(t == ValueTypeCodes.STRING)
                        {
                            string s = m.Memory.ReadString(addr + 1);
                            m.stack.Push(s);
                        }
                    }
                },
                new OpCode() {
                    Name = "LDPTR", BYTECODE = 0x0b,
                    Run = (m) => {
                        int l = m.Memory.ReadInt(m.IP); m.IP += 4;
                        uint addr = m.locals[l];
                        m.stack.Push(addr);
                    }
                },
                new OpCode() {
                    Name = "STPTR", BYTECODE = 0x0c,
                    Run = (m) => {
                        int l = m.Memory.ReadInt(m.IP); m.IP += 4;
                        uint addr = (uint)m.stack.Pop();
                        m.locals[l] = addr;
                    }
                },
                new OpCode() {
                    Name = "ARRAY", BYTECODE = 0x0d,
                    Run = (m) => {
                        int size = (int)m.stack.Pop();
                        uint addr = m.MAlloc(size * 4 + 4);
                        m.Memory.Write(addr, size);
                        m.stack.Push(addr);
                    }
                },
                new OpCode() {
                    Name = "FREEARRAY", BYTECODE = 0x0e,
                    Run = (m) => {
                        uint addr = (uint)m.stack.Pop();
                        int size = m.Memory.ReadInt(addr);
                        m.Free(addr, size * 4 + 4);
                    }
                },
                new OpCode() {
                    Name = "ST_ELEM", BYTECODE = 0x0f,
                    Run = (m) => {
                        object o = m.stack.Pop();
                        int index = (int)m.stack.Pop();
                        uint array = (uint)m.stack.Pop();
                        uint addr = m.Alloc(o);
                        m.Memory.Write((uint)(array + 4 + index * 4),addr);
                    }
                },
                new OpCode() {
                    Name = "ST_ELEM_PTR", BYTECODE = 0x10,
                    Run = (m) => {
                        uint paddr = (uint)m.stack.Pop();
                        int index = (int)m.stack.Pop();
                        uint array = (uint)m.stack.Pop();
                        m.Memory.Write((uint)(array + 4 + index * 4),paddr);
                    }
                },
                new OpCode() {
                    Name = "LD_ELEM", BYTECODE = 0x11,
                    Run = (m) => {
                        int index = (int)m.stack.Pop();
                        uint array = (uint)m.stack.Pop();
                        uint raddr = m.Memory.ReadUInt((uint)(array + 4 + index * 4));
                        m.stack.Push(raddr);
                    }
                },
                new OpCode() {
                    Name = "LDADDR", BYTECODE = 0x12,
                    Run = (m) => {
                        uint addr = (uint)m.stack.Pop();
                        byte t = m.Memory.Read(addr);
                        if(t == ValueTypeCodes.BYTE)
                        {
                            byte v = (byte)m.Memory.Read(addr + 1);
                            m.stack.Push(v);
                        }
                        else if (t == ValueTypeCodes.INT)
                        {
                            int v = (int)m.Memory.ReadInt(addr + 1);
                            m.stack.Push(v);
                        }
                        else if (t == ValueTypeCodes.STRING)
                        {
                            string v = (string)m.Memory.ReadString(addr + 1);
                            m.stack.Push(v);
                        }
                    }
                },
                new OpCode() {
                    Name = "MALLOC", BYTECODE = 0x13,
                    Run = (m) => {
                        int size = (int)m.stack.Pop();
                        uint addr = m.MAlloc(size);
                        m.stack.Push(addr);
                    }
                },
                new OpCode() {
                    Name = "FREE", BYTECODE = 0x14,
                    Run = (m) => {
                        int size = (int)m.stack.Pop();
                        uint addr = (uint)m.stack.Pop();
                        m.Free(addr, size);
                    }
                },
                new OpCode() {
                    Name = "SYSF_PRINT", BYTECODE = 0x15,
                    Run = (m) => {
                        object o = m.stack.Pop();
                        Console.Write(o);
                    }
                },
                new OpCode() {
                    Name = "SYSF_READLN", BYTECODE = 0x16,
                    Run = (m) => {
                        string s = Console.ReadLine();
                        m.stack.Push(s);
                    }
                },
                new OpCode() {
                    Name = "ADD", BYTECODE = 0x17,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            byte c = (byte)((byte)a + (byte)b);
                            m.stack.Push(c);
                        }
                        else if(a is int && b is int)
                        {
                            int c = (int)a + (int)b;
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "SUB", BYTECODE = 0x18,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            byte c = (byte)((byte)a - (byte)b);
                            m.stack.Push(c);
                        }
                        else if(a is int && b is int)
                        {
                            int c = (int)a - (int)b;
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "MUL", BYTECODE = 0x19,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            byte c = (byte)((byte)a * (byte)b);
                            m.stack.Push(c);
                        }
                        else if(a is int && b is int)
                        {
                            int c = (int)a * (int)b;
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "DIV", BYTECODE = 0x1a,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            byte c = (byte)((byte)a / (byte)b);
                            m.stack.Push(c);
                        }
                        else if(a is int && b is int)
                        {
                            int c = (int)a / (int)b;
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "MOD", BYTECODE = 0x1b,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            byte c = (byte)((byte)a % (byte)b);
                            m.stack.Push(c);
                        }
                        else if(a is int && b is int)
                        {
                            int c = (int)a % (int)b;
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "OR", BYTECODE = 0x1c,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            bool ba = ((byte)a == 1);
                            bool bb = ((byte)b == 1);
                            bool bc = ba || bb;
                            byte c = (byte)(bc ? 1 : 0);
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "AND", BYTECODE = 0x1d,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            bool ba = ((byte)a == 1);
                            bool bb = ((byte)b == 1);
                            bool bc = ba && bb;
                            byte c = (byte)(bc ? 1 : 0);
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "XOR", BYTECODE = 0x1e,
                    Run = (m) => {
                        object b = m.stack.Pop();
                        object a = m.stack.Pop();
                        if(a is byte && b is byte)
                        {
                            bool ba = ((byte)a == 1);
                            bool bb = ((byte)b == 1);
                            bool bc = ba ^ bb;
                            byte c = (byte)(bc ? 1 : 0);
                            m.stack.Push(c);
                        }
                    }
                },
                new OpCode() {
                    Name = "RET", BYTECODE = 0x1f,
                    Run = (m) => {
                        Tuple<uint, int> c = m.callstack.Pop();
                        m.IP = c.Item1;
                    }
                },
                new OpCode() {
                    Name = "MEMW", BYTECODE = 0x20,
                    Run = (m) => {
                        uint u = (uint)m.stack.Pop();
                        object o = m.stack.Pop();
                        if (o is byte)
                        {
                            m.Memory.Write(u, (byte)o);
                        }
                        else if (o is int)
                        {
                            m.Memory.Write(u, (int)o);
                        }
                        else if (o is uint)
                        {
                            m.Memory.Write(u, (uint)o);
                        }
                        else if (o is string)
                        {
                            m.Memory.Write(u, (string)o);
                        }
                    }
                },
                new OpCode() {
                    Name = "MEMR", BYTECODE = 0x21,
                    Run = (m) => {
                        byte t = (byte)m.stack.Pop();
                        uint u = (uint)m.stack.Pop();
                        if (t == ValueTypeCodes.BYTE)
                        {
                            byte b = m.Memory.Read(u);
                            m.stack.Push(b);
                        }
                        else if (t == ValueTypeCodes.INT)
                        {
                            int i = m.Memory.ReadInt(u);
                            m.stack.Push(i);
                        }
                        else if (t == ValueTypeCodes.UINT)
                        {
                            uint i = m.Memory.ReadUInt(u);
                            m.stack.Push(i);
                        }
                        else if (t == ValueTypeCodes.STRING)
                        {
                            string i = m.Memory.ReadString(u);
                            m.stack.Push(i);
                        }
                    }
                },
                new OpCode() {
                    Name = "SIZEOF", BYTECODE = 0x22,
                    Run = (m) => {
                        object o = m.stack.Pop();
                        if (o is byte)
                        {
                            m.stack.Push(1);
                        }
                        else if (o is int)
                        {
                            m.stack.Push(4);
                        }
                        else if (o is string)
                        {
                            m.stack.Push((o as string).Length + 1);
                        }
                    }
                }
            };
        }

        public uint Alloc(object val)
        {
            if (val is byte)
            {
                int i = AllocFindChunk(2);
                uint addr = freeList[i].chunkstart;
                uint naddr = addr + 2;
                freeList[i].chunkstart = naddr;
                Memory.Write(addr, ValueTypeCodes.BYTE);
                Memory.Write(addr + 1, (byte)val);
                return addr;
            }
            else if (val is int)
            {
                int i = AllocFindChunk(5);
                uint addr = freeList[i].chunkstart;
                uint naddr = addr + 5;
                freeList[i].chunkstart = naddr;
                Memory.Write(addr, ValueTypeCodes.INT);
                Memory.Write(addr + 1, (int)val);
                return addr;
            }
            else if (val is uint)
            {
                int i = AllocFindChunk(5);
                uint addr = freeList[i].chunkstart;
                uint naddr = addr + 5;
                freeList[i].chunkstart = naddr;
                Memory.Write(addr, ValueTypeCodes.UINT);
                Memory.Write(addr + 1, (uint)val);
                return addr;
            }
            else if (val is string)
            {
                int i = AllocFindChunk((val as string).Length + 2);
                uint addr = freeList[i].chunkstart;
                uint naddr = (uint)(addr + (val as string).Length + 2);
                freeList[i].chunkstart = naddr;
                Memory.Write(addr, ValueTypeCodes.STRING);
                Memory.Write(addr + 1, (val as string));
                return addr;
            }
            throw new Exception("Allocation failed!");
        }

        public uint MAlloc(int size)
        {
            int i = AllocFindChunk(size);
            uint addr = freeList[i].chunkstart;
            return addr;
        }

        private int AllocFindChunk(int size)
        {
            for (int i = 0; i < freeList.Count; i++)
            {
                if (freeList[i].size > size)
                {
                    return i;
                }
            }
            throw new OutOfMemoryException("No free chunks found large enough");
        }

        public void Free(uint address,int size)
        {
            MemChunk freechunk = new MemChunk() { chunkstart = address, size = size };
            freeList.Add(freechunk);
            freeList = freeList.OrderBy(c => c.chunkstart).ToList();
            MergeChunks();
        }

        public void MergeChunks()
        {
            if (freeList.Count > 1)
            {
                MemChunk last = null;
                bool b = false;
                for (int i = 0; i < freeList.Count; i++)
                {
                    b = false;
                    if (last != null)
                    {
                        if (last.chunkstart + last.size == freeList[i].chunkstart)
                        {
                            last.size = last.size + freeList[i].size;
                            freeList.RemoveAt(i);
                            i--;
                            b = true;
                        }
                        else if (freeList[i].chunkstart + freeList[i].size == last.chunkstart)
                        {
                            freeList[i].size = freeList[i].size + last.size;
                            freeList.RemoveAt(i - 1);
                        }
                    }
                    if (!b)
                    {
                        last = freeList[i];
                    }
                }
            }
        }
    }

    public static class ValueTypeCodes
    {
        public const byte NULL = 0x00;
        public const byte BYTE = 0x01;
        public const byte SHORT = 0x02;
        public const byte INT = 0x03;
        public const byte UINT = 0x04;
        public const byte LONG = 0x05;
        public const byte BOOL = 0x06;
        public const byte STRING = 0x07;
    }

    public class Buffer
    {
        // Different standard memory sizes
        public const int DEF_SIZE = 512;
        public const int KB = 1024;
        public const int MB = 1024 * 1000;
        public const int GB = 1024 * 1000 * 1000;

        /// <summary>
        /// For storing the data in the memory object
        /// </summary>
        internal byte[] data;
        internal uint pos = 0;

        internal int Size
        {
            get { return data.Length; }
        }

        //Constuctors
        public Buffer()
        {
            data = new byte[DEF_SIZE];
        }

        public Buffer(int size)
        {
            data = new byte[size];
            pos = 0;
        }

        public Buffer(int size, byte[] data)
        {
            this.data = new byte[size];
            if (data.Length < size)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    this.data[i] = data[i];
                }
            }
            pos = 0;
        }

        //Write methods
        public void Write(uint address, byte value)
        {
            data[address] = value;
        }

        public void Write(uint address, ushort value)
        {
            byte[] convertedValues = BitConverter.GetBytes(value);
            data[address + 0] = convertedValues[0];
            data[address + 1] = convertedValues[1];
        }

        public void Write(uint address, int value)
        {
            byte[] convertedValues = BitConverter.GetBytes(value);
            data[address + 0] = convertedValues[0];
            data[address + 1] = convertedValues[1];
            data[address + 2] = convertedValues[2];
            data[address + 3] = convertedValues[3];
        }

        public void Write(uint address, uint value)
        {
            byte[] convertedValues = BitConverter.GetBytes(value);
            data[address + 0] = convertedValues[0];
            data[address + 1] = convertedValues[1];
            data[address + 2] = convertedValues[2];
            data[address + 3] = convertedValues[3];
        }

        public void Write(uint address, float value)
        {
            byte[] convertedValues = BitConverter.GetBytes(value);
            data[address + 0] = convertedValues[0];
            data[address + 1] = convertedValues[1];
            data[address + 2] = convertedValues[2];
            data[address + 3] = convertedValues[3];
        }

        public void Write(uint address, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                Write((uint)(address + i), ((byte)value[i]));
            }
            Write((uint)(address + value.Length + 1), 0x00);
        }

        public byte Read(uint address)
        {
            return data[address];
        }

        internal ushort ReadUInt16(uint address)
        {
            return BitConverter.ToUInt16(new byte[] { data[address], data[address + 1] }, 0);
        }

        public int ReadInt(uint address)
        {
            return BitConverter.ToInt32(new byte[] { data[address], data[address + 1], data[address + 2], data[address + 3] },0);
        }

        public uint ReadUInt(uint address)
        {
            return BitConverter.ToUInt32(new byte[] { data[address], data[address + 1], data[address + 2], data[address + 3] }, 0);
        }

        public float ReadFloat(uint address)
        {
            return BitConverter.ToSingle(new byte[] { data[address], data[address + 1], data[address + 2], data[address + 3] }, 0);
        }

        public string ReadString(uint address)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (data[address + i] != 0x00)
            {
                sb.Append((char)(data[address + i]));
                i++;
            }
            return sb.ToString();
        }

        internal string ReadString(uint address, int len)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < len)
            {
                sb.Append((char)(data[address + i]));
                i++;
            }
            return sb.ToString();
        }
    }

    public class MemChunk
    {
        internal uint chunkstart;
        internal int size;
    }

    public delegate void OPRUN(VM m);

    public class OpCode
    {
        internal string Name { get; set; }
        internal byte BYTECODE { get; set; }
        internal OPRUN Run;
    }

    public class Interupt
    {
        internal string Name { get; set; }
        internal OPRUN Run;
    }
}

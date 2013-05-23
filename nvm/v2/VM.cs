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
                    Run = (m) => { uint addr = m.Memory.ReadUInt(m.IP); m.IP = addr; }
                }, //-------------------
                new OpCode() {
                    Name = "CALL", BYTECODE = 0x02,
                    Run = (m) => { 
                        uint addr = m.Memory.ReadUInt(m.IP); m.IP += 4;
                        m.callstack.Push(new Tuple<uint, int>(addr, -1));
                        m.IP = addr;
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
                    Name = "META", BYTECODE = 0x05,
                    Run = (m) => { }
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

                    }
                }
            };
        }

        public uint Alloc(object val)
        {
            if (val is byte)
            {
                for (int i = 0; i < freeList.Count; i++)
                {

                }
            }
        }

        public void Free(uint address,int size)
        {
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

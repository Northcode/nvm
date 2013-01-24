using System;
using System.Collections.Generic;
using System.Text;

namespace nvm
{
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

        internal ushort ReadUInt16(uint p)
        {
            return BitConverter.ToUInt16(new byte[] { data[p] , data[p + 1] },0);
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
    }
}

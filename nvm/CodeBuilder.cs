using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    class CodeBuilder
    {
        private Buffer buffer;
        private uint pos;
        private Dictionary<string, int> labels;

        public CodeBuilder(int size)
        {
            buffer = new Buffer(size);
            labels = new Dictionary<string, int>();
            pos = 0;
        }

        public void WriteBytes(params object[] args)
        {
            foreach (object arg in args)
            {
                if (arg is byte)
                {
                    buffer.Write(pos, (byte)arg);
                    pos++;
                }
                else if (arg is ushort)
                {
                    buffer.Write(pos, (ushort)arg);
                    pos += 2;
                }
                else if (arg is int)
                {
                    buffer.Write(pos, (int)arg);
                    pos += 4;
                }
                else if (arg is float)
                {
                    buffer.Write(pos, (float)arg);
                    pos += 4;
                }
                else if (arg is char)
                {
                    buffer.Write(pos, (byte)((char)arg));
                    pos++;
                }
                else if (arg is string)
                {
                    foreach (char c in (arg as string))
                    {
                        WriteBytes(c);
                    }
                }
            }
        }

        public void Jump(uint newaddress)
        {
            pos = newaddress;
        }

        public uint GetAddress()
        {
            return pos;
        }

        public byte[] GetCode()
        {
            return buffer.data;
        }

        public void AddLabel(string name, int address)
        {
            labels.Add(name, address);
        }

        public int Label(string name)
        {
            return labels[name];
        }
    }
}

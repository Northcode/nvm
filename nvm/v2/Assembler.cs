using nvm.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvmv2
{
    class Assembler
    {
        internal string code { get; set; }

        internal Dictionary<string, uint> labels;
        internal List<Tuple<int, string>> labelcalls;

        public byte[] Assemble()
        {
            List<byte> result = new List<byte>();

            labels = new Dictionary<string,uint>();
            labelcalls = new List<Tuple<int,string>>();

            code = code.Replace("\r", "");

            foreach (string line in code.Split('\n'))
            {
                var words = line.Split(new[] { '"' }).SelectMany((s, i) =>
                {
                    if (i % 2 == 1) return new[] { s };
                    return s.Split(new[] { ' ' },
                                   StringSplitOptions.RemoveEmptyEntries);
                }).ToList();

                foreach (string word in words)
                {
                    string trimmed = word.Trim();
                    int i = 0;
                    ushort u = 0;
                    byte b = 0;
                    if (int.TryParse(trimmed, out i))
                    {
                        byte[] vals = BitConverter.GetBytes(i);
                        result.AddRange(vals);
                    }
                    else if (trimmed.StartsWith("h"))
                    {
                        string sub = trimmed.Substring(1);
                        if (byte.TryParse(sub, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out b))
                        {
                            result.Add(b);
                        }
                    }
                    else if (trimmed.StartsWith("s"))
                    {
                        string sub = trimmed.Substring(1);
                        if (ushort.TryParse(sub, out u))
                        {
                            byte[] vals = BitConverter.GetBytes(u);
                            result.AddRange(vals);
                        }
                    }
                    else if (VM.opcodes.Any(p => p.Name == trimmed))
                    {
                        OpCode op = VM.opcodes.First(p => p.Name == trimmed);
                        result.Add(op.BYTECODE);
                    }
                    else if (word.EndsWith(":"))
                    {
                        labels.Add(word.Substring(0, word.Length - 1), (uint)result.Count);
                    }
                    else if (word.StartsWith(":"))
                    {
                        labelcalls.Add(new Tuple<int, string>(result.Count, word.Substring(1)));
                        result.AddRange(new byte[4]);
                    }
                    else
                    {
                        byte[] bytea = Encoding.ASCII.GetBytes(word);
                        result.AddRange(bytea);
                        result.Add((byte)0x00);
                    }
                }
            }

            foreach (KeyValuePair<string, uint> label in labels)
            {
                byte[] addr = BitConverter.GetBytes(label.Value);
                foreach (Tuple<int, string> call in labelcalls.FindAll(t => t.Item2 == label.Key))
                {
                    result[call.Item1] = addr[0];
                    result[call.Item1 + 1] = addr[1];
                    result[call.Item1 + 2] = addr[2];
                    result[call.Item1 + 3] = addr[3];
                }
            }

            return result.ToArray();
        }
    }
}

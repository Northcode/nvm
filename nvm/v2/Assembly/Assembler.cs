﻿using nvm.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.v2.Assembly
{
    public class Assembler
    {
        public string code { get; set; }
        public CompilerMeta CompilerMeta { get; set; }

        public ProgramMeta programMeta { get; private set; }

        internal Dictionary<string, uint> labels;
        internal List<Tuple<int, string>> labelcalls;

        public NcAssembly Assemble()
        {
            List<byte> result = new List<byte>();

            labels = new Dictionary<string,uint>();
            labelcalls = new List<Tuple<int,string>>();

            int localcount = 0;

            code = code.Replace("\r", "");

            if (CompilerMeta != null)
            {
                programMeta = new ProgramMeta();
                programMeta.AsmName = CompilerMeta.ProgramName;
                programMeta.localData = CompilerMeta.localMeta;
                programMeta.functionData = new Dictionary<uint, string>();
            }

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
                    if (word == "LOCALCNT")
	                {
		                localcount = -1;
	                }
                    else if (localcount == -1)
	                {
                        localcount = Convert.ToInt32(word);
	                }
                    else if (int.TryParse(trimmed, out i))
                    {
                        byte[] vals = BitConverter.GetBytes(i);
                        result.AddRange(vals);
                    }
                    else if (word == "byte")
                    {
                        result.Add(ValueTypeCodes.BYTE);
                    }
                    else if (word == "int")
                    {
                        result.Add(ValueTypeCodes.INT);
                    }
                    else if (word == "string")
                    {
                        result.Add(ValueTypeCodes.STRING);
                    }
                    else if (word == "uint")
                    {
                        result.Add(ValueTypeCodes.UINT);
                    }
                    else if (word == "true")
                    {
                        result.Add(1);
                    }
                    else if (word == "false")
                    {
                        result.Add(0);
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
                    else if (trimmed.StartsWith("h"))
                    {
                        string sub = trimmed.Substring(1);
                        if (byte.TryParse(sub, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out b))
                        {
                            result.Add(b);
                        }
                        else
                        {
                            string nword = word.Replace("\\n", "\n").Replace("\\'", "\"");
                            byte[] bytea = Encoding.ASCII.GetBytes(nword);
                            result.AddRange(bytea);
                            result.Add((byte)0x00);
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
                        else
                        {
                            string nword = word.Replace("\\n", "\n").Replace("\\'", "\"");
                            byte[] bytea = Encoding.ASCII.GetBytes(nword);
                            result.AddRange(bytea);
                            result.Add((byte)0x00);
                        }
                    }
                    else
                    {
                        string nword = word.Replace("\\n", "\n").Replace("\\'", "\"");
                        byte[] bytea = Encoding.ASCII.GetBytes(nword);
                        result.AddRange(bytea);
                        result.Add((byte)0x00);
                    }
                }
            }

            bool dometa = (programMeta != null);
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
                if (dometa)
                {
                    programMeta.functionData.Add(label.Value, label.Key);
                }
            }

            NcAssembly assembly = new NcAssembly();
            assembly.code = result.ToArray();
            return assembly;
        }
    }
}

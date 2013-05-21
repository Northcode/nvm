using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Objects
{
    public class Class
    {
        VirtualMachine machine;

        public string name { get; set; }
        public List<Field> fields { get; set; }
        public List<Tuple<string, uint>> functions { get; set; }

        public int GetSize()
        {
            return fields.Count;
        }

        public object New()
        {
            if (name == "byte")
            {
                return (byte)0x00;
            }
            else if (name == "int16")
            {
                return (short)0;
            }
            else if (name == "int32")
            {
                return 0;
            }
            else if (name == "string")
            {
                return "";
            }
            else
            {
                return new Instance() { parent = this, address = machine.Mmanager.NextFreeAddr((uint)this.GetSize()) };
            }
        }

        public static Class C_BYTE = new Class() { name = "byte", fields = new List<Field>(), functions = new List<Tuple<string, uint>>() };
        public static Class C_INT16 = new Class() { name = "int16", fields = new List<Field>(), functions = new List<Tuple<string, uint>>() };
        public static Class C_INT32 = new Class() { name = "int32", fields = new List<Field>(), functions = new List<Tuple<string, uint>>() };
        public static Class C_STRING = new Class() { name = "string", fields = new List<Field>(), functions = new List<Tuple<string, uint>>() };
    }
}

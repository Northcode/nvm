using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Objects
{
    public class Class : IClassContainer
    {
        internal string name;

        internal Field[] fields;
        internal Class[] subclasses;
        internal IClassContainer parent;

        internal int GetSize()
        {
            return fields.Length;
        }

        public Class GetClass(string Name)
        {
            foreach (Class c in subclasses)
            {
                if (c.name == Name)
                {
                    return c;
                }
            }
            throw new Exception("Class '" + Name + "' not found in class: " + name);
        }

        public VirtualMachine GetMachine()
        {
            if (parent is VirtualMachine)
            {
                return parent as VirtualMachine;
            }
            else
            {
                return parent.GetMachine();
            }
        }
    }
}

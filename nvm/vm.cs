using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    public class VirtualMachine : IClassContainer
    {
        internal Buffer memory;
        internal MemoryManager manager;
        internal Stack<object> stack;

        internal Stack<Call> callstack;
        internal Class[] classes;

        public Class GetClass(string Name)
        {
            foreach (Class c in classes)
            {
                if (c.name == Name)
                {
                    return c;
                }
            }
            throw new Exception("Class '" + Name + "' not found in root");
        }


        public VirtualMachine GetMachine()
        {
            return this;
        }
    }
}

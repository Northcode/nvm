using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace nvm
{
    interface IClassContainer
    {
        Class GetClass(string Name);
        bool ContainsClass(string ClassName);

        VirtualMachine GetMachine();
    }
}

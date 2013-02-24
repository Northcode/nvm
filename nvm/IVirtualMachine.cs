using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    interface IVirtualMachine
    {
        void Run();

        void SetRegister(byte reg, object val);

        object GetRegister(byte reg);
    }
}

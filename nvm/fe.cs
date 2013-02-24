using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    public enum Error_Flags : byte
    {
        NO_ERROR = 0x00,
        CODE_ADDRESS_OVERFLOW = 0x01,
    }
}

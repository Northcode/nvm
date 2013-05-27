using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.v2
{
    public static class ValueTypeCodes
    {
        public const byte NULL = 0x00;
        public const byte BYTE = 0x01;
        public const byte SHORT = 0x02;
        public const byte INT = 0x03;
        public const byte UINT = 0x04;
        public const byte LONG = 0x05;
        public const byte BOOL = 0x06;
        public const byte STRING = 0x07;
    }
}

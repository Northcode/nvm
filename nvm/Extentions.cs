using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    public static class Extentions
    {
        public static byte[] ToBytes(this int e)
        {
            return BitConverter.GetBytes(e);
        }

        public static byte[] ToBytes(this float e)
        {
            return BitConverter.GetBytes(e);
        }
    }
}

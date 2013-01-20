using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Objects
{
    class Field
    {
        internal byte type;
        internal string name;
        internal object newobj;

        internal object New()
        {
            return newobj;
        }
    }
}

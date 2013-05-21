using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Objects
{
    public class Field
    {
        public Class Parent { get; set; }
        public string Name { get; set; }
        public Class Type { get; set; }

        public object New()
        {
            return Type.New();
        }
    }
}

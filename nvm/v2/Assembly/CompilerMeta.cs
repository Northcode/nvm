using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.v2.Assembly
{
    public class CompilerMeta
    {
        public string ProgramName { get; set; }
        public Dictionary<int, string> localMeta { get; set; }
    }
}

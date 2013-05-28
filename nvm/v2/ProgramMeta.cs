using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.v2
{
    public class ProgramMeta
    {
        public Dictionary<uint, string> functionData { get; set; }
        public Dictionary<int, string> localData { get; set; }
        public string AsmName { get; set; }
        public List<uint> breakPoints { get; set; }
    }
}

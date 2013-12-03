using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.virtualmachine
{
    public interface VitrualMachineInstruction
    {

    }

    public class Local
	{
        object data;
	}

    public class VirtualMachine
    {
        public Stack<object> stack;
        public List<Local> locals;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nvm.Interupts
{
    class Interupt1 : Interupt
    {
        public void Run(VirtualMachine machine)
        {
            switch (machine.ah)
            {
                case 0x00:
                    string wspath = machine.manager.PopS();
                    string wsstr = machine.manager.PopS();
                    File.WriteAllText(wspath, wsstr);
                    break;
                case 0x01:
                    string rspath = machine.manager.PopS();
                    string rstxt = File.ReadAllText(rspath);
                    machine.manager.Push(rstxt);
                    break;
                default:
                    break;
            }
        }
    }
}

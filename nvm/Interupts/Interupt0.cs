using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Interupts
{
    class Interupt0 : Interupt
    {
        public void Run(VirtualMachine machine)
        {
            switch (machine.ah)
            {
                case 0x00:
                    Console.Write((char)machine.al);
                    break;
                case 0x01:
                    uint addr = (uint)machine.edx;
                    string str = machine.memory.ReadString(addr);
                    Console.Write(str);
                    break;
                case 0x02:
                    Console.WriteLine();
                    break;
                case 0x03:
                    Console.Write(machine.al);
                    break;
                case 0x04:
                    Console.Write(machine.ax);
                    break;
                case 0x05:
                    Console.Write(machine.eax);
                    break;
                default:
                    break;
            }
        }
    }
}

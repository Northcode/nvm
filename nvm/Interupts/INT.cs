using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm.Interupts
{
    class INT1 : Interupt
    {
        public void Run(VirtualMachine machine)
        {
            if (machine.ah == 0x00)
            {
                Console.Write(machine.Mmanager.PopS());
            }
            else if (machine.ah == 0x01)
            {
                Console.WriteLine(machine.Mmanager.PopS());
            }
            else if (machine.ah == 0x02)
            {
                Console.WriteLine();
            }
            else if (machine.ah == 0x03)
            {
                Console.Write(machine.al);
            }
            else if (machine.ah == 0x04)
            {
                Console.Write(machine.ax);
            }
            else if (machine.ah == 0x05)
            {
                Console.Write(machine.eax);
            }
            else if (machine.ah == 0x06)
            {
                Console.Write((char)machine.al);
            }
            else if (machine.ah == 0x07)
            {
                machine.Mmanager.Push(Console.Read());
            }
            else if (machine.ah == 0x08)
            {
                machine.Mmanager.Push(Console.ReadKey().KeyChar);
            }
            else if (machine.ah == 0x09)
            {
                machine.Mmanager.Push(Console.ReadLine());
            }
        }
    }
}

using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nvm
{
    class Program
    {
        public static void Main(string[] args)
        {
            VirtualMachine vm = new VirtualMachine();
            Buffer mem = new Buffer(512);
            vm.memory = mem;
            MemoryManager mm = new MemoryManager(vm, 0, 1, 30);
            vm.manager = mm;
            Class c = new Class() { name = "Test",  fields = new Field[] { new Field() { name = "A", type = MemoryManager.TYPE_4BYTE, newobj = 0 }, new Field() { name = "B", type = MemoryManager.TYPE_STRING, newobj = "HELLO WORLD!" } } , parent = vm };

            vm.classes = new Class[] { c };

            mm.StorePtr(0, mm.Alloc("TEST THIS"));
            mm.StorePtr(1, mm.Alloc(24));
            mm.StorePtr(2, mm.LoadPtr(1));

            Instance i = new Instance() { parent = c };

            mm.StorePtr(3, mm.Alloc(i));

            mem.Write(i.GetField(0).Item2 + 1, 12);

            uint adr = i.GetField(0).Item2;
            int a = mem.ReadInt(adr);

            Tuple<byte, uint> strfld = i.GetField(1);
            mm.Free(strfld.Item2,strfld.Item1);

            i.StoreField(1, mm.Alloc("Hello test"));

            Console.WriteLine(mem.ReadString(i.GetField(1).Item2 + 1));

            Console.ReadKey();
        }
    }
}

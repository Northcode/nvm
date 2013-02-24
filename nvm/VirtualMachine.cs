using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nvm.Objects;
using nvm.Codes;

namespace nvm
{
    public class VirtualMachine : IClassContainer, IVirtualMachine
    {
        Class[] classes;

        internal int height;
        internal int width;

        internal Buffer memory;
        internal MemoryManager Mmanager;
        internal Stack<Call> callstack;

        internal static OpCode[] opcodes;
        internal static Interupt[] interups;

        #region registers

        //8 Bit registers
        internal byte al;
        internal byte ah;
        internal byte bl;
        internal byte bh;
        internal byte cl;
        internal byte ch;
        internal byte dl;
        internal byte dh;

        //16 bit registers
        internal ushort ax;
        internal ushort bx;
        internal ushort cx;
        internal ushort dx;

        //32 bit registers
        internal int eax;
        internal int ebx;
        internal int ecx;
        internal int edx;

        //Memory pointer registers
        internal uint CS { get { return Mmanager.codeAddr; } }
        internal uint SS { get { return Mmanager.stackAddr; } }
        internal uint DS { get { return Mmanager.localAddr; } }
        internal uint HS { get { return Mmanager.heapAddr; } }
        internal uint IP;
        internal uint SP { get { return Mmanager.stackPointer; } }

        //FLAG Registers
        internal byte FJ;
        internal byte FE;
        internal bool RN;
        internal bool DBG;
        #endregion

        public VirtualMachine(Class[] classes, byte[] program, int stacksize, int staticsize, int RamSize)
        {
            this.classes = classes;
            this.callstack = new Stack<Call>();
            this.height = 30;
            this.width = 90;
            InitMemory(program, stacksize, staticsize, RamSize);
            InitOpCodes();
        }

        public static void InitOpCodes()
        {
            opcodes = new OpCode[] {
                new Codes.NOP(),
                new Codes.Registers.LD.LDAL(),
            };
        }

        private void InitMemory(byte[] program, int stacksize, int staticsize, int RamSize)
        {
            this.memory = new Buffer(RamSize);
            this.Mmanager = new MemoryManager(this, 0u, (uint)(program.Length + 4), (uint)(program.Length + 4 + stacksize + 4), (uint)(program.Length + 4 + stacksize + 4 + staticsize));
            uint i = 0;
            Array.ForEach<byte>(program, b =>
            {
                if (i < this.Mmanager.stackAddr - 4)
                {
                    this.memory.Write(i, b); i++;
                }
                else
                {
                    return;
                }
            });
        }

        public object GetRegister(byte reg)
        {
            switch (reg)
            {
                case 0x00: return al;
                case 0x01: return ah;
                case 0x02: return bl;
                case 0x03: return bh;
                case 0x04: return cl;
                case 0x05: return ch;
                case 0x06: return dl;
                case 0x07: return dh;

                case 0x08: return ax;
                case 0x09: return bx;
                case 0x0a: return cx;
                case 0x0b: return dx;

                case 0x0c: return eax;
                case 0x0d: return ebx;
                case 0x0e: return ecx;
                case 0x0f: return edx;
                default:
                    return 0;
            }
        }

        public void SetRegister(byte reg, object val)
        {
            switch (reg)
            {
                case 0x00: al = (byte)val; break;
                case 0x01: ah = (byte)val; break;
                case 0x02: bl = (byte)val; break;
                case 0x03: bh = (byte)val; break;
                case 0x04: cl = (byte)val; break;
                case 0x05: ch = (byte)val; break;
                case 0x06: dl = (byte)val; break;
                case 0x07: dh = (byte)val; break;

                case 0x08: ax = (ushort)val; break;
                case 0x09: bx = (ushort)val; break;
                case 0x0a: cx = (ushort)val; break;
                case 0x0b: dx = (ushort)val; break;

                case 0x0c: eax = (int)val; break;
                case 0x0d: ebx = (int)val; break;
                case 0x0e: ecx = (int)val; break;
                case 0x0f: edx = (int)val; break;
                default:
                    break;
            }
        }

        public bool DEBUG
        {
            get
            {
                return DBG;
            }
            set
            {
                DBG = value;
            }
        }

        public void Run()
        {
            RN = true;
            InitScreen();
            while (RN)
            {
                if (IP >= Mmanager.stackAddr) { FE = (byte)Error_Flags.CODE_ADDRESS_OVERFLOW; RN = false; DrawScreen(); }
                else
                {
                    DrawScreen();
                    if (DBG) { Console.ReadKey(); }
                    //Console.WriteLine("IP: " + IP);
                    byte ccode = memory.Read(IP);
                    IP++;
                    VirtualMachine.opcodes[ccode].Execute(this);
                }
            }
        }

        public Objects.Class GetClass(string Name)
        {
            return Array.Find<Class>(classes, p => p.name == Name);
        }

        public VirtualMachine GetMachine()
        {
            return this;
        }

        private void InitScreen()
        {
            Console.SetCursorPosition(0, 1);
            Console.BufferHeight = height;
            Console.BufferWidth = width;
            Console.WindowHeight = height;
            Console.WindowWidth = width;
            DrawScreen();
        }

        internal void DrawScreen()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine((" nvm running at address: " + IP + " | RUNNING: " + RN).PadRight(width));
            Console.SetCursorPosition(0, height - 10);
            RegDump();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(x, y);
            Console.SetWindowPosition(0, 0);
        }

        internal void RegDump()
        {
            Console.Write("----- REG DUMP -".PadRight(width, '-'));
            Console.Write((" AL: " + al.ToString().PadLeft(5) + " AH: " + ah.ToString().PadLeft(5) + " BL: " + bl.ToString().PadLeft(5) + " BH: " + bh.ToString().PadLeft(5) + " CL: " + cl.ToString().PadLeft(5) + " CH: " + ch.ToString() + " DL: " + dl.ToString().PadLeft(5) + " DH: " + dh.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" AX: " + ax.ToString().PadLeft(5) + " BX: " + bx.ToString().PadLeft(5) + " CX: " + cx.ToString().PadLeft(5) + " DX: " + dx.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" EAX: " + eax.ToString().PadLeft(8) + " EBX: " + ebx.ToString().PadLeft(8) + " ECX: " + ecx.ToString().PadLeft(8) + " EDX: " + edx.ToString().PadLeft(8)).PadRight(width));
            Console.Write((" -- SPECIAL REGISTERS: ").PadRight(width));
            Console.Write((" --> RN: " + RN.ToString().PadLeft(5) + "  | IP: " + IP.ToString().PadLeft(5) + (" : 0x" + (memory.data[IP]).ToString("X").PadLeft(2, '0')).PadRight(2) + ("(" + VirtualMachine.opcodes[memory.data[IP]].GetType().Name + ")").PadRight(10) + "  | SP: " + SP.ToString().PadLeft(5) + "  | FJ: " + FJ.ToString().PadLeft(5) + "  | FE: " + FE.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" -- MEMORY REGISTERS: ").PadRight(width));
            Console.Write((" --> CS: " + CS.ToString().PadLeft(5) + " | SS: " + SS.ToString().PadLeft(5) + "  | DS: " + DS.ToString().PadLeft(5) + "  | HS: " + HS.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" -- ERROR FLAG: " + FE.ToString("X").PadRight(2,'0') + " (" + Enum.GetValues(typeof(Error_Flags)).GetValue(FE).ToString() + ")").PadRight(width));
        }

        //-###########################################################################-

        public static byte FirstNibble(byte val)
        {
            return (byte)(val & 15);
        }

        public static byte LastNibble(byte val)
        {
            return (byte)((byte)(val >> 4) & 15);
        }
    }
}

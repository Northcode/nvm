using nvm.Codes;
using nvm.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace nvm
{
    public class VirtualMachine : IClassContainer
    {
        internal Buffer memory;
        internal MemoryManager manager;

        internal Stack<Call> callstack;
        internal Class[] classes;

        internal int height;
        internal int width;

        internal static OpCode[] codes;
        internal static Interupt[] interupts;

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
        internal uint CS { get { return manager.codeAddr; } }
        internal uint SS { get { return manager.stackAddr; } }
        internal uint DS { get { return manager.localAddr; } }
        internal uint HS { get { return manager.heapAddr; } }
        internal uint IP;
        internal uint SP { get { return manager.stackPointer; } }

        //FLAG Registers
        internal byte FJ;
        internal byte FE;
        internal bool RN;
        #endregion

        public VirtualMachine()
        {
            height = 26;
            width = 84;
            callstack = new Stack<Call>();
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

        public static byte FirstNibble(byte val)
        {
            return (byte)(val & 15);
        }

        public static byte LastNibble(byte val)
        {
            return (byte)((byte)(val >> 4) & 15);
        }

        public static void InitOpCodes()
        {
            VirtualMachine.codes = new OpCode[] { 
                new Codes.RegisterCodes.STAL(),             //0
                new Codes.RegisterCodes.STAH(),             //1
                new Codes.RegisterCodes.STBL(),             //2
                new Codes.RegisterCodes.STBH(),             //3
                new Codes.RegisterCodes.STCL(),             //4
                new Codes.RegisterCodes.STCH(),             //5
                new Codes.RegisterCodes.STDL(),             //6
                new Codes.RegisterCodes.STDH(),             //7

                new Codes.RegisterCodes._16_Bit.STAX(),     //8
                new Codes.RegisterCodes._16_Bit.STBX(),     //9
                new Codes.RegisterCodes._16_Bit.STCX(),     //a
                new Codes.RegisterCodes._16_Bit.STDX(),     //b

                new Codes.RegisterCodes._32_Bit.STEAX(),    //c
                new Codes.RegisterCodes._32_Bit.STEBX(),    //d
                new Codes.RegisterCodes._32_Bit.STECX(),    //e
                new Codes.RegisterCodes._32_Bit.STEDX(),    //f

                new Codes.Math.ADDBYTE(),                   //10
                new Codes.System.END(),                     //11
                new Codes.RegisterCodes.Mov.MOV(),          //12
                new Codes.RegisterCodes.Stack.PUSH(),       //13
                new Codes.RegisterCodes.Stack.POP(),        //14

		        new Codes.Math.ADDWORD(),                   //15
		        new Codes.Math.ADDU32(),                    //16
		        new Codes.Math.ADDDWORD(),                  //17

                new Codes.Math.SUBBYTE(),                   //18
                new Codes.Math.SUBWORD(),                   //19
                new Codes.Math.SUBDWORD(),                  //1a

                new Codes.Math.MULBYTE(),                   //1b
                new Codes.Math.MULWORD(),                   //1c
                new Codes.Math.MULDWORD(),                   //1d

                new Codes.Math.DIVBYTE(),                   //1e
                new Codes.Math.DIVWORD(),                   //1f
                new Codes.Math.DIVDWORD(),                  //20

                new Codes.System.INT(),                     //21

                new Codes.RegisterCodes.Stack.PUSHSTR(),    //22
                new Codes.RegisterCodes.Stack.POPSTR(),     //23

                new Codes.System.CALL(),                    //24
                new Codes.System.RET(),                     //25
                new Codes.System.JMP(),                     //26

                new Codes.System.STLOC(),                   //27
                new Codes.System.LDLOC(),                   //28
                new Codes.System.ALLOC(),                   //29
            };
        }

        public static void InitInterupt()
        {
            VirtualMachine.interupts = new Interupt[] {
                new Interupts.Interupt0(),
            };
        }

        public Class GetClass(string Name)
        {
            foreach (Class c in classes)
            {
                if (c.name == Name)
                {
                    return c;
                }
            }
            throw new Exception("Class '" + Name + "' not found in root");
        }

        public VirtualMachine GetMachine()
        {
            return this;
        }

        internal void Run(bool debug)
        {
            RN = true;
            InitScreen();
            while (RN)
            {
                DrawScreen();
                if (debug) { Console.ReadKey(); }
		    	//Console.WriteLine("IP: " + IP);
                byte ccode = memory.Read(IP);
                IP++;
                VirtualMachine.codes[ccode].Execute(this);
            }
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
			Console.SetCursorPosition(0,0);
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine((" nvm running at address: " + IP + " | RUNNING: " + RN).PadRight(width));
			Console.SetCursorPosition(0,height - 9);
			RegDump();
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.SetCursorPosition(x,y);
			Console.SetWindowPosition(0,0);
		}

        internal void RegDump()
        {
            Console.Write("----- REG DUMP -".PadRight(width, '-'));
            Console.Write((" AL: " + al.ToString().PadLeft(5) + " AH: " + ah.ToString().PadLeft(5) + " BL: " + bl.ToString().PadLeft(5) + " BH: " + bh.ToString().PadLeft(5) + " CL: " + cl.ToString().PadLeft(5) + " CH: " + ch.ToString() + " DL: " + dl.ToString().PadLeft(5) + " DH: " + dh.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" AX: " + ax.ToString().PadLeft(5) + " BX: " + bx.ToString().PadLeft(5) + " CX: " + cx.ToString().PadLeft(5) + " DX: " + dx.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" EAX: " + eax.ToString().PadLeft(8) + " EBX: " + ebx.ToString().PadLeft(8) + " ECX: " + ecx.ToString().PadLeft(8) + " EDX: " + edx.ToString().PadLeft(8)).PadRight(width));
            Console.Write((" -- SPECIAL REGISTERS: ").PadRight(width));
            Console.Write((" --> RN: " + RN.ToString().PadLeft(5) + "  | IP: " + IP.ToString().PadLeft(5) + (" : 0x" + (memory.data[IP]).ToString("X").PadLeft(2, '0')).PadRight(2) + ("(" + VirtualMachine.codes[memory.data[IP]].GetType().Name + ")").PadRight(10) + "  | SP: " + SP.ToString().PadLeft(5) + "  | FJ: " + FJ.ToString().PadLeft(5) + "  | FE: " + FE.ToString().PadLeft(5)).PadRight(width));
            Console.Write((" -- MEMORY REGISTERS: ").PadRight(width));
            Console.Write((" --> CS: " + CS.ToString().PadLeft(5) + " | SS: " + SS.ToString().PadLeft(5) + "  | DS: " + DS.ToString().PadLeft(5) + "  | HS: " + HS.ToString().PadLeft(5)).PadRight(width));
        }
    }
}

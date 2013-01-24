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
        internal Stack<object> stack;

        internal Stack<Call> callstack;
        internal Class[] classes;

        internal static OpCode[] codes;

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
        internal uint DS { get { return manager.staticAddr; } }
        internal uint HS { get { return manager.heapAddr; } }
        internal uint IP;
        internal uint SP { get { return manager.stackPointer; } }

        //FLAG Registers
        internal byte FJ;
        internal byte FE;
        internal bool RN;
        #endregion

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
                new Codes.RegisterCodes.STAL(), //0
                new Codes.RegisterCodes.STAH(), //1
                new Codes.RegisterCodes.STBL(), //2
                new Codes.RegisterCodes.STBH(), //3
                new Codes.RegisterCodes.STCL(), //4
                new Codes.RegisterCodes.STCH(), //5
                new Codes.RegisterCodes.STDL(), //6
                new Codes.RegisterCodes.STDH(), //7

                new Codes.RegisterCodes._16_Bit.STAX(), //8
                new Codes.RegisterCodes._16_Bit.STBX(), //9
                new Codes.RegisterCodes._16_Bit.STCX(), //a
                new Codes.RegisterCodes._16_Bit.STDX(), //b

                new Codes.RegisterCodes._32_Bit.STEAX(), //c
                new Codes.RegisterCodes._32_Bit.STEBX(), //d
                new Codes.RegisterCodes._32_Bit.STECX(), //e
                new Codes.RegisterCodes._32_Bit.STEDX(), //f

                new Codes.Math.ADDBYTE(), //10
                new Codes.System.END(), //11
                new Codes.RegisterCodes.Mov.MOV(), //12
                new Codes.RegisterCodes.Stack.PUSH(), //13
                new Codes.RegisterCodes.Stack.POP(), //14
		        new Codes.Math.ADDWORD(), //15
		        new Codes.Math.ADDU32(), //16
		        new Codes.Math.ADDDWORD(), //17
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
		    Console.SetCursorPosition(0,1);
		    DrawScreen();
            while (RN)
            {
                if (debug) { Console.ReadKey(); }
		    	//Console.WriteLine("IP: " + IP);
                byte ccode = memory.Read(IP);
                IP++;
                VirtualMachine.codes[ccode].Execute(this);
			    DrawScreen();
            }
        }

		internal void DrawScreen()
		{
			int x = Console.CursorLeft;
			int y = Console.CursorTop;
			Console.SetCursorPosition(0,0);
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine((" nvm running at address: " + IP + " | RUNNING: " + RN).PadRight(Console.WindowWidth - 1));
			Console.SetCursorPosition(0,16);
			RegDump();
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.SetCursorPosition(x,y);
			Console.SetWindowPosition(0,0);
		}

        internal void RegDump()
        {
            Console.WriteLine("#----------------------------------REGISTER DUMP------------------------------#".PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" AL:  " + al.ToString() + " | AH:  " + ah.ToString() + " | BL:  " + bl.ToString() + " | BH:  " + bh.ToString() + " | CL:  " + cl.ToString() + " | CH: " + ch.ToString() + " | DL:  " + dl.ToString() + " | DH:  " + dh.ToString()).PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" AX:  " + ax.ToString() + " | BX:  " + bx.ToString() + " | CX:  " + cx.ToString() + " | DX:  " + dx.ToString()).PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" EAX: " + eax.ToString() + " | EBX: " + ebx.ToString() + " | ECX: " + ecx.ToString() + " | EDX: " + edx.ToString()).PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" -- SPECIAL REGISTERS: ").PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" --> RN: " + RN.ToString() + "  | IP: " + IP.ToString() + "  | SP: " + SP.ToString() + "  | FJ: " + FJ.ToString() + "  | FE: " + FE.ToString()).PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" -- MEMORY REGISTERS: ").PadRight(Console.WindowWidth - 1));
            Console.WriteLine((" --> CS: " + CS.ToString() + " | SS: " + SS.ToString() + "  | DS: " + DS.ToString() + "  | HS: " + HS.ToString()).PadRight(Console.WindowWidth - 1));
            Console.WriteLine(("###-------------------------------------------------------------------------###").PadRight(Console.WindowWidth - 1));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nvm.v2.Debuging
{
    public partial class DebugWindow : Form
    {
        Debugger debugger;

        public DebugWindow(Debugger d)
        {
            debugger = d;
            InitializeComponent();
        }

        private void DebugWindow_Load(object sender, EventArgs e)
        {

        }

        public void Update()
        {
            varList.Items.Clear();
            for (int i = 0; i < debugger.process.locals.Length; i++)
            {
                uint u = debugger.process.locals[i];
                if (u != 0)
                {
                    byte t = debugger.process.Memory.Read(u);
                    if (t == ValueTypeCodes.BYTE)
                    {
                        byte b = debugger.process.Memory.Read(u + 1);
                        string varname = debugger.process.metadata.localData[i];
                        varList.Items.Add(varname + " (" + i + ") = " + b);
                    }
                    else if (t == ValueTypeCodes.INT)
                    {
                        int b = debugger.process.Memory.ReadInt(u + 1);
                        string varname = debugger.process.metadata.localData[i];
                        varList.Items.Add(varname + " (" + i + ") = " + b);
                    }
                    else if (t == ValueTypeCodes.UINT)
                    {
                        uint b = debugger.process.Memory.ReadUInt(u + 1);
                        string varname = debugger.process.metadata.localData[i];
                        varList.Items.Add(varname + " (" + i + ") = " + b);
                    }
                    else if (t == ValueTypeCodes.STRING)
                    {
                        string b = debugger.process.Memory.ReadString(u + 1);
                        string varname = debugger.process.metadata.localData[i];
                        varList.Items.Add(varname + " (" + i + ") = " + b);
                    }
                }
            }
            NumIp.Value = debugger.process.IP;
            byte opCV = debugger.process.Memory.Read(debugger.process.IP);
            string opCN = VM.opcodes[opCV].Name;
            txtopCode.Text = opCN + " (" + opCV + ")";
        }
    }
}

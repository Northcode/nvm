using nvm.v2.Fixes;
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
            fix.InvokeIfRequired(varList, () =>
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
            });
            fix.InvokeIfRequired(NumIp, () => NumIp.Value = debugger.process.IP);
            byte opCV = debugger.process.Memory.Read(debugger.process.IP);
            string opCN = VM.opcodes[opCV].Name;
            fix.InvokeIfRequired(txtopCode, () => { txtopCode.Text = opCN + " (" + opCV + ")"; });
            fix.InvokeIfRequired(textBox1, () => textBox1.Text = debugger.DisAssembler.ToString());
            fix.InvokeIfRequired(stackTop, () => { if (debugger.process.stack.Count > 0) { stackTop.Text = debugger.process.stack.Peek().ToString(); } });
            debugger.process.STEP = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            debugger.process.IP = (uint)NumIp.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            debugger.process.STEP = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}

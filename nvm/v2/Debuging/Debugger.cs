using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace nvm.v2.Debuging
{
    public class Debugger
    {
        public VM process { get; set; }
        DebugWindow wnd;
        public StringBuilder DisAssembler { get; internal set; }
        Thread th;

        public Debugger()
        {
            wnd = new DebugWindow(this);
            th = new Thread(thread);
            DisAssembler = new StringBuilder();
        }

        void thread()
        {
            Application.Run(wnd);
        }

        public void start()
        {
            th.Start();
        }

        public void Update()
        {
            wnd.Update();
        }
    }
}

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
        Thread th;

        public Debugger()
        {
            wnd = new DebugWindow(this);
            th = new Thread(thread);
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
            
        }
    }
}

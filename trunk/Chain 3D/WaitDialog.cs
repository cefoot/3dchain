using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Chain3D
{
    public partial class WaitDialog : Form
    {
        public delegate bool IsExcecutionReady();

        private IsExcecutionReady Ask;

        public delegate void CallWhenReady();

        private CallWhenReady Ready;

        public static void Show(IWin32Window owner, String title, IsExcecutionReady readyDel, CallWhenReady callback)
        {
            WaitDialog dlg = new WaitDialog();
            dlg.Text = title;
            dlg.Ask = readyDel;
            dlg.Ready = callback;
            dlg.ShowDialog(owner);
        }

        private WaitDialog()
        {
            InitializeComponent();
        }

        private void WaitDialog_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate()
            {
                while (true)
                {
                    Thread.Sleep(100);
                    bool ready = true;
                    if (Owner.InvokeRequired)
                    {
                        ready = (bool)Owner.Invoke(new IsExcecutionReady(Ask));
                    }
                    else
                    {
                        ready = Ask();
                    }
                    if (ready)
                    {

                        this.Invoke(new MethodInvoker(Close));
                        if (Owner.InvokeRequired)
                        {
                            Owner.Invoke(new MethodInvoker(Ready));
                        }
                        else
                        {
                            Ready();
                        }
                        return;
                    }
                }
            }));
            thread.Name = "WaitDialogThread";
            thread.Start();
        }
    }
}

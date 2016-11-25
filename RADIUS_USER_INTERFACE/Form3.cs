using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RADIUS_USER_INTERFACE
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public async void Progress(Object sender, System.EventArgs e)
        {
            
            if (labelX1.Text.Contains("Windows"))
            {
                await Task.Delay(4000);
            }
            else
            {
                progressBar1.Visible = true;
                await Task.Delay(8000);
            }
            Hide();
        }
        public string LabelText
        {
            get
            {
                return this.labelX1.Text;
            }
            set
            {
                this.labelX1.Text = value;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            this.ShowInTaskbar = false; 
            PlaceLowerRight();
            base.OnLoad(e);
        }

        private void PlaceLowerRight()
        {
            //Determine "rightmost" screen
            Screen rightmost = Screen.AllScreens[0];
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen;
            }
            this.Left = rightmost.WorkingArea.Right - this.Width;
            this.Top = rightmost.WorkingArea.Bottom - this.Height;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

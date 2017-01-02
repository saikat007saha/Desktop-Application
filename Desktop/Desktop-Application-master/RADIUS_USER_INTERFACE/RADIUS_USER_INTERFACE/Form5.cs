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
    public partial class Form5 : Form
    {
        public bool Okbuttonclicked;
        public String secretkey;
        public bool enterprise = false;
        
        public Form5()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            //this.ShowInTaskbar = false; 
            PlaceLowerRight();
            base.OnLoad(e);
        }
        public bool setmode
        {
            get
            {
                return this.enterprise;
            }
            set
            {
                this.enterprise = value;
            }
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
        

        public string LabelText
        {
            get
            {
                return this.label5.Text;
            }
            set
            {
                this.label5.Text = value;
            }
        }
        private void key_TextChanged(object sender, EventArgs e)
        {
            if (key.Text != "")
                OKbutton.Enabled = true;
            else
                OKbutton.Enabled = false;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                key.UseSystemPasswordChar = true;
            else
                key.UseSystemPasswordChar = false;
        }

        private void OKbutton_Click_1(object sender, EventArgs e)
        {
            PoziomLabs.Statistics st = new PoziomLabs.Statistics();
            PoziomLabs.CurrentWifi cw = new PoziomLabs.CurrentWifi(); 
            if (enterprise)
            {
                if (!st.CheckInternetConnection().Equals("Internet Access") || cw.display_current().Equals("Disconnected"))
                {
                    MessageBox.Show("Please check your internet connection"+Environment.NewLine+"Internet access is needed for verification purpose", "INTERNET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Okbuttonclicked = true;
                    secretkey = key.Text;
                    Hide();
                }
            }
            else
            {
                Okbuttonclicked = true;
                secretkey = key.Text;
                Hide();
            }
            //f.getkey = true;
        }

        private void CANCELbutton_Click(object sender, EventArgs e)
        {
            //Form1 f = new Form1(); 
            Okbuttonclicked = false;
            Hide();
        }
    }
}

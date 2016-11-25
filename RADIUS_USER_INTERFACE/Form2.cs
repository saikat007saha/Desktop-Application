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
    public partial class Form2 : Form
    {
        PoziomLabs.Statistics st = new PoziomLabs.Statistics();
        public Form2()
        {
                InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            //this.ShowInTaskbar = false; 
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
        private async void Form2_Load(object sender, EventArgs e)
        {
            //await Task.Delay(500);
            operation.Text = "Calculating download speed.Please wait.";
            circularProgress1.IsRunning = true;
            circularProgress1.Visible = true;
            await Task.Delay(1000);
            String ds = await CalculateSpeed('d');
            //await Task.Delay(1000);
            circularProgress1.Visible = false;
            dspeed.Text = ds;
            await Task.Delay(1000);
            operation.Text = "Calculating upload speed.Please wait.";
            circularProgress2.IsRunning = true;
            circularProgress2.Visible = true;
            await Task.Delay(1000);
            String us = await CalculateSpeed('u');
            //await Task.Delay(1000);
            circularProgress2.Visible = false;
            uspeed.Text = us;
            operation.Visible = false;
            labelX1.BringToFront();
            labelX1.Visible = true;
            await Task.Delay(3000);
            Hide();
        }
        public Task<String> CalculateSpeed(char c)
        {
            if (c == 'd')
                return Task.Run(() =>
                    {
                        return st.DownloadSpeed();
                    });
            else
                return Task.Run(() =>
                    {
                        return st.UploadSpeed();
                    });
        }
    }
}

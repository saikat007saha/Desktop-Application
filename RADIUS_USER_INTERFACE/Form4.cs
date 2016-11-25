using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoziomLabs;

namespace RADIUS_USER_INTERFACE
{
    public partial class Form4 : Form
    {
        private System.Timers.Timer timer;
        public string ssid1="";
        private int startPosX;
        private int startPosY;
        bool buttonclicked = false;
        //CurrentWifi cw = new CurrentWifi();       
        Netsh netsh = new Netsh();
        public Form4()
        {
            InitializeComponent();
            //Hide();
            TopMost = true;           
            // Pop doesn't need to be shown in task bar
            ShowInTaskbar = false;
            // Create and run timer for animation
            timer = new System.Timers.Timer();
            timer.Interval = 50;
            timer.Elapsed += timer_Tick;
        }
        public void timerstart()
        {
            reconnectmsg.Text = "Reconnect to \"" + ssid1 + "\"" + Environment.NewLine + " to get better " + Environment.NewLine + "Signal Strength"; 
            timer.Start();
        }
        protected override void OnLoad(EventArgs e)
        {
            // Move window out of screen
            startPosX = Screen.PrimaryScreen.WorkingArea.Width - Width;
            startPosY = Screen.PrimaryScreen.WorkingArea.Height;
            SetDesktopLocation(startPosX, startPosY);
            base.OnLoad(e);
            // Begin animation            
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            //Lift window by 8 pixels
            startPosY -= 8; 
            //If window is fully visible stop the timer
            if (startPosY < Screen.PrimaryScreen.WorkingArea.Height - Height)
            {
                timer.Stop();
                reconnectbutton.Enabled = true;
                await Task.Delay(8000);
                if(!buttonclicked)
                    Hide();
            }
            else
                SetDesktopLocation(startPosX, startPosY);
        }
        private async void reconnectbutton_Click(object sender, EventArgs e)
        {
            reconnectbutton.Enabled = false;
            buttonclicked = true;
            //String ssid = cw.display_current();
            circularProgress1.IsRunning = true;
            await Task.Delay(1000);
            reconnectmsg.Visible = false;
            reconnectbutton.Visible = false;
            reconnecting.Text += Environment.NewLine + ssid1 + " ....";
            await Task.Delay(500);
            reconnecting.Visible = true;
            circularProgress1.Visible = true;
            await Task.Delay(1000);
            String res = netsh.Connect(ssid1);
            await Task.Delay(8000);
            Hide();
        }
    }
}

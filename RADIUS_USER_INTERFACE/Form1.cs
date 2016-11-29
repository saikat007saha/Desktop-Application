using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PoziomLabs;
using System.Security.Principal;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using IWshRuntimeLibrary;
namespace RADIUS_USER_INTERFACE
{
    public partial class Form1 : Form
    {
        private ToolTip tt = new ToolTip();
        Netsh netsh = new Netsh();
        PostRequest req = new PostRequest();
        Statistics st = new Statistics();
        Firewall firewall = new Firewall();
        Cert cert = new Cert();
        XmlProfile xml = new XmlProfile();
        Account acc = new Account();
        Net_Stats ns = new Net_Stats();
        string msg = "Shared WIFI", security_key = "";
        delegate void List_of_wifi_thread(object sender, EventArgs e);
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        bool flag5 = false;
        bool flag7 = false;
        int length = 0, users, j1 = 1;

        String host, Multiple_ssid, Multiple_bssid;
        Thread th2, th3;

        public Form1()
        {
            InitializeComponent();
            th3 = new Thread(MultipleAccessPointSignal);
            StreamWriter w = System.IO.File.AppendText(Application.StartupPath + @"\test.txt");
            w.Close();
            try
            {
                if (IsRunningAsAdministrator() && !System.IO.File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\plugged.lnk"))
                    CreateShortcut("plugged", @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\", Application.ExecutablePath);
            }
            catch (Exception exc) { }

            if (!System.IO.File.Exists(Application.StartupPath + @"\test.txt"))
            {
                // Setting up start info of the new process of the same application
                ProcessStartInfo processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase);

                // Using operating shell and setting the ProcessStartInfo.Verb to “runas” will let it run as admin
                processStartInfo.UseShellExecute = true;
                processStartInfo.Verb = "runas";

                // Start the application as new process
                Process.Start(processStartInfo);
                StreamWriter w11 = System.IO.File.AppendText(Application.StartupPath + @"\test.txt");
                w11.Close();
                if (ConnectWIFI != null)
                {
                    ConnectWIFI.Visible = false;
                    ConnectWIFI.Icon = null; // required to make icon disappear
                    ConnectWIFI.Dispose();
                    ConnectWIFI = null;
                }
                //Form.ActiveForm.Close();
                Application.Exit();
                // Shut down the current (old) process               
            }

            System.Net.WebClient wc = new System.Net.WebClient();
            ContextMenu c = new ContextMenu(new MenuItem[] { new MenuItem("Exit", Exit) });
            ConnectWIFI.ContextMenu = c;
        }

        public Task CallAccRead(Label l1)
        {
            return Task.Run(() =>
            {
                th2 = new Thread(acc.AccRead);
                th2.Start();
                Thread.Sleep(8000);
                l1.Text = "Data Used : " + Account.res;
                return;
            });
        }

        public async void CallAccReadTask(Label l1)
        {
            await CallAccRead(l1);
        }

        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.Description = "Plugged";   // The description of the shortcut
            shortcut.IconLocation = @"c:\myicon.ico";           // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();
        }

        public static bool IsRunningAsAdministrator()
        {
            // Get current Windows user
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

            // Get current Windows user principal
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);

            // Return TRUE if user is in role "Administrator"
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void Exit(object sender, EventArgs e)
        {
            try
            {
                if (ConnectWIFI != null)
                {
                    ConnectWIFI.Visible = false;
                    ConnectWIFI.Icon = null; // required to make icon disappear
                    ConnectWIFI.Dispose();
                    ConnectWIFI = null;
                }
            }
            catch (Exception ex)
            {
                // handle the error
            }
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectWIFI.ShowBalloonTip(5);
            button1_Click(sender, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
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

        private async void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            String id = wifistatus();
            if (id.Equals("Disabled"))
            {
                //wifilist.Visible = false;
                labelX1.Visible = true;
                enable.Visible = true;
            }
            else
            {
                circularProgress1.IsRunning = true;
                circularProgress1.Visible = true;
                await Task.Delay(3000);
                this.Invoke(new List_of_wifi_thread(this.List_of_wifi), sender, e);
            }
        }

        private async void List_of_wifi(object sender, EventArgs e)
        {
            Label l1 = new Label();
            Label l2 = new Label();
            Label l = new Label();
            PictureBox picbox;
            Button dis = new Button();
            flag4 = false;
            panel3.Controls.Clear();
            String id = wifistatus();
            int xPos = 4;   //coordinates of wifi buttons
            int yPos = 4;
            WifiInfo wifi = new WifiInfo();
            string[] profiles = wifi.ssidList();
            int[] signal = wifi.signalStrengths();
            string[] mode = wifi.authTypes();
            string[] bssid = wifi.bssidList();
            length = profiles.Length;

            //sort

            int temp = 0;
            string temp1 = "";

            for (int write = 0; write < length; write++)
            {
                for (int sort = 0; sort < length - 1; sort++)
                {
                    if (signal[sort] < signal[sort + 1])
                    {
                        temp = signal[sort + 1];
                        signal[sort + 1] = signal[sort];
                        signal[sort] = temp;

                        temp1 = profiles[sort + 1];
                        profiles[sort + 1] = profiles[sort];
                        profiles[sort] = temp1;

                        temp1 = mode[sort + 1];
                        mode[sort + 1] = mode[sort];
                        mode[sort] = temp1;

                        temp1 = bssid[sort + 1];
                        bssid[sort + 1] = bssid[sort];
                        bssid[sort] = temp1;
                    }
                }
            }

            Button[] btnArray = new Button[length];
            int dist = 0;
            int xpic = 333;
            int ypic = 19;
            int xlabel = 237;
            int ylabel = 13;
            int xlabel1 = 9;
            int ylabel1 = 50;
            int xlabel2 = 9;
            int ylabel2 = 35;
            int xdisc = 242;
            int ydisc = 43;
            for (int i = 0; i < length; i++)
            {
                int h1 = 5;
                int w1 = 3;
                int x = 320;  //coordinates of wifi signal
                int y = 23 + dist;
                btnArray[i] = new System.Windows.Forms.Button();
                Button[] r = new Button[5];
                for (int j = 0; j < 5; j++)
                {
                    r[j] = new Button();
                    r[j].Height = h1;
                    r[j].Width = w1;
                    r[j].Top = y;
                    r[j].Left = x;
                    r[j].FlatStyle = FlatStyle.Flat;
                    r[j].BackColor = Color.Silver;
                    r[j].FlatAppearance.BorderSize = 0;
                    btnArray[i].FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
                    r[j].Visible = true;
                    h1 += 4;
                    x += 4;
                    y -= 4;
                    wifilist.Controls.Add(r[j]);
                }

                if (signal[i] >= 0 && signal[i] <= 20)
                {
                    r[0].BackColor = Color.MediumSeaGreen;
                }
                else if (signal[i] > 20 && signal[i] <= 40)
                {
                    r[0].BackColor = Color.MediumSeaGreen;
                    r[1].BackColor = Color.MediumSeaGreen;
                }
                else if (signal[i] > 40 && signal[i] <= 60)
                {
                    r[0].BackColor = Color.MediumSeaGreen;
                    r[1].BackColor = Color.MediumSeaGreen;
                    r[2].BackColor = Color.MediumSeaGreen;
                }
                else if (signal[i] > 60 && signal[i] <= 80)
                {
                    r[0].BackColor = Color.MediumSeaGreen;
                    r[1].BackColor = Color.MediumSeaGreen;
                    r[2].BackColor = Color.MediumSeaGreen;
                    r[3].BackColor = Color.MediumSeaGreen;
                }
                else
                {
                    r[0].BackColor = Color.MediumSeaGreen;
                    r[1].BackColor = Color.MediumSeaGreen;
                    r[2].BackColor = Color.MediumSeaGreen;
                    r[3].BackColor = Color.MediumSeaGreen;
                    r[4].BackColor = Color.MediumSeaGreen;
                }
                if (bssid[i].Equals("Multiple"))
                {
                    picbox = new PictureBox();
                    picbox.BackColor = Color.Transparent;
                    //picbox.ImageLocation = @"C:\Users\Harsh\Documents\Visual Studio 2012\Projects\RADIUS_USER_INTERFACE\RADIUS_USER_INTERFACE\Resources\share2.jpg";
                    picbox.Image = Properties.Resources.sharegreen;
                    picbox.Location = new Point(xpic, ypic);
                    picbox.Size = new Size(10, 10);
                    picbox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //picbox.MouseHover += new EventHandler(shared_tooltip_show);
                    //picbox.MouseLeave += new EventHandler(shared_tooltip_hide);
                    picbox.Visible = true;
                    picbox.BringToFront();
                    panel3.Controls.Add(picbox);
                    if (id.Equals(profiles[i]))
                    {
                        Multiple_ssid = profiles[i];
                        Multiple_bssid = WifiInfo.multiplebssid;
                        //MultipleAccessPoint();
                    }
                }
                btnArray[i].Width = 350; // size of wifi button                
                btnArray[i].Left = xPos;
                btnArray[i].Top = yPos;
                btnArray[i].Text = profiles[i];
                btnArray[i].ForeColor = Color.RoyalBlue;
                btnArray[i].TabStop = false;
                btnArray[i].BackColor = Color.White;
                btnArray[i].FlatStyle = FlatStyle.Flat;
                btnArray[i].FlatAppearance.BorderSize = 0;
                btnArray[i].FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
                if (id.Equals(profiles[i]))
                {
                    btnArray[i].Font = new Font("Cambria", 12, FontStyle.Bold);
                    btnArray[i].Height = 72;
                    btnArray[i].TextAlign = ContentAlignment.TopLeft;
                    yPos += 72;
                    dist += 72; //used for wifi signal
                    ypic += 72;
                    btnArray[i].MouseEnter += (sender1, e1) => { ButtonMouseEnter(sender1, e1, l, l1, l2, dis); };
                    btnArray[i].MouseLeave += (sender1, e1) => { ButtonMouseLeave(sender1, e1, l, l1, l2, dis); };

                    l.Width = 75;
                    l.Height = 15;
                    l.Top = ylabel;
                    l.Left = xlabel;
                    l.BackColor = Color.Transparent;
                    l.ForeColor = Color.DarkBlue;
                    l.Text = "Connected";
                    l.Font = new Font("Cambria", 10, FontStyle.Bold | FontStyle.Italic);
                    panel3.Controls.Add(l);

                    l1.Width = 185;
                    l1.Height = 15;
                    l1.Top = ylabel1;
                    l1.Left = xlabel1;
                    l1.BackColor = Color.Transparent;
                    l1.ForeColor = Color.DarkBlue;
                    l1.Text = "Users Connected : Calculating.....";
                    l1.Font = new Font("Cambria", 9, FontStyle.Italic);
                    panel3.Controls.Add(l1);
                    noofusers(l1);

                    l2.Width = 185;
                    l2.Height = 15;
                    l2.Top = ylabel2;
                    l2.Left = xlabel2;
                    l2.BackColor = Color.Transparent;
                    l2.ForeColor = Color.DarkBlue;
                    l2.Text = "Data Used : Calculating.......";
                    l2.Font = new Font("Cambria", 9, FontStyle.Italic);
                    ToolTip tt1 = new ToolTip();
                    l2.MouseEnter += (sender1, e1) => { DataMouseEnter(sender1, e1, l2, tt1); };
                    l2.MouseLeave += (sender1, e1) => { DataMouseLeave(sender1, e1, l2, tt1); };
                    //l2.Visible = false;
                    panel3.Controls.Add(l2);
                    CallAccReadTask(l2);

                    dis.Height = 26;
                    dis.Width = 98;
                    dis.Top = ydisc;
                    dis.Left = xdisc;
                    dis.FlatStyle = FlatStyle.Standard;
                    //dis.BackColor = Color.LightSteelBlue;
                    dis.FlatAppearance.BorderSize = 1;
                    dis.Text = "Disconnect";
                    dis.Font = new Font("Cambria", 10, FontStyle.Bold);
                    dis.Click += new System.EventHandler(WifiDisconnect);
                    panel3.Controls.Add(dis);
                }
                else
                {
                    btnArray[i].Font = new Font("Cambria", 12);
                    //btnArray[i].ForeColor = Color.MediumSeaGreen;
                    btnArray[i].Height = 30;
                    btnArray[i].TextAlign = ContentAlignment.MiddleLeft;
                    yPos += 30;
                    dist += 30; //used for wifi signal
                    ypic += 30;
                    if (mode[i].Equals("WPA2-Enterprise"))
                    {
                        btnArray[i].Click += new System.EventHandler(WifiConnect_enterprise);
                    }
                    else
                    {
                        btnArray[i].Click += new System.EventHandler(WifiConnect);
                    }
                }
                ydisc += 30;
                ylabel += 30;
                ylabel1 += 30;
                ylabel2 += 30;

                panel3.Controls.Add(btnArray[i]);

                for (int k = 0; k < 5; k++)
                {
                    panel3.Controls.Add(r[k]);
                }
                btnArray[i].SendToBack();
            }
            if (!id.Equals("Disconnected"))
            {
                wifiname.Text = "Currently Connected To : ";
                ssid.Text = id;
                internetconnection.Text = "( " + st.CheckInternetConnection() + " )";
                if (internetconnection.Text.Equals("( Internet Access )") == true)
                {
                    String quality = st.PingTimeAverage("www.stackoverflow.com", 4);
                    linequality.Text = quality;
                }
                else
                    linequality.Text = "Poor Speed";
                ssid.BringToFront();
                internetconnection.BringToFront();
                linequality.BringToFront();
            }
            else
            {
                wifiname.Text = "Not Connected";
                labelX4.BringToFront();
            }
            await Task.Delay(3000);
            circularProgress1.IsRunning = false;
            circularProgress1.Visible = false;
            circularProgress2.IsRunning = false;
            circularProgress2.Visible = false;
            panel2.Visible = true;
            if (!id.Equals("Disconnected"))
            {
                labelX4.Visible = false;
                ssid.Visible = true;
                internetconnection.Visible = true;
                linequality.Visible = true;
                if (internetconnection.Text.Equals("( Internet Access )") == true)
                    CheckSpeed.Enabled = true;
                else
                    CheckSpeed.Enabled = false;
                CheckSpeed.Visible = true;
                //cross.Visible = false;

                var t = new System.Threading.Thread(MyThreadStartMethod);
                t.SetApartmentState(System.Threading.ApartmentState.STA);
                t.Start();

                netunavailable.Visible = false;
                netavailable.BringToFront();
                netavailable.Visible = true;
            }
            else
            {
                ssid.Visible = false;
                internetconnection.Visible = false;
                linequality.Visible = false;
                CheckSpeed.Visible = false;
                labelX4.Visible = true;
                netavailable.Visible = false;
                netunavailable.BringToFront();
                netunavailable.Visible = true;
            }

            wifiname.Visible = true;
            refresh.Visible = true;
            wifilist.Visible = true;
            panel3.Visible = true;
        }

        public void MyThreadStartMethod()
        {
            string isp = "";
            string url = "https://www.whoismyisp.org/";
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            foreach (HtmlAgilityPack.HtmlNode nodeisp in doc.DocumentNode.SelectNodes("/html/body/div[2]/div/h1"))
            { 
                isp = nodeisp.InnerHtml;
            }
            try
            {
                url = "http://www.airtel.in/applications/airteldongle.jsp";
                doc = web.Load(url);
                foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("/html[1]/body[1]/iframe[1]/@src"))
                {
                    HtmlAgilityPack.HtmlAttribute attr = node.Attributes["src"];
                    System.Windows.Forms.WebBrowser webBrowser1 = new System.Windows.Forms.WebBrowser();
                    webBrowser1.Navigate(attr.Value);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    while (webBrowser1.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                        System.Windows.Forms.Application.DoEvents();
                    System.Windows.Forms.HtmlDocument doc1 = webBrowser1.Document;
                    int count = 0;
                    foreach (System.Windows.Forms.HtmlElement elem1 in doc1.GetElementsByTagName("script"))
                    {
                        count++;
                        if (count == 9)
                        {
                            string html = elem1.InnerHtml;
                            html = html.Substring(html.IndexOf("Utilized"), html.IndexOf("])") - html.IndexOf("Utilized"));
                            Console.WriteLine(html);
                            string[] data = html.Split(',');
                            CheckSpeed.Visible = false;
                            FUP.Visible = true;
                            FUP.Maximum = 100;
                            FUP.Step = 1;
                            FUP.Value = (Int32)(100 / (1 + (float.Parse(data[3].Substring(3, data[3].Length - 4)) / float.Parse(data[1].Substring(3, data[1].Length - 4)))));
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                
            }

        }

        public async void noofusers(Label l1)
        {
            await account(l1);
        }

        public Task account(Label l1)
        {
            return Task.Run(() =>
            {
                Thread worker;
                String host1 = st.GetHost1();
                for (j1 = 1; j1 <= 255; j1++)
                {
                    host = host1 + j1;
                    worker = new Thread(StartAcc);
                    worker.Start();
                }
                Thread.Sleep(4000);
                users = st.GetHost2();
                l1.Text = "Users Connected : " + users;
                return;
            });
        }

        public void StartAcc()
        {
            acc.PingHost(host);
        }

        public async void MultipleAccessPoint()
        {
            await MultipleAccessPointThread();
        }

        public Task MultipleAccessPointThread()
        {
            return Task.Run(() =>
            {
                return Task.Run(() =>
                {
                    if (th3 != null)
                        th3.Abort();
                    th3 = new Thread(MultipleAccessPointSignal);
                    th3.Start();
                    return;
                });
            });
        }

        public async void MultipleAccessPointSignal()
        {
            bool signal = false;
            while (true)
            {
                signal = acc.MultipleAccessPoint(Multiple_ssid, Multiple_bssid);
                //MessageBox.Show("Reconnect");
                if (!signal)
                {
                    Form4 f4 = new Form4();
                    f4.ssid1 = Multiple_ssid;
                    ConnectWIFI.Visible = false;
                    f4.timerstart();
                    Hide();
                    f4.ShowDialog();
                    await Task.Delay(1000);
                    ConnectWIFI.Visible = true;
                    Show();
                    this.Activate();
                }
                Thread.Sleep(300000);
            }
        }

        private void DataMouseEnter(Object sender, EventArgs e, Label l2, ToolTip tt1)
        {
            tt1.InitialDelay = 0;
            Label p = (Label)sender;
            if (!flag7)
            {
                tt1.Show("Data consumed in this session", p, 0);
                flag4 = true;
            }
        }

        private void DataMouseLeave(Object sender, EventArgs e, Label l2, ToolTip tt1)
        {
            Label p = (Label)sender;
            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
            {
                tt1.Hide(p);
                flag7 = false;
            }
        }

        private void ButtonMouseEnter(Object sender, EventArgs e, Label l, Label l1, Label l2, Button dis)
        {
            Button p = (Button)sender;
            //p.BackColor = Color.Lavender;
            l.BackColor = Color.Lavender;
            l1.BackColor = Color.Lavender;
            l2.BackColor = Color.Lavender;
        }

        private void ButtonMouseLeave(Object sender, EventArgs e, Label l, Label l1, Label l2, Button dis)
        {
            Button p = (Button)sender;
            //p.BackColor = Color.Transparent;
            l.BackColor = Color.Transparent;
            l1.BackColor = Color.Transparent;
            l2.BackColor = Color.Transparent;
        }

        public void WifiConnect_enterprise(Object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            Form3 f1 = new Form3();
            f1.LabelText = "Windows was unable to connect to  " + btn.Text;
            ConnectWIFI.Visible = false;
            f1.ShowDialog();
            ConnectWIFI.Visible = true;
            Show();
            this.Activate();
        }

        public async void WifiConnect(Object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            if (netsh.CheckProfile(btn.Text) == "not present")
            {
                Form5 f2 = new Form5();
                Hide();
                await Task.Delay(2000);
                ConnectWIFI.Visible = false;
                f2.ShowDialog();
                await Task.Delay(1000);
                ConnectWIFI.Visible = true;
                if (f2.Okbuttonclicked == true)
                {
                    security_key = f2.secretkey;
                    xml.GenerateWPA2psk(btn.Text, security_key, Application.StartupPath + @"\" + btn.Text + ".xml");
                    netsh.ProfileAdd(Application.StartupPath + @"\" + btn.Text + ".xml");
                    Form3 f = new Form3();
                    f.LabelText = "Connecting to " + btn.Text + " .........";
                    ConnectWIFI.Visible = false;
                    Hide();
                    f.Show();
                    await Task.Delay(2000);
                    String res = netsh.Connect(btn.Text);
                    if (!res.Contains("successfully"))
                    {
                        f.Hide();
                        Form3 f1 = new Form3();
                        f1.LabelText = "Windows was unable to connect to  " + btn.Text;
                        ConnectWIFI.Visible = false;
                        f1.ShowDialog();
                        ConnectWIFI.Visible = true;
                    }
                    else
                    {
                        panel3.Visible = false;
                        circularProgress2.Visible = true;
                        circularProgress2.IsRunning = true;
                        List_of_wifi(sender, e);
                    }
                    await Task.Delay(5000);
                    ConnectWIFI.Visible = true;
                    Show();
                    this.Activate();
                }
                else
                {
                    ConnectWIFI.Visible = true;
                    Show();
                    this.Activate();
                }
            }
            else
            {
                Form3 f = new Form3();
                f.LabelText = "Connecting to " + btn.Text + " .........";
                ConnectWIFI.Visible = false;
                Hide();
                f.Show();
                await Task.Delay(2000);
                String res = netsh.Connect(btn.Text);
                if (!res.Contains("successfully"))
                {
                    f.Hide();
                    Form3 f1 = new Form3();
                    f1.LabelText = "Windows was unable to connect to  " + btn.Text;
                    ConnectWIFI.Visible = false;
                    f1.ShowDialog();
                    ConnectWIFI.Visible = true;
                }
                else
                {
                    panel3.Visible = false;
                    circularProgress2.Visible = true;
                    circularProgress2.IsRunning = true;
                    await Task.Delay(3000);
                    List_of_wifi(sender, e);
                }
                await Task.Delay(5000);
                ConnectWIFI.Visible = true;
                Show();
                this.Activate();
            }
        }

        public async void WifiDisconnect(Object sender, System.EventArgs e)
        {
            Hide();
            Form3 f = new Form3();
            f.LabelText = "Disconnecting from Wifi";
            ConnectWIFI.Visible = false;
            f.Show();
            await Task.Delay(2000);
            this.Invoke(new List_of_wifi_thread(this.func1), sender, e);
            await Task.Delay(5000);
            ConnectWIFI.Visible = true;
            Show();
            this.Activate();
        }

        public void func1(Object sender, System.EventArgs e)
        {
            netsh.Disconnect();
            panel3.Visible = false;
            List_of_wifi(sender, e);
        }

        public string wifistatus()
        {
            CurrentWifi cw = new CurrentWifi();
            return (cw.display_current());
        }

        private void enable_Click(object sender, EventArgs e)
        {
            labelX1.Visible = false;
            enable.Visible = false;
            circularProgress1.Visible = true;
            circularProgress1.IsRunning = true;
            List_of_wifi(sender, e);
        }

        private async void refresh_Click(object sender, EventArgs e)
        {
            //panel2.Visible = false;
            //msg = null;
            panel3.Visible = false;
            circularProgress2.IsRunning = true;
            circularProgress2.Visible = true;
            await Task.Delay(3000);
            List_of_wifi(sender, e);
        }

        private void ConnectWIFI_Click(object sender, EventArgs e)
        {
            if (flag3 == false)
            {
                Show();

                //refresh_Click(sender, e);
                flag3 = true;
            }
            else
            {
                Hide();
                flag3 = false;
            }
        }

        private async void CheckSpeed_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            Hide();
            ConnectWIFI.Visible = false;
            await Task.Delay(1000);
            f.ShowDialog();
            ConnectWIFI.Visible = true;
            Show();
            this.Activate();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            Hide();
            flag3 = false;
        }

        private void refresh_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Refresh", (Button)sender);
        }
    }
}
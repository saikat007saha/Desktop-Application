namespace RADIUS_USER_INTERFACE
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.reconnectbutton = new System.Windows.Forms.Button();
            this.reconnectmsg = new DevComponents.DotNetBar.LabelX();
            this.circularProgress1 = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.reconnecting = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // reconnectbutton
            // 
            this.reconnectbutton.BackColor = System.Drawing.Color.Transparent;
            this.reconnectbutton.Enabled = false;
            this.reconnectbutton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.reconnectbutton.FlatAppearance.BorderSize = 4;
            this.reconnectbutton.Font = new System.Drawing.Font("Cambria", 12.25F, System.Drawing.FontStyle.Bold);
            this.reconnectbutton.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.reconnectbutton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.reconnectbutton.Location = new System.Drawing.Point(54, 85);
            this.reconnectbutton.Name = "reconnectbutton";
            this.reconnectbutton.Size = new System.Drawing.Size(120, 30);
            this.reconnectbutton.TabIndex = 46;
            this.reconnectbutton.Text = "RECONNECT";
            this.reconnectbutton.UseVisualStyleBackColor = false;
            this.reconnectbutton.Click += new System.EventHandler(this.reconnectbutton_Click);
            // 
            // reconnectmsg
            // 
            // 
            // 
            // 
            this.reconnectmsg.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.reconnectmsg.Font = new System.Drawing.Font("Cambria", 11.5F, System.Drawing.FontStyle.Bold);
            this.reconnectmsg.Location = new System.Drawing.Point(12, 11);
            this.reconnectmsg.Name = "reconnectmsg";
            this.reconnectmsg.Size = new System.Drawing.Size(228, 56);
            this.reconnectmsg.TabIndex = 45;
            this.reconnectmsg.Text = "Reconnect to get better\r\n  SIGNAL STRENGTH....";
            this.reconnectmsg.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // circularProgress1
            // 
            this.circularProgress1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.circularProgress1.BackgroundStyle.BackColorGradientType = DevComponents.DotNetBar.eGradientType.Radial;
            this.circularProgress1.BackgroundStyle.BorderBottomWidth = 1;
            this.circularProgress1.BackgroundStyle.BorderColor = System.Drawing.SystemColors.ScrollBar;
            this.circularProgress1.BackgroundStyle.BorderLeftWidth = 1;
            this.circularProgress1.BackgroundStyle.BorderRightWidth = 1;
            this.circularProgress1.BackgroundStyle.BorderTopWidth = 1;
            this.circularProgress1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.circularProgress1.Location = new System.Drawing.Point(84, 87);
            this.circularProgress1.Name = "circularProgress1";
            this.circularProgress1.ProgressBarType = DevComponents.DotNetBar.eCircularProgressType.Dot;
            this.circularProgress1.ProgressColor = System.Drawing.Color.Navy;
            this.circularProgress1.Size = new System.Drawing.Size(58, 36);
            this.circularProgress1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.circularProgress1.TabIndex = 47;
            this.circularProgress1.TabStop = false;
            this.circularProgress1.Tag = "0";
            this.circularProgress1.Visible = false;
            // 
            // reconnecting
            // 
            // 
            // 
            // 
            this.reconnecting.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.reconnecting.Font = new System.Drawing.Font("Cambria", 14F, System.Drawing.FontStyle.Bold);
            this.reconnecting.Location = new System.Drawing.Point(35, 1);
            this.reconnecting.Name = "reconnecting";
            this.reconnecting.Size = new System.Drawing.Size(153, 80);
            this.reconnecting.TabIndex = 48;
            this.reconnecting.Text = "Reconnecting to ";
            this.reconnecting.TextAlignment = System.Drawing.StringAlignment.Center;
            this.reconnecting.Visible = false;
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(234, 140);
            this.ControlBox = false;
            this.Controls.Add(this.reconnecting);
            this.Controls.Add(this.circularProgress1);
            this.Controls.Add(this.reconnectbutton);
            this.Controls.Add(this.reconnectmsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form4";
            this.Text = "ReConnect";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button reconnectbutton;
        private DevComponents.DotNetBar.LabelX reconnectmsg;
        private DevComponents.DotNetBar.Controls.CircularProgress circularProgress1;
        private DevComponents.DotNetBar.LabelX reconnecting;



    }
}
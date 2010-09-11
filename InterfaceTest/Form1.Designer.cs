namespace InterfaceTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.hardwareButton1 = new Microsoft.WindowsCE.Forms.HardwareButton();
            this.messageQueue1 = new System.Messaging.MessageQueue();
            this.tabStrip1 = new Tenor.Mobile.UI.TabStrip();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.kListControl1 = new Tenor.Mobile.UI.KListControl();
            this.headerStrip2 = new Tenor.Mobile.UI.HeaderStrip();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.headerStrip1 = new Tenor.Mobile.UI.HeaderStrip();
            this.textBox2 = new Tenor.Mobile.UI.TextControl();
            this.textBox1 = new Tenor.Mobile.UI.TextControl();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.notificationWithSoftKeys1 = new Tenor.Mobile.UI.NotificationWithSoftKeys();
            this.tabStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem7);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.menuItem5);
            this.menuItem1.MenuItems.Add(this.menuItem2);
            this.menuItem1.MenuItems.Add(this.menuItem3);
            this.menuItem1.MenuItems.Add(this.menuItem4);
            this.menuItem1.MenuItems.Add(this.menuItem6);
            this.menuItem1.Text = "Itens";
            // 
            // menuItem5
            // 
            this.menuItem5.Text = "5";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "10";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "20";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Text = "30";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Text = "List Files";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Text = "Add";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // messageQueue1
            // 
            this.messageQueue1.Formatter = new System.Messaging.XmlMessageFormatter(new string[0]);
            // 
            // tabStrip1
            // 
            this.tabStrip1.Controls.Add(this.tabPage1);
            this.tabStrip1.Controls.Add(this.tabPage2);
            this.tabStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabStrip1.Location = new System.Drawing.Point(0, 0);
            this.tabStrip1.Name = "tabStrip1";
            this.tabStrip1.SelectedIndex = 0;
            this.tabStrip1.Size = new System.Drawing.Size(240, 268);
            this.tabStrip1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.kListControl1);
            this.tabPage1.Controls.Add(this.headerStrip2);
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 245);
            this.tabPage1.Text = "tabPage1";
            // 
            // kListControl1
            // 
            this.kListControl1.DefaultItemHeight = 38;
            this.kListControl1.DefaultItemWidth = 80;
            this.kListControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            this.kListControl1.Location = new System.Drawing.Point(0, 54);
            this.kListControl1.Name = "kListControl1";
            this.kListControl1.SeparatorColor = System.Drawing.SystemColors.InactiveBorder;
            this.kListControl1.Size = new System.Drawing.Size(240, 191);
            this.kListControl1.TabIndex = 1;
            this.kListControl1.Resize += new System.EventHandler(this.kListControl1_Resize);
            // 
            // headerStrip2
            // 
            this.headerStrip2.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerStrip2.Location = new System.Drawing.Point(0, 0);
            this.headerStrip2.Name = "headerStrip2";
            this.headerStrip2.Size = new System.Drawing.Size(240, 54);
            this.headerStrip2.TabIndex = 2;
            this.headerStrip2.Text = "headerStrip2";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Controls.Add(this.headerStrip1);
            this.tabPage2.Controls.Add(this.textBox2);
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.domainUpDown1);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(240, 245);
            this.tabPage2.Text = "tabPage2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(101, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 92);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(215, 129);
            // 
            // headerStrip1
            // 
            this.headerStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(240, 38);
            this.headerStrip1.TabIndex = 2;
            this.headerStrip1.Text = "Now Playing";
            this.headerStrip1.Click += new System.EventHandler(this.headerStrip1_Click);
            // 
            // textBox2
            // 
            this.textBox2.AcceptsReturn = false;
            this.textBox2.AcceptsTab = false;
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox2.HideSelection = true;
            this.textBox2.InputMode = Microsoft.WindowsCE.Forms.InputMode.Default;
            this.textBox2.Location = new System.Drawing.Point(7, 66);
            this.textBox2.MaxLength = 32767;
            this.textBox2.Modified = false;
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '\0';
            this.textBox2.ReadOnly = false;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBox2.SelectedText = "";
            this.textBox2.SelectionLength = 0;
            this.textBox2.SelectionStart = 0;
            this.textBox2.Size = new System.Drawing.Size(166, 22);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.textBox2.WordWrap = true;
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = false;
            this.textBox1.AcceptsTab = false;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox1.HideSelection = true;
            this.textBox1.InputMode = Microsoft.WindowsCE.Forms.InputMode.Default;
            this.textBox1.Location = new System.Drawing.Point(18, 43);
            this.textBox1.MaxLength = 32767;
            this.textBox1.Modified = false;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.PasswordChar = '\0';
            this.textBox1.ReadOnly = false;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBox1.SelectedText = "";
            this.textBox1.SelectionLength = 0;
            this.textBox1.SelectionStart = 0;
            this.textBox1.Size = new System.Drawing.Size(215, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.textBox1.WordWrap = true;
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.ContextMenu = this.contextMenu1;
            this.domainUpDown1.Items.Add("0");
            this.domainUpDown1.Items.Add("1");
            this.domainUpDown1.Items.Add("2");
            this.domainUpDown1.Items.Add("3");
            this.domainUpDown1.Items.Add("4");
            this.domainUpDown1.Items.Add("5");
            this.domainUpDown1.Items.Add("6");
            this.domainUpDown1.Items.Add("7");
            this.domainUpDown1.Items.Add("8");
            this.domainUpDown1.Items.Add("9");
            this.domainUpDown1.Location = new System.Drawing.Point(60, 52);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.Size = new System.Drawing.Size(100, 22);
            this.domainUpDown1.TabIndex = 0;
            this.domainUpDown1.Text = "domainUpDown1";
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.Add(this.menuItem8);
            this.contextMenu1.MenuItems.Add(this.menuItem9);
            // 
            // menuItem8
            // 
            this.menuItem8.Text = "d";
            // 
            // menuItem9
            // 
            this.menuItem9.Text = "d";
            // 
            // notificationWithSoftKeys1
            // 
            this.notificationWithSoftKeys1.NotificationId = new System.Guid("a877d65f-239c-47a7-9304-0d347f580408");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tabStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabStrip1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem6;
        private Tenor.Mobile.UI.TabStrip tabStrip1;
        private System.Windows.Forms.TabPage tabPage1;
        private Tenor.Mobile.UI.KListControl kListControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DomainUpDown domainUpDown1;
        private Microsoft.WindowsCE.Forms.HardwareButton hardwareButton1;
        private Tenor.Mobile.UI.NotificationWithSoftKeys notificationWithSoftKeys1;
        private System.Messaging.MessageQueue messageQueue1;
        private System.Windows.Forms.MenuItem menuItem7;
        private Tenor.Mobile.UI.TextControl textBox1;
        private Tenor.Mobile.UI.TextControl textBox2;
        private Tenor.Mobile.UI.HeaderStrip headerStrip1;
        private Tenor.Mobile.UI.HeaderStrip headerStrip2;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
    }
}


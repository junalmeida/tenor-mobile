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
            this.tabStrip1 = new Tenor.Mobile.UI.TabStrip();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.kListControl1 = new Tenor.Mobile.UI.KListControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.hardwareButton1 = new Microsoft.WindowsCE.Forms.HardwareButton();
            this.messageQueue1 = new System.Messaging.MessageQueue();
            this.tabStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
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
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 245);
            this.tabPage1.Text = "tabPage1";
            // 
            // kListControl1
            // 
            this.kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            this.kListControl1.Location = new System.Drawing.Point(0, 84);
            this.kListControl1.Name = "kListControl1";
            this.kListControl1.SeparatorColor = System.Drawing.SystemColors.InactiveBorder;
            this.kListControl1.Size = new System.Drawing.Size(240, 161);
            this.kListControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.domainUpDown1);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(192, 74);
            this.tabPage2.Text = "tabPage2";
            // 
            // domainUpDown1
            // 
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
            // messageQueue1
            // 
            this.messageQueue1.Formatter = new System.Messaging.XmlMessageFormatter(new string[0]);
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
        private System.Messaging.MessageQueue messageQueue1;
    }
}


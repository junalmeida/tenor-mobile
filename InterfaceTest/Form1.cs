using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tenor.Mobile.Diagnostics;
using Tenor.Mobile.Drawing;
using Tenor.Mobile.Device;

namespace InterfaceTest
{
    public partial class Form1 : Form
    {
        Tenor.Mobile.Location.Cell cell;


        public Form1()
        {
            InitializeComponent();

            Guid g = new Guid("b877d65f-239c-47a7-9304-0d347f580408");
            if (!Notification.Exists(g))
            {
                Notification not = Notification.Create(g);
                not.Text = "Test";
            }


            kListControl1.DrawSeparators = true;
            kListControl1.AddItem("Select an option", "");
            kListControl1.SelectedItem = kListControl1[0];


            cell = new Tenor.Mobile.Location.Cell(true);
            cell.LocationChanged += new EventHandler(cell_TowerChanged);
        }

        void cell_TowerChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Threading.ThreadStart(delegate() {
                    textBox1.Text = string.Format("{0} | {1}", cell.Latitude, cell.Longitude);
                    textBox2.Text = new Tenor.Mobile.Location.Geolocation(cell.Latitude.Value, cell.Longitude.Value).ToString();
                }));
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            for (int i = 0; i <= 10; i++)
            {
                int size = r.Next(25, 70);
                kListControl1.AddItem("Item " + i.ToString(), i, size);
            }
            kListControl1.SelectedItem = kListControl1[0];

            notificationWithSoftKeys1.Icon = this.Icon;
            notificationWithSoftKeys1.Caption = this.Text;
            notificationWithSoftKeys1.Text = "Test";
            notificationWithSoftKeys1.LeftSoftKey.Title = "View";
            //notificationWithSoftKeys1.AvoidBubble = true;
            notificationWithSoftKeys1.Visible = true;
            //Tenor.Mobile.Device.Leds.Vibrate(500);

        }

  

        private void test(object sender, EventArgs e)
        {
            this.Activate();
        }



        private void menuItem3_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Horizontal;
            for (int i = 0; i <= 20; i++)
            {
                int size = r.Next(25, 70);
                kListControl1.AddItem("Item " + i.ToString(), i, size);
            }

        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            foreach (Window w in Window.GetWindows())
            {
                kListControl1.AddItem(w.Text + " (" + w.ClassName +  ") " + w.Visible.ToString(), w);
            }

        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            foreach (Process p in Process.GetProcesses())
            {
                kListControl1.AddItem(p.FileName, p);
            }
        }

        private struct FF
        {
            public FileInfo file;
            public Icon icon;
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.DrawItem += new Tenor.Mobile.UI.DrawItemEventHandler(kListControl1_DrawItem);
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            foreach (FileInfo f in new DirectoryInfo("\\Windows").GetFiles("*.exe"))
            {
                kListControl1.AddItem(f.Name, new FF() { file = f, icon = Tenor.Mobile.Drawing.IconHelper.ExtractAssociatedIcon(f.FullName) });
            }
        }

        void kListControl1_DrawItem(object sender, Tenor.Mobile.UI.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            SolidBrush textBrush;
            if (e.Item.Selected)
            {
                SolidBrush backBrush;
                backBrush = new SolidBrush(SystemColors.Highlight);
                textBrush = new SolidBrush(SystemColors.HighlightText);
                g.FillRectangle(backBrush, e.Bounds);
            }
            else
            {
                //backBrush = new SolidBrush(SystemColors.Window);
                textBrush = new SolidBrush(SystemColors.ControlText);
            }

            FF ff = (FF)e.Item.Value;
            Rectangle iconRect = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, ff.icon.Width, ff.icon.Height);
            RoundedRectangle.Fill(g, new Pen(SystemColors.ControlDark), SystemColors.ControlDark, iconRect, new Size(8, 8));
            g.DrawIcon(ff.icon, iconRect.X, iconRect.Y);
            Rectangle r = e.Bounds;
            r.X += 32;

            g.DrawString("  " + ff.file.Name, kListControl1.Font, textBrush, r, format);
            r.Y += 14;
            g.DrawString("  " + ff.file.CreationTime.ToString(), kListControl1.Font, textBrush, r, format);


            Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Bottom - 2, Convert.ToInt32(e.Bounds.Width / 2), 2);
            GradientFill.Fill(g, rect, SystemColors.Window, SystemColors.Control, GradientFill.FillDirection.LeftToRight);
            rect.Offset(rect.Width, 0);
            GradientFill.Fill(g, rect, SystemColors.Control, SystemColors.Window, GradientFill.FillDirection.LeftToRight);
        }

        private void tabStrip1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            kListControl1.AddItem("Teste " + kListControl1.Count.ToString(), null);

            if (!notificationWithSoftKeys1.Visible)
            {
                notificationWithSoftKeys1.Icon = this.Icon;
                notificationWithSoftKeys1.Caption = this.Text + "\t1 of 2";
                notificationWithSoftKeys1.Text = "Test";
                notificationWithSoftKeys1.LeftSoftKey.Title = "View";
                notificationWithSoftKeys1.AvoidBubble = true;
                notificationWithSoftKeys1.InitialDuration = 0;
                notificationWithSoftKeys1.Spinners = true;
                notificationWithSoftKeys1.SpinnerClick += new Tenor.Mobile.UI.SpinnerClickEventHandler(notificationWithSoftKeys1_SpinnerClick);
                notificationWithSoftKeys1.Visible = true;
            }
        }

        void notificationWithSoftKeys1_SpinnerClick(object sender, Tenor.Mobile.UI.SpinnerClickEventArgs e)
        {
            
        }

        private void kListControl1_Resize(object sender, EventArgs e)
        {
            if (kListControl1.Count > 0)
            {
                kListControl1.SelectedItem = null;
                kListControl1.EnsureVisible(kListControl1[kListControl1.Count - 1]);
            }
        }
    }
}
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

namespace InterfaceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            kListControl1.DrawSeparators = true;
            kListControl1.AddItem("Select an option", "");
            kListControl1.SelectItem(0, 0);
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            for (int i = 0; i <= 10; i++)
            {
                kListControl1.AddItem("Item " + i.ToString(), i);
            }
            kListControl1.SelectItem(0, 0);

            notificationWithSoftKeys1.Icon = this.Icon;
            notificationWithSoftKeys1.Caption = this.Text;
            notificationWithSoftKeys1.Text = "Test";
            notificationWithSoftKeys1.LeftSoftKey.Title = "View";
            notificationWithSoftKeys1.Visible = true;
        }

        private void test(object sender, EventArgs e)
        {
            this.Activate();
        }



        private void menuItem3_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Horizontal;
            for (int i = 0; i <= 20; i++)
            {
                kListControl1.AddItem("Item " + i.ToString(), i);
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
            g.DrawIcon(ff.icon, e.Bounds.X + 1, e.Bounds.Y + 1);
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
    }
}
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InterfaceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            kListControl1.DrawSeparators = true;
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            for (int i = 0; i <= 10; i++)
            {
                kListControl1.AddItem("Item " + i.ToString(), i);
            }
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
            for (int i = 0; i <= 30; i++)
            {
                kListControl1.AddItem("Item " + i.ToString(), i);
            }

        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Vertical;
            for (int i = 0; i <= 5; i++)
            {
                kListControl1.AddItem("Item " + i.ToString(), i);
            }
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            kListControl1.Clear();
            kListControl1.Layout = Tenor.Mobile.UI.KListLayout.Grid;
            for (int i = 0; i <= 40; i++)
            {
                kListControl1.AddItem("Item " + i.ToString(), i);
            }

        }

        private void tabStrip1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
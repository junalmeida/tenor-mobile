using System;

using System.Collections.Generic;
using System.Windows.Forms;

namespace InterfaceTest
{
    static class Program
    {
        static Tenor.Mobile.Location.WorldPosition loc;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            loc = new Tenor.Mobile.Location.WorldPosition(true, true);
            loc.LocationChanged += new EventHandler(loc_LocationChanged);
            Application.Run(new Form1());
        }

        static void loc_LocationChanged(object sender, EventArgs e)
        {
            
        }
    }
}
using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Tenor.Mobile.UI
{
    internal static class Extensions
    {
        internal static bool IsDesignMode(Control control)
        {
            ISite site = control.Site;
            return ((site != null) && site.DesignMode);
        }
    }
}

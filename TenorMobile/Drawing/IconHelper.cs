using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Tenor.Mobile.Drawing
{
    public static class IconHelper
    {
        public static Icon ExtractAssociatedIcon(string filename)
        {
            return ExtractAssociatedIcon(filename, true);
        }

        public static Icon ExtractAssociatedIcon(string filename, bool largeIcon)
        {
            NativeMethods.SHFILEINFO psfi = new NativeMethods.SHFILEINFO();
            if (NativeMethods.SHGetFileInfo(
                filename, 0, ref psfi, Marshal.SizeOf(psfi), NativeMethods.SHGFI.ICON | (largeIcon ? NativeMethods.SHGFI.LARGEICON : NativeMethods.SHGFI.SMALLICON)) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return Icon.FromHandle(psfi.hIcon);
        }
    }
}

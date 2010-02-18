using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Tenor.Mobile.Drawing
{
    /// <summary>
    /// Provides functions to manipulate icons.
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// Extracts an icon from a document or an executable.
        /// </summary>
        /// <param name="filename">Full path of a file.</param>
        /// <returns>An Icon.</returns>
        public static Icon ExtractAssociatedIcon(string filename)
        {
            return ExtractAssociatedIcon(filename, true);
        }

        /// <summary>
        /// Extracts an icon from a document or an executable.
        /// </summary>
        /// <param name="filename">Full path of a file.</param>
        /// <param name="largeIcon"></param>
        /// <returns></returns>
        public static Icon ExtractAssociatedIcon(string filename, bool largeIcon)
        {
            Tenor.Mobile.NativeMethods.SHFILEINFO psfi = new Tenor.Mobile.NativeMethods.SHFILEINFO();
            if (Tenor.Mobile.NativeMethods.SHGetFileInfo(
                filename, 0, ref psfi, Marshal.SizeOf(psfi), Tenor.Mobile.NativeMethods.SHGFI.ICON | (largeIcon ? Tenor.Mobile.NativeMethods.SHGFI.LARGEICON : Tenor.Mobile.NativeMethods.SHGFI.SMALLICON)) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return Icon.FromHandle(psfi.hIcon);
        }
    }
}

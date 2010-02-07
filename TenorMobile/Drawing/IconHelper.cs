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

using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Tenor.Mobile
{
    internal static class NativeMethods
    {

        [DllImport("coredll.dll", SetLastError = false)]
        internal static extern int GetLastError();


        [Flags()]
        internal enum SWP
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040
        }

        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

        [DllImport("coredll.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, GW uCmd);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern IntPtr GetWindowLong(IntPtr hWnd, GWL nIndex);

        //[DllImport("coredll.dll", SetLastError = true)]
        //internal static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, WndProcDelegate newProc);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);



        [DllImport("coredll.dll")]
        internal static extern int SendMessage(IntPtr hWnd, WMSG Msg, int wParam, ref RECT lParam);

        //[DllImport("coredll.dll")]
        //internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVBKIMAGE lParam);
        [DllImport("coredll.dll")]
        internal static extern int SendMessage(IntPtr hWnd, WMSG Msg, int wParam, IntPtr lParam);
        [DllImport("coredll.dll")]
        internal static extern int SendMessage(IntPtr hWnd, WMSG Msg, int wParam, int lParam);
        [DllImport("coredll.dll")]
        internal static extern int SendMessage(IntPtr hWnd, WMSG msg, int wParam, string lParam);

        internal enum GW
        {
            HWNDFIRST,
            HWNDLAST,
            HWNDNEXT,
            HWNDPREV,
            OWNER,
            CHILD
        }

        internal enum GWL
        {
            EXSTYLE = -20,
            ID = -12,
            STYLE = -16,
            USERDATA = -21,
            WNDPROC = -4
        }


        internal enum WMSG
        {
            /* WINDOW */
            WM_PAINT = 0x000F,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            /* TAB CONTROL*/
            TCM_FIRST = 0x1300,
            TCM_SETPADDING = (TCM_FIRST + 43),
            TCM_SETITEMSIZE = (TCM_FIRST + 41),
            TCM_GETIMAGELIST = (TCM_FIRST + 2),
            TCM_SETIMAGELIST = (TCM_FIRST + 3),
            TCM_SETITEM = (TCM_FIRST + 61)
        }

        internal static int MakeLParam(int a, int b)
        {
            return (a | (b << 16));
            //return (((short)wHigh << 16) | (wLow & 0xffff));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINTAPI
        {
            public int cx;
            public int cy;
        }

        internal static IntPtr MakeLParam(Point point)
        {
            return (IntPtr)((point.Y * 0x10000) + point.X);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class TCITEM
        {
            public int mask;
            public UInt32 dwState;
            public UInt32 dwStateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public UInt32 lParam;
        }
        internal const int TCIF_IMAGE = 0x0002;
        internal const int TCIF_TEXT = 0x0001;



        [DllImport("coredll")]
        internal static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, SHGFI uFlags);
       
        [Flags]
        internal enum SHGFI
        {
            ATTRIBUTES = 0x800,
            DISPLAYNAME = 0x200,
            ICON = 0x100,
            LARGEICON = 0,
            PIDL = 8,
            SELECTICON = 0x40000,
            SMALLICON = 1,
            SYSICONINDEX = 0x4000,
            TYPENAME = 0x400,
            USEFILEATTRIBUTES = 0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            internal IntPtr hIcon;
            internal int iIcon;
            internal int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            internal string szTypeName;
        }


        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hWnd, WMSG Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, ref IntPtr lpdwResult);

        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hWnd, WMSG Msg, int wParam, StringBuilder lParam, uint fuFlags, uint uTimeout, ref IntPtr lpdwResult);


        [DllImport("coredll.dll", EntryPoint = "GetMessageW", SetLastError = true)]
        internal static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("coredll.dll", EntryPoint = "TranslateMessage", SetLastError = true)]
        internal static extern bool TranslateMessage(out MSG lpMsg);

        [DllImport("coredll.dll", EntryPoint = "DispatchMessage", SetLastError = true)]
        internal static extern bool DispatchMessage(ref MSG lpMsg);

        internal struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public int pt_x;
            public int pt_y;
        }

        /// <summary>
        /// Defines a message filter interface.
        /// </summary>
        /// <remarks>This interface allows an application to capture a message before it is dispatched to a control or form.
        /// <para>A class that implements the IMessageFilter interface can be added to the application's message pump to filter out a message or perform other operations before the message is dispatched to a form or control. To add the message filter to an application's message pump, use the <see cref="M:OpenNETCF.Windows.Forms.ApplicationEx.AddMessageFilter(OpenNETCF.Windows.Forms.IMessageFilter)"/> method in the <see cref="T:OpenNETCF.Windows.Forms.ApplicationEx"/> class.</para></remarks>
        internal interface IMessageFilter
        {
            /// <summary>
            /// Filters out a message before it is dispatched.
            /// </summary>
            /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
            /// <returns>true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.</returns>
            bool PreFilterMessage(ref Microsoft.WindowsCE.Forms.Message m);
        }

    }
}

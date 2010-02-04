using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Tenor.Mobile
{
    internal static class NativeMethods
    {
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
            /* TAB CONTROL*/
            TCM_FIRST = 0x1300,
            TCM_SETPADDING = (TCM_FIRST + 43),
            TCM_SETITEMSIZE = (TCM_FIRST + 41)
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
    }
}

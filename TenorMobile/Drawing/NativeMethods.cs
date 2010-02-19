using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Tenor.Mobile.Drawing
{

    internal sealed class NativeMethods
    {
        internal struct TRIVERTEX
        {
            public int x;
            public int y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;
            public TRIVERTEX(int x, int y, Color color)
                : this(x, y, color.R, color.G, color.B, color.A)
            {
            }
            public TRIVERTEX(
                int x, int y,
                ushort red, ushort green, ushort blue,
                ushort alpha)
            {
                this.x = x;
                this.y = y;
                this.Red = (ushort)(red << 8);
                this.Green = (ushort)(green << 8);
                this.Blue = (ushort)(blue << 8);
                this.Alpha = (ushort)(alpha << 8);
            }
        }
        internal struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;
            public GRADIENT_RECT(uint ul, uint lr)
            {
                this.UpperLeft = ul;
                this.LowerRight = lr;
            }
        }


        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        internal extern static bool GradientFill(
            IntPtr hdc,
            TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        internal const int GRADIENT_FILL_RECT_H = 0x00000000;
        internal const int GRADIENT_FILL_RECT_V = 0x00000001;




        internal const int PS_SOLID = 0;
        internal const int PS_DASH = 1;

        [DllImport("coredll.dll")]
        internal static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

        [DllImport("coredll.dll")]
        internal static extern int SetBrushOrgEx(IntPtr hdc, int nXOrg, int nYOrg, ref Point lppt);
        [DllImport("coredll.dll")]
        internal static extern IntPtr CreateSolidBrush(uint color);
        [DllImport("coredll.dll")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobject);

        [DllImport("coredll.dll")]
        internal static extern bool DeleteObject(IntPtr hgdiobject);

        [DllImport("coredll.dll")]
        internal static extern bool RoundRect(
            IntPtr hdc,
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidth,
            int nHeight);

    }
}
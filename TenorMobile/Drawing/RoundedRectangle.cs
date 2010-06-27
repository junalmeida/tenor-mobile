using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Tenor.Mobile.Drawing
{
    public static class RoundedRectangle
    {
        public static void Fill(
    Graphics graphics,
    Pen border,
    Color color,
    Rectangle rectangle,
    Size ellipseSize)
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                var lppt = new Point();
                var hdc = graphics.GetHdc();
                var style = border.DashStyle == DashStyle.Solid ? NativeMethods.PS_SOLID : NativeMethods.PS_DASH;
                var hpen = NativeMethods.CreatePen(style, (int)border.Width, GetColorRef(border.Color));
                var hbrush = NativeMethods.CreateSolidBrush(GetColorRef(color));
                try
                {

                    NativeMethods.SetBrushOrgEx(hdc, rectangle.Left, rectangle.Top, ref lppt);
                    NativeMethods.SelectObject(hdc, hpen);

                    if (!color.IsEmpty && !color.Equals(Color.Transparent))
                        NativeMethods.SelectObject(hdc, hbrush);

                    NativeMethods.RoundRect(hdc,
                              rectangle.Left,
                              rectangle.Top,
                              rectangle.Right,
                              rectangle.Bottom,
                              ellipseSize.Width,
                              ellipseSize.Height);

                }
                finally
                {
                    NativeMethods.SetBrushOrgEx(hdc, lppt.Y, lppt.X, ref lppt);
                    NativeMethods.DeleteObject(hpen);
                    NativeMethods.DeleteObject(hbrush);
                    graphics.ReleaseHdc(hdc);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(color))
                    graphics.FillRectangle(brush, rectangle);
                graphics.DrawRectangle(border, rectangle);
            }
        }

        static uint GetColorRef(Color value)
        {
            return 0x00000000 | ((uint)value.B << 16) | ((uint)value.G << 8) | (uint)value.R;
        }
    }
}

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
            Brush brush,
            Rectangle rectangle,
            Size ellipseSize)
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                var lppt = new Point();
                var hdc = graphics.GetHdc();

                IntPtr hpen = IntPtr.Zero;
                if (border != null && !border.Color.IsEmpty && !Color.Equals(border.Color, Color.Transparent))
                {
                    var style = border.DashStyle == DashStyle.Solid ? NativeMethods.PS_SOLID : NativeMethods.PS_DASH;
                    hpen = NativeMethods.CreatePen(style, (int)border.Width, GetColorRef(border.Color));
                }

                IntPtr hbrush = IntPtr.Zero;
                if (brush is SolidBrush)
                {
                    SolidBrush sbrush = (SolidBrush)brush;
                    if (!sbrush.Color.IsEmpty && !sbrush.Color.Equals(Color.Transparent))
                        hbrush = NativeMethods.CreateSolidBrush(GetColorRef(sbrush.Color));
                }
                else if (brush is TextureBrush)
                {
                    TextureBrush tbrush = (TextureBrush)brush;
                    Bitmap bmp = tbrush.Image as Bitmap;
                    if (bmp == null)
                        throw new NotSupportedException("Image of texturebrush is not supported.");
                    hbrush = NativeMethods.CreatePatternBrush(bmp.GetHbitmap());
                }
                else
                    throw new NotSupportedException(brush.GetType().Name + " is not supported.");

                try
                {

                    NativeMethods.SetBrushOrgEx(hdc, rectangle.Left, rectangle.Top, ref lppt);
                    if (!IntPtr.Equals(hpen, IntPtr.Zero))
                        NativeMethods.SelectObject(hdc, hpen);

                    if (!IntPtr.Equals(hbrush, IntPtr.Zero))
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
                    if (!IntPtr.Equals(hpen, IntPtr.Zero))
                        NativeMethods.DeleteObject(hpen);
                    if (!IntPtr.Equals(hbrush, IntPtr.Zero))
                        NativeMethods.DeleteObject(hbrush);
                    graphics.ReleaseHdc(hdc);
                }
            }
            else
            {
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

using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tenor.Mobile.Drawing
{
    public static class Strings
    {
        public static Size Measure(Graphics gr, string text, Font font, Rectangle rect)
        {
            Tenor.Mobile.NativeMethods.RECT bounds = new Tenor.Mobile.NativeMethods.RECT();
            bounds.left = rect.Left;
            bounds.right = rect.Right;
            bounds.bottom = rect.Bottom;
            bounds.right = rect.Right;

            IntPtr hFont = font.ToHfont();
            IntPtr hdc = gr.GetHdc();
            IntPtr originalObject = NativeMethods.SelectObject(hdc, hFont);
            int flags = NativeMethods.DT_CALCRECT | NativeMethods.DT_WORDBREAK;
            NativeMethods.DrawText(hdc, text, text.Length, ref bounds, flags);
            NativeMethods.SelectObject(hdc, originalObject);
            gr.ReleaseHdc(hdc);
            return new Size(bounds.right - bounds.left, bounds.bottom - bounds.top);

        }
    }
}
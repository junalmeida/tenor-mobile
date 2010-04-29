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


        public static Color ToColor(string htmlColor)
        {
            Color color = Color.Empty;
            if (!string.IsNullOrEmpty(htmlColor))
            {
                if ((htmlColor[0] == '#') && ((htmlColor.Length == 7) || (htmlColor.Length == 4)))
                {
                    if (htmlColor.Length == 7)
                    {
                        color = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 0x10), Convert.ToInt32(htmlColor.Substring(3, 2), 0x10), Convert.ToInt32(htmlColor.Substring(5, 2), 0x10));
                    }
                    else
                    {
                        string str = char.ToString(htmlColor[1]);
                        string str2 = char.ToString(htmlColor[2]);
                        string str3 = char.ToString(htmlColor[3]);
                        color = Color.FromArgb(Convert.ToInt32(str + str, 0x10), Convert.ToInt32(str2 + str2, 0x10), Convert.ToInt32(str3 + str3, 0x10));
                    }
                }
       
            }
            return color;
        }

 

 

    }
}
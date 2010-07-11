using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Tenor.Mobile.Drawing
{
    public static class GraphicsEx
    {



        private const int SRCCOPY=0xCC0020;
        [DllImport("coredll")]
        private static extern int BitBlt(IntPtr hdcDest, int nXDest , int nYDest, int nWidth ,int nHeight , IntPtr hdcSrc , int nXSrc ,int nYSrc, int dwRop);
        public static Image CopyFromScreen(Graphics g, Rectangle source)
        {
            // Create compatible graphics
            Bitmap bmp = new Bitmap(source.Width, source.Height);
            using (Graphics gxComp = Graphics.FromImage(bmp))
            {

                IntPtr hdcSrc = g.GetHdc();
                IntPtr hdcDest = gxComp.GetHdc();

                try
                {
                    //' Blit the image data
                    BitBlt(hdcDest, 0, 0, source.Width, source.Height, hdcSrc, source.Left, source.Top, SRCCOPY);
                }
                catch
                {
                    bmp.Dispose();
                    throw;
                }
                finally
                {
                    gxComp.ReleaseHdc(hdcDest);
                    g.ReleaseHdc(hdcSrc);
                }

                //bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                return bmp;
            }
        }
    }
}

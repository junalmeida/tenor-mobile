using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace Tenor.Mobile.Drawing
{
    public static class AlphaImage
    {


        /// <summary>
        /// Converts any Stream into an array of bytes.
        /// </summary>
        /// <param name="stream">The desired stream.</param>
        /// <returns>An array of bytes.</returns>
        private static byte[] StreamToBytes(Stream stream)
        {

            stream.Seek(0, SeekOrigin.Begin);

            byte[] data = new byte[System.Convert.ToInt32(stream.Length)];
            int offset = 0;
            int total = (int)stream.Length;
            int remaining = total;

            while (remaining > 0 || total == -1)
            {
                if (total == -1)
                    remaining = 1024 * 2;

                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    break;
                }
                remaining -= read;
                offset += read;
            }

            return data;
        }

        public static void DrawImage(Stream src, Graphics destGraphics, Rectangle destRect)
        {
            DrawImage(src, Rectangle.Empty, destGraphics, destRect);
        }
        /// <summary>
        /// Draws an image with alpha blending.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcRect"></param>
        /// <param name="destGraphics"></param>
        /// <param name="destRect"></param>
        public static void DrawImage(Stream src, Rectangle srcRect, Graphics destGraphics, Rectangle destRect)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (destGraphics == null)
                throw new ArgumentNullException("destGraphics");
            if (destRect.IsEmpty)
                throw new ArgumentException("The destination rectangle cannot be empty.", "destRect");

            IImagingFactory factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
            // Load the image with alpha data from an embedded resource through Imaging
            // !! If your data source is not a MemoryStream, you will need to get your image data into a byte array and use it below. !!
            byte[] pbBuf = StreamToBytes(src);
            uint cbBuf = (uint)src.Length;
            IImage imagingResource;
            factory.CreateImageFromBuffer(pbBuf, cbBuf, BufferDisposalFlag.BufferDisposalFlagNone, out imagingResource);


            IntPtr hdcDest = destGraphics.GetHdc();
            try
            {
                // Ask the Image object from Imaging to draw itself.
                if (srcRect.IsEmpty)
                    imagingResource.Draw(hdcDest, destRect, null);
                else
                {
                    try
                    {
                        imagingResource.Draw(hdcDest, destRect, srcRect);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new NotSupportedException("srcRect not supported.", ex);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                destGraphics.ReleaseHdc(hdcDest);
            }
        }
    }


    // Pulled from gdipluspixelformats.h in the Windows Mobile 5.0 Pocket PC SDK
    internal enum PixelFormatID : int
    {
        PixelFormatIndexed = 0x00010000,  // Indexes into a palette
        PixelFormatGDI = 0x00020000,      // Is a GDI-supported format
        PixelFormatAlpha = 0x00040000,    // Has an alpha component
        PixelFormatPAlpha = 0x00080000,   // Pre-multiplied alpha
        PixelFormatExtended = 0x00100000, // Extended color 16 bits/channel
        PixelFormatCanonical = 0x00200000,
        PixelFormatUndefined = 0,
        PixelFormatDontCare = 0,
        PixelFormat1bppIndexed = (1 | (1 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat4bppIndexed = (2 | (4 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat8bppIndexed = (3 | (8 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat16bppRGB555 = (5 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppRGB565 = (6 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppARGB1555 = (7 | (16 << 8) | PixelFormatAlpha | PixelFormatGDI),
        PixelFormat24bppRGB = (8 | (24 << 8) | PixelFormatGDI),
        PixelFormat32bppRGB = (9 | (32 << 8) | PixelFormatGDI),
        PixelFormat32bppARGB = (10 | (32 << 8) | PixelFormatAlpha | PixelFormatGDI | PixelFormatCanonical),
        PixelFormat32bppPARGB = (11 | (32 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatGDI),
        PixelFormat48bppRGB = (12 | (48 << 8) | PixelFormatExtended),
        PixelFormat64bppARGB = (13 | (64 << 8) | PixelFormatAlpha | PixelFormatCanonical | PixelFormatExtended),
        PixelFormat64bppPARGB = (14 | (64 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatExtended),
        PixelFormatMax = 15
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    internal enum BufferDisposalFlag : int
    {
        BufferDisposalFlagNone,
        BufferDisposalFlagGlobalFree,
        BufferDisposalFlagCoTaskMemFree,
        BufferDisposalFlagUnmapView
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    internal enum InterpolationHint : int
    {
        InterpolationHintDefault,
        InterpolationHintNearestNeighbor,
        InterpolationHintBilinear,
        InterpolationHintAveraging,
        InterpolationHintBicubic
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    internal struct ImageInfo
    {
        // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart1;
        public uint GuidPart2;
        public uint GuidPart3;
        public uint GuidPart4;
        public PixelFormatID pixelFormat;
        public uint Width;
        public uint Height;
        public uint TileWidth;
        public uint TileHeight;
        public double Xdpi;
        public double Ydpi;
        public uint Flags;
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    [ComImport, Guid("327ABDA7-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    internal interface IImagingFactory
    {
        uint CreateImageFromStream(); // This is a place holder
        uint CreateImageFromFile(string filename, out IImage image);
        // We need the MarshalAs attribute here to keep COM interop from sending the buffer down as a Safe Array.
        uint CreateImageFromBuffer([MarshalAs(UnmanagedType.LPArray)] byte[] buffer, uint size, BufferDisposalFlag disposalFlag, out IImage image);
        uint CreateNewBitmap();            // This is a place holder
        uint CreateBitmapFromImage();      // This is a place holder
        uint CreateBitmapFromBuffer();     // This is a place holder
        uint CreateImageDecoder();         // This is a place holder
        uint CreateImageEncoderToStream(); // This is a place holder
        uint CreateImageEncoderToFile();   // This is a place holder
        uint GetInstalledDecoders();       // This is a place holder
        uint GetInstalledEncoders();       // This is a place holder
        uint InstallImageCodec();          // This is a place holder
        uint UninstallImageCodec();        // This is a place holder
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    [ComImport, Guid("327ABDA9-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    internal interface IImage
    {
        uint GetPhysicalDimension(out Size size);
        uint GetImageInfo(out ImageInfo info);
        uint SetImageFlags(uint flags);
        uint Draw(IntPtr hdc, RECT dstRect, RECT srcRect);
        uint PushIntoSink(); // This is a place holder
        uint GetThumbnail(uint thumbWidth, uint thumbHeight, out IImage thumbImage);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(Rectangle Rectangle)
            : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value - _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public Point Location
        {
            get { return new Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                _Right = value.Height + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(this.Left, this.Top, this.Width, this.Height);
        }
        public static Rectangle ToRectangle(RECT Rectangle)
        {
            return Rectangle.ToRectangle();
        }
        public static RECT FromRectangle(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return Rectangle.ToRectangle();
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }
        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Right.GetHashCode() ^ Top.GetHashCode() ^ Bottom.GetHashCode();
        }
    }


    //[StructLayout(LayoutKind.Sequential)]
    //internal struct SIZE
    //{
    //    public int cx;
    //    public int cy;

    //    public SIZE(int cx, int cy)
    //    {
    //        this.cx = cx;
    //        this.cy = cy;
    //    }

    //    public static implicit operator Size(SIZE size)
    //    {
    //        return new Size(size.cx, size.cy);
    //    }
    //    public static implicit operator SIZE(Size size)
    //    {
    //        return new SIZE(size.Width, size.Height);
    //    }
    //} 
}

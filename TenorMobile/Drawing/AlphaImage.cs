using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tenor.Mobile.Drawing
{
    public class AlphaImage : IDisposable
    {
        Image image;
        Tenor.Mobile.Drawing.IImaging.IImage iimage;

        public AlphaImage(Image image)
        {
            this.image = image;
        }


        public AlphaImage(Stream data)
            : this(IO.StreamToBytes(data)) { }


        public AlphaImage(byte[] data)
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                try
                {
                    Tenor.Mobile.Drawing.IImaging.IImagingFactory factory = IImaging.CreateFactory();

                    factory.CreateImageFromBuffer(data,
                        Convert.ToUInt32(data.Length), IImaging.BufferDisposalFlag.BufferDisposalFlagNone,
                        out iimage);
                }
                catch (COMException ex)
                {
                    throw new ExternalException("Cannot initialize drawing.", ex);
                }
            }
            else
            {
                using (MemoryStream mem = new MemoryStream(data))
                    image = new Bitmap(mem);
            }
        }



        public int Width
        {
            get
            {
                return Size.Width;
            }
        }

        public int Height
        {
            get
            {
                return Size.Height;
            }
        }

        Size size;
        public Size Size
        {
            get
            {
                if (size.IsEmpty)
                    size = GetSize();
                return size;
            }
        }

        private Size GetSize()
        {
            if (image != null)
                return image.Size;
            else if (iimage != null)
            {
                Tenor.Mobile.Drawing.IImaging.ImageInfo info;
                iimage.GetImageInfo(out info);
                return new Size(Convert.ToInt32(info.Width), Convert.ToInt32(info.Height));
            }
            else
                return new Size();
        }


        public void Dispose()
        {
            image = null;
            iimage = null;
        }

 

        /// <summary>
        /// Draws an image with alpha blending.
        /// </summary>
        public void Draw(Graphics g, Rectangle destination)
        {
            Draw(g, Rectangle.Empty, destination, Color.Empty);
        }

        /// <summary>
        /// Draws an image with alpha blending.
        /// </summary>
        public void Draw(Graphics g, Rectangle destination, Color key)
        {
            Draw(g, Rectangle.Empty, destination, key);
        }

        /// <summary>
        /// Draws an image with alpha blending.
        /// </summary>
        public void Draw(Graphics g, Rectangle source, Rectangle destination, Color key)
        {
            if (image != null)
                DrawImageManaged(g, source, destination, key);
            else if (iimage != null)
            {
                if (!Color.Equals(key, Color.Empty))
                    throw new NotSupportedException("Color key is not supported with streams.");
                else if (!Rectangle.Equals(source, Rectangle.Empty))
                    throw new NotSupportedException("Source rectangle is not supported with streams.");
                else
                    DrawImageGdi(g, destination);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }


        private void DrawImageManaged(Graphics g, Rectangle source, Rectangle destination, Color key)
        {
            if (g == null)
                throw new ArgumentNullException("g");


            ImageAttributes attr = new ImageAttributes();
            if (key.IsEmpty)
            {
                if (image is Bitmap)
                    key = ((Bitmap)image).GetPixel(0, 0);
            }
            attr.SetColorKey(key, key);
            if (source.IsEmpty)
                source = new Rectangle(0, 0, image.Width, image.Height);

            g.DrawImage(image, destination, source.Left, source.Top, source.Width, source.Height, GraphicsUnit.Pixel, attr);
        }



        /// <summary>
        /// Draws an image with alpha blending.
        /// </summary>
        private void DrawImageGdi(Graphics g, Rectangle destination)
        {
            if (g == null)
                throw new ArgumentNullException("g");
            if (destination.IsEmpty)
                throw new ArgumentException("The destination rectangle cannot be empty.", "destination");

            IntPtr hdc = g.GetHdc();
            try
            {

                Tenor.Mobile.NativeMethods.RECT rect = new Tenor.Mobile.NativeMethods.RECT()
                {
                    left = destination.X,
                    top = destination.Y,
                    right = destination.Right,
                    bottom = destination.Bottom
                };

                iimage.Draw(hdc, ref rect, IntPtr.Zero);
            }
            catch (COMException ex)
            {
                throw new ExternalException("Unable to draw transparent image.", ex);
            }
            catch
            {
                throw;
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }

        }
    }

    internal class IImaging
    {
        #region stałe i struktury

        public enum PixelFormatID : int
        {
            PixelFormatIndexed = 0x00010000,
            PixelFormatGDI = 0x00020000,
            PixelFormatAlpha = 0x00040000,
            PixelFormatPAlpha = 0x00080000,
            PixelFormatExtended = 0x00100000,
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

        public enum BufferDisposalFlag : int
        {
            BufferDisposalFlagNone,
            BufferDisposalFlagGlobalFree,
            BufferDisposalFlagCoTaskMemFree,
            BufferDisposalFlagUnmapView
        }

        public enum InterpolationHint : int
        {
            InterpolationHintDefault,
            InterpolationHintNearestNeighbor,
            InterpolationHintBilinear,
            InterpolationHintAveraging,
            InterpolationHintBicubic
        }

        public struct BitmapData
        {
            public uint Width;
            public uint Height;
            public int Stride;
            public PixelFormatID PixelFormat;
            public IntPtr Scan0;
            public IntPtr Reserved;
        }

        public struct ImageInfo
        {
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

        #endregion

        #region interfejsy

        [ComImport, Guid("327ABDA7-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(false)]
        public interface IImagingFactory
        {
            uint CreateImageFromStream(IStream stream, out IImage image);
            uint CreateImageFromFile(string filename, out IImage image);
            uint CreateImageFromBuffer([MarshalAs(UnmanagedType.LPArray)] byte[] buffer, uint size, BufferDisposalFlag disposalFlag, out IImage image);
            uint CreateNewBitmap(uint width, uint height, PixelFormatID pixelFormat, out IBitmapImage bitmap);
            uint CreateBitmapFromImage(IImage image, uint width, uint height, PixelFormatID pixelFormat, InterpolationHint hints, out IBitmapImage bitmap);
            uint CreateBitmapFromBuffer(BitmapData bitmapData, out IBitmapImage bitmap);
            uint CreateImageDecoder();	// pominięte
            uint CreateImageEncoderToStream();	// pominięte
            uint CreateImageEncoderToFile();	// pominięte
            uint GetInstalledDecoders();	// pominięte
            uint GetInstalledEncoders();	// pominięte
            uint InstallImageCodec();	// pominięte
            uint UninstallImageCodec();	// pominięte
        }

        [ComImport, Guid("327ABDA9-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(false)]
        public interface IImage
        {
            uint GetPhysicalDimension(out Size size);
            uint GetImageInfo(out ImageInfo info);
            uint SetImageFlags(uint flags);
            uint Draw(IntPtr hdc, ref Tenor.Mobile.NativeMethods.RECT dstRect, IntPtr srcRect);
            uint PushIntoSink();	// pominięte
            uint GetThumbnail(uint thumbWidth, uint thumbHeight, out IImage thumbImage);
        }

        [ComImport, Guid("327ABDAA-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(false)]
        public interface IBitmapImage
        {
            uint GetSize(out Size size);
            uint GetPixelFormatID(out PixelFormatID pixelFormat);
            uint LockBits(ref Rectangle rect, uint flags, PixelFormatID pixelFormat, out BitmapData lockedBitmapData);
            uint UnlockBits(ref BitmapData lockedBitmapData);
            uint GetPalette();	// pominięte
            uint SetPalette();	// pominięte
        }

        [ComImport, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(false)]
        public interface IStream
        {
            void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbRead);
            void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten);
            void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);
            void SetSize(long libNewSize);
            void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);
            void Commit(int grfCommitFlags);
            void Revert();
            void LockRegion(long libOffset, long cb, int dwLockType);
            void UnlockRegion(long libOffset, long cb, int dwLockType);
            void Stat();	// pominięte
            void Clone(out IStream ppstm);
        }

        #endregion

        #region metody

        public static IImagingFactory CreateFactory()
        {
            return (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
        }

        #endregion
    }

}
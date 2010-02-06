using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Tenor.Mobile.UI
{
    /// <summary>
    /// The TabControl style for new WM 6.5 devices.
    /// </summary>
    public class TabStrip : TabControl
    {
        /// <summary>
        /// Creates a new instance of the TabStrip.
        /// </summary>
        public TabStrip()
            : base()
        {
        }


        int oldTabCount = 0;
        SizeF scaleFactor;
        /// <summary>
        /// Scales a control's location, size, padding and margin.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="specified"></param>
        protected override void ScaleControl(System.Drawing.SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            scaleFactor = factor;
            CheckIfResizeIsNeeded();
        }

        bool stupidDesigner;
        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CheckIfResizeIsNeeded();

            if (Extensions.IsDesignMode(this))
            {
                if (!stupidDesigner)
                {
                    //We need to rebind tabpages. If not, TabControl will look weird.
                    //This happens even with an empty class that inherit from TabControl.
                    this.SelectedIndex = this.SelectedIndex;
                    List<TabPage> pages = new List<TabPage>();
                    foreach (Control c in Controls)
                        pages.Add((TabPage)c);
                    Controls.Clear();
                    TabPages.Clear();
                    foreach (TabPage p in pages)
                        TabPages.Add(p);
                    stupidDesigner = true;
                }
            }
        }

        private void CheckIfResizeIsNeeded()
        {
            if (oldTabCount != TabPages.Count)
            {
                ResizeTabs();
                ApplyTabImages();
                oldTabCount = TabPages.Count;
            }
        }


        /// <summary>
        /// Raises the HandleCreated event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            ApplyStyles();
            SetImageList();
        }

        private void ResizeTabs()
        {
            if (!Extensions.IsDesignMode(this))
            {
                try
                {
                    if (scaleFactor.Height > 1)
                    {
                        int baseHeight = 20;
                        int newHeight = Convert.ToInt32(baseHeight * scaleFactor.Height);

                        //get the native C++ tab control.
                        IntPtr window = NativeMethods.GetWindow(this.Handle, NativeMethods.GW.CHILD);
                        int num = NativeMethods.SendMessage(window, NativeMethods.WMSG.TCM_SETITEMSIZE, 0, NativeMethods.MakeLParam(new Point(0, newHeight)));

                        int offset = 11;

                        foreach (TabPage t in this.TabPages)
                        {
                            t.Text = string.Format("  {0}  ", t.Text);
                            NativeMethods.SetWindowPos(t.Handle, IntPtr.Zero, 0, 0, t.Width, t.Height - (newHeight - baseHeight) + offset, NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOREPOSITION | NativeMethods.SWP.SWP_NOZORDER);
                        }
                    }
                }
                catch (Win32Exception)
                { }

            }
        }

   
        private void ApplyStyles()
        {
            if (!Extensions.IsDesignMode(this))
            {
                try
                {
                    //get the native C++ tab control.
                    IntPtr window = NativeMethods.GetWindow(this.Handle, NativeMethods.GW.CHILD);
                    //get current tabstrip style
                    int num = NativeMethods.GetWindowLong(window, NativeMethods.GWL.STYLE).ToInt32();
                    //add winmo 6.5 style
                    num = NativeMethods.SetWindowLong(window, NativeMethods.GWL.STYLE, (int)(num | 0x4000));

                }
                catch (Win32Exception)
                { }
            }
        }


        //private ImageList imageList;
        //public ImageList ImageList
        //{
        //    get
        //    {
        //        return imageList;
        //    }
        //    set
        //    {
        //        imageList = value;
        //        SetImageList();
        //        ApplyTabImages();
        //    }
        //}


       private void SetImageList()
       {
//            /*
//lResult = SendMessage(    // returns LRESULT in lResult
//   hWndControl,           // (HWND) handle to destination control
//   TCM_SETIMAGELIST,      // (UINT) message ID
//   wParam,                // = 0; not used, must be zero 
//   lParam                 // = (LPARAM)(HIMAGELIST) himl;
//);
//             */
//            if (!Extensions.IsDesignMode(this))
//            {
//                IntPtr imHandle = IntPtr.Zero;
//                if (imageList != null)
//                {
//                    //since .net does not expose imagelist handle, we use brute force.
//                    System.Reflection.FieldInfo m_himl = imageList.GetType().GetField("m_himl", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
//                    imHandle = (IntPtr)m_himl.GetValue(imageList);
//                }


//                IntPtr window = NativeMethods.GetWindow(this.Handle, NativeMethods.GW.CHILD);

//                int num = NativeMethods.SendMessage(
//                    window,
//                    NativeMethods.WMSG.TCM_SETIMAGELIST,
//                    0,
//                    imHandle
//                );
//            }
        }


        public void ApplyTabImages()
        {
//            if (!Extensions.IsDesignMode(this))
//            {
//                IntPtr window = NativeMethods.GetWindow(this.Handle, NativeMethods.GW.CHILD);
//                foreach (TabPage page in TabPages)
//                {
//                    int tabIndex = TabPages.IndexOf(page);
//                    int imageIndex = (imageList == null ? -1 : (tabIndex < imageList.Images.Count ? tabIndex : -1));

//                    IntPtr pItem = IntPtr.Zero;
//                    NativeMethods.TCITEM item = new NativeMethods.TCITEM();

//                    try
//                    {
//                        item.mask = 0;
//                        item.pszText = IntPtr.Zero;
//                        item.cchTextMax = 0;

//                        item.mask |= NativeMethods.TCIF_IMAGE;

//                        item.iImage = imageIndex;
//                        item.lParam = 0;

//                        pItem = Marshal.AllocHGlobal(Marshal.SizeOf(item));
//                        Marshal.StructureToPtr(item, pItem, false);

//                        int lResult = NativeMethods.SendMessage(window, NativeMethods.WMSG.TCM_SETITEM, tabIndex, pItem);
//                    }
//                    finally
//                    {
//                        if (pItem != null)
//                            Marshal.FreeHGlobal(pItem);
//                    }
//                }
//            }
        }
    }
}

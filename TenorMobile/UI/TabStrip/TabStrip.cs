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
        public TabStrip() : base()
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

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CheckIfResizeIsNeeded();
        }

        private void CheckIfResizeIsNeeded()
        {
            if (oldTabCount != TabPages.Count)
            {
                ResizeTabs();
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
    }
}

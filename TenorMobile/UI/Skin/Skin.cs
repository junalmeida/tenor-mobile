﻿using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Tenor.Mobile.UI
{
    public abstract class Skin
    {
       
        private static Skin current;
        /// <summary>
        /// Keeps the current skin for Tenor Mobile controls.
        /// </summary>
        public static Skin Current
        {
            get
            {
                if (current == null)
                    current = CreateSkin();
                return current;
            }
            set { current = value; }
        }

        private static Skin CreateSkin()
        {
            string oem = (Device.Device.Manufacturer + " " + Device.Device.OemInfo);
            Skin skin = null;
            if (oem.ToLower().IndexOf("samsung") > -1)
                skin = new Samsung();
            else
            {
                //todo: change to generic skin
                skin = new Samsung();
            }

            using (ContainerControl control = new ContainerControl())
            {
                SizeF qvga = new SizeF(96F, 96F);
                control.AutoScaleDimensions = qvga;
                control.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;


                skin.ScaleFactor = new Size(Convert.ToInt32(control.CurrentAutoScaleDimensions.Width / qvga.Width), Convert.ToInt32(control.CurrentAutoScaleDimensions.Height / qvga.Height));
            }
            return skin;

        }

        public Size ScaleFactor { get; private set; }

        internal abstract void DrawHeaderBackGround(HeaderStrip control, PaintEventArgs eventArgs);
        internal abstract void DrawHeaderText(HeaderStrip control, PaintEventArgs e);
        internal abstract void DrawTabs(HeaderStrip control, PaintEventArgs e);
        public abstract void ApplyColorsToControl(Control control);

        protected Form FindForm(Control control)
        {
            if (control == null)
                return null;
            else
            {
                Form form = control as Form;
                if (form == null)
                    return FindForm(control.Parent);
                else
                    return form;
            }
        }

        internal abstract void DrawListItemBackground(Graphics g, Rectangle bounds, int index, bool selected);

        internal abstract Color ControlBackColor
        { get; }
        
    }
}

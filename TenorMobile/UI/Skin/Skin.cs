using System;

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
            Skin skin = null;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                string oem = (Device.Device.Manufacturer + " " + Device.Device.OemInfo);
                if (oem.ToLower().IndexOf("samsung") > -1)
                    skin = new Samsung();
                else
                {
                    //todo: change to generic skin
                    skin = new Samsung();
                }

            }
            else
            {
                //For test purposes
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
        internal abstract void DrawTextControlBackground(TextControl control, Graphics g, Rectangle bounds);

        public abstract Color ControlBackColor
        { get; }

        public abstract Color SelectedBackColor
        { get; }

        public abstract Color AlternateBackColor
        { get; }

        public abstract Color TextHighLight
        { get; }

        public abstract Color TextForeColor
        { get; }


        public abstract Color TextBackGround
        { get; }



    }
}

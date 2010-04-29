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
            Samsung s = new Samsung();
            s.ScaleFactor = new Size(2, 2);
            return s;
        }

        protected Size ScaleFactor { get; private set; }

        internal abstract void DrawHeaderBackGround(Size controlSize, PaintEventArgs eventArgs);
        internal abstract void DrawHeaderText(string text, Size controlSize, PaintEventArgs e);
        internal abstract void DrawTabs(IList<HeaderTab> tabs, Size size, PaintEventArgs e);
    }
}

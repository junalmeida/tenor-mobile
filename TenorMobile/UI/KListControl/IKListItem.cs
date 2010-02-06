using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tenor.Mobile.UI
{
    /// <summary>
    /// Represents a single item on the KListControl
    /// </summary>
    public sealed class KListItem : object
    {
        internal KListItem()
        {
        }

        #region IKListItem Members

        public KListControl Parent { get; internal set; }

        public Rectangle Bounds { get; internal set; }

        public int XIndex { get; internal set; }

        public int YIndex { get; internal set; }

        public bool Selected { get; internal set; }

        public string Text { get; set; }

        public object Value { get; set; }

        internal void Render(Graphics g, Rectangle bounds)
        {
            if (Parent != null)
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                SolidBrush textBrush;
                if (Selected)
                {
                    SolidBrush backBrush;
                    backBrush = new SolidBrush(SystemColors.Highlight);
                    textBrush = new SolidBrush(SystemColors.HighlightText);
                    g.FillRectangle(backBrush, bounds);
                }
                else
                {
                    //backBrush = new SolidBrush(SystemColors.Window);
                    textBrush = new SolidBrush(SystemColors.ControlText);
                }

                g.DrawString("  " + Text, Parent.Font, textBrush, bounds, format);
            }
        }

        #endregion
    }

    public delegate void DrawItemEventHandler(object sender, DrawItemEventArgs e);

    /// <summary>
    /// The arguments necessary to draw an item.
    /// </summary>
    public sealed class DrawItemEventArgs : EventArgs
    {
        public Graphics Graphics { get; internal set; }
        public KListItem Item { get; internal set; }
        public Rectangle Bounds { get; internal set; }
    }
}

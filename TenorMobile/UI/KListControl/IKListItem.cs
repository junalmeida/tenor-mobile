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

        /// <summary>
        /// Gets the KListControl instance.
        /// </summary>
        public KListControl Parent { get; internal set; }

        /// <summary>
        /// Gets the bounds of this item.
        /// </summary>
        public Rectangle Bounds { get; internal set; }

        /// <summary>
        /// Gets the horizontal index of this item.
        /// </summary>
        public int XIndex { get; internal set; }

        /// <summary>
        /// Gets the vertical index of this item.
        /// </summary>
        public int YIndex { get; internal set; }

        private int width = 0;
        private int height = 0;
        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }

        private bool selected;
        /// <summary>
        /// Indicates whether this item is selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            internal set
            {
                selected = value;
            }
        }

        /// <summary>
        /// Gets the text of this item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the value associated.
        /// </summary>
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
                    if (!Parent.Skinnable)
                    {
                        SolidBrush backBrush;
                        backBrush = new SolidBrush(SystemColors.Highlight);
                        g.FillRectangle(backBrush, bounds);
                        backBrush.Dispose();

                        textBrush = new SolidBrush(SystemColors.HighlightText);
                    }
                    else
                        textBrush = new SolidBrush(Tenor.Mobile.UI.Skin.Current.TextHighLight);
        
                }
                else
                {
                    //backBrush = new SolidBrush(SystemColors.Window);
                    if (!Parent.Skinnable)
                        textBrush = new SolidBrush(SystemColors.ControlText);
                    else
                        textBrush = new SolidBrush(Tenor.Mobile.UI.Skin.Current.TextForeColor);
                }

                g.DrawString("  " + Text, Parent.Font, textBrush, bounds, format);

                textBrush.Dispose();
                format.Dispose();
            }
        }


  

        #endregion
    }

    /// <summary>
    /// The delegate that represents the DrawItem event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DrawItemEventHandler(object sender, DrawItemEventArgs e);

    /// <summary>
    /// The arguments necessary to draw an item.
    /// </summary>
    public sealed class DrawItemEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics.
        /// </summary>
        public Graphics Graphics { get; internal set; }
        /// <summary>
        /// Gets the item.
        /// </summary>
        public KListItem Item { get; internal set; }
        /// <summary>
        /// Gets the Bounds.
        /// </summary>
        public Rectangle Bounds { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tenor.Mobile.UI
{

    /// <summary>
    /// Interface for items contained within the list.
    /// </summary>
    public interface IKListItem
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        KListControl Parent { get; set; }

        /// <summary>
        /// The unscrolled bounds for this item.
        /// </summary>
        Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        int XIndex { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        int YIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IKListItem"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Renders the specified graphics object.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        void Render(Graphics g, Rectangle bounds);
    }
}

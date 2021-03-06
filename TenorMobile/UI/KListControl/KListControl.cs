﻿using System;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace Tenor.Mobile.UI
{
    /// <summary>
    /// A Kinetic list control.
    /// </summary>
    public class KListControl : Control, IEnumerable
    {


        public bool Skinnable
        { get; set; }

        /// <summary>
        /// Controls whether to draw separators between list items.
        /// </summary>
        public bool DrawSeparators
        { get; set; }


        /// <summary>
        /// Controls whether to draw the scrollbar.
        /// </summary>
        public bool DrawScrollbar
        { get; set; }

        /// <summary>
        /// Gets or sets the color of the separators.
        /// </summary>
        public Color SeparatorColor
        { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KListControl"/> class.
        /// </summary>
        public KListControl()
        {
            this.ParentChanged += new EventHandler(KListControl_Initialize);
            m_timer.Interval = 10;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            SeparatorColor = SystemColors.InactiveBorder;
            DrawSeparators = false;
            DrawScrollbar = true;
            Skinnable = true;

        }

        void KListControl_Initialize(object sender, EventArgs e)
        {
            CreateBackBuffer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Extensions.IsDesignMode(this))
            {
                this.Clear();
                for (int i = 1; i <= 10; i++)
                    AddItem("Item " + i.ToString(), i);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"></see> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            CleanupBackBuffer();

            m_timer.Enabled = false;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Occurs when the item needs to be rendered.
        /// </summary>
        public event DrawItemEventHandler DrawItem;



        /// <summary>
        /// Occurs when the selected item changes.
        /// </summary>
        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, e);
        }

        /// <summary>
        /// Occurs when the selected item is clicked on (after already being selected).
        /// </summary>
        public event EventHandler SelectedItemClicked;
        protected virtual void OnSelectedItemClicked(EventArgs e)
        {
            if (SelectedItemClicked != null)
                SelectedItemClicked(this, e);
        }
        /// <summary>
        /// Gets the <see cref="KListItem"/> at the specified index.
        /// </summary>
        public KListItem this[int index]
        {
            get
            {
                return m_items[0][index];
            }
        }

        /// <summary>
        /// Gets the <see cref="KListItem"/> at the specified index.
        /// </summary>
        public KListItem this[int x, int y]
        {
            get
            {
                return m_items[x][y];
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public KListItem SelectedItem
        {
            get
            {
                return m_selectedItem;
            }
            set
            {
                if (value != null)
                {
                    SelectItem(value.XIndex, value.YIndex);
                }
                else
                {
                    if (m_selectedItem != null)
                    {
                        m_selectedItem.Selected = false;
                    } 
                    m_selectedItem = null;
                    m_selectedIndex = new Point(-1, -1);
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Selects an item.
        /// </summary>
        private void SelectItem(int xIndex, int yIndex)
        {
            Point selectedIndex = new Point(xIndex, yIndex);
            if (selectedIndex != m_selectedIndex)
            {
                KListItem item = null;
                if (m_items.Count > selectedIndex.X &&
                    m_items[selectedIndex.X].Count > selectedIndex.Y)
                {
                    item = m_items[selectedIndex.X][selectedIndex.Y];

                    if (m_selectedItem != null)
                    {
                        m_selectedItem.Selected = false;
                    }
                    m_selectedIndex = selectedIndex;
                    m_selectedItem = item;
                    m_selectedItem.Selected = true;

                    OnSelectedItemChanged(new EventArgs());
                }
            }

            m_velocity.X = 0;
            m_velocity.Y = 0;
        }


        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                if (m_items.Count == 0)
                    return 0;
                else
                    return m_items[0].Count;
            }
        }

        /// <summary>
        /// Gets or sets the maximum scroll velocity.
        /// </summary>
        /// <value>The maximum velocity.</value>
        public int MaxVelocity
        {
            get
            {
                return m_maxVelocity;
            }
            set
            {
                m_maxVelocity = value;
            }
        }

        private int itemHeight = 38;
        /// <summary>
        /// Gets or sets the height of items in the control.
        /// </summary>
        /// <value>The height of the items.</value>
        public int DefaultItemHeight
        {
            get
            {
                return itemHeight;
            }
            set
            {
                itemHeight = value;
                Reset();
            }
        }


        private int itemWidth = 80;
        /// <summary>
        /// Gets or sets the height of items in the control.
        /// </summary>
        /// <value>The height of the items.</value>
        public int DefaultItemWidth
        {
            get
            {
                return itemWidth;
            }
            set
            {
                itemWidth = value;
                Reset();
            }
        }

        //private Size GetItemSize()
        //{
        //    int width = this.Width;
        //    int height = this.Height;

        //    if (Layout == KListLayout.Vertical || Layout == KListLayout.Grid)
        //    {
        //        // In vertical mode, we just use the full bounds, other modes use m_itemWidth.
        //        height = Convert.ToInt32(ItemHeight * scaleFactor.Height);
        //    }

        //    if (Layout == KListLayout.Horizontal || Layout == KListLayout.Grid)
        //    {
        //        // In horizontal mode, we just use the full bounds, other modes use m_itemHeight.
        //        width = Convert.ToInt32(ItemWidth * scaleFactor.Width);
        //    }

        //    return new Size(width, height);
        //}


        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        public KListLayout Layout
        {
            get
            {
                return m_layout;
            }
            set
            {
                m_layout = value;
            }
        }




        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="value">A value related to the item.</param>
        public void AddItem(string text, object value)
        {
            AddItem(text, value, 0);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="value">A value related to the item.</param>
        public void AddItem(string text, object value, int size)
        {
            if (m_layout == KListLayout.Grid)
            {
                throw new NotSupportedException("List is not in grid mode");
            }

            KListItem item = new KListItem();
            item.Text = text;
            item.Value = value;
            item.XIndex = m_layout == KListLayout.Vertical ? 0 : m_items.Count;
            item.YIndex = m_layout == KListLayout.Horizontal ? 0 :
                m_items.Count > 0 ? m_items[0].Count : 0;

            if (m_layout == KListLayout.Vertical || m_layout == KListLayout.Grid)
                item.Height = size;
            if (m_layout == KListLayout.Horizontal || m_layout == KListLayout.Grid)
                item.Width = size;



            AddItem(item);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="text">The text for the item.</param>
        /// <param name="value">A value related to the item.</param>
        private void AddItem(int x, int y, string text, object value)
        {
            if (m_layout != KListLayout.Grid)
            {
                throw new NotSupportedException("List is in grid mode");
            }

            KListItem item = new KListItem();
            item.Text = text;
            item.Value = value;
            item.XIndex = x;
            item.YIndex = y;
            AddItem(item);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void AddItem(KListItem item)
        {
            item.Parent = this;
            item.Selected = false;
            if (m_items.Count <= item.XIndex)
            {
                m_items.Add(new List<KListItem>());
            }
            m_items[item.XIndex].Add(item);
            item.Bounds = ItemBounds(item.XIndex, item.YIndex);
            Reset();
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RemoveItem(KListItem item)
        {
            if (m_items.Count > item.XIndex &&
                m_items[item.XIndex].Count > item.YIndex)
            {
                m_items[item.XIndex].RemoveAt(item.YIndex);
            }
            Reset();
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            m_items.Clear();
            m_selectedItem = null;
            m_selectedIndex = new Point(-1, -1);
            Reset();
        }

        /// <summary>
        /// Invalidates the item (when visible).
        /// </summary>
        /// <param name="item">The item.</param>
        public void Invalidate(KListItem item)
        {
            Rectangle itemBounds = item.Bounds;
            itemBounds.Offset(-m_offset.X, -m_offset.Y);
            if (new Rectangle(0,0, this.Width, this.Height).IntersectsWith(itemBounds))
            {
                Invalidate(itemBounds);
            }
        }

        /// <summary>
        /// Begins updates - suspending layout recalculation.
        /// </summary>
        public void BeginUpdate()
        {
            m_updating = true;
        }

        /// <summary>
        /// Ends updates - re-enabling layout recalculation.
        /// </summary>
        public void EndUpdate()
        {
            m_updating = false;
            Reset();
        }

        /// <summary>
        /// Called when the control is resized.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CreateBackBuffer();
            Reset();
        }

        /// <summary>
        /// Handles the Tick event of the m_timer control.
        /// </summary>
        private void m_timer_Tick(object sender, EventArgs e)
        {
            if (!Capture && (m_velocity.Y != 0 || m_velocity.X != 0))
            {
                m_offset.Offset(m_velocity.X, m_velocity.Y);

                ClipScrollPosition();

                // Slow down
                if (((++m_timerCount) % 10) == 0)
                {
                    if (m_velocity.Y < 0)
                    {
                        m_velocity.Y++;
                    }
                    else if (m_velocity.Y > 0)
                    {
                        m_velocity.Y--;
                    }
                    if (m_velocity.X < 0)
                    {
                        m_velocity.X++;
                    }
                    else if (m_velocity.X > 0)
                    {
                        m_velocity.X--;
                    }

                    if (m_velocity.Y != 0)
                    {

                        const double MaxPeriod = 1000;
                        double speed = (double)Math.Abs(m_velocity.Y);

                        double period = (1D - (speed / MaxVelocity)) * MaxPeriod;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(string.Format("{0}, {1}", speed, period));
#endif
                    }

                }

                if (m_velocity.Y == 0 && m_velocity.X == 0)
                {
                    m_timer.Enabled = false;
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing
        }

        KListItem lastItemPaint;
        /// <summary>
        /// Paints the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_backBuffer != null)
            {
                if (Skinnable && !Extensions.IsDesignMode(this))
                    m_backBuffer.Clear(Skin.Current.ControlBackColor);
                else
                    m_backBuffer.Clear(BackColor);

                
                base.OnPaint(new PaintEventArgs(m_backBuffer, e.ClipRectangle));

                KListItem startItem = FindItem(0, 0);
                if (lastItemPaint == null)
                    lastItemPaint = startItem;
                else if (lastItemPaint != startItem)
                {
                    Tenor.Mobile.Device.Device.HapticSoft();
                    lastItemPaint = startItem;
                }

                List<List<KListItem>>.Enumerator xEnumerator = m_items.GetEnumerator();
                bool moreX = xEnumerator.MoveNext();
                int i = 0;
                while (moreX && i < startItem.XIndex)
                {
                    moreX = xEnumerator.MoveNext();
                    i++;
                }

                while (moreX)
                {
                    List<KListItem> yList = xEnumerator.Current;
                    if (yList != null)
                    {
                        List<KListItem>.Enumerator yEnumerator = yList.GetEnumerator();
                        bool moreY = yEnumerator.MoveNext();
                        int j = 0;
                        while (moreY && j < startItem.YIndex)
                        {
                            moreY = yEnumerator.MoveNext();
                            j++;
                        }

                        while (moreY)
                        {
                            KListItem item = yEnumerator.Current;
                            if (item != null)
                            {
                                Rectangle itemRect = item.Bounds;
                                itemRect.Offset(-m_offset.X, -m_offset.Y);
                                if (new Rectangle(0, 0, this.Width, this.Height).IntersectsWith(itemRect))
                                {
                                    if (this.DrawSeparators)
                                    {

                                        using (SolidBrush whitePen = new SolidBrush(SeparatorColor))
                                        {
                                            if (m_layout == KListLayout.Vertical || m_layout == KListLayout.Grid)
                                            {
                                                int borderSize = Convert.ToInt32(1 * scaleFactor.Height);
                                                //m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Top, itemRect.Right, itemRect.Top);
                                                Rectangle rect = new Rectangle(itemRect.Left, itemRect.Bottom - borderSize, itemRect.Width, borderSize);
                                                m_backBuffer.FillRectangle(whitePen, rect);
                                                //m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Bottom - 1, itemRect.Right, itemRect.Bottom - 1);
                                            }
                                            if (m_layout == KListLayout.Horizontal || m_layout == KListLayout.Grid)
                                            {
                                                int borderSize = Convert.ToInt32(1 * scaleFactor.Width);
                                                //m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Top, itemRect.Left, itemRect.Bottom);
                                                Rectangle rect = new Rectangle(itemRect.Right - borderSize, itemRect.Top, borderSize, itemRect.Height);
                                                m_backBuffer.FillRectangle(whitePen, rect);
                                                //m_backBuffer.DrawLine(whitePen, itemRect.Right - 1, itemRect.Top, itemRect.Right - 1, itemRect.Bottom);
                                            }
                                        }
                                    }

                                    if (Skinnable && m_layout == KListLayout.Vertical && !Extensions.IsDesignMode(this))
                                        Skin.Current.DrawListItemBackground(m_backBuffer, itemRect, item.YIndex, item.Selected);

                                    if (DrawItem == null)
                                        item.Render(m_backBuffer, itemRect);
                                    else
                                    {
                                        DrawItemEventArgs drawArgs = new DrawItemEventArgs() { Graphics = m_backBuffer, Item = item, Bounds = itemRect };
                                        OnDrawItem(drawArgs);                                        
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            moreY = yEnumerator.MoveNext();
                        }
                    }

                    moreX = xEnumerator.MoveNext();

                    if (DrawScrollbar)
                    {
                        int scrollSize = 3;

                        if ((m_layout == KListLayout.Vertical || m_layout == KListLayout.Grid) && MaxYOffset > 0)
                        {

                            int scrollHeight = (this.Height * 100) / (MaxYOffset + this.Height); // percentage

                            if (scrollHeight < 100)
                            {
                                int scrollWidth = Convert.ToInt32(scrollSize * scaleFactor.Width);
                                scrollHeight = (this.Height * scrollHeight) / 100;

                                int scrollTop = (100 * m_offset.Y) / MaxYOffset;
                                scrollTop = ((this.Height - scrollHeight) * scrollTop) / 100;


                                Rectangle scroll = new Rectangle(this.Width - (scrollWidth * 2), scrollTop, scrollWidth, scrollHeight);
                                SolidBrush brush = new SolidBrush(SystemColors.ScrollBar);
                                m_backBuffer.FillRectangle(brush, scroll);
                            }
                        }
                        if ((m_layout == KListLayout.Horizontal || m_layout == KListLayout.Grid) && MaxXOffset > 0)
                        {
                            int scrollWidth = (this.Width * 100) / (MaxXOffset + this.Width); // percentage

                            if (scrollWidth < 100)
                            {
                                int scrollHeight = Convert.ToInt32(scrollSize * scaleFactor.Height);

                                scrollWidth = (this.Width * scrollWidth) / 100;

                                int scrollLeft = (100 * m_offset.X) / MaxXOffset;
                                scrollLeft = ((this.Width - scrollWidth) * scrollLeft) / 100;


                                Rectangle scroll = new Rectangle(scrollLeft, this.Height - (scrollHeight * 2), scrollWidth, scrollHeight);
                                SolidBrush brush = new SolidBrush(SystemColors.ScrollBar);
                                m_backBuffer.FillRectangle(brush, scroll);
                            }
                        }
                    }
                }


                e.Graphics.DrawImage(m_backBufferBitmap, 0, 0);

            }
            else
            {
                base.OnPaint(e);
            }
        }

        /// <summary>
        /// Called when the user clicks on the control with the mouse.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Tenor.Mobile.Device.Device.HapticSoft();
            Capture = true;

            m_mouseDown.X = e.X;
            m_mouseDown.Y = e.Y;
            m_mousePrev = m_mouseDown;
        }

        /// <summary>
        /// Called when the user moves the mouse over the control.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                Point currPos = new Point(e.X, e.Y);

                int distanceX = m_mousePrev.X - currPos.X;
                int distanceY = m_mousePrev.Y - currPos.Y;

                m_velocity.X = distanceX / 2;
                m_velocity.Y = distanceY / 2;
                ClipVelocity();

                m_offset.Offset(distanceX, distanceY);
                ClipScrollPosition();

                m_mousePrev = currPos;

                Invalidate();
            }
        }


        /// <summary>
        /// Called when the user releases a mouse button.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            KListItem item = FindItem(m_mouseDown.X, m_mouseDown.Y);
            if (item != null)
            {
                Size itemSize = item.Bounds.Size;
                // Did the click end on the same item it started on?
                bool sameX = Math.Abs(e.X - m_mouseDown.X) < itemSize.Width;
                bool sameY = Math.Abs(e.Y - m_mouseDown.Y) < itemSize.Height;

                if ((m_layout == KListLayout.Vertical && sameY) ||
                    (m_layout == KListLayout.Horizontal && sameX) ||
                    (m_layout == KListLayout.Grid && sameX && sameY))
                {
                    // Yes, so select that item.
                    //Point selectedIndex = FindIndex(e.X, e.Y);
                    //SelectItem(selectedIndex.X, selectedIndex.Y);
                    if (item == m_selectedItem)
                        OnSelectedItemClicked(new EventArgs());
                    SelectItem(item.XIndex, item.YIndex);
                }
                else
                {
                    m_timer.Enabled = true;
                }

                m_mouseDown.Y = -1;
                Capture = false;

                Invalidate();
            }
        }



        /// <summary>
        /// Resets the drawing of the list.
        /// </summary>
        private void Reset()
        {
            if (!m_updating)
            {
                m_timer.Enabled = false;
                /*
                if (m_selectedItem != null)
                {
                    m_selectedItem.Selected = false;
                    m_selectedItem = null;
                }
                m_selectedIndex = new Point(-1, -1);
                 */
                Capture = false;
                m_velocity.X = 0;
                m_velocity.Y = 0;


                int xIndex = 0, yIndex = 0;

                foreach (var i in m_items)
                {
                    foreach (var j in i)
                    {
                        j.XIndex = xIndex; j.YIndex = yIndex;
                        j.Bounds = ItemBounds(j.XIndex, j.YIndex);

                        yIndex++;
                    }
                    xIndex++;
                }

                EnsureVisible();


                Invalidate();
            }
        }

        /// <summary>
        /// Ensures that the selected item is visible. If there's no item selected, it moves the scroll to top.
        /// </summary>
        public void EnsureVisible()
        {
            EnsureVisible(m_selectedItem);
        }

        /// <summary>
        /// Ensures that the item is visible.
        /// </summary>
        /// <param name="item">A KListItem to make visible.</param>
        public void EnsureVisible(KListItem item)
        {
            if (item != null)
            {
                if ((-m_offset.X) + item.Bounds.Right > this.Width)
                    m_offset.X = item.Bounds.X - (this.Width - item.Bounds.Width);
                if ((-m_offset.X) + item.Bounds.X < 0)
                    m_offset.X = item.Bounds.X;

                if ((-m_offset.Y) + item.Bounds.Bottom > this.Height)
                    m_offset.Y = item.Bounds.Y - (this.Height - item.Bounds.Height);
                if ((-m_offset.Y) + item.Bounds.Y < 0)
                    m_offset.Y = item.Bounds.Y;

            }
            else
            {
                //do we need to reset offset?
                //m_offset.X = 0;
                //m_offset.Y = 0;
            }
            if (m_offset.X > MaxXOffset)
                m_offset.X = MaxXOffset;
            if (m_offset.Y > MaxYOffset)
                m_offset.Y = MaxYOffset;
        }


        /// <summary>
        /// Cleans up the background paint buffer.
        /// </summary>
        private void CleanupBackBuffer()
        {
            if (m_backBufferBitmap != null)
            {
                m_backBufferBitmap.Dispose();
                m_backBufferBitmap = null;
                m_backBuffer.Dispose();
                m_backBuffer = null;
            }
        }

        /// <summary>
        /// Creates the background paint buffer.
        /// </summary>
        private void CreateBackBuffer()
        {
            CleanupBackBuffer();
            if (this.Width > 0 && this.Height > 0)
            {
                m_backBufferBitmap = new Bitmap(this.Width, this.Height);
                m_backBuffer = Graphics.FromImage(m_backBufferBitmap);
            }
        }

        /// <summary>
        /// Clips the scroll position.
        /// </summary>
        private void ClipScrollPosition()
        {
            if (m_offset.X < 0)
            {
                m_offset.X = 0;
                m_velocity.X = 0;
            }
            else if (m_offset.X > MaxXOffset)
            {
                m_offset.X = MaxXOffset;
                m_velocity.X = 0;
            }
            if (m_offset.Y < 0)
            {
                m_offset.Y = 0;
                m_velocity.Y = 0;
                Tenor.Mobile.Device.Device.HapticBounce();
            }
            else if (m_offset.Y > MaxYOffset)
            {
                m_offset.Y = MaxYOffset;
                m_velocity.Y = 0;
                Tenor.Mobile.Device.Device.HapticBounce();
            }
        }

        /// <summary>
        /// Clips the velocity.
        /// </summary>
        private void ClipVelocity()
        {
            m_velocity.X = Math.Min(m_velocity.X, m_maxVelocity);
            m_velocity.X = Math.Max(m_velocity.X, -m_maxVelocity);

            m_velocity.Y = Math.Min(m_velocity.Y, m_maxVelocity);
            m_velocity.Y = Math.Max(m_velocity.Y, -m_maxVelocity);
        }

        /// <summary>
        /// Finds the bounds for the specified item.
        /// </summary>
        /// <param name="xIndex">The item x index.</param>
        /// <param name="yIndex">The item y index.</param>
        /// <returns>The item bounds.</returns>
        private Rectangle ItemBounds(int xIndex, int yIndex)
        {
            KListItem item = this[xIndex, yIndex];
            int itemX = 0;//(itemSize.Width * xIndex);
            int itemY = 0;//(itemSize.Height * yIndex);

            if (m_layout == KListLayout.Vertical)
            {
                Size itemSize = new Size(this.Width, (item.Height > 0 ? item.Height : DefaultItemHeight));
                itemY = (yIndex > 0 ? this[xIndex, yIndex - 1].Bounds.Bottom : 0);
                return new Rectangle(0, itemY, this.Width, itemSize.Height);
            }
            else if (m_layout == KListLayout.Horizontal)
            {
                Size itemSize = new Size((item.Width > 0 ? item.Width : DefaultItemWidth), this.Height);
                itemX = (xIndex > 0 ? this[xIndex - 1, yIndex].Bounds.Right : 0);
                return new Rectangle(itemX, 0, itemSize.Width, this.Height);
            }
            else
            {
                Size itemSize = new Size((item.Width > 0 ? item.Width : DefaultItemWidth), (item.Height > 0 ? item.Height : DefaultItemHeight));
                itemX = (xIndex > 0 ? this[xIndex, yIndex].Bounds.Right : 0);
                itemY = (yIndex > 0 ? this[xIndex, yIndex].Bounds.Bottom : 0);
                return new Rectangle(itemX, itemY, itemSize.Width, itemSize.Height);
            }
        }

        ///// <summary>
        ///// Finds the index for the specified y offset.
        ///// </summary>
        ///// <param name="x">The x offset.</param>
        ///// <param name="y">The y offset.</param>
        ///// <returns></returns>
        //private Point FindIndex(int x, int y)
        //{
        //    for (int i = 0; i < m_items.Count; i++)
        //        for (int j = 0; i < m_items[i].Count; j++)
        //        {
        //            Rectangle itemBounds = m_items[i][j].Bounds;
        //            itemBounds.Offset(-m_offset.X, -m_offset.Y);
        //            if (itemBounds.Contains(x,y))
                       
        //        }


        //    Size itemSize = GetItemSize();
        //    Point index = new Point(0, 0);

        //    if (m_layout == KListLayout.Vertical)
        //    {
        //        index.Y = ((y + m_offset.Y) / (itemSize.Height));
        //    }
        //    else if (m_layout == KListLayout.Horizontal)
        //    {
        //        index.X = ((x + m_offset.X) / (itemSize.Width));
        //    }
        //    else
        //    {
        //        index.X = ((x + m_offset.X) / (itemSize.Width));
        //        index.Y = ((y + m_offset.Y) / (itemSize.Height));
        //    }

        //    return index;
        //}


        /// <summary>
        /// Finds the index for the specified y offset.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns></returns>
        private KListItem FindItem(int x, int y)
        {
            for (int i = 0; i < m_items.Count; i++)
                for (int j = 0; j < m_items[i].Count; j++)
                {
                    Rectangle itemBounds = m_items[i][j].Bounds;
                    itemBounds.Offset(-m_offset.X, -m_offset.Y);
                    if (itemBounds.Contains(x, y))
                        return m_items[i][j];
                }
            return null;
            //Point p = FindIndex(x, y);
            //return this[x, y];
        }

        /// <summary>
        /// Gets the maximum x offset.
        /// </summary>
        /// <value>The maximum x offset.</value>
        private int MaxXOffset
        {
            get
            {
                if (m_items.Count > 0)
                    return Math.Max(m_items[m_items.Count - 1][0].Bounds.Right - this.Width, 0);
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets the maximum y offset.
        /// </summary>
        /// <value>The maximum y offset.</value>
        private int MaxYOffset
        {
            get
            {
                if (m_items.Count > 0)
                    return Math.Max(m_items[0][m_items[0].Count - 1].Bounds.Bottom - this.Height, 0);
                else
                    return 0;
            }
        }

        List<List<KListItem>> m_items = new List<List<KListItem>>();

        // Properties
        int m_maxVelocity = 20;
        KListLayout m_layout = KListLayout.Vertical;
        bool m_updating = false;

        // Background drawing
        Bitmap m_backBufferBitmap;
        Graphics m_backBuffer;

        // Motion variables
        Point m_selectedIndex = new Point(-1, -1);
        KListItem m_selectedItem = null;
        Point m_velocity = new Point(0, 0);
        Point m_mouseDown = new Point(-1, -1);
        Point m_mousePrev = new Point(-1, -1);
        Timer m_timer = new Timer();
        int m_timerCount = 0;
        Point m_offset = new Point();


        

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_items[0].GetEnumerator();
        }


        SizeF scaleFactor = new SizeF(1, 1);

        /// <summary>
        /// Raises the ScaleControl event.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="specified"></param>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            scaleFactor = factor;
            itemHeight *= Convert.ToInt32(factor.Height);
            itemWidth *= Convert.ToInt32(factor.Width);
            base.ScaleControl(factor, specified);
        }

        /// <summary>
        /// Raises the DrawItem element for each item on this list.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            if (DrawItem != null)
                DrawItem(this, e);
        }

        /// <summary>
        /// Sorts the current list items.
        /// </summary>
        /// <param name="comparer"></param>
        public virtual void Sort(Comparison<KListItem> comparer)
        {
            if (Layout == KListLayout.Vertical)
            {
                if (m_items.Count > 0 && m_items[0].Count > 1)
                {
                    m_items[0].Sort(comparer);
                    Reset();
                }
            }
            else
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Layout Mode.
    /// </summary>
    public enum KListLayout
    {
        /// <summary>
        /// Vertically scrolling list.
        /// </summary>
        Vertical,

        /// <summary>
        /// Horizontally scrolling list.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Scrolling grid.
        /// </summary>
        Grid
    }
}

using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Tenor.Mobile.UI
{
    public class HeaderStrip : Control
    {
        public HeaderStrip()
        {
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Skin.Current.DrawHeaderBackGround(this, e);
            Skin.Current.DrawTabs(this, e);
            Skin.Current.DrawHeaderText(this, e);
        }

        /// <summary>
        /// Gets or sets the Text of this HeaderStrip.
        /// </summary>
        public string Text { get; set; }

        private TabsCollection tabs;
        public TabsCollection Tabs
        {
            get
            {
                if (tabs == null)
                    tabs = new TabsCollection(this);
                return tabs;
            }
        }

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (HeaderTab tab in Tabs)
                {
                    if (tab.area.Contains(new Point(e.X, e.Y)))
                    {
                        SelectTab(tab);
                        return;
                    }
                }
            }
            base.OnMouseUp(e);
        }
        
        private void SelectTab(HeaderTab tab)
        {
            if (tab.Selected == false)
            {
                tab.Selected = true;
                Tenor.Mobile.Device.Device.HapticSoft();
                OnSelectedTabChanged(new EventArgs());
            }
        }

        public event EventHandler SelectedTabChanged;
        private void OnSelectedTabChanged(EventArgs eventArgs)
        {
            if (SelectedTabChanged != null)
                SelectedTabChanged(this, eventArgs);
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            internal set
            {
                selectedIndex = value;
                Invalidate();
            }
        }

        #endregion
    }


    public class HeaderTab
    {
        public HeaderTab(string text, Image image)
        {
            this.Text = text;
            this.Image = image;
        }

        public string Text
        { get; set; }

        public int TabIndex { get { return (collection == null ? -1 : collection.IndexOf(this)); } }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;

                if (collection != null && value)
                {
                    foreach (HeaderTab tab in collection)
                    {
                        if (tab != this && tab.selected != false)
                        {
                            tab.selected = false;
                        }
                        else if (tab == this)
                        {
                            collection.Control.SelectedIndex = collection.IndexOf(tab);
                        }
                    }
                }
                Update();
            }
        }

        private Image image;
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                Update();
            }
        }




        private void Update()
        {
            if (collection != null)
                collection.Control.Invalidate();
        }

        internal TabsCollection collection = null;
        internal Rectangle area;
    }


    public class TabsCollection : System.Collections.ObjectModel.Collection<HeaderTab>
    {
        
        internal HeaderStrip Control { get; private set; }
        internal TabsCollection(HeaderStrip control)
        {
            this.Control = control;
        }

        protected override void InsertItem(int index, HeaderTab item)
        {
            base.InsertItem(index, item);
            item.collection = this;
            EnsureSelection();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            EnsureSelection();
        }

        private void EnsureSelection()
        {
            bool alreadySelected = false;
            foreach (HeaderTab tab in this)
            {
                if (tab.Selected && !alreadySelected)
                    alreadySelected = true;
                else if (tab.Selected)
                    tab.Selected = false;
            }
            if (!alreadySelected && this.Count > 0)
                this[0].Selected = true;
                
            Control.Invalidate();
        }

    }
}

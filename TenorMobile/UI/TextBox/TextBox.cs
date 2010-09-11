using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;
using System.Threading;

namespace Tenor.Mobile.UI
{
    public partial class TextControl : Control
    {

        TextBox text = null;
        InputPanel input = null;
        ContextMenu context = null;

        public TextControl()
        {
            text = new TextBox();
            text.ForeColor = Skin.Current.TextForeColor;
            text.BackColor = Skin.Current.TextBackGround;
            text.BorderStyle = BorderStyle.None;
            text.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(text);
            text.GotFocus += new EventHandler(text_GotFocus);
            text.LostFocus += new EventHandler(text_LostFocus);
            text.KeyDown += new KeyEventHandler(text_KeyDown);
            text.KeyPress += new KeyPressEventHandler(text_KeyPress);
            text.KeyUp += new KeyEventHandler(text_KeyUp);
            if (!Extensions.IsDesignMode(this))
            {
                input = new InputPanel();
            }

            context = new ContextMenu();
            context.Popup += new EventHandler(context_Popup);
            text.ContextMenu = context;
        }


        void text_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        void text_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        void text_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        public override ContextMenu ContextMenu
        {
            get
            {
                return base.ContextMenu;
            }
            set
            {
                base.ContextMenu = value;
                if (value == null)
                    text.ContextMenu = context;
                else
                    text.ContextMenu = value;
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.BackColor = Tenor.Mobile.UI.Skin.Current.ControlBackColor;
            base.OnParentChanged(e);
            ResetbackBuffer();
        }

        void context_Popup(object sender, EventArgs e)
        {
            foreach (MenuItem mnu in context.MenuItems)
                mnu.Dispose();
            context.MenuItems.Clear();
            if (text.PasswordChar != '\0')
                return;
            MenuItem menu = null;
            menu = new MenuItem()
            {
                Text = "Cut"
            };
            menu.Click += new EventHandler(menuCut_Click);
            context.MenuItems.Add(menu);
            menu.Enabled = (!text.ReadOnly && text.SelectionLength > 0);
            menu = new MenuItem()
            {
                Text = "Copy"
            };
            menu.Click += new EventHandler(menuCopy_Click);
            context.MenuItems.Add(menu);
            menu.Enabled = (text.SelectionLength > 0);
            menu = new MenuItem()
            {
                Text = "Paste"
            };
            menu.Click += new EventHandler(menuPaste_Click);
            context.MenuItems.Add(menu);
            menu.Enabled = (!text.ReadOnly && Clipboard.GetDataObject() != null);
        }

        void menuPaste_Click(object sender, EventArgs e)
        {
            string text = Clipboard.GetDataObject().GetData(System.Windows.Forms.DataFormats.Text) as string;
            if (!string.IsNullOrEmpty(text))
            {
                string current = this.Text;
                int pos = SelectionStart;
                this.Text = current.Substring(0, this.SelectionStart) + text + current.Substring(this.SelectionStart + this.SelectionLength);
                SelectionStart = pos;
                SelectionLength = text.Length;
            }
        }

        void menuCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(text.Text.Substring(text.SelectionStart, text.SelectionLength));
        }

        void menuCut_Click(object sender, EventArgs e)
        {
            menuCopy_Click(sender, e);
            int pos = SelectionStart;
            string current = this.Text;
            this.Text = current.Substring(0, this.SelectionStart) + current.Substring(this.SelectionStart + this.SelectionLength);
            SelectionStart = pos;
        }

        System.Threading.Timer timer = null;
        static bool focus = false;
        void text_GotFocus(object sender, EventArgs e)
        {
            if (input != null)
            {
                Microsoft.WindowsCE.Forms.InputModeEditor.SetInputMode(this.text, this.InputMode);
                input.Enabled = true;
                focus = true;
            }
        }

        void text_LostFocus(object sender, EventArgs e)
        {
            focus = false;
            if (timer != null)
            {
                timer.Dispose();
            }
            timer = new System.Threading.Timer(
                new System.Threading.TimerCallback(timer_Callback),
                null,
                1000, System.Threading.Timeout.Infinite);
        }

        void timer_Callback(object state)
        {
            if (this.InvokeRequired)
                this.Invoke(new ThreadStart(CloseInput));
            else
                CloseInput();
            
        }

        private void CloseInput()
        {
            timer.Dispose();
            if (!focus)
                input.Enabled = false;
        }

        public override bool Focused
        {
            get
            {
                return base.Focused || text.Focused;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        Bitmap backBufferBmp;
        Graphics backBuffer;

        private void ResetbackBuffer()
        {
            if (backBuffer != null)
                backBuffer.Dispose();
            if (backBufferBmp != null)
                backBufferBmp.Dispose();
            backBuffer = null;
            backBufferBmp = null;
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            if (backBuffer == null && this.Width > 0 && this.Height > 0)
            {
                backBufferBmp = new Bitmap(this.Width, this.Height);
                backBuffer = Graphics.FromImage(backBufferBmp);

                Tenor.Mobile.UI.Skin.Current.DrawTextControlBackground(this, backBuffer, new Rectangle(0, 0, backBufferBmp.Width, backBufferBmp.Height));
            }
            if (backBufferBmp != null)
                pe.Graphics.DrawImage(backBufferBmp, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (text != null)
            {
                text.Location = new Point(
                    Convert.ToInt32(5 * Tenor.Mobile.UI.Skin.Current.ScaleFactor.Width),
                    Convert.ToInt32(2 * Tenor.Mobile.UI.Skin.Current.ScaleFactor.Height));
                text.Size = new SizeF(
                    this.Width - 10 * Tenor.Mobile.UI.Skin.Current.ScaleFactor.Width,
                    this.Height - 4 * Tenor.Mobile.UI.Skin.Current.ScaleFactor.Height).ToSize();
                text.Top = (this.Height / 2) - (text.Height / 2);
            }
            ResetbackBuffer();
        }



        public InputMode InputMode
        {
            get;
            set;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            text.Focus();
        }



        #region TextBox Methods and Properties


        // Summary:
        //     Gets or sets a value indicating whether pressing ENTER in a multiline System.Windows.Forms.TextBox
        //     control creates a new line of text in the control or activates the default
        //     button for the form.
        //
        // Returns:
        //     true if the ENTER key creates a new line of text in a multiline version of
        //     the control; false if the ENTER key activates the default button for the
        //     form. The default is false.
        public bool AcceptsReturn { get { return text.AcceptsReturn; } set { text.AcceptsReturn = value; } }
        //
        // Summary:
        //     Gets or sets the character used to mask characters of a password in a single-line
        //     System.Windows.Forms.TextBox control.
        //
        // Returns:
        //     The character used to mask characters entered in a single-line System.Windows.Forms.TextBox
        //     control. Set the value of this property to 0 (character value) if you do
        //     not want the control to mask characters as they are typed. Equals 0 (character
        //     value) by default.
        public char PasswordChar { get { return text.PasswordChar; } set { text.PasswordChar = value; } }
        //
        // Summary:
        //     Gets or sets which scroll bars should appear in a multiline System.Windows.Forms.TextBox
        //     control.
        //
        // Returns:
        //     One of the System.Windows.Forms.ScrollBars enumeration values that indicates
        //     whether a multiline System.Windows.Forms.TextBox control appears with no
        //     scroll bars, a horizontal scroll bar, a vertical scroll bar, or both. The
        //     default is ScrollBars.None.
        //
        // Exceptions:
        //   System.ComponentModel.InvalidEnumArgumentException:
        //     A value that is not within the range of valid values for the enumeration
        //     was assigned to the property.
        public ScrollBars ScrollBars { get { return text.ScrollBars; } set { text.ScrollBars = value; } }
        //
        // Summary:
        //     Gets or sets how text is aligned in a System.Windows.Forms.TextBox control.
        //
        // Returns:
        //     One of the System.Windows.Forms.HorizontalAlignment enumeration values that
        //     specifies how text is aligned in the control. The default is HorizontalAlignment.Left.
        //
        // Exceptions:
        //   System.ComponentModel.InvalidEnumArgumentException:
        //     A value that is not within the range of valid values for the enumeration
        //     was assigned to the property.
        public HorizontalAlignment TextAlign { get { return text.TextAlign; } set { text.TextAlign = value; } }



        // Summary:
        //     Gets or sets a value indicating whether pressing the TAB key in a multiline
        //     text box control types a TAB character in the control instead of moving the
        //     focus to the next control in the tab order.
        //
        // Returns:
        //     true if users can enter tabs in a multiline text box using the TAB key; false
        //     if pressing the TAB key moves the focus. The default is false.
        public bool AcceptsTab { get { return text.AcceptsTab; } set { text.AcceptsTab = value; } }
        //
        // Summary:
        //     Gets a value indicating whether the user can undo the previous operation
        //     in a text box control.
        //
        // Returns:
        //     true if the user can undo the previous operation performed in a text box
        //     control; otherwise, false.
        public bool CanUndo { get { return text.CanUndo; } }
        //
        // Summary:
        //     Gets or sets a value indicating whether the selected text in the text box
        //     control remains highlighted when the control loses focus.
        //
        // Returns:
        //     true if the selected text does not appear highlighted when the text box control
        //     loses focus; false, if the selected text remains highlighted when the text
        //     box control loses focus. The default is true.
        public bool HideSelection { get { return text.HideSelection; } set { text.HideSelection = value; } }
        //
        // Summary:
        //     Gets or sets the maximum number of characters the user can type or paste
        //     into the text box control.
        //
        // Returns:
        //     The number of characters that can be entered into the control. The default
        //     is 32767.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The value assigned to the property is less than 0.
        public virtual int MaxLength { get { return text.MaxLength; } set { text.MaxLength = value; } }
        //
        // Summary:
        //     Gets or sets a value that indicates that the text box control has been modified
        //     by the user since the control was created or its contents were last set.
        //
        // Returns:
        //     true if the control's contents have been modified; otherwise, false. The
        //     default is false.
        public bool Modified { get { return text.Modified; } set { text.Modified = value; } }
        //
        // Summary:
        //     Gets or sets a value indicating whether this is a multiline text box control.
        //
        // Returns:
        //     true if the control is a multiline text box control; otherwise, false. The
        //     default is false.
        public virtual bool Multiline { get { return text.Multiline; } set { text.Multiline = value; } }
        //
        // Summary:
        //     Gets or sets a value indicating whether text in the text box is read-only.
        //
        // Returns:
        //     true if the text box is read-only; otherwise, false. The default is false.
        public bool ReadOnly { get { return text.ReadOnly; } set { text.ReadOnly = value; } }
        //
        // Summary:
        //     Gets or sets a value indicating the currently selected text in the control.
        //
        // Returns:
        //     A string that represents the currently selected text in the text box.
        public string SelectedText { get { return text.SelectedText; } set { text.SelectedText = value; } }
        //
        // Summary:
        //     Gets or sets the number of characters selected in the text box.
        //
        // Returns:
        //     The number of characters selected in the text box.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The assigned value is less than zero.
        public int SelectionLength { get { return text.SelectionLength; } set { text.SelectionLength = value; } }
        //
        // Summary:
        //     Gets or sets the starting point of text selected in the text box.
        //
        // Returns:
        //     The starting position of text selected in the text box.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The assigned value is less than zero.
        public int SelectionStart { get { return text.SelectionStart; } set { text.SelectionStart = value; } }
        //
        // Summary:
        //     Gets the length of text in the control.
        //
        // Returns:
        //     The number of characters contained in the text of the control.
        public int TextLength { get { return text.TextLength; } }
        //
        // Summary:
        //     Indicates whether a multiline text box control automatically wraps words
        //     to the beginning of the next line when necessary.
        //
        // Returns:
        //     true if the multiline text box control wraps words; false if the text box
        //     control automatically scrolls horizontally when the user types past the right
        //     edge of the control. The default is true.
        public bool WordWrap { get { return text.WordWrap; } set { text.WordWrap = value; } }

        // Summary:
        //     Scrolls the contents of the control to the current caret position.
        public void ScrollToCaret() { text.ScrollToCaret(); }
        //
        // Summary:
        //     Selects a range of text in the text box.
        //
        // Parameters:
        //   start:
        //     The position of the first character in the current text selection within
        //     the text box.
        //
        //   length:
        //     The number of characters to select.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The value of the start parameter is less than zero.
        public void Select(int start, int length) { text.Select(start, length); }
        //
        // Summary:
        //     Selects all text in the text box.
        public void SelectAll() { text.SelectAll(); }
        //
        // Summary:
        //     Undoes the last edit operation in the text box.
        public void Undo() { text.Undo(); }


        public override string Text
        {
            get
            {
                return text.Text;
            }
            set
            {
                text.Text = value;
            }
        }
        #endregion
    }
}

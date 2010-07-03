using System;

using System.Collections.Generic;
using System.Text;
using Tenor.Mobile.Drawing;
using System.Drawing;
using System.Windows.Forms;

namespace Tenor.Mobile.UI
{
    /// <summary>
    /// Render controls using Samsung's default interface style.
    /// </summary>
    public class Samsung : Skin
    {
        internal Samsung()
        {
        }

        private const string HeaderStartColor = "#394D6B";
        private const string HeaderEndColor = "#213442";
        private const string BorderLineColor = "#4A5963";

        private const string HeaderFontColor = "#FFFFFF";
        private const int HeaderFontSize = 13;
        private const int HeaderSelectedTabStrip = 14;
        private const int HeaderSelectedTabStripFontSize = 8;
        private const string HeaderSelectedTabStripFontColor = "#43CBF5";
        private const string HeaderSelectedTabStripBorderColor = "#4A5963";

        public override Color ControlBackColor
        {
            get { return Strings.ToColor("#000000"); }
        }

        public override Color TextForeColor
        {
            get { return Strings.ToColor("#FFFFFF"); }
        }

        public override Color TextHighLight
        {
            get { return Strings.ToColor("#FFFFFF"); }
        }

        internal override void DrawHeaderBackGround(HeaderStrip control, PaintEventArgs eventArgs)
        {
            Size controlSize = control.Size;
            if (control.Tabs.Count > 0)
            {
                controlSize.Height -= HeaderSelectedTabStrip * ScaleFactor.Height;
            }

            Rectangle rect = new Rectangle(eventArgs.ClipRectangle.X, 0, eventArgs.ClipRectangle.Width, controlSize.Height);
            Color start = Strings.ToColor(HeaderStartColor);
            Color end = Strings.ToColor(HeaderEndColor);
            GradientFill.Fill(eventArgs.Graphics, rect, start, end, GradientFill.FillDirection.TopToBottom);

            eventArgs.Graphics.DrawLine(new Pen(Strings.ToColor(BorderLineColor)), rect.X, rect.Height - 1, rect.Width, rect.Height - 1);
        }

        internal override void DrawHeaderText(HeaderStrip control, PaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(control.Text) && control.Tabs.Count == 0)
            {
                using (Font font = new Font(FontFamily.GenericSansSerif, HeaderFontSize, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Strings.ToColor(HeaderFontColor)))
                {
                    SizeF textSize = e.Graphics.MeasureString(control.Text, font);

                    e.Graphics.DrawString(control.Text, font, brush, 12 * ScaleFactor.Width, 7 * ScaleFactor.Height);
                }
            }

        }

        internal override void DrawTabs(HeaderStrip control, PaintEventArgs e)
        {
            if (control.Tabs.Count == 0)
                return;
            Size size = control.Size;
            size.Height -= HeaderSelectedTabStrip * ScaleFactor.Height;
            
            IList<HeaderTab> tabs = control.Tabs;
            int bottomHeight = 15 * ScaleFactor.Height;

            Point offset = new Point(2 * ScaleFactor.Width, 2 * ScaleFactor.Width);
            int defaultWidth = 46 * ScaleFactor.Width;
            Size corner = new Size(8 * ScaleFactor.Width, 8 * ScaleFactor.Height);

            Pen penBorder = new Pen(Strings.ToColor(BorderLineColor));
            try
            {
                HeaderTab selected = null;
                foreach (HeaderTab tab in tabs)
                {
                    tab.area = new Rectangle(offset.X + tab.TabIndex * defaultWidth, offset.Y, defaultWidth, size.Height - offset.Y + corner.Height);
                    if (tab.Selected)
                    {
                        selected = tab;
                        using (SolidBrush backColor = new SolidBrush(Color.Black))
                            RoundedRectangle.Fill(e.Graphics, penBorder, backColor, tab.area, corner);
                    }

                    if (tab.Image != null)
                    {
                        //draw icon
                        Rectangle destImg;
                        if (tab.Image.Size.Height > tab.area.Height || tab.Image.Size.Width > tab.area.Width)
                        {
                            int oX = 8 * ScaleFactor.Width;
                            int oY = 8 * ScaleFactor.Height;

                            destImg = new Rectangle(tab.area.X + oX, tab.area.Y + oY, tab.area.Width - (oX * 2), tab.area.Height - ((tab.area.Y + oY) * 2));
                        }
                        else
                        {
                            destImg = new Rectangle(tab.area.X + (tab.area.Width / 2) - (tab.Image.Width / 2), (tab.area.Height / 2) - (tab.Image.Height / 2), tab.Image.Width, tab.Image.Height);
                        }

                        using (AlphaImage image = new AlphaImage(tab.Image))
                        {
                            image.Draw(e.Graphics, destImg);
                        }
                    }

                    if (!tab.Selected && (tab.TabIndex == tabs.Count - 1 || !tabs[tab.TabIndex + 1].Selected))
                    {
                        Point sep = new Point(tab.area.Right, offset.Y);
                        DrawSeparator(e, sep, size.Height - (sep.Y * 2), Orientation.Vertical);
                    }
                }

                Font font = new Font(FontFamily.GenericSansSerif, HeaderSelectedTabStripFontSize,FontStyle.Regular);
                SolidBrush brush = new SolidBrush(ControlBackColor);
                SolidBrush fBrush = new SolidBrush(Strings.ToColor(HeaderSelectedTabStripFontColor));
                Rectangle rect = new Rectangle(0, size.Height, control.Width, control.Height - size.Height);
                e.Graphics.FillRectangle(brush, rect);
                SizeF textSize = e.Graphics.MeasureString(selected.Text, font);

                Rectangle stringRect = rect;
                stringRect.X = Convert.ToInt32(selected.area.X + ((selected.area.Width / 2) - (textSize.Width / 2)));
                if (stringRect.X < 0)
                    stringRect.X = 2 * ScaleFactor.Width;

                e.Graphics.DrawString(selected.Text, font, fBrush, stringRect);
                e.Graphics.DrawLine(new Pen(Strings.ToColor(HeaderSelectedTabStripBorderColor)), rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                brush.Dispose(); fBrush.Dispose(); font.Dispose();
            }
            finally
            {
                penBorder.Dispose();
            }
        }


        private const string Separator1L = "#29455A";
        private const string Separator1D = "#182C39";
        private const string Separator2L = "#42596B";
        private const string Separator2D = "#6B7584";


        private void DrawSeparator(PaintEventArgs e, Point point, int widthHeight, Orientation orientation)
        {
            for (int i = 0; i < 2; i++)
            {
                Color colorL = (i == 0 ? Strings.ToColor(Separator1L) : Strings.ToColor(Separator2L));
                Color colorD = (i == 0 ? Strings.ToColor(Separator1D) : Strings.ToColor(Separator2D));
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        {
                            Rectangle rect = new Rectangle(point.X, point.Y + i, widthHeight / 3, 1);
                            if (e.ClipRectangle.IntersectsWith(rect))
                                GradientFill.Fill(e.Graphics, rect, colorL, colorD, GradientFill.FillDirection.LeftToRight);

                            rect.Offset(rect.Width, 0);
                            if (e.ClipRectangle.IntersectsWith(rect))
                                e.Graphics.FillRectangle(new SolidBrush(colorD), rect);

                            rect.Offset(rect.Width, 0);
                            if (e.ClipRectangle.IntersectsWith(rect))
                                GradientFill.Fill(e.Graphics, rect, colorD, colorL, GradientFill.FillDirection.LeftToRight);

                        }
                        break;
                    case Orientation.Vertical:
                        {
                            Rectangle rect = new Rectangle(point.X + i, point.Y, 1, widthHeight / 3);
                            if (e.ClipRectangle.IntersectsWith(rect))
                                GradientFill.Fill(e.Graphics, rect, colorL, colorD, GradientFill.FillDirection.TopToBottom);

                            rect.Offset(0, rect.Height);
                            if (e.ClipRectangle.IntersectsWith(rect))
                                e.Graphics.FillRectangle(new SolidBrush(colorD), rect);

                            rect.Offset(0, rect.Height);
                            if (e.ClipRectangle.IntersectsWith(rect))
                                GradientFill.Fill(e.Graphics, rect, colorD, colorL, GradientFill.FillDirection.TopToBottom);
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        private const string SelectedBackColorGradient = "#0069B5";
        private const string AlternateBackColor = "#101010";
        private const string SelectedBackColor = "#009AFF";
        private const string ListSeparatorColor = "#181C18";

        public override void ApplyColorsToControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            control.BackColor = ControlBackColor;
            control.ForeColor = TextForeColor;
        }




        internal override void DrawListItemBackground(Graphics g, Rectangle bounds, int index, bool selected)
        {
            if (selected)
            {
                Color gradT = Strings.ToColor(SelectedBackColorGradient);
                Color selectedColor = Strings.ToColor(SelectedBackColor);
                Rectangle gradBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 5 * ScaleFactor.Height);

                GradientFill.Fill(g, gradBounds, gradT, selectedColor, GradientFill.FillDirection.TopToBottom);
                gradBounds = new Rectangle(bounds.X, bounds.Y + gradBounds.Height, bounds.Width, bounds.Height - gradBounds.Height);

                SolidBrush brush = new SolidBrush(selectedColor);
                g.FillRectangle(brush, gradBounds);
                brush.Dispose();
            }
            else
            {
                Color color;
                if (index % 2 == 0)
                    color = Strings.ToColor(AlternateBackColor);
                else
                    color = ControlBackColor;

                SolidBrush brush = new SolidBrush(color);
                g.FillRectangle(brush, bounds);
                brush.Dispose();

                Pen pen = new Pen(Strings.ToColor(ListSeparatorColor));
                g.DrawLine(pen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
                pen.Dispose();
            }
        }


    }
}

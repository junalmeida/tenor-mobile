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

        internal override void DrawHeaderBackGround(Size controlSize, PaintEventArgs eventArgs)
        {
            Rectangle rect = new Rectangle(eventArgs.ClipRectangle.X, 0, eventArgs.ClipRectangle.Width, controlSize.Height);
            Color start = Strings.ToColor(HeaderStartColor);
            Color end = Strings.ToColor(HeaderEndColor);
            GradientFill.Fill(eventArgs.Graphics, rect, start, end, GradientFill.FillDirection.TopToBottom);

            eventArgs.Graphics.DrawLine(new Pen(Strings.ToColor(BorderLineColor)), rect.X, rect.Height - 1, rect.Width, rect.Height - 1);
        }

        internal override void DrawHeaderText(string text, Size controlSize, PaintEventArgs e)
        {
            using (Font font = new Font(FontFamily.GenericSansSerif, HeaderFontSize, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Strings.ToColor(HeaderFontColor)))
            {
                SizeF textSize = e.Graphics.MeasureString(text, font);

                e.Graphics.DrawString(text, font, brush, 12 * ScaleFactor.Width , 7 * ScaleFactor.Height);
            }
            
        }

        internal override void DrawTabs(IList<HeaderTab> tabs, Size size, PaintEventArgs e)
        {
            foreach (HeaderTab tab in tabs)
            {
                Point offset = new Point(5 * ScaleFactor.Width, 2 * ScaleFactor.Width);

                int defaultWidth = 46 * ScaleFactor.Width;
                Size corner = new Size(8 * ScaleFactor.Width, 8 * ScaleFactor.Height);

                tab.area = new Rectangle(offset.X + tab.TabIndex * defaultWidth, offset.Y, defaultWidth, size.Height - offset.Y + corner.Height);
                if (tab.Selected)
                {
                    using (Pen penBorder = new Pen(Strings.ToColor(BorderLineColor)))
                    {
                        Color backColor = Color.Black;
                        RoundedRectangle.Fill(e.Graphics, penBorder, Color.Black, tab.area, corner);
                    }
                }
                else
                {
                }
                //draw icon

                if (tab.Image != null)
                {
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
                    AlphaImage.DrawImage(tab.Image as Bitmap, e.Graphics, destImg);
                }

                if (!tab.Selected && (tab.TabIndex == tabs.Count - 1 || !tabs[tab.TabIndex + 1].Selected))
                {
                    Point sep = new Point(tab.area.Right, offset.Y);
                    DrawSeparator(e, sep, size.Height - (sep.Y * 2), Orientation.Vertical);
                }
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

    }
}

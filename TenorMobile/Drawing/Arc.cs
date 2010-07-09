using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tenor.Mobile.Drawing
{
    public class Arc
    {
        public static Point[] CreateArc(float startAngle, float sweepAngle, int pointsInArc, int radius, int xOffset, int yOffset, int lineWidth)
        {
            if (pointsInArc < 0)
                pointsInArc = 0;

            if (pointsInArc > 360)
                pointsInArc = 360;

            Point[] points = new Point[pointsInArc * 2];
            int xo;
            int yo;
            int xi;
            int yi;
            float degs;
            double rads;

            for (int p = 0; p < pointsInArc; p++)
            {
                degs = startAngle + ((sweepAngle / pointsInArc) * p);

                rads = (degs * (Math.PI / 180));

                xo = (int)(radius * Math.Sin(rads));
                yo = (int)(radius * Math.Cos(rads));
                xi = (int)((radius - lineWidth) * Math.Sin(rads));
                yi = (int)((radius - lineWidth) * Math.Cos(rads));

                xo += (radius + xOffset);
                yo = radius - yo + yOffset;
                xi += (radius + xOffset);
                yi = radius - yi + yOffset;

                points[p] = new Point(xo, yo);
                points[(pointsInArc * 2) - (p + 1)] = new Point(xi, yi);
            }

            return points;
        }

    }
}

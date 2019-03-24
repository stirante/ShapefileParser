using System.Globalization;
using System.Text;

namespace ShapefileParser
{
    public class Shape
    {
        public Shape(Point2D[] points)
        {
            Points = points;
        }

        public Point2D[] Points { get; }

        /// <summary>
        ///     Returns if given point is inside shape
        ///     From http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>Is point inside shape</returns>
        public bool IsInside(Point2D point)
        {
            int i, j;
            var c = false;
            for (i = 0, j = Points.Length - 1; i < Points.Length; j = i++)
            {
                if (Points[i].Y > point.Y != Points[j].Y > point.Y &&
                    point.X < (Points[j].X - Points[i].X) * (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) +
                    Points[i].X)
                {
                    c = !c;
                }
            }

            return c;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var point in Points)
            {
                sb.Append(
                    $"{{lat: {point.Y.ToString("G", CultureInfo.InvariantCulture)}, lng: {point.X.ToString("G", CultureInfo.InvariantCulture)}}},");
            }

            return $"[{sb.ToString().Substring(0, sb.ToString().Length - 1)}]";
        }
    }
}
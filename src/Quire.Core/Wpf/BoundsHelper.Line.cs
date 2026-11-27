namespace Quire.Wpf;

public static partial class BoundsHelper
{
    public static global::System.Windows.Point FindPointOnPathBetween(global::System.Windows.Rect box1, global::System.Windows.Rect box2, double gradient)
    {
        if (box1 == box2)
        {
            throw new global::System.ArgumentException("Rectangles are identical");
        }
        if (box1.IntersectsWith(box2))
        {
            throw new global::System.ArgumentException("Rectangles overlap, there is no point 'between' them");
        }
        global::System.Windows.Point a = FindCenter(box1);
        global::System.Windows.Point b = FindCenter(box2);
        Line intersectLine = Line.CalculateLine(a, b);
        global::System.Windows.Point a2 = IntersectRectangle(box1, intersectLine);
        global::System.Windows.Point b2 = IntersectRectangle(box2, intersectLine);
        return Line.CalculateLine(a2, b2).NPoint(gradient);
    }

    public static global::System.Windows.Point FindCenter(global::System.Windows.Rect rect)
    {
        double num = rect.Width / 2.0;
        double halfHeight = rect.Height / 2.0;
        global::System.Windows.Point topLeft = rect.TopLeft;
        topLeft.Offset(num, halfHeight);
        return topLeft;
    }

    private static global::System.Windows.Point IntersectRectangle(global::System.Windows.Rect rect, Line intersectLine)
    {
        global::System.Windows.Point val;
        Line line = Line.CalculateLine(rect.TopLeft, rect.TopRight);
        val = intersectLine.Intersect(line);
        if (intersectLine.Contains(val) && rect.Contains(val))
        {
            return val;
        }
        line = Line.CalculateLine(rect.TopRight, rect.BottomRight);
        val = intersectLine.Intersect(line);
        if (intersectLine.Contains(val) && rect.Contains(val))
        {
            return val;
        }
        line = Line.CalculateLine(rect.BottomRight, rect.BottomLeft);
        val = intersectLine.Intersect(line);
        if (intersectLine.Contains(val) && rect.Contains(val))
        {
            return val;
        }
        line = Line.CalculateLine(rect.BottomLeft, rect.TopLeft);
        val = intersectLine.Intersect(line);
        if (intersectLine.Contains(val) && rect.Contains(val))
        {
            return val;
        }
        return new global::System.Windows.Point(double.NaN, double.NaN);
    }

    private partial struct Line
    {
        public static Line CalculateLine(global::System.Windows.Point a, global::System.Windows.Point b)
        {
            Line result = default;
            result.Begin = a;
            result.End = b;
            result.Slope = CalculateSlope(a, b);
            if (double.IsNaN(result.Slope))
            {
                result.YItercept = double.NaN;
            }
            else
            {
                result.YItercept = a.Y - result.Slope * a.X;
            }
            return result;
        }

        private double Length()
        {
            return global::System.Math.Sqrt(global::System.Math.Pow(End.X - Begin.X, 2.0) + global::System.Math.Pow(End.Y - Begin.Y, 2.0));
        }

        private static double CalculateSlope(global::System.Windows.Point start, global::System.Windows.Point end)
        {
            double num = end.X - start.X;
            double deltaY = end.Y - start.Y;
            if (num == 0.0)
            {
                return double.NaN;
            }
            return deltaY / num;
        }

        public global::System.Windows.Point NPoint(double gradient)
        {
            if (double.IsNaN(Slope))
            {
                double num = gradient * Length();
                return new global::System.Windows.Point(Begin.X, Begin.Y + num);
            }
            double deltaX = End.X - Begin.X;
            double x = Begin.X + gradient * deltaX;
            double y = CalculateY(x);
            return new global::System.Windows.Point(x, y);
        }

        public bool Contains(global::System.Windows.Point p)
        {
            double num = global::System.Math.Min(Begin.X, End.X);
            double right = global::System.Math.Max(Begin.X, End.X);
            double top = global::System.Math.Min(Begin.Y, End.Y);
            double bottom = global::System.Math.Max(Begin.Y, End.Y);
            if (num <= p.X && p.X <= right && top <= p.Y && p.Y <= bottom)
            {
                return true;
            }
            return false;
        }

        public global::System.Windows.Point Intersect(Line line)
        {
            return Intersect(this, line);
        }

        public static global::System.Windows.Point Intersect(Line l1, Line l2)
        {
            if (l1.Slope == l2.Slope)
            {
                return new global::System.Windows.Point(double.NaN, double.NaN);
            }
            double num;
            double intersectionY;
            if (double.IsNaN(l1.Slope))
            {
                num = l1.Begin.X;
                intersectionY = l2.Slope * num + l2.YItercept;
            }
            else if (double.IsNaN(l2.Slope))
            {
                num = l2.Begin.X;
                intersectionY = l1.Slope * num + l1.YItercept;
            }
            else
            {
                num = (l2.YItercept - l1.YItercept) / (l1.Slope - l2.Slope);
                intersectionY = l1.Slope * num + l1.YItercept;
            }
            return new global::System.Windows.Point(num, intersectionY);
        }
    }
}

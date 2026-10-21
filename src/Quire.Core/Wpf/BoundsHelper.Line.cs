namespace Quire.Wpf;

public static partial class BoundsHelper
{
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
            double num2 = end.Y - start.Y;
            if (num == 0.0)
            {
                return double.NaN;
            }
            return num2 / num;
        }

        public global::System.Windows.Point NPoint(double gradient)
        {
            if (double.IsNaN(Slope))
            {
                double num = gradient * Length();
                return new global::System.Windows.Point(Begin.X, Begin.Y + num);
            }
            double num2 = End.X - Begin.X;
            double num3 = Begin.X + gradient * num2;
            double num4 = CalculateY(num3);
            return new global::System.Windows.Point(num3, num4);
        }

        public bool Contains(global::System.Windows.Point p)
        {
            double num = global::System.Math.Min(Begin.X, End.X);
            double num2 = global::System.Math.Max(Begin.X, End.X);
            double num3 = global::System.Math.Min(Begin.Y, End.Y);
            double num4 = global::System.Math.Max(Begin.Y, End.Y);
            if (num <= p.X && p.X <= num2 && num3 <= p.Y && p.Y <= num4)
            {
                return true;
            }
            return false;
        }

        public global::System.Windows.Point Intersect(Line line)
        {
            return Intersect(this, line);
        }
    }
}

namespace Quire.Wpf;

public static partial class PointExtension
{
    public static bool IsNaN(this global::System.Windows.Point point)
    {
        if (!double.IsNaN(point.X))
        {
            return double.IsNaN(point.Y);
        }
        return true;
    }

    public static global::System.Windows.Point Floor(this global::System.Windows.Point point)
    {
        global::System.Windows.Point result = default;
        result.X = global::System.Math.Floor(point.X);
        result.Y = global::System.Math.Floor(point.Y);
        return result;
    }

    public static global::System.Windows.Point Round(this global::System.Windows.Point point)
    {
        global::System.Windows.Point result = default;
        result.X = global::System.Math.Round(point.X);
        result.Y = global::System.Math.Round(point.Y);
        return result;
    }

    public static bool IsEqualTo(this global::System.Windows.Point first, global::System.Windows.Point second, global::Quire.DoubleComparer doubleComparer = default)
    {
        return (doubleComparer ?? Epsilon).Compare(first, second) == 0;
    }
}

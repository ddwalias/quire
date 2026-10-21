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
}

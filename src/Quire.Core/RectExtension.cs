namespace Quire;

public static partial class RectExtension
{
    public static bool ContainsAccountingForMeasurementError(this global::System.Windows.Rect rect, global::System.Windows.Point pt)
    {
        if (pt.X.IsBetween(rect.TopLeft.X, rect.BottomRight.X, isInclusive: true, 1E-05))
        {
            return pt.Y.IsBetween(rect.TopLeft.Y, rect.BottomRight.Y, isInclusive: true, 1E-05);
        }
        return false;
    }

    public static bool IntersectsWithAccountingForMeasurementError(this global::System.Windows.Rect rect, global::System.Windows.Rect rectAnother)
    {
        if (rect.IntersectsWith(rectAnother))
        {
            return true;
        }
        if (!rect.ContainsAccountingForMeasurementError(rectAnother.TopLeft) && !rect.ContainsAccountingForMeasurementError(rectAnother.TopRight) && !rect.ContainsAccountingForMeasurementError(rectAnother.BottomLeft))
        {
            return rect.ContainsAccountingForMeasurementError(rectAnother.BottomRight);
        }
        return true;
    }

    public static bool IsValidSizeRect(this global::System.Windows.Rect rect)
    {
        if (!rect.IsEmpty && rect.Height > 0.0)
        {
            return rect.Width > 0.0;
        }
        return false;
    }

    public static bool IsValidSizeRect(this global::System.Windows.Int32Rect rect)
    {
        if (!rect.IsEmpty && rect.Height > 0)
        {
            return rect.Width > 0;
        }
        return false;
    }

    public static bool IntersectsWithNonZeroArea(this global::System.Windows.Rect rect1, global::System.Windows.Rect rect2)
    {
        global::System.Windows.Rect val = global::System.Windows.Rect.Intersect(rect1, rect2);
        if (!val.IsEmpty && !val.Width.IsEqualTo(0.0))
        {
            return !val.Height.IsEqualTo(0.0);
        }
        return false;
    }

    public static bool IsEqualTo(this global::System.Windows.Rect first, global::System.Windows.Rect second, global::Quire.DoubleComparer doubleComparer = default)
    {
        return (doubleComparer ?? UiLayoutComparer).Compare(first, second) == 0;
    }
}

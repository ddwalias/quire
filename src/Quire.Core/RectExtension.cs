namespace Quire;

public static partial class RectExtension
{
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
}

namespace Quire;

public static partial class SizeExtension
{
    public static bool IsEqualTo(this global::System.Windows.Size size1, global::System.Windows.Size size2, double eps = double.NaN)
    {
        if (size1.IsEmpty || size2.IsEmpty)
        {
            return size1.IsEmpty == size2.IsEmpty;
        }
        if (size1.Width.IsEqualTo(size2.Width, eps))
        {
            return size1.Height.IsEqualTo(size2.Height, eps);
        }
        return false;
    }

    public static global::System.Windows.Size Round(this global::System.Windows.Size size)
    {
        return new global::System.Windows.Size(global::System.Math.Round(size.Width), global::System.Math.Round(size.Height));
    }

    public static global::System.Windows.Size RoundWithDecimal(this global::System.Windows.Size size, int decimals = 2)
    {
        return new global::System.Windows.Size(global::System.Math.Round(size.Width, decimals), global::System.Math.Round(size.Height, decimals));
    }
}

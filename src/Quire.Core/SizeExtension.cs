namespace Quire;

public static partial class SizeExtension
{
    public static global::System.Windows.Size Round(this global::System.Windows.Size size)
    {
        return new global::System.Windows.Size(global::System.Math.Round(size.Width), global::System.Math.Round(size.Height));
    }

    public static global::System.Windows.Size RoundWithDecimal(this global::System.Windows.Size size, int decimals = 2)
    {
        return new global::System.Windows.Size(global::System.Math.Round(size.Width, decimals), global::System.Math.Round(size.Height, decimals));
    }
}

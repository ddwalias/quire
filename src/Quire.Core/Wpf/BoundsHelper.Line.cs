namespace Quire.Wpf;

public static partial class BoundsHelper
{
    private partial struct Line
    {
        private double Length()
        {
            return global::System.Math.Sqrt(global::System.Math.Pow(End.X - Begin.X, 2.0) + global::System.Math.Pow(End.Y - Begin.Y, 2.0));
        }
    }
}

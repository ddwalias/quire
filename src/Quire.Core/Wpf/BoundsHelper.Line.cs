namespace Quire.Wpf;

public static partial class BoundsHelper
{
    private partial struct Line
    {
        private double Length()
        {
            return global::System.Math.Sqrt(global::System.Math.Pow(End.X - Begin.X, 2.0) + global::System.Math.Pow(End.Y - Begin.Y, 2.0));
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
    }
}

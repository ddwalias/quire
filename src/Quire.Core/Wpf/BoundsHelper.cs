#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class BoundsHelper
{
    partial struct Line
    {
        [MirrorVerified("Release IL")]
        public double Slope;

        [MirrorVerified("Release IL")]
        public double YItercept;

        [MirrorVerified("Release IL")]
        private double CalculateY(double x) => Slope * x + YItercept;
    }
}

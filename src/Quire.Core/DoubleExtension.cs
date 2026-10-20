#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class DoubleExtension
{
    [MirrorVerified("Release IL")]
    private static double Epsilon = 1E-10;

    [MirrorVerified("PtcDoubleExtensionMechanicalDifferentialTests")]
    public static double Round(this double num) => Math.Round(num);

    [MirrorVerified("Release IL")]
    public static bool AreSame(this double value1, double value2)
    {
        if (double.IsNaN(value1) && double.IsNaN(value2))
        {
            return true;
        }
        return value1 == value2;
    }

    [MirrorVerified("Release IL")]
    public static bool IsEqualTo(this double value1, double value2, double eps = double.NaN)
    {
        if (eps != 0.0)
        {
            return Math.Abs(value1 - value2) < (double.IsNaN(eps) ? Epsilon : eps);
        }
        return value1 == value2;
    }

    [MirrorVerified("Release IL")]
    public static bool IsGreaterThan(this double value1, double value2, double eps = double.NaN)
    {
        return value1 - value2 > (double.IsNaN(eps) ? Epsilon : eps);
    }

    [MirrorVerified("Release IL")]
    public static bool IsLessThan(this double value1, double value2, double eps = double.NaN)
    {
        return value2 - value1 > (double.IsNaN(eps) ? Epsilon : eps);
    }

    [MirrorVerified("Release IL")]
    public static bool IsValid(this double num)
    {
        if (!double.IsNaN(num))
        {
            return !double.IsInfinity(num);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public static int IntFloor(this double num, double eps = double.NaN) => (int)Floor(num, eps);

    [MirrorVerified("Release IL")]
    public static int IntCeiling(this double num, double eps = double.NaN) => (int)Ceiling(num, eps);

    [MirrorVerified("Release IL")]
    public static int IntRound(this double num) => (int)Round(num);
}

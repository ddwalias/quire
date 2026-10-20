#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class DoubleExtension
{
    [MirrorVerified("PtcDoubleExtensionMechanicalDifferentialTests")]
    public static double Round(this double num) => Math.Round(num);

    [MirrorVerified("Release IL")]
    public static int IntFloor(this double num, double eps = double.NaN) => (int)Floor(num, eps);

    [MirrorVerified("Release IL")]
    public static int IntCeiling(this double num, double eps = double.NaN) => (int)Ceiling(num, eps);

    [MirrorVerified("Release IL")]
    public static int IntRound(this double num) => (int)Round(num);
}

#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class DoubleExtension
{
    [MirrorVerified("PtcDoubleExtensionMechanicalDifferentialTests")]
    public static double Round(this double num) => Math.Round(num);
}

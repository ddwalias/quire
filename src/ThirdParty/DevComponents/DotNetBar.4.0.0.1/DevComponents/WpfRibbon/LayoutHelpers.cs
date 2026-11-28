#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

internal partial class LayoutHelpers
{
    [MirrorVerified("Release IL")]
    public static bool IsZero(double value) => Math.Abs(value) < 2.220446049250313E-15;

    [MirrorVerified("Release IL")]
    public static bool IsClose(double val1, double val2) => Math.Abs(val1 - val2) < 1.0;

    [MirrorVerified("Release IL")]
    public LayoutHelpers() { }
}

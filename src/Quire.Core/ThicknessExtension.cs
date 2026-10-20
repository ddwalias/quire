#nullable disable
using Quire.Mirror;
using System.Windows;

namespace Quire;

public static partial class ThicknessExtension
{
    [MirrorVerified("Release IL")]
    public static bool IsEqualTo(this Thickness thickness1, Thickness thickness2, double eps = double.NaN)
    {
        if (thickness1.Left.IsEqualTo(thickness2.Left, eps)
            && thickness1.Top.IsEqualTo(thickness2.Top, eps)
            && thickness1.Right.IsEqualTo(thickness2.Right, eps))
        {
            return thickness1.Bottom.IsEqualTo(thickness2.Bottom, eps);
        }
        return false;
    }
}

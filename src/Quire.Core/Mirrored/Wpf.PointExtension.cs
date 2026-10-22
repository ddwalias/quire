#nullable disable
#pragma warning disable

namespace Quire.Wpf;

static partial class PointExtension
{
    public static readonly global::Quire.DoubleComparer Epsilon = new global::Quire.DoubleComparer(1E-05);
    public static readonly global::System.Windows.Point NaN = new global::System.Windows.Point(double.NaN, double.NaN);
    public static readonly global::System.Windows.Point Zero = new global::System.Windows.Point(0.0, 0.0);
}

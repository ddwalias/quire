using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="Utilities"/> against docs/mirror/evidence/Mcd.Utilities.md; UtilitiesDifferentialTests
/// compares with Mathcad.
/// </summary>
public class UtilitiesTests
{
    public UtilitiesTests() => RuntimeThread.Ensure();

    [Fact]
    public void Scaling_divides_in_place()
    {
        double[] v = [2, 4, double.NaN];
        Utilities.ScaleVector(v, 2);
        Assert.Equal([1, 2, double.NaN], v);
        Assert.Throws<IndexOutOfRangeException>(() => Utilities.ScaleVector([], 2));
        Assert.Throws<NullReferenceException>(() => Utilities.ScaleVector(null!, 2));
    }

    [Fact]
    public void Non_finite_values_are_floating_point_errors()
    {
        Assert.False(Utilities.IsFinite(double.NaN));
        Assert.True(Utilities.IsFinite(double.MaxValue));
        Utilities.AssertFinite(-0.0);
        Assert.Equal("floating point error", Assert.Throws<AppException>(() => Utilities.AssertFinite(double.NegativeInfinity)).Message);
        Assert.Throws<AppException>(() => Utilities.AssertFinite(new num_val(1, double.NaN)));
        Assert.NotNull(new Utilities());
    }
}

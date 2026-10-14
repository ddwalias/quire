#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>Numeric helpers (docs/mirror/evidence/Mcd.Utilities.md).</summary>
public sealed partial class Utilities
{
    [MirrorVerified("UtilitiesDifferentialTests")]
    public Utilities() { }

    /// <summary>Divides every element by <paramref name="scalar"/> in place, last to first; an empty vector throws
    /// <see cref="IndexOutOfRangeException"/>.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static void ScaleVector(double[] vector, double scalar)
    {
        _ = vector[0];
        for (int i = vector.Length - 1; i >= 0; i--) vector[i] /= scalar;
    }

    /// <summary>Null for a dimensionless or polymorphic product; otherwise its basis units in the calling thread's
    /// unit system.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static Units GetUnits(Type.Product product) =>
        product.IsDimensionless || product.IsPolymorphic ? null : RuntimeOptions.the_unit_system.Normalize(product);

    /// <summary>Neither infinite nor NaN.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static bool IsFinite(double d) => double.IsFinite(d);

    /// <summary>Raises "floating point error" for an infinite or NaN value.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static void AssertFinite(double d)
    {
        if (!IsFinite(d)) AppException.Raise("floating point error");
    }

    /// <summary>Both parts finite, the real part checked first.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static void AssertFinite(num_val v)
    {
        AssertFinite(v.rp);
        AssertFinite(v.ip);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public static object Divide(object value, double scalar)
    {
        if (scalar == 1) return value;
        if (scalar == 0) AppException.Raise("illegal factor");
        return value switch { NumVal n => n / scalar, MatVal m => m / scalar, _ => value };
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public static object Multiply(object value, double scalar) => scalar == 1 ? value : Divide(value, 1 / scalar);
}

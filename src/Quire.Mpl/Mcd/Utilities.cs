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
    public unsafe static void ScaleVector(double[] vector, double scalar)
    {
        fixed (double* p = &vector[0])
        {
            int idx = vector.Length;
            while (--idx >= 0)
            {
                p[idx] /= scalar;
            }
        }
    }

    /// <summary>Null for a dimensionless or polymorphic product; otherwise its basis units in the calling thread's
    /// unit system.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static Units GetUnits(Type.Product product) =>
        product.IsDimensionless || product.IsPolymorphic ? null : RuntimeOptions.the_unit_system.Normalize(product);

    /// <summary>Neither infinite nor NaN.</summary>
    [MirrorVerified("UtilitiesDifferentialTests")]
    public static unsafe bool IsFinite(double d) => (((int*)&d)[1] & 0x7FF00000) != 0x7FF00000;

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

    [MirrorVerified("UtilitiesDifferentialTests")]
    public static object Divide(object value, double scalar)
    {
        if (scalar != 1.0)
        {
            if (scalar == 0.0) AppException.Raise("illegal factor");
            if (value is NumVal nv) return nv / scalar;
            if (value is MatVal mv) return mv / scalar;
        }
        return value;
    }

    [MirrorVerified("UtilitiesDifferentialTests")]
    public static object Multiply(object value, double scalar)
    {
        if (scalar != 1.0) return Divide(value, 1.0 / scalar);
        return value;
    }
}

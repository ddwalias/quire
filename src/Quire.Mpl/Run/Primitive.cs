#nullable disable
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>Mpl's primitive comparisons: 1 when the comparison holds, else 0 (docs/mirror/evidence/Mpl.Run.Primitive.md).</summary>
public partial class Primitive
{
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int ceq_i4(int x, int y) => x == y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cne_i4(int x, int y) => x != y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int clt_i4(int x, int y) => x < y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cle_i4(int x, int y) => x <= y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cgt_i4(int x, int y) => x > y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cge_i4(int x, int y) => x >= y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int ceq_f8(double x, double y) => x == y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cne_f8(double x, double y) => x != y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int clt_f8(double x, double y) => x < y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cle_f8(double x, double y) => x <= y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cgt_f8(double x, double y) => x > y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cge_f8(double x, double y) => x >= y ? 1 : 0;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object Throw(Exception e) => throw e;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_matrix(object o) => o as MatVal;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_number(object o) => o as NumVal;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_string(object o) => o as string;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_function(object o) => o as Function;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_range(object o) => o as RngVal;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object identity(object o) => o;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object dup(object o) => throw new AppException("may not capture pseudo function");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object build(object k, object z) => throw new AppException("build: must inline");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object dfoldr(object f, object n, object xs) => throw new AppException("dfoldr: must inline");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static void conv_void(object o) { }

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object invalid(object o) => throw new AppException("compiler error");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object mpl_type(object o) => throw new AppException("compiler error");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object rv(double r) => new NumVal(r, 0.0);

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object rrv(double r, double i) => new NumVal(r, i);

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object seq(object x, object y) => y;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object seq_void(object x, object y) => throw new AppException("compiler error");
}

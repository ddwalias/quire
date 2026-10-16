#nullable disable
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
}

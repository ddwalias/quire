#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class DoubleComparer
{
    [MirrorVerified("PtcDoubleComparerMechanicalDifferentialTests")]
    private DoubleComparer() : this(1E-10) { }

    [MirrorVerified("PtcDoubleComparerMechanicalDifferentialTests")]
    public double Epsilon { get; private set; }
}

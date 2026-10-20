#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class RealSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests")]
    public RealSimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests")]
    public double Value { get; set; }

    [MirrorVerified("PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Real: {0}", Value);

    [MirrorVerified("PtcCalculationCoreRealSimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is RealSimpleResultNode other)
        {
            return other.Value.Equals(Value);
        }
        return false;
    }
}

#nullable disable
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
}

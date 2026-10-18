#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ComplexSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreComplexSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreComplexSimpleResultNodeMechanicalDifferentialTests")]
    public ComplexSimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreComplexSimpleResultNodeMechanicalDifferentialTests")]
    public double RValue { get; set; }

    [MirrorVerified("PtcCalculationCoreComplexSimpleResultNodeMechanicalDifferentialTests")]
    public double IValue { get; set; }
}

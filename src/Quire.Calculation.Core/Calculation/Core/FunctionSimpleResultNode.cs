#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class FunctionSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests")]
    public FunctionSimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests")]
    public string Description { get; set; }
}

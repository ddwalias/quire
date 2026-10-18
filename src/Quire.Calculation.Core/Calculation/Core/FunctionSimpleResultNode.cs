#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("PtcCalculationCoreFunctionSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Function Description: \"{0}\"", Description);
}

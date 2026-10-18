#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class StringSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests")]
    public StringSimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests")]
    public string Value { get; set; }
}

#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EmptySimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreEmptySimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => "Empty";

    [MirrorVerified("PtcCalculationCoreEmptySimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreEmptySimpleResultNodeMechanicalDifferentialTests")]
    public EmptySimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreEmptySimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj) => obj is EmptySimpleResultNode;
}

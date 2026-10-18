#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class SymbolicSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreSymbolicSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreSymbolicSimpleResultNodeMechanicalDifferentialTests")]
    public SymbolicSimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreSymbolicSimpleResultNodeMechanicalDifferentialTests")]
    public string Xml { get; set; }

    [MirrorVerified("PtcCalculationCoreSymbolicSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Symbolic: {0}", Xml);
}

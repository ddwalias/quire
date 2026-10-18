#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ImaginarySimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreImaginarySimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreImaginarySimpleResultNodeMechanicalDifferentialTests")]
    public ImaginarySimpleResultNode() { }

    [MirrorVerified("PtcCalculationCoreImaginarySimpleResultNodeMechanicalDifferentialTests")]
    public double Value { get; set; }
}

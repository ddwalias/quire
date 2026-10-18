#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("PtcCalculationCoreImaginarySimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Imaginary: {0}i", Value);
}

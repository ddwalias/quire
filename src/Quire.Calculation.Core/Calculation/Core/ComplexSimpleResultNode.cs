#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("PtcCalculationCoreComplexSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Complex: {0}+{1}i", RValue, IValue);

    [MirrorVerified("PtcCalculationCoreComplexSimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is ComplexSimpleResultNode { RValue: var rValue } other && rValue.Equals(RValue))
        {
            return other.IValue.Equals(IValue);
        }
        return false;
    }
}

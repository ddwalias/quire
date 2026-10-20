#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("String: \"{0}\"", Value);

    [MirrorVerified("PtcCalculationCoreStringSimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is StringSimpleResultNode other)
        {
            return other.Value.Equals(Value);
        }
        return false;
    }
}

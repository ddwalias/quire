#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class UnitedValueSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    public SimpleResultNode Value { get; set; }

    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    public IList<Unit> Units { get; private set; }

    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    public UnitedValueSimpleResultNode()
    {
        Units = new List<Unit>();
    }
}

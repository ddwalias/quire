#nullable disable
using System.Collections.Generic;
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

    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    private bool isUnitsEquals(IList<Unit> comparedUnits)
    {
        bool result = false;
        if (Units.Count == comparedUnits.Count)
        {
            for (int i = 0; i < Units.Count; i++)
            {
                if (!Units[i].CanonicalName.Equals(comparedUnits[i].CanonicalName))
                {
                    return false;
                }
            }
            result = true;
        }
        return result;
    }

    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is UnitedValueSimpleResultNode unitedValueSimpleResultNode && Value.Equals(unitedValueSimpleResultNode.Value))
        {
            return isUnitsEquals(unitedValueSimpleResultNode.Units);
        }
        return false;
    }

    [MirrorVerified("PtcCalculationCoreUnitedValueSimpleResultNodeMechanicalDifferentialTests")]
    public void AddUnit(string canonicalName, int powerNumerator, int powerDenominator)
    {
        Units.Add(new Unit
        {
            CanonicalName = canonicalName,
            Symbol = DefaultUnitDescriptorTable.Instance[canonicalName].SymbolResultNode,
            PowerNumerator = powerNumerator,
            PowerDenominator = powerDenominator
        });
    }
}

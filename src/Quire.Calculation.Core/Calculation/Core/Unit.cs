#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial struct Unit
{
    [MirrorVerified("PtcCalculationCoreUnitMechanicalDifferentialTests")]
    public string CanonicalName;

    [MirrorVerified("PtcCalculationCoreUnitMechanicalDifferentialTests")]
    public string Symbol;

    [MirrorVerified("PtcCalculationCoreUnitMechanicalDifferentialTests")]
    public int PowerNumerator;

    [MirrorVerified("PtcCalculationCoreUnitMechanicalDifferentialTests")]
    public int PowerDenominator;
}

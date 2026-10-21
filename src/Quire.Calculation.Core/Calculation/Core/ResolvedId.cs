#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial struct ResolvedId
{
    [MirrorVerified("PtcCalculationCoreResolvedIdMechanicalDifferentialTests")]
    public string Name;

    [MirrorVerified("PtcCalculationCoreResolvedIdMechanicalDifferentialTests")]
    public int Tag;

    [MirrorVerified("PtcCalculationCoreResolvedIdMechanicalDifferentialTests")]
    public ICalculatable Calculatable;
}

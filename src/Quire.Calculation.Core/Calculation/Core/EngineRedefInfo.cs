#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial struct EngineRedefInfo
{
    [MirrorVerified("PtcCalculationCoreEngineRedefInfoMechanicalDifferentialTests")]
    public string OldLocation { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineRedefInfoMechanicalDifferentialTests")]
    public EngineDefType OldType { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineRedefInfoMechanicalDifferentialTests")]
    public string Name { get; set; }
}

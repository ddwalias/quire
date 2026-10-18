#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial struct EngineResult
{
    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public IResultNode ResultNode { get; private set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineErrorCollection Errors { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineRedefInfo[] Warnings { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public SynchronizationStatus SynchronizationStatus { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public string SerializationTag { get; set; }
}

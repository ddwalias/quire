#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ResultManager
{
    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public int CreateOldResultRefId(IResultNode resultNode, ICalculatableResultOwner calculatable) => AddOldResult(resultNode, calculatable);

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private static readonly IResultManager _instance;

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static IResultManager Instance => _instance;

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private Dictionary<int, Pair<EngineResult, ICalculatableResultOwner>> _results;

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public IEnumerable<KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>>> Results => _results;
}

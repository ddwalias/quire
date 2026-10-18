#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class SimpleCalculatable
{
    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    private CalculatingStatus _calculatingStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetCalculatingStatus(CalculatingStatus computingStatus) => _calculatingStatus = computingStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    private IResultNode _evaluationResult;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetResult(IResultNode result) => _evaluationResult = result;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    private EngineError _error;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetError(EngineError error) => _error = error;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap) { }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    private IList<EngineRedefInfo> _warnings;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings) => _warnings = warnings;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap) { }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual IResultNode Result => _evaluationResult;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public IList<EngineRedefInfo> EngineWarnings => _warnings;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    private SynchronizationStatus _syncStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual SynchronizationStatus SynchronizationStatus => _syncStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual CalculatingStatus CalculatingStatus => _calculatingStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual EngineError Error => _error;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual string Rl { get; set; }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    protected RealSimpleResultNode _systemOrigin;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public RealSimpleResultNode SystemOrigin => _systemOrigin;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Action<ICalcWithSyncStatus> Synchronized;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Action<ICalculatable> RequestEvaluation;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Action<ICalculatable> TreatAsUpToDate;
}

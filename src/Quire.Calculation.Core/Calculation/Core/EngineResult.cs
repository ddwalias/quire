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

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineResult(EngineResult result, SynchronizationStatus synchronizationStatus)
    {
        this = default;
        ResultNode = result.ResultNode ?? new EmptySimpleResultNode();
        Errors = result.Errors;
        Warnings = result.Warnings;
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = synchronizationStatus;
    }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineResult(EngineResult result, IResultNode resultNode)
    {
        this = default;
        ResultNode = resultNode ?? new EmptySimpleResultNode();
        Errors = null;
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = result.SynchronizationStatus;
    }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineResult(EngineResult result, IEnumerable<EngineRedefInfo> warnings)
    {
        this = default;
        ResultNode = result.ResultNode;
        Errors = result.Errors;
        Warnings = warnings?.ToArray();
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = result.SynchronizationStatus;
    }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineResult(IResultNode resultNode, EngineErrorCollection errorCollection, SynchronizationStatus synchronizationStatus)
    {
        this = default;
        if (resultNode != null && errorCollection != null && !(resultNode is IEmptyResultNode) && errorCollection.Count != 0)
        {
            throw new InvalidOperationException("logic error: An EngineResult may not have both a calculated result value and an error collection.");
        }
        ResultNode = resultNode ?? new EmptySimpleResultNode();
        Errors = errorCollection;
        Warnings = null;
        SynchronizationStatus = synchronizationStatus;

    }
}

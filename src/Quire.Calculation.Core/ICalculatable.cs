namespace Quire.Calculation.Core;

// Values match Mathcad's Ptc.Calculation.Core.SynchronizationStatus: comparisons and constants compile into callers.
public enum SynchronizationStatus
{
    Pending = 0,
    Synchronized = 3,
}

public enum CalculatingStatus
{
    Idle,
    Calculating,
}

/// <summary>A worksheet item the engine calculates (math region, plot, solve block…).</summary>
public partial interface ICalculatable
{
    /// <summary>The region serialised as an engine S-expression ("Rl").</summary>
    string Rl { get; }

    void SetSynchronizationStatus(SynchronizationStatus status);

    event global::System.Action<ICalculatable> RequestEvaluation;

    event global::System.Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    event global::System.Action<ICalculatable> TreatAsUpToDate;

    void Commit();

    void SetStatusSynchronized();

    void SetCalculatingStatus(CalculatingStatus computingStatus);

    void SetResult(IResultNode result);

    void SetError(EngineError error);

    void SetIdentifierLabels(global::System.Collections.Generic.IDictionary<ResolvedId, string[]> idLabelMap);

    void SetAutoSelectAlgorithm(global::System.Collections.Generic.IDictionary<int, string> autoAlgorithmMap);

    void SetRedefinitionWarnings(global::System.Collections.Generic.IList<EngineRedefInfo> warnings);
}

namespace Quire.Calculation.Core;

public enum SynchronizationStatus
{
    Pending,
    Synchronized,
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

    /// <summary>Raised when <see cref="Rl"/> changes.</summary>
    event EventHandler? RlChanged;

    SynchronizationStatus SynchronizationStatus { get; }

    void SetSynchronizationStatus(SynchronizationStatus status);

    void SetResult(CalculationResult result);

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

public sealed class CalculatableEventArgs(ICalculatable calculatable, ICalculatable? previous) : EventArgs
{
    public ICalculatable Calculatable { get; } = calculatable;

    /// <summary>The calculatable now immediately before it in worksheet order, or null when it is first.</summary>
    public ICalculatable? Previous { get; } = previous;
}

/// <summary>An ordered collection of calculatables, typically one worksheet.</summary>
public partial interface ICalculatableContext
{
    IReadOnlyList<ICalculatable> Calculatables { get; }

    new event EventHandler<CalculatableEventArgs>? CalculatableAdded;
    new event EventHandler<CalculatableEventArgs>? CalculatableRemoved;
    new event EventHandler<CalculatableEventArgs>? CalculatableMoved;

    event EventHandler? RecalculateAllRequested;
    event EventHandler? CalculateNowRequested;
    event EventHandler? AutoCalcChanged;
    event EventHandler? UnitSystemUpdated;

    bool IsAutoCalcOn { get; }

    /// <summary>Name of the display unit system: si, uscs or cgs.</summary>
    string UnitSystem { get; }
}

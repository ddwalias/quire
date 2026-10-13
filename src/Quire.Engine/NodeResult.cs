using Quire.Values;

namespace Quire.Engine;

public enum NodeStatus
{
    Invalid,
    Computing,
    NotComputing,
    Ready,
    TreatedReady,
}

public enum NodeType
{
    Empty,
    Definition,
    GlobalDefinition,
    Evaluation,
    Expression,
    SolveBlock,
    Plot,
}

public sealed record EngineError(string Message, string? Symbol = null);

/// <summary>How the UI should present a value: <c>shown = (si - Offset) / Scale</c>.</summary>
public sealed record DisplayTransform(double Scale, double Offset = 0);

/// <summary>The outcome of evaluating one node (region).</summary>
public sealed class NodeResult
{
    public NodeStatus Status { get; init; } = NodeStatus.Ready;
    public NodeType Type { get; init; }
    public Value? Value { get; init; }
    public EngineError? Error { get; init; }
    public DisplayTransform? Display { get; init; }

    /// <summary>True when <see cref="Value"/> is a column of results produced by iterating a range variable.</summary>
    public bool IsRangeTable { get; init; }

    /// <summary>Per-child results for aggregates such as solve blocks.</summary>
    public IReadOnlyList<NodeResult>? Children { get; init; }

    public IReadOnlyList<string> DefinedNames { get; init; } = [];
    public string? Warning { get; init; }

    public bool HasError => Error != null || (Children?.Any(c => c.HasError) ?? false);

    public static NodeResult Failed(NodeType type, CalcException ex) =>
        new() { Type = type, Status = NodeStatus.Invalid, Error = new EngineError(ex.Message, ex.Symbol) };
}

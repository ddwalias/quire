using System.Collections;
using Quire.Mcd;

namespace Quire.Engine.Internal;

/// <summary>What a node's Rl statement does.</summary>
internal enum NodeKind
{
    Empty,
    Definition,
    GlobalDefinition,
    Evaluation,
    Expression,
    SolveBlock,
    Plot,
}

internal sealed record EngineError(string Message, string? Symbol = null);

/// <summary>How the UI should present a value: <c>shown = (si - Offset) / Scale</c>.</summary>
internal sealed record DisplayTransform(double Scale, double Offset = 0);

/// <summary>The outcome of evaluating one node (region).</summary>
internal sealed class NodeResult
{
    public NodeStatus Status { get; init; } = NodeStatus.NODE_READY;
    public NodeKind Kind { get; init; }
    public DimVal? Value { get; init; }
    public EngineError? Error { get; init; }
    public DisplayTransform? Display { get; init; }

    /// <summary>True when <see cref="Value"/> is a column of results produced by iterating a range variable.</summary>
    public bool IsRangeTable { get; init; }

    /// <summary>Per-child results for aggregates such as solve blocks.</summary>
    public IReadOnlyList<NodeResult>? Children { get; init; }

    public IReadOnlyList<string> DefinedNames { get; init; } = [];
    public string? Warning { get; init; }

    public bool HasError => Error != null || (Children?.Any(c => c.HasError) ?? false);

    public static NodeResult Failed(NodeKind kind, CalcException ex) =>
        new() { Kind = kind, Status = NodeStatus.NODE_INVALID, Error = new EngineError(ex.Message, ex.Symbol) };

    /// <summary>The result as <see cref="IEngine.GetValueSync"/> returns it.</summary>
    public IDictionary ToDictionary()
    {
        var d = new Hashtable
        {
            [ResultKeys.Status] = Status,
            [ResultKeys.Value] = Value,
            [ResultKeys.Table] = IsRangeTable,
            [ResultKeys.Defined] = DefinedNames.ToArray(),
        };
        if (Error != null)
        {
            d[ResultKeys.Error] = Error.Message;
            d[ResultKeys.Symbol] = Error.Symbol;
        }
        if (Warning != null) d[ResultKeys.Warning] = Warning;
        if (Display != null)
        {
            d[ResultKeys.DisplayScale] = Display.Scale;
            d[ResultKeys.DisplayOffset] = Display.Offset;
        }
        if (Children != null) d[ResultKeys.Children] = new ArrayList(Children.Select(c => c.ToDictionary()).ToArray());
        return d;
    }
}

/// <summary>The entries of a result dictionary from <see cref="IEngine.GetValueSync"/>. These are provisional Quire
/// keys, not verified Mathcad equivalents; see docs/mirror/PLAN.md §3b before relying on or changing them.</summary>
internal static class ResultKeys
{
    /// <summary><see cref="NodeStatus"/>.</summary>
    public const string Status = "status";

    /// <summary>The computed <see cref="DimVal"/>, or null.</summary>
    public const string Value = "value";

    /// <summary>True when the value is a column of results produced by iterating a range variable.</summary>
    public const string Table = "table";

    /// <summary>The names the node defines (string[]).</summary>
    public const string Defined = "defined";

    /// <summary>The error message, when the node failed.</summary>
    public const string Error = "error";

    /// <summary>The identifier the error is about, when known.</summary>
    public const string Symbol = "symbol";

    public const string Warning = "warning";

    /// <summary>A unit override such as <c>x = km/hr</c>: shown = (value − offset) / scale (doubles).</summary>
    public const string DisplayScale = "display-scale";

    public const string DisplayOffset = "display-offset";

    /// <summary>Per-child result dictionaries for aggregates such as solve blocks (IList).</summary>
    public const string Children = "children";
}

namespace Quire.Calculation.Core;

public interface IEngineError
{
    string Message { get; }

    /// <summary>The identifier the error is about, if any; the editor highlights it.</summary>
    string? Symbol { get; }
}

public sealed record EngineError(string Message, string? Symbol = null) : IEngineError;

/// <summary>A unit factor in a displayed result, e.g. ("m", 1), ("s", -2).</summary>
public sealed record UnitPower(string Name, double Exponent);

public enum ResultValueType
{
    Empty,
    UnitedValue,
    String,
    Matrix,
    Function,
    Trace2D,
}

/// <summary>Display-ready result tree; values are already converted to their display units.</summary>
public interface IResultNode
{
    ResultValueType ValueType { get; }
}

public interface IEmptyResultNode : IResultNode;

public interface IUnitedValueResultNode : IResultNode
{
    double DisplayValue { get; }

    /// <summary>Units to show after the number. Empty when unitless or when the user supplied a unit override.</summary>
    IReadOnlyList<UnitPower> Units { get; }
}

public interface IStringResultNode : IResultNode
{
    string Value { get; }
}

public interface IMatrixResultNode : IResultNode
{
    int Rows { get; }
    int Cols { get; }

    /// <summary>Row-major display values.</summary>
    IReadOnlyList<double> DisplayValues { get; }

    /// <summary>Units shared by every element (shown once after the matrix); empty when unitless or mixed.</summary>
    IReadOnlyList<UnitPower> Units { get; }

    /// <summary>Per-element units when elements have different units, else null.</summary>
    IReadOnlyList<IReadOnlyList<UnitPower>>? ElementUnits { get; }

    /// <summary>True when the result came from iterating a range variable (shown as a table).</summary>
    bool IsRangeTable { get; }
}

public interface IFunctionResultNode : IResultNode
{
    string Signature { get; }
}

public sealed record TraceData(string Label, double[] X, double[] Y, IReadOnlyList<UnitPower> XUnits, IReadOnlyList<UnitPower> YUnits);

public interface ITrace2DResultNode : IResultNode
{
    IReadOnlyList<TraceData> Traces { get; }
}

public sealed record EmptyResultNode : IEmptyResultNode
{
    public static readonly EmptyResultNode Instance = new();
    public ResultValueType ValueType => ResultValueType.Empty;
}

public sealed record UnitedValueResultNode(double DisplayValue, IReadOnlyList<UnitPower> Units) : IUnitedValueResultNode
{
    public ResultValueType ValueType => ResultValueType.UnitedValue;
}

public sealed record StringResultNode(string Value) : IStringResultNode
{
    public ResultValueType ValueType => ResultValueType.String;
}

public sealed record MatrixResultNode(int Rows, int Cols, IReadOnlyList<double> DisplayValues, IReadOnlyList<UnitPower> Units,
    IReadOnlyList<IReadOnlyList<UnitPower>>? ElementUnits, bool IsRangeTable) : IMatrixResultNode
{
    public ResultValueType ValueType => ResultValueType.Matrix;
}

public sealed record FunctionResultNode(string Signature) : IFunctionResultNode
{
    public ResultValueType ValueType => ResultValueType.Function;
}

public sealed record Trace2DResultNode(IReadOnlyList<TraceData> Traces) : ITrace2DResultNode
{
    public ResultValueType ValueType => ResultValueType.Trace2D;
}

/// <summary>The engine's answer for one calculatable.</summary>
public sealed class EngineResult
{
    public IResultNode Value { get; init; } = EmptyResultNode.Instance;
    public IEngineError? Error { get; init; }
    public string? Warning { get; init; }

    /// <summary>Per-child results for aggregates (solve blocks), in the order of their children.</summary>
    public IReadOnlyList<EngineResult>? Children { get; init; }

    public bool HasError => Error != null || (Children?.Any(c => c.HasError) ?? false);

    public static readonly EngineResult Empty = new();
}

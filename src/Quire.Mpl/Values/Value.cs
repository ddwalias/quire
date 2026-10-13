using System.Globalization;

namespace Quire.Values;

/// <summary>Base of every runtime value produced by the engine.</summary>
public abstract class Value;

/// <summary>A real number carrying a physical dimension; <see cref="V"/> is in SI base units.</summary>
public sealed class NumVal(double v, Dim d = default) : Value
{
    public double V { get; } = v;
    public Dim D { get; } = d;

    public static readonly NumVal Zero = new(0);
    public static readonly NumVal One = new(1);
    public static NumVal Bool(bool b) => b ? One : Zero;

    public bool IsUnitless => D.IsNone;

    public override string ToString() =>
        D.IsNone ? V.ToString("G10", CultureInfo.InvariantCulture) : $"{V.ToString("G10", CultureInfo.InvariantCulture)} [{D}]";
}

public sealed class StrVal(string s) : Value
{
    public string S { get; } = s;
    public override string ToString() => $"\"{S}\"";
}

/// <summary>Row-major matrix of numbers. Vectors are single-column matrices.</summary>
public sealed class MatVal : Value
{
    private readonly NumVal[] _data;

    public const int MaxElements = 10_000_000;

    public MatVal(int rows, int cols, NumVal[] data)
    {
        CheckSize(rows, cols);
        if (data.Length != rows * cols) throw new ArgumentException("Matrix data size mismatch");
        Rows = rows;
        Cols = cols;
        _data = data;
    }

    public int Rows { get; }
    public int Cols { get; }
    public int Length => _data.Length;
    public bool IsVector => Cols == 1 || Rows == 1;

    public NumVal this[int r, int c] => _data[r * Cols + c];
    public NumVal this[int i] => _data[i];
    public IReadOnlyList<NumVal> Elements => _data;

    public static MatVal Column(IReadOnlyList<NumVal> items) => new(items.Count, 1, [.. items]);

    public static MatVal Create(int rows, int cols, Func<int, int, NumVal> f)
    {
        CheckSize(rows, cols);
        var data = new NumVal[rows * cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                data[r * cols + c] = f(r, c);
        return new MatVal(rows, cols, data);
    }

    private static void CheckSize(int rows, int cols)
    {
        if (rows < 0 || cols < 0 || (long)rows * cols > MaxElements)
            throw new ArgumentException($"A {rows}×{cols} matrix is too large (limit {MaxElements:N0} elements)");
    }

    public MatVal Map(Func<NumVal, NumVal> f) => new(Rows, Cols, _data.Select(f).ToArray());

    public override string ToString() => $"[{Rows}x{Cols}]";
}

/// <summary>A range variable such as <c>x := 0, 0.5 .. 10</c>; it iterates the expressions that use it.</summary>
public sealed class RngVal(IReadOnlyList<NumVal> values) : Value
{
    public IReadOnlyList<NumVal> Values { get; } = values;
    public MatVal ToColumn() => MatVal.Column(Values);
    public override string ToString() => $"range[{Values.Count}]";
}

/// <summary>A callable: user-defined or built-in.</summary>
public abstract class FunctionValue : Value
{
    public abstract string Name { get; }
    public abstract IReadOnlyList<string> Parameters { get; }
    public override string ToString() => $"{Name}({string.Join(", ", Parameters)})";
}

/// <summary>Plot data produced by a <c>(plot2d …)</c> node: one or more x/y traces in SI magnitudes.</summary>
public sealed class TraceVal(IReadOnlyList<Trace> traces) : Value
{
    public IReadOnlyList<Trace> Traces { get; } = traces;
}

public sealed record Trace(string Label, double[] X, double[] Y, Dim XDim, Dim YDim);

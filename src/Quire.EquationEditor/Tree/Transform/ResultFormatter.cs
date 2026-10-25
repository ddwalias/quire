using System.Globalization;
using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.EquationEditor.Tree.Transform;

public partial interface IResultTreeFormatter
{
    MathExpr? Format(IResultNode result, ResultFormatOptions format);
}

/// <summary>Turns display-ready results into trees so they render like any other math.</summary>
public sealed class ResultFormatter : IResultTreeFormatter
{
    public static readonly ResultFormatter Instance = new();

    /// <summary>The formatter is stateless, so a clone is the same instance.</summary>
    public object Clone() => this;

    public MathExpr? Format(IResultNode result, ResultFormatOptions format) => result switch
    {
        UnitedValueResultNode u => WithUnits(Number(u.DisplayValue, format), u.Units),
        IMatrixResultNode m => WithUnits(Matrix(m, format), m.Units),
        IStringResultNode s => new StringNode(s.Value),
        IFunctionResultNode f => new IdNode(f.Signature),
        _ => null,
    };

    private static MatrixNode Matrix(IMatrixResultNode m, ResultFormatOptions format) =>
        new(m.Rows, m.Cols, m.DisplayValues.Select((v, i) =>
            m.ElementUnits is { } eu ? WithUnits(Number(v, format), eu[i]) : Number(v, format)).ToArray());

    public static MathExpr WithUnits(MathExpr value, IReadOnlyList<UnitPower> units)
    {
        var u = UnitExpression(units);
        return u == null ? value : new BinaryNode("mult", value, u, Implicit: true);
    }

    /// <summary>m·s⁻² becomes m / s^2.</summary>
    public static MathExpr? UnitExpression(IReadOnlyList<UnitPower> units)
    {
        MathExpr? Product(IEnumerable<UnitPower> ps) => ps
            .Select(p => p.Exponent == 1 ? (MathExpr)new IdNode(p.Name) : new BinaryNode("pow", new IdNode(p.Name), Exponent(p.Exponent)))
            .Aggregate((MathExpr?)null, (acc, x) => acc == null ? x : new BinaryNode("mult", acc, x));

        var num = Product(units.Where(p => p.Exponent > 0));
        var den = Product(units.Where(p => p.Exponent < 0).Select(p => p with { Exponent = -p.Exponent }));
        if (den == null) return num;
        return new BinaryNode("div", num ?? new NumberNode("1"), den);
    }

    private static MathExpr Exponent(double e) =>
        new NumberNode(e.ToString("0.###", CultureInfo.InvariantCulture));

    /// <summary>Formats a number, switching to a·10ⁿ outside the exponential threshold.</summary>
    public static MathExpr Number(double v, ResultFormatOptions f)
    {
        if (double.IsNaN(v)) return new IdNode("NaN");
        if (double.IsPositiveInfinity(v)) return new IdNode("∞");
        if (double.IsNegativeInfinity(v)) return new UnaryNode("neg", new IdNode("∞"));
        if (v < 0) return new UnaryNode("neg", Number(-v, f));
        if (v == 0) return new NumberNode("0");

        double rounded = Math.Round(v, Math.Min(f.Decimals, 15), MidpointRounding.AwayFromZero);
        int exp = (int)Math.Floor(Math.Log10(rounded > 0 ? rounded : v));
        bool exponential = exp >= f.ExponentialThreshold || exp < -f.ExponentialThreshold || rounded == 0;
        if (!exponential) return new NumberNode(Fixed(rounded, f));

        double mant = Math.Round(v / Math.Pow(10, exp), f.Decimals, MidpointRounding.AwayFromZero);
        if (mant >= 10) { mant /= 10; exp++; }
        var power = new BinaryNode("pow", new NumberNode("10"), exp < 0 ? new UnaryNode("neg", new NumberNode((-exp).ToString(CultureInfo.InvariantCulture))) : new NumberNode(exp.ToString(CultureInfo.InvariantCulture)));
        return new BinaryNode("mult", new NumberNode(Fixed(mant, f)), power);
    }

    private static string Fixed(double v, ResultFormatOptions f)
    {
        var s = v.ToString("F" + Math.Clamp(f.Decimals, 0, 15), CultureInfo.InvariantCulture);
        if (!f.ShowTrailingZeros && s.Contains('.')) s = s.TrimEnd('0').TrimEnd('.');
        return s;
    }

    /// <summary>Plain-text form of a formatted number (used for tables and plot ticks).</summary>
    public static string NumberText(double v, ResultFormatOptions f) => MathNodeToLinearTransform.Write(Number(v, f)).Replace("*10^", "·10^");
}

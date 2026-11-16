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

    /// <summary>Formats a display result with its units shown after it, as m·s⁻² becomes m / s^2.</summary>
    public MathExpr? Format(IResultNode result, ResultFormatOptions format)
    {
        MathExpr? value;
        IList<Unit>? units = null;
        switch (result)
        {
            case IUnitedValueResultNode united:
                value = Format(united.Value, format);
                units = united.Units;
                break;
            case IMatrixResultNode matrix:
                value = Matrix(matrix, format);
                break;
            case IRealResultNode real:
                value = Number(real.Value, format);
                break;
            case IStringResultNode str:
                value = new StringNode(str.Value);
                break;
            case IFunctionResultNode function:
                value = new IdNode(function.Description);
                break;
            default:
                return null;
        }
        if (value == null || units == null) return value;

        // Positive powers multiply above the line, negative powers below it.
        MathExpr? numerator = null, denominator = null;
        foreach (var unit in units)
        {
            double exponent = (double)unit.PowerNumerator / unit.PowerDenominator;
            if (exponent == 0) continue;
            double magnitude = Math.Abs(exponent);
            MathExpr factor = magnitude == 1 ? new IdNode(unit.Symbol) : new BinaryNode("pow", new IdNode(unit.Symbol), Exponent(magnitude));
            if (exponent > 0) numerator = numerator == null ? factor : new BinaryNode("mult", numerator, factor);
            else denominator = denominator == null ? factor : new BinaryNode("mult", denominator, factor);
        }
        MathExpr? unitExpression = denominator == null ? numerator : new BinaryNode("div", numerator ?? new NumberNode("1"), denominator);
        return unitExpression == null ? value : new BinaryNode("mult", value, unitExpression, Implicit: true);
    }

    /// <summary>Each element is formatted on its own; an element without a display form shows as "?".</summary>
    private static MatrixNode Matrix(IMatrixResultNode m, ResultFormatOptions format)
    {
        var elements = new MathExpr[m.Rows * m.Columns];
        for (int r = 0; r < m.Rows; r++)
        {
            for (int c = 0; c < m.Columns; c++)
            {
                elements[r * m.Columns + c] = Instance.Format(m[r, c], format) ?? new IdNode("?");
            }
        }
        return new MatrixNode(m.Rows, m.Columns, elements);
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

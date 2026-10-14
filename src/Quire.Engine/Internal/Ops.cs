using System.Globalization;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>Dimension-checked arithmetic on engine values.</summary>
internal static class Ops
{
    public static NumVal Num(DimVal v, string what = "value") => v switch
    {
        NumVal n => n,
        MatVal { Rows: 1, Cols: 1 } m => m.At(0, 0),
        _ => throw new CalcException($"Expected a scalar for {what}, got {Describe(v)}"),
    };

    public static double Unitless(DimVal v, string what)
    {
        var n = Num(v, what);
        if (!n.IsDimless) throw new CalcException($"{what} must be unitless (got {n.m_dim})");
        return n.Real;
    }

    public static string Describe(DimVal v) => v switch
    {
        NumVal => "a scalar",
        MatVal m => $"a {m.Rows}×{m.Cols} matrix",
        StrVal => "a string",
        RngVal => "a range",
        Function f => $"function {f.Name}",
        _ => v.GetType().Name,
    };

    public static DimVal Normalize(DimVal v) => v is RngVal r ? r.ToColumn() : v;

    public static DimVal Add(DimVal a, DimVal b) => AddSub(a, b, +1);
    public static DimVal Sub(DimVal a, DimVal b) => AddSub(a, b, -1);

    private static DimVal AddSub(DimVal a, DimVal b, int sign)
    {
        a = Normalize(a); b = Normalize(b);
        string op = sign > 0 ? "+" : "−";
        switch (a, b)
        {
            case (NumVal x, NumVal y):
                if (x.m_dim != y.m_dim) throw CalcException.UnitsMismatch($"{x.m_dim} {op} {y.m_dim}");
                return Val.Num(x.Real + sign * y.Real, x.m_dim);
            case (MatVal x, MatVal y):
                if (x.Rows != y.Rows || x.Cols != y.Cols)
                    throw new CalcException($"Matrix sizes don't match ({x.Rows}×{x.Cols} {op} {y.Rows}×{y.Cols})");
                return Val.Create(x.Rows, x.Cols, (r, c) => (NumVal)AddSub(x.At(r, c), y.At(r, c), sign));
            case (MatVal x, NumVal y):
                return x.Map(e => (NumVal)AddSub(e, y, sign));
            case (NumVal x, MatVal y):
                return y.Map(e => (NumVal)AddSub(x, e, sign));
            case (StrVal x, StrVal y) when sign > 0:
                return new StrVal(x.Field1 + y.Field1);
            default:
                throw new CalcException($"Can't apply {op} to {Describe(a)} and {Describe(b)}");
        }
    }

    public static DimVal Mul(DimVal a, DimVal b)
    {
        a = Normalize(a); b = Normalize(b);
        switch (a, b)
        {
            case (NumVal x, NumVal y):
                return Val.Num(x.Real * y.Real, Dim.Mult(x.m_dim, y.m_dim));
            case (MatVal x, NumVal y):
                return x.Map(e => Val.Num(e.Real * y.Real, Dim.Mult(e.m_dim, y.m_dim)));
            case (NumVal x, MatVal y):
                return y.Map(e => Val.Num(x.Real * e.Real, Dim.Mult(x.m_dim, e.m_dim)));
            case (MatVal x, MatVal y):
                if (x.Cols == 1 && y.Cols == 1 && x.Rows == y.Rows && x.Rows > 1)
                    return Dot(x, y);
                if (x.Cols != y.Rows)
                    throw new CalcException($"Matrix sizes don't allow multiplication ({x.Rows}×{x.Cols} · {y.Rows}×{y.Cols})");
                return Val.Create(x.Rows, y.Cols, (r, c) =>
                {
                    DimVal acc = Mul(x.At(r, 0), y.At(0, c));
                    for (int k = 1; k < x.Cols; k++) acc = Add(acc, Mul(x.At(r, k), y.At(k, c)));
                    return (NumVal)acc;
                });
            default:
                throw new CalcException($"Can't multiply {Describe(a)} by {Describe(b)}");
        }
    }

    public static NumVal Dot(MatVal x, MatVal y)
    {
        if (x.Length() != y.Length()) throw new CalcException("Vectors must have the same length");
        DimVal acc = Mul(x.At(0), y.At(0));
        for (int i = 1; i < x.Length(); i++) acc = Add(acc, Mul(x.At(i), y.At(i)));
        return (NumVal)acc;
    }

    public static DimVal Div(DimVal a, DimVal b)
    {
        a = Normalize(a); b = Normalize(b);
        return (a, b) switch
        {
            (NumVal x, NumVal y) => Val.Num(x.Real / y.Real, Dim.Div(x.m_dim, y.m_dim)),
            (MatVal x, NumVal y) => x.Map(e => Val.Num(e.Real / y.Real, Dim.Div(e.m_dim, y.m_dim))),
            (NumVal, MatVal) => throw new CalcException("Can't divide by a matrix; use M^-1"),
            _ => throw new CalcException($"Can't divide {Describe(a)} by {Describe(b)}"),
        };
    }

    public static DimVal Neg(DimVal a) => Normalize(a) switch
    {
        NumVal x => Val.Num(-x.Real, x.m_dim),
        MatVal m => m.Map(e => Val.Num(-e.Real, e.m_dim)),
        var v => throw new CalcException($"Can't negate {Describe(v)}"),
    };

    /// <summary>The dimension of <paramref name="d"/> raised to <paramref name="p"/>, as a calculation error when
    /// an exponent would not be exact. <see cref="Dim.Pwr"/> itself rounds silently.</summary>
    public static Dim Pwr(Dim d, double p)
    {
        if (Dim.IsDimensioned(d))
        {
            foreach (int c in d.Coordinates)
            {
                double scaled = c * p;
                if (Math.Abs(scaled - Math.Round(scaled)) > 1e-6 || Math.Abs(scaled) > int.MaxValue)
                    throw new CalcException($"Units can't be raised to the power {p.ToString("G6", CultureInfo.InvariantCulture)}");
            }
        }
        return Dim.Pwr(d, p);
    }

    public static DimVal Pow(DimVal a, DimVal b)
    {
        a = Normalize(a);
        double p = Unitless(b, "Exponent");
        switch (a)
        {
            case NumVal x:
            {
                var r = Math.Pow(x.Real, p);
                if (double.IsNaN(r) && !double.IsNaN(x.Real))
                    throw new CalcException("Result is complex (negative base with fractional exponent)");
                return Val.Num(r, Pwr(x.m_dim, p));
            }
            case MatVal m:
            {
                if (m.Rows != m.Cols) throw new CalcException("Only square matrices can be raised to a power");
                if (p != Math.Floor(p)) throw new CalcException("Matrix power must be an integer");
                var basis = p < 0 ? MatrixOps.Inverse(m) : m;
                int n = (int)Math.Abs(p);
                DimVal acc = MatrixOps.Identity(m.Rows, Dim.identity);
                for (int i = 0; i < n; i++) acc = Mul(acc, basis);
                return acc;
            }
            default:
                throw new CalcException($"Can't raise {Describe(a)} to a power");
        }
    }

    public static NumVal Compare(DimVal a, DimVal b, string op)
    {
        a = Normalize(a); b = Normalize(b);
        if (a is StrVal sa && b is StrVal sb)
        {
            return op switch
            {
                "op_eq" => Val.Bool(sa.Field1 == sb.Field1),
                "op_neq" => Val.Bool(sa.Field1 != sb.Field1),
                _ => Val.Bool(Cmp(string.CompareOrdinal(sa.Field1, sb.Field1), 0, op)),
            };
        }
        var x = Num(a, "comparison");
        var y = Num(b, "comparison");
        if (x.m_dim != y.m_dim) throw CalcException.UnitsMismatch("comparison");
        return Val.Bool(Cmp(x.Real, y.Real, op));
    }

    private static bool Cmp(double x, double y, string op) => op switch
    {
        "op_eq" => NumVal.FuzzyEquals(x, y, 1e-12),
        "op_neq" => !Cmp(x, y, "op_eq"),
        "op_lt" => x < y,
        "op_gt" => x > y,
        "op_leq" => x <= y || Cmp(x, y, "op_eq"),
        "op_geq" => x >= y || Cmp(x, y, "op_eq"),
        _ => throw new CalcException($"Unknown comparison {op}"),
    };

    public static bool Truthy(DimVal v) => Num(v, "condition").Real != 0;
}

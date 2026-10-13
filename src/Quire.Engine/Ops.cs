using Quire.Values;

namespace Quire.Engine;

/// <summary>Dimension-checked arithmetic on engine values.</summary>
public static class Ops
{
    public static NumVal Num(Value v, string what = "value") => v switch
    {
        NumVal n => n,
        MatVal { Rows: 1, Cols: 1 } m => m[0],
        _ => throw new CalcException($"Expected a scalar for {what}, got {Describe(v)}"),
    };

    public static double Unitless(Value v, string what)
    {
        var n = Num(v, what);
        if (!n.IsUnitless) throw new CalcException($"{what} must be unitless (got {n.D})");
        return n.V;
    }

    public static string Describe(Value v) => v switch
    {
        NumVal => "a scalar",
        MatVal m => $"a {m.Rows}×{m.Cols} matrix",
        StrVal => "a string",
        RngVal => "a range",
        FunctionValue f => $"function {f.Name}",
        _ => v.GetType().Name,
    };

    public static Value Normalize(Value v) => v is RngVal r ? r.ToColumn() : v;

    public static Value Add(Value a, Value b) => AddSub(a, b, +1);
    public static Value Sub(Value a, Value b) => AddSub(a, b, -1);

    private static Value AddSub(Value a, Value b, int sign)
    {
        a = Normalize(a); b = Normalize(b);
        string op = sign > 0 ? "+" : "−";
        switch (a, b)
        {
            case (NumVal x, NumVal y):
                if (x.D != y.D) throw CalcException.UnitsMismatch($"{x.D} {op} {y.D}");
                return new NumVal(x.V + sign * y.V, x.D);
            case (MatVal x, MatVal y):
                if (x.Rows != y.Rows || x.Cols != y.Cols)
                    throw new CalcException($"Matrix sizes don't match ({x.Rows}×{x.Cols} {op} {y.Rows}×{y.Cols})");
                return MatVal.Create(x.Rows, x.Cols, (r, c) => (NumVal)AddSub(x[r, c], y[r, c], sign));
            case (MatVal x, NumVal y):
                return x.Map(e => (NumVal)AddSub(e, y, sign));
            case (NumVal x, MatVal y):
                return y.Map(e => (NumVal)AddSub(x, e, sign));
            case (StrVal x, StrVal y) when sign > 0:
                return new StrVal(x.S + y.S);
            default:
                throw new CalcException($"Can't apply {op} to {Describe(a)} and {Describe(b)}");
        }
    }

    public static Value Mul(Value a, Value b)
    {
        a = Normalize(a); b = Normalize(b);
        switch (a, b)
        {
            case (NumVal x, NumVal y):
                return new NumVal(x.V * y.V, x.D * y.D);
            case (MatVal x, NumVal y):
                return x.Map(e => new NumVal(e.V * y.V, e.D * y.D));
            case (NumVal x, MatVal y):
                return y.Map(e => new NumVal(x.V * e.V, x.D * e.D));
            case (MatVal x, MatVal y):
                if (x.Cols == 1 && y.Cols == 1 && x.Rows == y.Rows && x.Rows > 1)
                    return Dot(x, y);
                if (x.Cols != y.Rows)
                    throw new CalcException($"Matrix sizes don't allow multiplication ({x.Rows}×{x.Cols} · {y.Rows}×{y.Cols})");
                return MatVal.Create(x.Rows, y.Cols, (r, c) =>
                {
                    Value acc = Mul(x[r, 0], y[0, c]);
                    for (int k = 1; k < x.Cols; k++) acc = Add(acc, Mul(x[r, k], y[k, c]));
                    return (NumVal)acc;
                });
            default:
                throw new CalcException($"Can't multiply {Describe(a)} by {Describe(b)}");
        }
    }

    public static NumVal Dot(MatVal x, MatVal y)
    {
        if (x.Length != y.Length) throw new CalcException("Vectors must have the same length");
        Value acc = Mul(x[0], y[0]);
        for (int i = 1; i < x.Length; i++) acc = Add(acc, Mul(x[i], y[i]));
        return (NumVal)acc;
    }

    public static Value Div(Value a, Value b)
    {
        a = Normalize(a); b = Normalize(b);
        return (a, b) switch
        {
            (NumVal x, NumVal y) => new NumVal(x.V / y.V, x.D / y.D),
            (MatVal x, NumVal y) => x.Map(e => new NumVal(e.V / y.V, e.D / y.D)),
            (NumVal, MatVal) => throw new CalcException("Can't divide by a matrix; use M^-1"),
            _ => throw new CalcException($"Can't divide {Describe(a)} by {Describe(b)}"),
        };
    }

    public static Value Neg(Value a) => Normalize(a) switch
    {
        NumVal x => new NumVal(-x.V, x.D),
        MatVal m => m.Map(e => new NumVal(-e.V, e.D)),
        var v => throw new CalcException($"Can't negate {Describe(v)}"),
    };

    public static Value Pow(Value a, Value b)
    {
        a = Normalize(a);
        double p = Unitless(b, "Exponent");
        switch (a)
        {
            case NumVal x:
            {
                var r = Math.Pow(x.V, p);
                if (double.IsNaN(r) && !double.IsNaN(x.V))
                    throw new CalcException("Result is complex (negative base with fractional exponent)");
                return new NumVal(r, x.D.Pow(p));
            }
            case MatVal m:
            {
                if (m.Rows != m.Cols) throw new CalcException("Only square matrices can be raised to a power");
                if (p != Math.Floor(p)) throw new CalcException("Matrix power must be an integer");
                var basis = p < 0 ? MatrixOps.Inverse(m) : m;
                int n = (int)Math.Abs(p);
                Value acc = MatrixOps.Identity(m.Rows, Dim.None);
                for (int i = 0; i < n; i++) acc = Mul(acc, basis);
                return acc;
            }
            default:
                throw new CalcException($"Can't raise {Describe(a)} to a power");
        }
    }

    public static NumVal Compare(Value a, Value b, string op)
    {
        a = Normalize(a); b = Normalize(b);
        if (a is StrVal sa && b is StrVal sb)
        {
            return op switch
            {
                "op_eq" => NumVal.Bool(sa.S == sb.S),
                "op_neq" => NumVal.Bool(sa.S != sb.S),
                _ => NumVal.Bool(Cmp(string.CompareOrdinal(sa.S, sb.S), 0, op)),
            };
        }
        var x = Num(a, "comparison");
        var y = Num(b, "comparison");
        if (x.D != y.D) throw CalcException.UnitsMismatch("comparison");
        return NumVal.Bool(Cmp(x.V, y.V, op));
    }

    private static bool Cmp(double x, double y, string op) => op switch
    {
        "op_eq" => x == y || Math.Abs(x - y) <= 1e-12 * Math.Max(Math.Abs(x), Math.Abs(y)),
        "op_neq" => !Cmp(x, y, "op_eq"),
        "op_lt" => x < y,
        "op_gt" => x > y,
        "op_leq" => x <= y || Cmp(x, y, "op_eq"),
        "op_geq" => x >= y || Cmp(x, y, "op_eq"),
        _ => throw new CalcException($"Unknown comparison {op}"),
    };

    public static bool Truthy(Value v) => Num(v, "condition").V != 0;
}

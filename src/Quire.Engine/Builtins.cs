using System.Globalization;
using Quire.Values;

namespace Quire.Engine;

/// <summary>The built-in function library.</summary>
public static class Builtins
{
    private static readonly Dictionary<string, BuiltinFunction> Table = new(StringComparer.Ordinal);

    public static bool TryGet(string name, out BuiltinFunction fn) => Table.TryGetValue(name, out fn!);
    public static IEnumerable<string> Names => Table.Keys;

    private static void Def(string name, int min, int max, Func<Value[], Interpreter, Value> f) =>
        Table[name] = new BuiltinFunction(name, min, max, f);

    private static void Def(string name, int arity, Func<Value[], Value> f) => Def(name, arity, arity, (a, _) => f(a));

    /// <summary>Unitless scalar function applied element-wise to matrices.</summary>
    private static void Scalar(string name, Func<double, double> f) =>
        Def(name, 1, a => Map(a[0], x => new NumVal(Check(f(Unitless(x, name)), name))));

    private static Value Map(Value v, Func<NumVal, NumVal> f) => Ops.Normalize(v) switch
    {
        NumVal n => f(n),
        MatVal m => m.Map(f),
        var other => throw new CalcException($"Expected a number, got {Ops.Describe(other)}"),
    };

    private static double Unitless(NumVal n, string fn)
    {
        if (!n.IsUnitless) throw new CalcException($"{fn}: the argument must be unitless (got {n.D})", fn);
        return n.V;
    }

    private static double Check(double v, string fn)
    {
        if (double.IsNaN(v)) throw new CalcException($"{fn}: the result is not a real number", fn);
        return v;
    }

    private static IReadOnlyList<NumVal> Flatten(Value[] args, string fn)
    {
        var list = new List<NumVal>();
        foreach (var a in args)
        {
            switch (Ops.Normalize(a))
            {
                case NumVal n: list.Add(n); break;
                case MatVal m: list.AddRange(m.Elements); break;
                default: throw new CalcException($"{fn}: expected numbers", fn);
            }
        }
        if (list.Count == 0) throw new CalcException($"{fn}: no values", fn);
        var d = list[0].D;
        if (list.Any(x => x.D != d)) throw CalcException.UnitsMismatch(fn);
        return list;
    }

    private static MatVal Vec(Value v, string fn)
    {
        var m = MatrixOps.AsMatrix(v, fn);
        if (!m.IsVector) throw new CalcException($"{fn}: expected a vector", fn);
        return m;
    }

    private static int Int(Value v, string what)
    {
        double d = Ops.Unitless(v, what);
        if (d != Math.Floor(d)) throw new CalcException($"{what} must be an integer");
        return (int)d;
    }

    static Builtins()
    {
        // trigonometric and elementary
        Scalar("sin", Math.Sin); Scalar("cos", Math.Cos); Scalar("tan", Math.Tan);
        Scalar("cot", x => 1 / Math.Tan(x)); Scalar("sec", x => 1 / Math.Cos(x)); Scalar("csc", x => 1 / Math.Sin(x));
        Scalar("asin", Math.Asin); Scalar("acos", Math.Acos); Scalar("atan", Math.Atan);
        Scalar("sinh", Math.Sinh); Scalar("cosh", Math.Cosh); Scalar("tanh", Math.Tanh);
        Scalar("asinh", Math.Asinh); Scalar("acosh", Math.Acosh); Scalar("atanh", Math.Atanh);
        Scalar("exp", Math.Exp); Scalar("ln", Math.Log);
        Scalar("erf", Erf); Scalar("erfc", x => 1 - Erf(x)); Scalar("Γ", Gamma); Scalar("gamma", Gamma);
        Def("atan2", 2, a =>
        {
            var x = Ops.Num(a[0]); var y = Ops.Num(a[1]);
            if (x.D != y.D) throw CalcException.UnitsMismatch("atan2");
            return new NumVal(Math.Atan2(y.V, x.V));
        });
        Def("log", 1, 2, (a, _) =>
        {
            double b = a.Length == 2 ? Ops.Unitless(a[1], "log base") : 10;
            return Map(a[0], x => new NumVal(Check(Math.Log(Unitless(x, "log")) / Math.Log(b), "log")));
        });
        Def("sqrt", 1, a => Sqrt(a[0]));
        Def("nthroot", 2, a => NthRoot(a[0], a[1]));
        Def("abs", 1, a => Abs(a[0]));
        Def("fact", 1, a => Factorial(a[0]));
        Def("sign", 1, a => Map(a[0], x => new NumVal(Math.Sign(x.V))));
        Def("signum", 1, a => Map(a[0], x => x.V == 0 ? NumVal.Zero : new NumVal(x.V / Math.Abs(x.V))));
        Scalar("floor", Math.Floor); Scalar("ceil", Math.Ceiling); Scalar("trunc", Math.Truncate);
        Def("round", 1, 2, (a, _) =>
        {
            int n = a.Length == 2 ? Int(a[1], "round digits") : 0;
            return Map(a[0], x => new NumVal(n >= 0
                ? Math.Round(Unitless(x, "round"), Math.Min(n, 15), MidpointRounding.AwayFromZero)
                : Math.Round(Unitless(x, "round") / Math.Pow(10, -n), MidpointRounding.AwayFromZero) * Math.Pow(10, -n)));
        });
        Def("mod", 2, a =>
        {
            var x = Ops.Num(a[0]); var y = Ops.Num(a[1]);
            if (x.D != y.D) throw CalcException.UnitsMismatch("mod");
            if (y.V == 0) throw new CalcException("mod: division by zero");
            return new NumVal(x.V % y.V, x.D);
        });
        Def("gcd", 2, int.MaxValue, (a, _) => new NumVal(a.Select(v => (long)Ops.Unitless(v, "gcd")).Aggregate(Gcd)));
        Def("lcm", 2, int.MaxValue, (a, _) => new NumVal(a.Select(v => (long)Ops.Unitless(v, "lcm")).Aggregate((x, y) => x / Gcd(x, y) * y)));
        Def("UnitsOf", 1, a => new NumVal(1, Ops.Num(a[0]).D));

        // statistics and aggregates
        Def("max", 1, int.MaxValue, (a, _) => Flatten(a, "max").MaxBy(x => x.V)!);
        Def("min", 1, int.MaxValue, (a, _) => Flatten(a, "min").MinBy(x => x.V)!);
        Def("mean", 1, int.MaxValue, (a, _) => { var l = Flatten(a, "mean"); return new NumVal(l.Average(x => x.V), l[0].D); });
        Def("median", 1, int.MaxValue, (a, _) =>
        {
            var l = Flatten(a, "median"); var s = l.Select(x => x.V).Order().ToArray();
            double m = s.Length % 2 == 1 ? s[s.Length / 2] : (s[s.Length / 2 - 1] + s[s.Length / 2]) / 2;
            return new NumVal(m, l[0].D);
        });
        Def("var", 1, int.MaxValue, (a, _) => Variance(Flatten(a, "var"), sample: false));
        Def("Var", 1, int.MaxValue, (a, _) => Variance(Flatten(a, "Var"), sample: true));
        Def("stdev", 1, int.MaxValue, (a, _) => Sqrt(Variance(Flatten(a, "stdev"), sample: false)));
        Def("Stdev", 1, int.MaxValue, (a, _) => Sqrt(Variance(Flatten(a, "Stdev"), sample: true)));

        // vectors and matrices
        Def("length", 1, a => new NumVal(Vec(a[0], "length").Length));
        Def("rows", 1, a => new NumVal(MatrixOps.AsMatrix(a[0], "rows").Rows));
        Def("cols", 1, a => new NumVal(MatrixOps.AsMatrix(a[0], "cols").Cols));
        Def("last", 1, 1, (a, i) => new NumVal(Vec(a[0], "last").Length - 1 + i.Origin(i.CallSiteEnv)));
        Def("identity", 1, a => MatrixOps.Identity(Int(a[0], "identity size"), Dim.None));
        Def("transpose", 1, a => MatrixOps.Transpose(MatrixOps.AsMatrix(a[0], "transpose")));
        Def("inverse", 1, a => MatrixOps.Inverse(MatrixOps.AsMatrix(a[0], "inverse")));
        Def("det", 1, a => MatrixOps.Determinant(MatrixOps.AsMatrix(a[0], "det")));
        Def("tr", 1, a =>
        {
            var m = MatrixOps.AsMatrix(a[0], "tr");
            if (m.Rows != m.Cols) throw new CalcException("tr requires a square matrix");
            Value acc = m[0, 0];
            for (int i = 1; i < m.Rows; i++) acc = Ops.Add(acc, m[i, i]);
            return acc;
        });
        Def("rank", 1, a => new NumVal(Rank(MatrixOps.Uniform(MatrixOps.AsMatrix(a[0], "rank"), "rank").Values)));
        Def("lsolve", 2, a => MatrixOps.LSolve(MatrixOps.AsMatrix(a[0], "lsolve"), MatrixOps.AsMatrix(a[1], "lsolve")));
        Def("augment", 1, int.MaxValue, (a, _) => MatrixOps.Augment(a.Select(x => MatrixOps.AsMatrix(x, "augment")).ToList()));
        Def("stack", 1, int.MaxValue, (a, _) => MatrixOps.Stack(a.Select(x => MatrixOps.AsMatrix(x, "stack")).ToList()));
        Def("submatrix", 5, 5, (a, i) =>
        {
            var m = MatrixOps.AsMatrix(a[0], "submatrix");
            int o = i.Origin(i.CallSiteEnv);
            int r0 = Int(a[1], "row") - o, r1 = Int(a[2], "row") - o, c0 = Int(a[3], "column") - o, c1 = Int(a[4], "column") - o;
            if (r0 < 0 || c0 < 0 || r1 >= m.Rows || c1 >= m.Cols || r1 < r0 || c1 < c0)
                throw new CalcException("submatrix: indices are out of range");
            return MatVal.Create(r1 - r0 + 1, c1 - c0 + 1, (r, c) => m[r0 + r, c0 + c]);
        });
        Def("col", 2, 2, (a, i) =>
        {
            var m = MatrixOps.AsMatrix(a[0], "col");
            int c = Int(a[1], "column") - i.Origin(i.CallSiteEnv);
            if (c < 0 || c >= m.Cols) throw new CalcException("col: index is out of range");
            return MatVal.Create(m.Rows, 1, (r, _) => m[r, c]);
        });
        Def("row", 2, 2, (a, i) =>
        {
            var m = MatrixOps.AsMatrix(a[0], "row");
            int r = Int(a[1], "row") - i.Origin(i.CallSiteEnv);
            if (r < 0 || r >= m.Rows) throw new CalcException("row: index is out of range");
            return MatVal.Create(1, m.Cols, (_, c) => m[r, c]);
        });
        Def("matrix", 3, 3, (a, i) =>
        {
            int rows = Int(a[0], "rows"), cols = Int(a[1], "cols");
            var f = a[2] as FunctionValue ?? throw new CalcException("matrix(m, n, f): f must be a function of (i, j)");
            int o = i.Origin(i.CallSiteEnv);
            return MatVal.Create(rows, cols, (r, c) => Ops.Num(i.Call(f, [new NumVal(r + o), new NumVal(c + o)]), "matrix element"));
        });
        Def("linspace", 3, a =>
        {
            var lo = Ops.Num(a[0]); var hi = Ops.Num(a[1]); int n = Int(a[2], "count");
            if (lo.D != hi.D) throw CalcException.UnitsMismatch("linspace");
            if (n < 2) throw new CalcException("linspace needs at least 2 points");
            if (n > MatVal.MaxElements) throw new CalcException("linspace: too many points");
            return MatVal.Column(Enumerable.Range(0, n).Select(k => new NumVal(lo.V + (hi.V - lo.V) * k / (n - 1), lo.D)).ToArray());
        });
        Def("sort", 1, a => MatVal.Column(Vec(a[0], "sort").Elements.OrderBy(x => x.V).ToArray()));
        Def("reverse", 1, a => MatVal.Column(Vec(a[0], "reverse").Elements.Reverse().ToArray()));
        Def("cross", 2, a =>
        {
            var u = Vec(a[0], "cross"); var v = Vec(a[1], "cross");
            if (u.Length != 3 || v.Length != 3) throw new CalcException("cross needs two 3-element vectors");
            NumVal C(int i, int j) => (NumVal)Ops.Sub(Ops.Mul(u[i], v[j]), Ops.Mul(u[j], v[i]));
            return MatVal.Column([C(1, 2), C(2, 0), C(0, 1)]);
        });
        Def("dot", 2, a => Ops.Dot(Vec(a[0], "dot"), Vec(a[1], "dot")));
        Def("norme", 1, a => { var v = Vec(a[0], "norme"); return Sqrt(Ops.Dot(v, v)); });
        Def("linterp", 3, a => Interpolate(a, "linterp", spline: false));
        Def("spline", 3, a => Interpolate(a, "spline", spline: true));

        // strings
        Def("concat", 1, int.MaxValue, (a, _) => new StrVal(string.Concat(a.Select(x => x is StrVal s ? s.S : throw new CalcException("concat expects strings")))));
        Def("strlen", 1, a => a[0] is StrVal s ? new NumVal(s.S.Length) : throw new CalcException("strlen expects a string"));
        Def("num2str", 1, a => new StrVal(Ops.Num(a[0]).V.ToString("G15", CultureInfo.InvariantCulture)));
        Def("str2num", 1, a => a[0] is StrVal s && double.TryParse(s.S, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)
            ? new NumVal(d) : throw new CalcException("str2num: not a number"));
        Def("error", 1, a => throw new CalcException(a[0] is StrVal s ? s.S : "error"));
    }

    // ---------------------------------------------------------------- shared helpers

    public static Value Sqrt(Value v) => Map(v, x =>
    {
        if (x.V < 0) throw new CalcException("sqrt of a negative number is not real");
        return new NumVal(Math.Sqrt(x.V), x.D.Pow(0.5));
    });

    public static Value NthRoot(Value v, Value nv)
    {
        int n = Int(nv, "root index");
        if (n == 0) throw new CalcException("Root index can't be zero");
        return Map(v, x =>
        {
            double r;
            if (x.V < 0)
            {
                if (n % 2 == 0) throw new CalcException("Even root of a negative number is not real");
                r = -Math.Pow(-x.V, 1.0 / n);
            }
            else r = Math.Pow(x.V, 1.0 / n);
            return new NumVal(r, x.D.Pow(1.0 / n));
        });
    }

    public static Value Abs(Value v) => Ops.Normalize(v) switch
    {
        NumVal n => new NumVal(Math.Abs(n.V), n.D),
        MatVal m when m.IsVector && m.Length > 1 => Sqrt(Ops.Dot(m, m)),
        MatVal m when m.Rows == m.Cols => MatrixOps.Determinant(m),
        var other => throw new CalcException($"Can't take |…| of {Ops.Describe(other)}"),
    };

    public static Value Factorial(Value v) => Map(v, x =>
    {
        double n = Unitless(x, "factorial");
        if (n < 0 || n != Math.Floor(n)) throw new CalcException("Factorial needs a non-negative integer");
        if (n > 170) throw new CalcException("Factorial is too large");
        double r = 1;
        for (int i = 2; i <= n; i++) r *= i;
        return new NumVal(r);
    });

    private static NumVal Variance(IReadOnlyList<NumVal> l, bool sample)
    {
        int n = l.Count;
        if (sample && n < 2) throw new CalcException("Sample variance needs at least two values");
        double m = l.Average(x => x.V);
        double s = l.Sum(x => (x.V - m) * (x.V - m)) / (sample ? n - 1 : n);
        return new NumVal(s, l[0].D.Pow(2));
    }

    private static Value Interpolate(Value[] a, string fn, bool spline)
    {
        var vx = Vec(a[0], fn); var vy = Vec(a[1], fn);
        if (vx.Length != vy.Length || vx.Length < 2) throw new CalcException($"{fn}: vx and vy need the same length (≥ 2)");
        var xd = vx[0].D; var yd = vy[0].D;
        var xs = vx.Elements.Select(e => e.V).ToArray();
        var ys = vy.Elements.Select(e => e.V).ToArray();
        for (int i = 1; i < xs.Length; i++)
            if (xs[i] <= xs[i - 1]) throw new CalcException($"{fn}: vx must be strictly increasing");
        var y2 = spline ? Numerics.SplineCoefficients(xs, ys) : null;
        return Map(a[2], x =>
        {
            if (x.D != xd) throw CalcException.UnitsMismatch(fn);
            double y;
            if (y2 != null) y = Numerics.SplineEval(xs, ys, y2, x.V);
            else
            {
                int k = Array.BinarySearch(xs, x.V);
                if (k < 0) k = Math.Clamp(~k - 1, 0, xs.Length - 2);
                else k = Math.Min(k, xs.Length - 2);
                double t = (x.V - xs[k]) / (xs[k + 1] - xs[k]);
                y = ys[k] + t * (ys[k + 1] - ys[k]);
            }
            return new NumVal(y, yd);
        });
    }

    private static long Gcd(long a, long b)
    {
        a = Math.Abs(a); b = Math.Abs(b);
        while (b != 0) (a, b) = (b, a % b);
        return a;
    }

    private static int Rank(double[,] a)
    {
        int rows = a.GetLength(0), cols = a.GetLength(1), rank = 0;
        var m = (double[,])a.Clone();
        double tol = 1e-12 * Math.Max(1, m.Cast<double>().Max(Math.Abs));
        for (int c = 0; c < cols && rank < rows; c++)
        {
            int p = rank;
            for (int r = rank + 1; r < rows; r++) if (Math.Abs(m[r, c]) > Math.Abs(m[p, c])) p = r;
            if (Math.Abs(m[p, c]) <= tol) continue;
            for (int j = 0; j < cols; j++) (m[rank, j], m[p, j]) = (m[p, j], m[rank, j]);
            for (int r = rank + 1; r < rows; r++)
            {
                double f = m[r, c] / m[rank, c];
                for (int j = c; j < cols; j++) m[r, j] -= f * m[rank, j];
            }
            rank++;
        }
        return rank;
    }

    private static double Erf(double x)
    {
        // Abramowitz–Stegun 7.1.26 refined with a series for small |x|.
        if (Math.Abs(x) < 2)
        {
            double sum = x, term = x, x2 = x * x;
            for (int n = 1; n < 60; n++)
            {
                term *= -x2 / n;
                sum += term / (2 * n + 1);
                if (Math.Abs(term) < 1e-17) break;
            }
            return 2 / Math.Sqrt(Math.PI) * sum;
        }
        // continued fraction for erfc
        double ax = Math.Abs(x), f = 0;
        for (int n = 60; n >= 1; n--) f = n / 2.0 / (ax + f);
        double erfc = Math.Exp(-ax * ax) / Math.Sqrt(Math.PI) / (ax + f);
        return Math.Sign(x) * (1 - erfc);
    }

    private static double Gamma(double x)
    {
        if (x < 0.5) return Math.PI / (Math.Sin(Math.PI * x) * Gamma(1 - x));
        double[] g = [0.99999999999980993, 676.5203681218851, -1259.1392167224028, 771.32342877765313,
            -176.61502916214059, 12.507343278686905, -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7];
        x -= 1;
        double a = g[0], t = x + 7.5;
        for (int i = 1; i < 9; i++) a += g[i] / (x + i);
        return Math.Sqrt(2 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a;
    }
}

using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>Functions whose arguments are evaluated lazily or bind variables.</summary>
internal sealed partial class Interpreter
{
    private static readonly HashSet<string> SpecialForms =
        ["if", "sum", "prod", "Σ", "∏", "integral", "deriv", "root", "find", "minerr", "minimize", "maximize"];

    public static bool IsSpecialForm(string name) => SpecialForms.Contains(name);

    private DimVal EvalSpecial(string name, Sexpr[] args, Env env)
    {
        switch (name)
        {
            case "if":
                RequireArgs(name, args, 3, 3);
                return Ops.Truthy(Eval(args[0], env)) ? Eval(args[1], env) : Eval(args[2], env);
            case "sum" or "Σ": return SumProduct(args, env, product: false);
            case "prod" or "∏": return SumProduct(args, env, product: true);
            case "integral": return Integral(args, env);
            case "deriv": return Derivative(args, env);
            case "root": return Root(args, env);
            case "find": return Find(args, env, requireSolution: true);
            case "minerr": return Find(args, env, requireSolution: false);
            case "minimize": return Optimize(args, env, maximize: false);
            case "maximize": return Optimize(args, env, maximize: true);
            default: throw new CalcException($"Unknown function {name}");
        }
    }

    private static void RequireArgs(string name, Sexpr[] args, int min, int max)
    {
        if (args.Length < min || args.Length > max)
            throw new CalcException(min == max
                ? $"{name} expects {min} argument(s)"
                : $"{name} expects {min}–{max} arguments", name);
    }

    private static string VarName(Sexpr e, string fn) =>
        Rl.SymName(e) ?? throw new CalcException($"{fn}: expected a variable name, got {e}");

    /// <summary>sum(v) · sum(expr, i) over range i · sum(expr, i, a, b) over integers a..b.</summary>
    private DimVal SumProduct(Sexpr[] args, Env env, bool product)
    {
        string fn = product ? "prod" : "sum";
        RequireArgs(fn, args, 1, 4);
        IEnumerable<DimVal> terms;
        switch (args.Length)
        {
            case 1:
                terms = MatrixOps.AsMatrix(Eval(args[0], env), fn).Elements();
                break;
            case 2:
            {
                var v = VarName(args[1], fn);
                if (!env.TryLookupUser(v, out var r) || r is not RngVal range)
                    throw new CalcException($"{fn}(expr, {v}): {v} must be a range variable", v);
                terms = range.Values().Select(x => Eval(args[0], env.With(v, x)));
                break;
            }
            case 4:
            {
                var v = VarName(args[1], fn);
                double a = Ops.Unitless(Eval(args[2], env), $"{fn} lower limit");
                double b = Ops.Unitless(Eval(args[3], env), $"{fn} upper limit");
                if (a != Math.Floor(a) || b != Math.Floor(b)) throw new CalcException($"{fn} limits must be integers");
                if (b - a > MaxIterations) throw new CalcException($"{fn} has too many terms");
                terms = Enumerable.Range(0, Math.Max(0, (int)(b - a) + 1)).Select(i => Eval(args[0], env.With(v, new NumVal(a + i))));
                break;
            }
            default:
                throw new CalcException($"{fn} takes (vector), (expr, i) or (expr, i, a, b)");
        }

        DimVal? acc = null;
        foreach (var t in terms)
        {
            CheckCancelled();
            acc = acc == null ? t : product ? Ops.Mul(acc, t) : Ops.Add(acc, t);
        }
        return acc ?? (product ? NumVal.One : NumVal.Zero);
    }

    /// <summary>integral(expr, x, a, b)</summary>
    private DimVal Integral(Sexpr[] args, Env env)
    {
        RequireArgs("integral", args, 4, 4);
        var v = VarName(args[1], "integral");
        var a = EvalNum(args[2], env, "lower limit");
        var b = EvalNum(args[3], env, "upper limit");
        var xd = double.IsInfinity(a.Real) ? b.m_dim : a.m_dim;
        if (!double.IsInfinity(a.Real) && !double.IsInfinity(b.Real) && a.m_dim != b.m_dim) throw CalcException.UnitsMismatch("integral limits");

        Dim? bodyDim = null;
        double F(double t)
        {
            var y = EvalNum(args[0], env.With(v, Val.Num(t, xd)), "integrand");
            if (bodyDim == null) bodyDim = y.m_dim;
            else if (bodyDim != y.m_dim) throw CalcException.UnitsMismatch("integrand");
            return y.Real;
        }
        var result = Numerics.Integrate(F, a.Real, b.Real);
        return Val.Num(result, Dim.Mult(bodyDim ?? Dim.identity, xd));
    }

    /// <summary>deriv(expr, x [, n]) evaluated at the current value of x.</summary>
    private DimVal Derivative(Sexpr[] args, Env env)
    {
        RequireArgs("deriv", args, 2, 3);
        var v = VarName(args[1], "deriv");
        if (!env.TryLookupUser(v, out var at)) throw new CalcException($"deriv: give {v} a value to evaluate the derivative at", v);
        var x0 = Ops.Num(at, v);
        int order = args.Length == 3 ? (int)Ops.Unitless(Eval(args[2], env), "derivative order") : 1;
        if (order < 0 || order > 5) throw new CalcException("Derivative order must be between 0 and 5");
        Dim bodyDim = Dim.identity;
        double F(double t)
        {
            var y = EvalNum(args[0], env.With(v, Val.Num(t, x0.m_dim)), "derivative body");
            bodyDim = y.m_dim;
            return y.Real;
        }
        var d = Numerics.Derivative(F, x0.Real, order);
        return Val.Num(d, Dim.Div(bodyDim, Ops.Pwr(x0.m_dim, order)));
    }

    /// <summary>root(expr, x) from the current value of x, or root(expr, x, a, b) on a bracket.</summary>
    private DimVal Root(Sexpr[] args, Env env)
    {
        RequireArgs("root", args, 2, 4);
        var v = VarName(args[1], "root");
        NumVal? lo = null, hi = null;
        if (args.Length == 4)
        {
            lo = EvalNum(args[2], env, "root bracket");
            hi = EvalNum(args[3], env, "root bracket");
            if (lo.m_dim != hi.m_dim) throw CalcException.UnitsMismatch("root bracket");
        }
        else if (args.Length == 3) throw new CalcException("root takes (expr, x) or (expr, x, a, b)");

        NumVal guess;
        if (lo != null) guess = lo;
        else if (env.TryLookupUser(v, out var g)) guess = Ops.Num(g, v);
        else throw new CalcException($"root: give {v} a guess value first", v);

        double F(double t) => EvalNum(args[0], env.With(v, Val.Num(t, guess.m_dim)), "root expression").Real;
        double x = lo != null ? Numerics.Brent(F, lo.Real, hi!.Real) : Numerics.FindRoot(F, guess.Real);
        return Val.Num(x, guess.m_dim);
    }

    // ---------------------------------------------------------------- solve blocks

    private (string[] Names, NumVal[] Guesses) Unknowns(Sexpr[] args, Env env, string fn)
    {
        if (args.Length == 0) throw new CalcException($"{fn} needs the unknowns, e.g. {fn}(x, y)");
        var names = args.Select(a => VarName(a, fn)).ToArray();
        var guesses = names.Select(n =>
        {
            if (!env.TryLookupUser(n, out var g)) throw new CalcException($"{fn}: {n} needs a guess value", n);
            return Ops.Num(g, $"guess for {n}");
        }).ToArray();
        return (names, guesses);
    }

    private Env Bind(Env env, string[] names, NumVal[] like, double[] x)
    {
        for (int i = 0; i < names.Length; i++) env = env.With(names[i], Val.Num(x[i], like[i].m_dim));
        return env;
    }

    /// <summary>Residuals of every constraint at the given unknowns, plus each residual's magnitude scale.</summary>
    private (double[] Residuals, double[] Scales) Residuals(Env env)
    {
        var res = new List<double>();
        var scales = new List<double>();
        foreach (var c in Solve!.Constraints)
        {
            var l = Ops.Normalize(Eval(Rl.Nth(c, 1), env));
            var r = Ops.Normalize(Eval(Rl.Nth(c, 2), env));
            var ls = l is MatVal lm ? lm.Elements() : [Ops.Num(l)];
            var rs = r is MatVal rm ? rm.Elements() : [Ops.Num(r)];
            if (ls.Count != rs.Count) throw new CalcException("Constraint sides have different sizes");
            for (int i = 0; i < ls.Count; i++)
            {
                if (ls[i].m_dim != rs[i].m_dim) throw CalcException.UnitsMismatch("constraint");
                double diff = ls[i].Real - rs[i].Real;
                res.Add(Rl.Head(c) switch
                {
                    "op_eq" => diff,
                    "op_lt" or "op_leq" => Math.Max(0, diff),
                    "op_gt" or "op_geq" => Math.Max(0, -diff),
                    _ => throw new CalcException("Constraints must be comparisons such as x + y = 3"),
                });
                scales.Add(Math.Max(Math.Abs(ls[i].Real), Math.Abs(rs[i].Real)));
            }
        }
        return ([.. res], [.. scales]);
    }

    private DimVal Find(Sexpr[] args, Env env, bool requireSolution)
    {
        string fn = requireSolution ? "find" : "minerr";
        if (Solve == null || Solve.Constraints.Count == 0)
            throw new CalcException($"{fn} must be used in a solve block with at least one constraint", fn);
        var (names, guesses) = Unknowns(args, env, fn);
        var x0 = guesses.Select(g => g.Real).ToArray();
        var (_, scale0) = Residuals(Bind(env, names, guesses, x0));
        double typical = scale0.DefaultIfEmpty(0).Max();

        var x = Numerics.LevenbergMarquardt(x => Residuals(Bind(env, names, guesses, x)).Residuals, x0);

        if (requireSolution)
        {
            var (r, s) = Residuals(Bind(env, names, guesses, x));
            for (int i = 0; i < r.Length; i++)
            {
                double tol = Math.Max(1e-6 * s[i], 1e-10 * typical);
                if (!(Math.Abs(r[i]) <= tol))
                    throw new CalcException("No solution was found. Try other guess values or use minerr.", fn);
            }
        }
        return Pack(names, guesses, x);
    }

    private static DimVal Pack(string[] names, NumVal[] like, double[] x) =>
        names.Length == 1
            ? Val.Num(x[0], like[0].m_dim)
            : Val.Column(x.Select((v, i) => Val.Num(v, like[i].m_dim)).ToArray());

    /// <summary>minimize(f, x, y…) / maximize(f, x, y…), honouring solve-block constraints as penalties.</summary>
    private DimVal Optimize(Sexpr[] args, Env env, bool maximize)
    {
        string fn = maximize ? "maximize" : "minimize";
        if (args.Length < 2) throw new CalcException($"{fn}(f, x, …) needs a function and its variables");
        var f = Eval(args[0], env) as Function ?? throw new CalcException($"{fn}: first argument must be a function name");
        var (names, guesses) = Unknowns(args[1..], env, fn);
        var x0 = guesses.Select(g => g.Real).ToArray();
        double sign = maximize ? -1 : 1;
        double Objective(double[] x) => sign * Ops.Num(Call(f, x.Select((v, i) => (DimVal)Val.Num(v, guesses[i].m_dim)).ToArray()), "objective").Real;

        double weight = 1e8 * (1 + Math.Abs(Objective(x0)));
        double Penalised(double[] x)
        {
            double obj = Objective(x);
            if (Solve == null || Solve.Constraints.Count == 0) return obj;
            var (r, s) = Residuals(Bind(env, names, guesses, x));
            double p = 0;
            for (int i = 0; i < r.Length; i++) p += Math.Pow(r[i] / Math.Max(s[i], 1e-12), 2);
            return obj + weight * p;
        }
        var x = Numerics.NelderMead(Penalised, x0);
        x = Numerics.NelderMead(Penalised, x); // restart to escape a collapsed simplex
        return Pack(names, guesses, x);
    }

    internal void WithSolveContext(SolveContext? ctx, Action action)
    {
        var saved = Solve;
        Solve = ctx;
        try { action(); }
        finally { Solve = saved; }
    }
}

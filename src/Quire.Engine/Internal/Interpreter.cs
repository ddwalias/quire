using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>Solve-block constraints visible to <c>find</c>, <c>minerr</c>, <c>minimize</c> and <c>maximize</c>.</summary>
internal sealed record SolveContext(IReadOnlyList<SCons> Constraints);

/// <summary>Evaluates Rl expressions and statements.</summary>
internal sealed partial class Interpreter
{
    public const int MaxCallDepth = 400;
    public const int MaxIterations = 1_000_000;

    private int _depth;

    public SolveContext? Solve { get; private set; }

    /// <summary>The scope of the innermost function call site; built-ins use it to read ORIGIN.</summary>
    public Env CallSiteEnv { get; private set; } = Env.Empty;

    public int Origin(Env env) =>
        env.TryLookupUser("ORIGIN", out var o) ? (int)Ops.Unitless(o, "ORIGIN") : 0;

    private static void CheckCancelled()
    {
        if (Rts.IsCancellationRequested()) throw new OperationCanceledException();
    }

    // ---------------------------------------------------------------- expressions

    public DimVal Eval(Sexpr e, Env env)
    {
        switch (e)
        {
            case Tok { Field1: real_constant r }: return new NumVal(r.Field1);
            case Tok { Field1: int_constant i }: return new NumVal(i.Field1);
            case Tok { Field1: str_constant s }: return new StrVal(s.Field1);
            case Tok { Field1: Sym s }: return env.Lookup(s.name);
            case SCons l: return EvalList(l, env);
            default: throw new CalcException("Unknown expression");
        }
    }

    public NumVal EvalNum(Sexpr e, Env env, string what = "value") => Ops.Num(Eval(e, env), what);

    private DimVal EvalList(SCons l, Env env)
    {
        CheckCancelled();
        var head = Rl.Head(l) ?? throw new CalcException("Malformed expression");
        Sexpr A1() => Rl.Nth(l, 1);
        Sexpr A2() => Rl.Nth(l, 2);
        switch (head)
        {
            case "op_add": return Ops.Add(Eval(A1(), env), Eval(A2(), env));
            case "op_sub": return Ops.Sub(Eval(A1(), env), Eval(A2(), env));
            case "op_mult": return EvalMult(l, env);
            case "op_div": return Ops.Div(Eval(A1(), env), Eval(A2(), env));
            case "op_power": return Ops.Pow(Eval(A1(), env), Eval(A2(), env));
            case "op_negate": return Ops.Neg(Eval(A1(), env));
            case "op_parens": return Eval(A1(), env);
            case "op_factorial": return Builtins.Factorial(Eval(A1(), env));
            case "op_abs": return Builtins.Abs(Eval(A1(), env));
            case "op_sqrt": return Builtins.Sqrt(Eval(A1(), env));
            case "op_rootn": return Builtins.NthRoot(Eval(A2(), env), Eval(A1(), env));
            case "op_transpose": return MatrixOps.AsMatrix(Eval(A1(), env), "Transpose").Transpose();
            case "op_det": return MatrixOps.Determinant(MatrixOps.AsMatrix(Eval(A1(), env), "Determinant"));
            case "op_eq" or "op_neq" or "op_lt" or "op_gt" or "op_leq" or "op_geq":
                return Ops.Compare(Eval(A1(), env), Eval(A2(), env), head);
            case "op_and": return Val.Bool(Ops.Truthy(Eval(A1(), env)) && Ops.Truthy(Eval(A2(), env)));
            case "op_or": return Val.Bool(Ops.Truthy(Eval(A1(), env)) || Ops.Truthy(Eval(A2(), env)));
            case "op_not": return Val.Bool(!Ops.Truthy(Eval(A1(), env)));
            case "op_apply": return EvalApply(l, env);
            case "op_index": return EvalIndex(l, env);
            case "matrix": return EvalMatrix(l, env);
            case "op_range2": return MakeRange(EvalNum(A1(), env), null, EvalNum(A2(), env));
            case "op_range3": return MakeRange(EvalNum(A1(), env), EvalNum(A2(), env), EvalNum(Rl.Nth(l, 3), env));
            case "op_sigmasum": return SumProduct(Rl.Args(l), env, product: false);
            case "op_sigmaproduct": return SumProduct(Rl.Args(l), env, product: true);
            case "op_integral": return Integral(Rl.Args(l), env);
            case "op_deriv_1": return Derivative(Rl.Args(l), env);
            case "op_deriv_n": return Derivative(Rl.Args(l), env);
            default: throw new CalcException($"Unknown operator '{head}'");
        }
    }

    private DimVal EvalMult(SCons l, Env env)
    {
        // "20 °C" (or t °C with unitless t) is an absolute temperature. After a dimensioned
        // operand, as in J/(kg °C), the scale acts as a temperature difference.
        var right = Rl.Nth(l, 2);
        if (Rl.SymName(right) is { } s && !env.IsUserDefined(s) && BuiltinUnits.TryGetAffine(s, out var affine))
        {
            var left = Ops.Normalize(Eval(Rl.Nth(l, 1), env));
            if (left is NumVal { IsDimless: true } n) return Val.Num(n.Real * affine.Scale + affine.Offset, Val.Temperature);
            if (left is MatVal m && m.Elements().All(e => e.IsDimless))
                return m.Map(e => Val.Num(e.Real * affine.Scale + affine.Offset, Val.Temperature));
            return Ops.Mul(left, Val.Num(affine.Scale, Val.Temperature));
        }
        return Ops.Mul(Eval(Rl.Nth(l, 1), env), Eval(right, env));
    }

    private DimVal EvalApply(SCons l, Env env)
    {
        var items = Rl.Items(l);
        var args = items[2..];
        var callee = Rl.SymName(items[1]);
        if (callee != null && !env.IsUserDefined(callee) && IsSpecialForm(callee))
            return EvalSpecial(callee, args, env);

        var f = (callee != null ? env.LookupFunction(callee) : Eval(items[1], env)) as Function
                ?? throw new CalcException($"'{callee ?? items[1].ToString()}' is not a function", callee);
        var values = args.Select(a => Eval(a, env)).ToArray();
        var saved = CallSiteEnv;
        CallSiteEnv = env;
        try { return Call(f, values); }
        finally { CallSiteEnv = saved; }
    }

    public DimVal Call(Function f, DimVal[] args)
    {
        switch (f)
        {
            case BuiltinFunction b:
                return b.Invoke(args, this);
            case UserFunction u:
            {
                if (args.Length != u.Parameters.Count)
                    throw new CalcException($"{u.Name} expects {u.Parameters.Count} argument(s), got {args.Length}", u.Name);
                if (++_depth > MaxCallDepth)
                {
                    _depth = 0;
                    throw new CalcException($"Recursion is too deep in {u.Name}", u.Name);
                }
                try
                {
                    var scope = u.Closure;
                    for (int i = 0; i < args.Length; i++) scope = scope.With(u.Parameters[i], args[i]);
                    return Eval(u.Body, scope);
                }
                finally { if (_depth > 0) _depth--; }
            }
            default:
                return f.InvokeVec(args) as DimVal ?? throw new CalcException($"{f.Name} returned no value", f.Name);
        }
    }

    private DimVal EvalIndex(SCons l, Env env)
    {
        var items = Rl.Items(l);
        var target = Ops.Normalize(Eval(items[1], env));
        int origin = Origin(env);
        var idx = items[2..].Select(i => ToIndex(Eval(i, env), origin)).ToArray();
        switch (target)
        {
            case MatVal m when idx.Length == 1:
                if (!m.IsVector) throw new CalcException("A matrix needs two indices: M[i, j]");
                if (idx[0] < 0 || idx[0] >= m.Length()) throw new CalcException($"Index {idx[0] + origin} is out of range");
                return m.At(idx[0]);
            case MatVal m when idx.Length == 2:
                if (idx[0] < 0 || idx[0] >= m.Rows || idx[1] < 0 || idx[1] >= m.Cols)
                    throw new CalcException($"Index ({idx[0] + origin}, {idx[1] + origin}) is out of range");
                return m.At(idx[0], idx[1]);
            case StrVal s when idx.Length == 1:
                if (idx[0] < 0 || idx[0] >= s.Field1.Length) throw new CalcException("Index is out of range");
                return new StrVal(s.Field1[idx[0]].ToString());
            default:
                throw new CalcException($"Can't index {Ops.Describe(target)}");
        }
    }

    private static int ToIndex(DimVal v, int origin)
    {
        double d = Ops.Unitless(v, "Index");
        if (d != Math.Floor(d)) throw new CalcException("Index must be an integer");
        return (int)d - origin;
    }

    private DimVal EvalMatrix(SCons l, Env env)
    {
        var items = Rl.Items(l);
        int Size(Sexpr e) => e is Tok { Field1: real_constant r } ? (int)r.Field1
            : e is Tok { Field1: int_constant i } ? i.Field1 : throw new CalcException("Malformed matrix");
        int rows = Size(items[1]), cols = Size(items[2]);
        if (items.Length != 3 + rows * cols) throw new CalcException("Malformed matrix");
        var data = new NumVal[rows * cols];
        for (int i = 0; i < data.Length; i++)
            data[i] = Ops.Num(Eval(items[3 + i], env), "matrix element");
        return Val.Create(rows, cols, (r, c) => data[r * cols + c]);
    }

    public static RngVal MakeRange(NumVal start, NumVal? second, NumVal end)
    {
        if (start.m_dim != end.m_dim || (second != null && second.m_dim != start.m_dim))
            throw CalcException.UnitsMismatch("range");
        double step;
        if (second != null) step = second.Real - start.Real;
        else if (start.IsDimless) step = end.Real >= start.Real ? 1 : -1;
        else throw new CalcException("A range with units needs a step: a, a + step .. b");
        if (step == 0) throw new CalcException("Range step can't be zero");
        if ((end.Real - start.Real) * step < 0) throw new CalcException("Range step points away from the end value");
        double count = Math.Floor((end.Real - start.Real) / step + 1e-9) + 1;
        if (count > MaxIterations) throw new CalcException("Range has too many values");
        return RngVal.IncrRange(start.Real, step, (int)count, start.m_dim);
    }

    // ---------------------------------------------------------------- range iteration

    private static readonly HashSet<string> AggregateFunctions =
    [
        "sum", "prod", "mean", "median", "stdev", "Stdev", "var", "Var", "max", "min", "length", "rows", "cols",
        "last", "sort", "reverse", "linterp", "spline", "norme", "Σ", "∏",
    ];

    private static readonly HashSet<string> SolverFunctions = ["find", "minerr", "minimize", "maximize"];

    /// <summary>Range variables referenced (outside of bound special-form variables) by <paramref name="e"/>.</summary>
    public List<string> FreeRangeVariables(Sexpr e, Env env)
    {
        var found = new List<string>();
        Collect(e, []);
        return found;

        void Collect(Sexpr x, HashSet<string> bound)
        {
            if (Rl.SymName(x) is { } name)
            {
                if (!bound.Contains(name) && !found.Contains(name)
                    && env.TryLookupUser(name, out var v) && v is RngVal)
                    found.Add(name);
                return;
            }
            if (x is not SCons l) return;
            var head = Rl.Head(l);
            if (head is "op_deriv_1" or "op_deriv_n")
            {
                CollectDerivative(Rl.Args(l), bound);
                return;
            }
            if (head is "op_sigmasum" or "op_sigmaproduct" or "op_integral")
            {
                CollectBound(Rl.Args(l), bound);
                return;
            }
            var items = Rl.Items(l);
            if (head == "op_apply" && Rl.SymName(items.ElementAtOrDefault(1)) is { } f && !env.IsUserDefined(f))
            {
                var args = items[2..];
                if (SolverFunctions.Contains(f)) return;
                if (f == "deriv" && args.Length >= 2)
                {
                    CollectDerivative(args, bound);
                    return;
                }
                if (f is "sum" or "prod" or "integral" or "root" && args.Length >= 2)
                {
                    CollectBound(args, bound);
                    return;
                }
                foreach (var a in args)
                {
                    if (AggregateFunctions.Contains(f) && a is Tok { Field1: Sym }) continue;
                    Collect(a, bound);
                }
                return;
            }
            foreach (var item in items.Skip(1)) Collect(item, bound);
        }

        // (expr x [n]) — the derivative is taken at the current value of x, which may itself be a range.
        void CollectDerivative(Sexpr[] args, HashSet<string> bound)
        {
            CollectBound(args, bound);
            if (args.Length > 1) Collect(args[1], bound);
        }

        // (expr var [limits…]) — var is bound inside expr; limits are evaluated outside.
        void CollectBound(Sexpr[] args, HashSet<string> bound)
        {
            if (args.Length == 0) return;
            var inner = new HashSet<string>(bound);
            if (args.Length > 1 && Rl.SymName(args[1]) is { } v) inner.Add(v);
            Collect(args[0], inner);
            foreach (var lim in args.Skip(2)) Collect(lim, bound);
        }
    }

    /// <summary>Evaluates <paramref name="e"/>; if it uses a range variable, evaluates once per range value.</summary>
    public DimVal EvalIterated(Sexpr e, Env env, out bool isTable)
    {
        isTable = false;
        var vars = FreeRangeVariables(e, env);
        if (vars.Count == 0) return Eval(e, env);
        if (vars.Count > 1) throw new CalcException($"Expression uses more than one range variable ({string.Join(", ", vars)})");
        env.TryLookupUser(vars[0], out var rv);
        var results = ((RngVal)rv).Values().Select(x => Ops.Num(Eval(e, env.With(vars[0], x)), "range result")).ToList();
        isTable = true;
        return Val.Column(results);
    }
}

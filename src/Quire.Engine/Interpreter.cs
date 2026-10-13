using Quire.Mpl.Run;
using Quire.Values;

namespace Quire.Engine;

/// <summary>Solve-block constraints visible to <c>find</c>, <c>minerr</c>, <c>minimize</c> and <c>maximize</c>.</summary>
public sealed record SolveContext(IReadOnlyList<SList> Constraints);

/// <summary>Evaluates Rl expressions and statements.</summary>
public sealed partial class Interpreter
{
    public const int MaxCallDepth = 400;
    public const int MaxIterations = 1_000_000;

    private int _depth;

    public SolveContext? Solve { get; private set; }

    /// <summary>The scope of the innermost function call site; built-ins use it to read ORIGIN.</summary>
    public Env CallSiteEnv { get; private set; } = Env.Empty;

    public CancellationToken Cancellation { get; set; }

    public int Origin(Env env) =>
        env.TryLookupUser("ORIGIN", out var o) ? (int)Ops.Unitless(o, "ORIGIN") : 0;

    // ---------------------------------------------------------------- expressions

    public Value Eval(Sexpr e, Env env)
    {
        switch (e)
        {
            case Atom { IsNumber: true } a: return new NumVal(a.Number!.Value);
            case Atom a: return new StrVal(a.Text!);
            case Sym s: return env.Lookup(s.Name);
            case SList l: return EvalList(l, env);
            default: throw new CalcException("Unknown expression");
        }
    }

    public NumVal EvalNum(Sexpr e, Env env, string what = "value") => Ops.Num(Eval(e, env), what);

    private Value EvalList(SList l, Env env)
    {
        Cancellation.ThrowIfCancellationRequested();
        var head = l.Head ?? throw new CalcException("Malformed expression");
        switch (head)
        {
            case "op_add": return Ops.Add(Eval(l[1], env), Eval(l[2], env));
            case "op_sub": return Ops.Sub(Eval(l[1], env), Eval(l[2], env));
            case "op_mult": return EvalMult(l, env);
            case "op_div": return Ops.Div(Eval(l[1], env), Eval(l[2], env));
            case "op_power": return Ops.Pow(Eval(l[1], env), Eval(l[2], env));
            case "op_negate": return Ops.Neg(Eval(l[1], env));
            case "op_parens": return Eval(l[1], env);
            case "op_factorial": return Builtins.Factorial(Eval(l[1], env));
            case "op_abs": return Builtins.Abs(Eval(l[1], env));
            case "op_sqrt": return Builtins.Sqrt(Eval(l[1], env));
            case "op_rootn": return Builtins.NthRoot(Eval(l[2], env), Eval(l[1], env));
            case "op_transpose": return MatrixOps.Transpose(MatrixOps.AsMatrix(Eval(l[1], env), "Transpose"));
            case "op_det": return MatrixOps.Determinant(MatrixOps.AsMatrix(Eval(l[1], env), "Determinant"));
            case "op_eq" or "op_neq" or "op_lt" or "op_gt" or "op_leq" or "op_geq":
                return Ops.Compare(Eval(l[1], env), Eval(l[2], env), head);
            case "op_and": return NumVal.Bool(Ops.Truthy(Eval(l[1], env)) && Ops.Truthy(Eval(l[2], env)));
            case "op_or": return NumVal.Bool(Ops.Truthy(Eval(l[1], env)) || Ops.Truthy(Eval(l[2], env)));
            case "op_not": return NumVal.Bool(!Ops.Truthy(Eval(l[1], env)));
            case "op_apply": return EvalApply(l, env);
            case "op_index": return EvalIndex(l, env);
            case "matrix": return EvalMatrix(l, env);
            case "op_range2": return MakeRange(EvalNum(l[1], env), null, EvalNum(l[2], env));
            case "op_range3": return MakeRange(EvalNum(l[1], env), EvalNum(l[2], env), EvalNum(l[3], env));
            case "op_sigmasum": return SumProduct(l.Args.ToArray(), env, product: false);
            case "op_sigmaproduct": return SumProduct(l.Args.ToArray(), env, product: true);
            case "op_integral": return Integral(l.Args.ToArray(), env);
            case "op_deriv_1": return Derivative(l.Args.ToArray(), env);
            case "op_deriv_n": return Derivative(l.Args.ToArray(), env);
            default: throw new CalcException($"Unknown operator '{head}'");
        }
    }

    private Value EvalMult(SList l, Env env)
    {
        // "20 °C" (or t °C with unitless t) is an absolute temperature. After a dimensioned
        // operand, as in J/(kg °C), the scale acts as a temperature difference.
        if (l[2] is Sym s && !env.IsUserDefined(s.Name) && Units.TryGetAffine(s.Name, out var affine))
        {
            var left = Ops.Normalize(Eval(l[1], env));
            if (left is NumVal { IsUnitless: true } n) return new NumVal(n.V * affine.Scale + affine.Offset, Dim.Temperature);
            if (left is MatVal m && m.Elements.All(e => e.IsUnitless))
                return m.Map(e => new NumVal(e.V * affine.Scale + affine.Offset, Dim.Temperature));
            return Ops.Mul(left, new NumVal(affine.Scale, Dim.Temperature));
        }
        return Ops.Mul(Eval(l[1], env), Eval(l[2], env));
    }

    private Value EvalApply(SList l, Env env)
    {
        var args = l.Items.Skip(2).ToArray();
        if (l[1] is Sym fs && !env.IsUserDefined(fs.Name) && IsSpecialForm(fs.Name))
            return EvalSpecial(fs.Name, args, env);

        var f = (l[1] is Sym callee ? env.LookupFunction(callee.Name) : Eval(l[1], env)) as FunctionValue
                ?? throw new CalcException($"'{(l[1] as Sym)?.Name ?? l[1].ToString()}' is not a function", (l[1] as Sym)?.Name);
        var values = args.Select(a => Eval(a, env)).ToArray();
        var saved = CallSiteEnv;
        CallSiteEnv = env;
        try { return Call(f, values); }
        finally { CallSiteEnv = saved; }
    }

    public Value Call(FunctionValue f, Value[] args)
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
                throw new CalcException($"Can't call {f.Name}");
        }
    }

    private Value EvalIndex(SList l, Env env)
    {
        var target = Ops.Normalize(Eval(l[1], env));
        int origin = Origin(env);
        var idx = l.Items.Skip(2).Select(i => ToIndex(Eval(i, env), origin)).ToArray();
        switch (target)
        {
            case MatVal m when idx.Length == 1:
                if (!m.IsVector) throw new CalcException("A matrix needs two indices: M[i, j]");
                if (idx[0] < 0 || idx[0] >= m.Length) throw new CalcException($"Index {idx[0] + origin} is out of range");
                return m[idx[0]];
            case MatVal m when idx.Length == 2:
                if (idx[0] < 0 || idx[0] >= m.Rows || idx[1] < 0 || idx[1] >= m.Cols)
                    throw new CalcException($"Index ({idx[0] + origin}, {idx[1] + origin}) is out of range");
                return m[idx[0], idx[1]];
            case StrVal s when idx.Length == 1:
                if (idx[0] < 0 || idx[0] >= s.S.Length) throw new CalcException("Index is out of range");
                return new StrVal(s.S[idx[0]].ToString());
            default:
                throw new CalcException($"Can't index {Ops.Describe(target)}");
        }
    }

    private static int ToIndex(Value v, int origin)
    {
        double d = Ops.Unitless(v, "Index");
        if (d != Math.Floor(d)) throw new CalcException("Index must be an integer");
        return (int)d - origin;
    }

    private Value EvalMatrix(SList l, Env env)
    {
        int rows = (int)((Atom)l[1]).Number!.Value, cols = (int)((Atom)l[2]).Number!.Value;
        var data = new NumVal[rows * cols];
        for (int i = 0; i < data.Length; i++)
            data[i] = Ops.Num(Eval(l[3 + i], env), "matrix element");
        return new MatVal(rows, cols, data);
    }

    public static RngVal MakeRange(NumVal start, NumVal? second, NumVal end)
    {
        if (start.D != end.D || (second != null && second.D != start.D))
            throw CalcException.UnitsMismatch("range");
        double step;
        if (second != null) step = second.V - start.V;
        else if (start.IsUnitless) step = end.V >= start.V ? 1 : -1;
        else throw new CalcException("A range with units needs a step: a, a + step .. b");
        if (step == 0) throw new CalcException("Range step can't be zero");
        if ((end.V - start.V) * step < 0) throw new CalcException("Range step points away from the end value");
        double count = Math.Floor((end.V - start.V) / step + 1e-9) + 1;
        if (count > MaxIterations) throw new CalcException("Range has too many values");
        var values = new NumVal[(int)count];
        for (int i = 0; i < values.Length; i++) values[i] = new NumVal(start.V + i * step, start.D);
        return new RngVal(values);
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
            switch (x)
            {
                case Sym s:
                    if (!bound.Contains(s.Name) && !found.Contains(s.Name)
                        && env.TryLookupUser(s.Name, out var v) && v is RngVal)
                        found.Add(s.Name);
                    break;
                case SList l when l.Head is "op_deriv_1" or "op_deriv_n":
                    CollectDerivative(l.Items.Skip(1).ToArray(), bound);
                    break;
                case SList l when l.Head is "op_sigmasum" or "op_sigmaproduct" or "op_integral":
                    CollectBound(l.Items.Skip(1).ToArray(), bound);
                    break;
                case SList { Head: "op_apply" } l when l[1] is Sym f && !env.IsUserDefined(f.Name):
                    var args = l.Items.Skip(2).ToArray();
                    if (SolverFunctions.Contains(f.Name)) break;
                    if (f.Name == "deriv" && args.Length >= 2)
                    {
                        CollectDerivative(args, bound);
                        break;
                    }
                    if (f.Name is "sum" or "prod" or "integral" or "root" && args.Length >= 2)
                    {
                        CollectBound(args, bound);
                        break;
                    }
                    foreach (var a in args)
                    {
                        if (AggregateFunctions.Contains(f.Name) && a is Sym) continue;
                        Collect(a, bound);
                    }
                    break;
                case SList l:
                    foreach (var item in l.Items.Skip(1)) Collect(item, bound);
                    break;
            }
        }

        // (expr x [n]) — the derivative is taken at the current value of x, which may itself be a range.
        void CollectDerivative(Sexpr[] args, HashSet<string> bound)
        {
            CollectBound(args, bound);
            Collect(args[1], bound);
        }

        // (expr var [limits…]) — var is bound inside expr; limits are evaluated outside.
        void CollectBound(Sexpr[] args, HashSet<string> bound)
        {
            var inner = new HashSet<string>(bound);
            if (args.Length > 1 && args[1] is Sym v) inner.Add(v.Name);
            Collect(args[0], inner);
            foreach (var lim in args.Skip(2)) Collect(lim, bound);
        }
    }

    /// <summary>Evaluates <paramref name="e"/>; if it uses a range variable, evaluates once per range value.</summary>
    public Value EvalIterated(Sexpr e, Env env, out bool isTable)
    {
        isTable = false;
        var vars = FreeRangeVariables(e, env);
        if (vars.Count == 0) return Eval(e, env);
        if (vars.Count > 1) throw new CalcException($"Expression uses more than one range variable ({string.Join(", ", vars)})");
        env.TryLookupUser(vars[0], out var rv);
        var results = ((RngVal)rv).Values.Select(x => Ops.Num(Eval(e, env.With(vars[0], x)), "range result")).ToList();
        isTable = true;
        return MatVal.Column(results);
    }
}

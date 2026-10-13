using Quire.Mpl.Rl;
using Quire.Values;

namespace Quire.Engine;

/// <summary>
/// Region-level statements:
/// <code>
/// (define x e) (define (f a b) e) (define (op_index v i) e)   (global …)   (define-eval target e [unit])
/// (eval e [unit])   (expr e)   (noop)
/// (solveblock (guesses s…) (constraints c…) (solver s))
/// (plot2d (x e) (y e…) [(xunit u)] [(yunit u)])
/// </code>
/// </summary>
public sealed partial class Interpreter
{
    public NodeResult Execute(Sexpr stmt, ref Env env)
    {
        if (stmt is not SList { Head: { } head } l) return Failed(NodeType.Expression, "Malformed statement");
        var type = TypeOf(head);
        try
        {
            switch (head)
            {
                case "noop":
                    return new NodeResult { Type = NodeType.Empty };
                case "define" or "global" or "define-eval":
                    return Define(l, ref env, type, show: head == "define-eval");
                case "eval":
                {
                    var value = EvalIterated(l[1], env, out bool table);
                    return new NodeResult
                    {
                        Type = type, Value = value, IsRangeTable = table,
                        Display = l.Count > 2 ? DisplayFor(value, l[2], env) : null,
                    };
                }
                case "expr":
                    return new NodeResult { Type = type, Value = EvalIterated(l[1], env, out _) };
                case "solveblock":
                    return SolveBlock(l, ref env);
                case "plot2d":
                    return new NodeResult { Type = type, Value = Plot(l, env) };
                default:
                    return Failed(type, $"Unknown statement '{head}'");
            }
        }
        catch (CalcException ex)
        {
            return NodeResult.Failed(type, ex);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (InsufficientExecutionStackException)
        {
            return Failed(type, "Expression is nested too deeply");
        }
        catch (OutOfMemoryException)
        {
            return Failed(type, "Not enough memory for this calculation");
        }
        catch (Exception ex)
        {
            return Failed(type, ex.Message);
        }
    }

    public static NodeType TypeOf(string head) => head switch
    {
        "define" or "define-eval" => NodeType.Definition,
        "global" => NodeType.GlobalDefinition,
        "eval" => NodeType.Evaluation,
        "expr" => NodeType.Expression,
        "solveblock" => NodeType.SolveBlock,
        "plot2d" => NodeType.Plot,
        _ => NodeType.Empty,
    };

    private static NodeResult Failed(NodeType type, string message) =>
        new() { Type = type, Status = NodeStatus.Invalid, Error = new EngineError(message) };

    private NodeResult Define(SList l, ref Env env, NodeType type, bool show)
    {
        var target = l[1];
        string name;
        Value value;
        bool table = false;
        switch (target)
        {
            case Sym s:
                name = s.Name;
                value = EvalIterated(l[2], env, out table);
                env = env.With(name, value);
                break;
            case SList { Head: "op_index" } ix:
                name = VarName(ix[1], "definition");
                value = AssignIndexed(name, ix.Items.Skip(2).ToArray(), l[2], env);
                env = env.With(name, value);
                break;
            case SList fl when fl.Count >= 1 && fl[0] is Sym fname:
            {
                name = fname.Name;
                var ps = fl.Items.Skip(1).Select(p => VarName(p, "function definition")).ToArray();
                if (ps.Distinct().Count() != ps.Length) throw new CalcException($"{name}: parameter names must be different");
                var fn = new UserFunction(name, ps, l[2]);
                env = env.With(name, fn);
                fn.Closure = env;
                value = fn;
                break;
            }
            default:
                throw new CalcException("Can't define this expression");
        }

        return new NodeResult
        {
            Type = type,
            Value = value,
            IsRangeTable = table,
            DefinedNames = [name],
            Warning = RedefinitionWarning(name, env),
            Display = show && l.Count > 3 ? DisplayFor(value, l[3], env) : null,
        };
    }

    private static string? RedefinitionWarning(string name, Env envAfter)
    {
        if (Units.IsUnit(name)) return $"This redefines the built-in unit {name}";
        if (Units.Constants.ContainsKey(name)) return $"This redefines the built-in constant {name}";
        if (Builtins.TryGet(name, out _) || IsSpecialForm(name)) return $"This redefines the built-in function {name}";
        return null;
    }

    private Value AssignIndexed(string name, Sexpr[] indices, Sexpr expr, Env env)
    {
        if (indices.Length is < 1 or > 2) throw new CalcException("Use v[i] or M[i, j] on the left side");
        int origin = Origin(env);

        var grid = new List<List<NumVal>>();
        if (env.TryLookupUser(name, out var existing) && Ops.Normalize(existing) is MatVal m)
            for (int r = 0; r < m.Rows; r++) grid.Add(Enumerable.Range(0, m.Cols).Select(c => m[r, c]).ToList());
        int cols = grid.Count > 0 ? grid[0].Count : 0;
        int existingRows = grid.Count, existingCols = cols;
        var assigned = new HashSet<(int R, int C)>();

        var vars = new List<string>();
        foreach (var e in indices.Append(expr))
            foreach (var v in FreeRangeVariables(e, env))
                if (!vars.Contains(v)) vars.Add(v);

        int count = 0;
        void Assign(Env scope)
        {
            if (++count > MaxIterations) throw new CalcException("Too many assignments");
            var idx = indices.Select(i => ToIndex(Eval(i, scope), origin)).ToArray();
            int r = idx[0], c = idx.Length > 1 ? idx[1] : 0;
            if (r < 0 || c < 0) throw new CalcException($"Index is below ORIGIN ({origin})");
            var value = EvalNum(expr, scope, "element");
            while (grid.Count <= r) grid.Add(Enumerable.Repeat(NumVal.Zero, cols).ToList());
            if (c >= cols)
            {
                foreach (var row in grid) row.AddRange(Enumerable.Repeat(NumVal.Zero, c + 1 - cols));
                cols = c + 1;
            }
            grid[r][c] = value;
            assigned.Add((r, c));
        }

        void Iterate(int k, Env scope)
        {
            if (k == vars.Count) { Assign(scope); return; }
            scope.TryLookupUser(vars[k], out var rv);
            foreach (var x in ((RngVal)rv).Values) Iterate(k + 1, scope.With(vars[k], x));
        }

        Iterate(0, env);
        // padding created for new cells takes the units of the assigned values (0 fits any unit)
        var padDim = assigned.Count > 0 ? grid[assigned.First().R][assigned.First().C].D : Dim.None;
        return MatVal.Create(grid.Count, cols, (r, c) =>
            !assigned.Contains((r, c)) && !(r < existingRows && c < existingCols) && grid[r][c].V == 0 ? new NumVal(0, padDim) : grid[r][c]);
    }

    /// <summary>Display transform for a unit override placeholder such as <c>x = km/hr</c> or <c>T = °C</c>.</summary>
    private DisplayTransform DisplayFor(Value value, Sexpr unitExpr, Env env)
    {
        var sample = Ops.Normalize(value) switch
        {
            NumVal n => n,
            MatVal m when m.Length > 0 => m[0],
            _ => throw new CalcException("Only numbers can be shown in other units"),
        };
        if (unitExpr is Sym s && !env.IsUserDefined(s.Name) && Units.TryGetAffine(s.Name, out var affine))
        {
            if (sample.D != Dim.Temperature) throw CalcException.UnitsMismatch($"{s.Name} needs a temperature");
            return new DisplayTransform(affine.Scale, affine.Offset);
        }
        var unit = EvalNum(unitExpr, env, "display unit");
        if (unit.D != sample.D) throw new CalcException($"The display unit ({unit.D}) doesn't match the result ({sample.D})");
        if (unit.V == 0) throw new CalcException("The display unit can't be zero");
        return new DisplayTransform(unit.V);
    }

    private NodeResult SolveBlock(SList l, ref Env env)
    {
        Sexpr[] Section(string name) =>
            l.Items.OfType<SList>().FirstOrDefault(s => s.Head == name)?.Items.Skip(1).ToArray() ?? [];

        var children = new List<NodeResult>();
        var local = env;
        foreach (var g in Section("guesses")) children.Add(Execute(g, ref local));

        var constraints = new List<SList>();
        foreach (var c in Section("constraints"))
        {
            if (c is SList { Head: "op_eq" or "op_lt" or "op_gt" or "op_leq" or "op_geq" } cmp)
            {
                constraints.Add(cmp);
                children.Add(new NodeResult { Type = NodeType.Expression });
            }
            else if (c is SList { Head: "unparsed" }) children.Add(Failed(NodeType.Expression, "This line has a syntax error"));
            else children.Add(Failed(NodeType.Expression, "A constraint must be a comparison such as x + y = 3"));
        }

        var solver = Section("solver");
        if (solver.Length == 0)
            return new NodeResult { Type = NodeType.SolveBlock, Children = children, Status = NodeStatus.Invalid,
                Error = new EngineError("The solve block needs a solver line such as s := find(x, y)") };

        var before = local;
        NodeResult solverResult = null!;
        var ctx = new SolveContext(constraints);
        var after = local;
        WithSolveContext(ctx, () => solverResult = Execute(solver[0], ref after));
        children.Add(solverResult);

        var exported = after.BindingsSince(before);
        foreach (var (n, v) in exported) env = env.With(n, v);

        return new NodeResult
        {
            Type = NodeType.SolveBlock,
            Status = children.Any(c => c.HasError) ? NodeStatus.Invalid : NodeStatus.Ready,
            Value = solverResult.Value,
            Children = children,
            DefinedNames = exported.Select(e => e.Name).Distinct().ToArray(),
        };
    }
}

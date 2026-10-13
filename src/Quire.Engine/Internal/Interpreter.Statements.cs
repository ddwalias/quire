using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>
/// Region-level statements:
/// <code>
/// (define x e) (define (f a b) e) (define (op_index v i) e)   (global …)   (define-eval target e [unit])
/// (eval e [unit])   (expr e)   (noop)
/// (solveblock (guesses s…) (constraints c…) (solver s))
/// (plot2d (x e) (y e…) [(xunit u)] [(yunit u)])
/// </code>
/// </summary>
internal sealed partial class Interpreter
{
    public NodeResult Execute(Sexpr stmt, ref Env env)
    {
        if (stmt is not SCons l || Rl.Head(l) is not { } head) return Failed(NodeKind.Expression, "Malformed statement");
        var kind = KindOf(head);
        try
        {
            switch (head)
            {
                case "noop":
                    return new NodeResult { Kind = NodeKind.Empty };
                case "define" or "global" or "define-eval":
                    return Define(l, ref env, kind, show: head == "define-eval");
                case "eval":
                {
                    var value = EvalIterated(Rl.Nth(l, 1), env, out bool table);
                    return new NodeResult
                    {
                        Kind = kind, Value = value, IsRangeTable = table,
                        Display = Rl.Count(l) > 2 ? DisplayFor(value, Rl.Nth(l, 2), env) : null,
                    };
                }
                case "expr":
                    return new NodeResult { Kind = kind, Value = EvalIterated(Rl.Nth(l, 1), env, out _) };
                case "solveblock":
                    return SolveBlock(l, ref env);
                case "plot2d":
                    return new NodeResult { Kind = kind, Value = Plot(l, env) };
                default:
                    return Failed(kind, $"Unknown statement '{head}'");
            }
        }
        catch (CalcException ex)
        {
            return NodeResult.Failed(kind, ex);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (InsufficientExecutionStackException)
        {
            return Failed(kind, "Expression is nested too deeply");
        }
        catch (OutOfMemoryException)
        {
            return Failed(kind, "Not enough memory for this calculation");
        }
        catch (Exception ex)
        {
            return Failed(kind, ex.Message);
        }
    }

    public static NodeKind KindOf(string head) => head switch
    {
        "define" or "define-eval" => NodeKind.Definition,
        "global" => NodeKind.GlobalDefinition,
        "eval" => NodeKind.Evaluation,
        "expr" => NodeKind.Expression,
        "solveblock" => NodeKind.SolveBlock,
        "plot2d" => NodeKind.Plot,
        _ => NodeKind.Empty,
    };

    private static NodeResult Failed(NodeKind kind, string message) =>
        new() { Kind = kind, Status = NodeStatus.NODE_INVALID, Error = new EngineError(message) };

    private NodeResult Define(SCons l, ref Env env, NodeKind kind, bool show)
    {
        var target = Rl.Nth(l, 1);
        var expr = Rl.Nth(l, 2);
        string name;
        DimVal value;
        bool table = false;
        if (Rl.SymName(target) is { } symbol)
        {
            name = symbol;
            value = EvalIterated(expr, env, out table);
            env = env.With(name, value);
        }
        else if (target is SCons ix && Rl.Head(ix) == "op_index")
        {
            var items = Rl.Items(ix);
            name = VarName(items[1], "definition");
            value = AssignIndexed(name, items[2..], expr, env);
            env = env.With(name, value);
        }
        else if (target is SCons fl && Rl.Head(fl) is { } fname)
        {
            name = fname;
            var ps = Rl.Args(fl).Select(p => VarName(p, "function definition")).ToArray();
            if (ps.Distinct().Count() != ps.Length) throw new CalcException($"{name}: parameter names must be different");
            var fn = new UserFunction(name, ps, expr, this);
            env = env.With(name, fn);
            fn.Closure = env;
            value = fn;
        }
        else throw new CalcException("Can't define this expression");

        return new NodeResult
        {
            Kind = kind,
            Value = value,
            IsRangeTable = table,
            DefinedNames = [name],
            Warning = RedefinitionWarning(name),
            Display = show && Rl.Count(l) > 3 ? DisplayFor(value, Rl.Nth(l, 3), env) : null,
        };
    }

    private static string? RedefinitionWarning(string name)
    {
        if (BuiltinUnits.IsUnit(name)) return $"This redefines the built-in unit {name}";
        if (BuiltinUnits.Constants.ContainsKey(name)) return $"This redefines the built-in constant {name}";
        if (Builtins.TryGet(name, out _) || IsSpecialForm(name)) return $"This redefines the built-in function {name}";
        return null;
    }

    private DimVal AssignIndexed(string name, Sexpr[] indices, Sexpr expr, Env env)
    {
        if (indices.Length is < 1 or > 2) throw new CalcException("Use v[i] or M[i, j] on the left side");
        int origin = Origin(env);

        var grid = new List<List<NumVal>>();
        if (env.TryLookupUser(name, out var existing) && Ops.Normalize(existing) is MatVal m)
            for (int r = 0; r < m.Rows; r++) grid.Add(Enumerable.Range(0, m.Cols).Select(c => m.At(r, c)).ToList());
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
            Val.CheckSize(r + 1, Math.Max(c + 1, cols));
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
            foreach (var x in ((RngVal)rv).Values()) Iterate(k + 1, scope.With(vars[k], x));
        }

        Iterate(0, env);
        // padding created for new cells takes the units of the assigned values (0 fits any unit)
        var padDim = assigned.Count > 0 ? grid[assigned.First().R][assigned.First().C].m_dim : Dim.identity;
        return Val.Create(grid.Count, cols, (r, c) =>
            !assigned.Contains((r, c)) && !(r < existingRows && c < existingCols) && grid[r][c].Real == 0 ? Val.Num(0, padDim) : grid[r][c]);
    }

    /// <summary>Display transform for a unit override placeholder such as <c>x = km/hr</c> or <c>T = °C</c>.</summary>
    private DisplayTransform DisplayFor(DimVal value, Sexpr unitExpr, Env env)
    {
        var sample = Ops.Normalize(value) switch
        {
            NumVal n => n,
            MatVal m when m.Length() > 0 => m.At(0),
            _ => throw new CalcException("Only numbers can be shown in other units"),
        };
        if (Rl.SymName(unitExpr) is { } s && !env.IsUserDefined(s) && BuiltinUnits.TryGetAffine(s, out var affine))
        {
            if (sample.m_dim != Val.Temperature) throw CalcException.UnitsMismatch($"{s} needs a temperature");
            return new DisplayTransform(affine.Scale, affine.Offset);
        }
        var unit = EvalNum(unitExpr, env, "display unit");
        if (unit.m_dim != sample.m_dim) throw new CalcException($"The display unit ({unit.m_dim}) doesn't match the result ({sample.m_dim})");
        if (unit.Real == 0) throw new CalcException("The display unit can't be zero");
        return new DisplayTransform(unit.Real);
    }

    private NodeResult SolveBlock(SCons l, ref Env env)
    {
        Sexpr[] Section(string name) => Rl.Part(l, name) is { } part ? Rl.Args(part) : [];

        var children = new List<NodeResult>();
        var local = env;
        foreach (var g in Section("guesses")) children.Add(Execute(g, ref local));

        var constraints = new List<SCons>();
        foreach (var c in Section("constraints"))
        {
            var head = Rl.Head(c);
            if (head is "op_eq" or "op_lt" or "op_gt" or "op_leq" or "op_geq")
            {
                constraints.Add((SCons)c);
                children.Add(new NodeResult { Kind = NodeKind.Expression });
            }
            else if (head == "unparsed") children.Add(Failed(NodeKind.Expression, "This line has a syntax error"));
            else children.Add(Failed(NodeKind.Expression, "A constraint must be a comparison such as x + y = 3"));
        }

        var solver = Section("solver");
        if (solver.Length == 0)
            return new NodeResult { Kind = NodeKind.SolveBlock, Children = children, Status = NodeStatus.NODE_INVALID,
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
            Kind = NodeKind.SolveBlock,
            Status = children.Any(c => c.HasError) ? NodeStatus.NODE_INVALID : NodeStatus.NODE_READY,
            Value = solverResult.Value,
            Children = children,
            DefinedNames = exported.Select(e => e.Name).Distinct().ToArray(),
        };
    }
}

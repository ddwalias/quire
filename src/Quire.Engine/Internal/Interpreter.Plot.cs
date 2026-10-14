using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

internal sealed partial class Interpreter
{
    public const int QuickPlotPoints = 401;

    /// <summary>
    /// <c>(plot2d (x e) (y e…) [(xunit u)] [(yunit u)])</c>. If the x expression is an undefined name the
    /// plot is a QuickPlot over −10..10; if it names a range variable each y is evaluated per range value;
    /// otherwise x and every y must evaluate to vectors of equal length.
    /// </summary>
    private TraceVal Plot(SCons l, Env env)
    {
        SCons? Part(string name) => Rl.Part(l, name);
        var xExpr = Part("x") is { } xp ? Rl.Args(xp).FirstOrDefault() : null;
        if (xExpr == null) throw new CalcException("The plot needs an x-axis expression");
        var yExprs = Part("y") is { } yp ? Rl.Args(yp) : [];
        if (yExprs.Length == 0) throw new CalcException("The plot needs at least one y-axis expression");
        var xUnit = Part("xunit") is { } xu ? EvalNum(Rl.Nth(xu, 1), env, "x-axis unit") : null;
        var yUnit = Part("yunit") is { } yu ? EvalNum(Rl.Nth(yu, 1), env, "y-axis unit") : null;

        var traces = new List<Trace>();
        if (Rl.SymName(xExpr) is { } xs && (IsUndefined(xs, env) || (env.TryLookupUser(xs, out var rv) && rv is RngVal)))
        {
            IReadOnlyList<NumVal> xValues = env.TryLookupUser(xs, out var r) && r is RngVal range
                ? range.Values().ToArray()
                : Enumerable.Range(0, QuickPlotPoints).Select(i => NumVal.RealDv(-10 + 20.0 * i / (QuickPlotPoints - 1))).ToArray();
            bool quick = !env.IsUserDefined(xs);
            foreach (var y in yExprs)
            {
                var X = new double[xValues.Count];
                var Y = new double[xValues.Count];
                Dim yd = Dim.identity;
                bool any = false;
                CalcException? first = null;
                for (int i = 0; i < xValues.Count; i++)
                {
                    X[i] = xValues[i].Real;
                    try
                    {
                        var v = EvalNum(y, env.With(xs, xValues[i]), "y value");
                        if (any && v.m_dim != yd) throw CalcException.UnitsMismatch("y values");
                        yd = v.m_dim;
                        any = true;
                        Y[i] = v.Real;
                    }
                    catch (CalcException ex) when (quick)
                    {
                        first ??= ex;
                        Y[i] = double.NaN;
                    }
                }
                if (!any) throw first ?? new CalcException("Nothing to plot");
                traces.Add(Scale(new Trace(y.ToString(), X, Y, xValues.Count > 0 ? xValues[0].m_dim : Dim.identity, yd), xUnit, yUnit));
            }
            return new TraceVal(traces);
        }

        var xv = Vector(EvalIterated(xExpr, env, out _), "x-axis");
        foreach (var y in yExprs)
        {
            var yv = Vector(EvalIterated(y, env, out _), "y-axis");
            if (yv.Count != xv.Count)
                throw new CalcException($"x and y have different lengths ({xv.Count} and {yv.Count})");
            traces.Add(Scale(new Trace(y.ToString(), xv.Select(p => p.Real).ToArray(), yv.Select(p => p.Real).ToArray(),
                Uniform(xv, "x"), Uniform(yv, "y")), xUnit, yUnit));
        }
        return new TraceVal(traces);
    }

    private static bool IsUndefined(string name, Env env)
    {
        try { env.Lookup(name); return false; }
        catch (CalcException) { return true; }
    }

    private static IReadOnlyList<NumVal> Vector(DimVal v, string axis) => Ops.Normalize(v) switch
    {
        MatVal m when m.IsVector => m.Elements(),
        NumVal n => [n],
        var other => throw new CalcException($"The {axis} expression must be a vector or range, got {Ops.Describe(other)}"),
    };

    private static Dim Uniform(IReadOnlyList<NumVal> values, string axis)
    {
        var d = values.Count > 0 ? values[0].m_dim : Dim.identity;
        if (values.Any(v => v.m_dim != d)) throw CalcException.UnitsMismatch($"{axis} values");
        return d;
    }

    private static Trace Scale(Trace t, NumVal? xUnit, NumVal? yUnit)
    {
        if (xUnit != null)
        {
            if (xUnit.m_dim != t.XDim) throw new CalcException("The x-axis unit doesn't match the x values");
            t = t with { X = t.X.Select(v => v / xUnit.Real).ToArray(), XDim = Dim.identity };
        }
        if (yUnit != null)
        {
            if (yUnit.m_dim != t.YDim) throw new CalcException("The y-axis unit doesn't match the y values");
            t = t with { Y = t.Y.Select(v => v / yUnit.Real).ToArray(), YDim = Dim.identity };
        }
        return t;
    }
}

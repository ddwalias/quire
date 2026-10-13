using Quire.Calculation.Core;
using Quire.Engine;
using Quire.Values;

namespace Quire.Calculation;

/// <summary>Converts engine values (SI) into display-ready result nodes for a unit system.</summary>
public static class ValueToIResultNodeTransform
{
    public static EngineResult Transform(NodeResult node, UnitSystem system) => new()
    {
        Value = node.Value == null ? EmptyResultNode.Instance : MakeNode(node.Value, node.Display, node.IsRangeTable, system),
        Error = node.Error == null ? null : new Core.EngineError(node.Error.Message, node.Error.Symbol),
        Warning = node.Warning,
        Children = node.Children?.Select(c => Transform(c, system)).ToArray(),
    };

    public static IResultNode MakeNode(Value value, DisplayTransform? display, bool isTable, UnitSystem system)
    {
        switch (value)
        {
            case NumVal n:
            {
                var (v, units) = MakeUnitedValue(n, display, system);
                return new UnitedValueResultNode(v, units);
            }
            case MatVal m:
                return MakeMatrix(m, display, isTable, system);
            case RngVal r:
                return MakeMatrix(r.ToColumn(), display, true, system);
            case StrVal s:
                return new StringResultNode(s.S);
            case FunctionValue f:
                return new FunctionResultNode(f.ToString());
            case TraceVal t:
                return new Trace2DResultNode(t.Traces.Select(tr => MakeTrace(tr, system)).ToArray());
            default:
                return EmptyResultNode.Instance;
        }
    }

    public static (double Value, IReadOnlyList<UnitPower> Units) MakeUnitedValue(NumVal n, DisplayTransform? display, UnitSystem system)
    {
        if (display != null) return ((n.V - display.Offset) / display.Scale, []);
        return (n.V / system.DisplayScale(n.D), AddUnit(n.D, system));
    }

    public static IReadOnlyList<UnitPower> AddUnit(Dim d, UnitSystem system) =>
        system.DisplayFactors(d).Select(f => new UnitPower(f.Unit.Name, f.Exponent)).ToArray();

    private static MatrixResultNode MakeMatrix(MatVal m, DisplayTransform? display, bool isTable, UnitSystem system)
    {
        var d = m.Length > 0 ? m[0].D : Dim.None;
        bool uniform = m.Elements.All(e => e.D == d);
        if (uniform || display != null)
        {
            var values = m.Elements.Select(e => MakeUnitedValue(e, display, system).Value).ToArray();
            var units = display != null ? [] : AddUnit(d, system);
            return new MatrixResultNode(m.Rows, m.Cols, values, units, null, isTable);
        }
        var parts = m.Elements.Select(e => MakeUnitedValue(e, null, system)).ToArray();
        return new MatrixResultNode(m.Rows, m.Cols, parts.Select(p => p.Value).ToArray(), [], parts.Select(p => p.Units).ToArray(), isTable);
    }

    private static TraceData MakeTrace(Trace t, UnitSystem system)
    {
        double xs = system.DisplayScale(t.XDim), ys = system.DisplayScale(t.YDim);
        return new TraceData(t.Label,
            t.X.Select(v => v / xs).ToArray(), t.Y.Select(v => v / ys).ToArray(),
            AddUnit(t.XDim, system), AddUnit(t.YDim, system));
    }
}

using System.Collections;
using Quire.Calculation.Core;
using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Calculation;

/// <summary>Converts engine results (values in SI) into display-ready result nodes for a unit system.</summary>
public partial class ValueToIResultNodeTransform
{
    /// <summary>Declared explicitly: the mirrored part adds Mathcad's constructors, which would hide the implicit one.</summary>
    public ValueToIResultNodeTransform() { }

    /// <summary>Transforms a result dictionary from <see cref="Engine.IEngine.GetValueSync"/>.</summary>
    public static CalculationResult Transform(IDictionary result, UnitSystem system)
    {
        var display = result[ResultKeys.DisplayScale] is double scale
            ? new DisplayTransform(scale, result[ResultKeys.DisplayOffset] as double? ?? 0)
            : null;
        return new()
        {
            Value = result[ResultKeys.Value] is DimVal v
                ? MakeNode(v, display, result[ResultKeys.Table] is true, system)
                : EmptyResultNode.Instance,
            Error = result[ResultKeys.Error] is string message ? new Core.EngineError(message, result[ResultKeys.Symbol] as string) : null,
            Warning = result[ResultKeys.Warning] as string,
            Children = (result[ResultKeys.Children] as IList)?.Cast<IDictionary>().Select(c => Transform(c, system)).ToArray(),
        };
    }

    internal static IResultNode MakeNode(DimVal value, DisplayTransform? display, bool isTable, UnitSystem system)
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
                return new StringResultNode(s.Field1);
            case Function f:
                return new FunctionResultNode(f.ToString());
            case TraceVal t:
                return new Trace2DResultNode(t.Traces.Select(tr => MakeTrace(tr, system)).ToArray());
            default:
                return EmptyResultNode.Instance;
        }
    }

    internal static (double Value, IReadOnlyList<UnitPower> Units) MakeUnitedValue(NumVal n, DisplayTransform? display, UnitSystem system)
    {
        if (display != null) return ((n.Real - display.Offset) / display.Scale, []);
        var units = BuiltinUnits.DisplayUnits(system, n.m_dim);
        return (n.Real / units.Magnitude, AddUnit(units));
    }

    public static IReadOnlyList<UnitPower> AddUnit(Units units) =>
        units.Powers.Select(p => new UnitPower(p.Base.Symbol, p.Exponent)).ToArray();

    private static MatrixResultNode MakeMatrix(MatVal m, DisplayTransform? display, bool isTable, UnitSystem system)
    {
        var elements = m.Elements();
        var d = elements.Count > 0 ? elements[0].m_dim : Dim.identity;
        bool uniform = elements.All(e => e.m_dim == d);
        if (uniform || display != null)
        {
            var values = elements.Select(e => MakeUnitedValue(e, display, system).Value).ToArray();
            var units = display != null ? [] : AddUnit(BuiltinUnits.DisplayUnits(system, d));
            return new MatrixResultNode(m.Rows, m.Cols, values, units, null, isTable);
        }
        var parts = elements.Select(e => MakeUnitedValue(e, null, system)).ToArray();
        return new MatrixResultNode(m.Rows, m.Cols, parts.Select(p => p.Value).ToArray(), [], parts.Select(p => p.Units).ToArray(), isTable);
    }

    private static TraceData MakeTrace(Trace t, UnitSystem system)
    {
        Units xu = BuiltinUnits.DisplayUnits(system, t.XDim), yu = BuiltinUnits.DisplayUnits(system, t.YDim);
        return new TraceData(t.Label,
            t.X.Select(v => v / xu.Magnitude).ToArray(), t.Y.Select(v => v / yu.Magnitude).ToArray(),
            AddUnit(xu), AddUnit(yu));
    }
}

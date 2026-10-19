using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Quire.Calculation.Core.Trace2DResults;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for ListResultNode's inherited list accessors.</summary>
public class PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Trace2DResults.ListResultNode`1";
    private static Type QBase => typeof(ListResultNode<double>);
    private static Type MBase => Mathcad.Type("Ptc.Calculation.Core", Contract).MakeGenericType(typeof(double));
    private static Type QDoubleList => typeof(DoubleListResultNode);
    private static Type MDoubleList => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Trace2DResults.DoubleListResultNode");

    [SkippableFact]
    public void Count_get_06000254()
    {
        Mathcad.Require();
        var (q, m) = Targets([1, 2, 3]);
        MethodComparison.Compare("ListResultNode<double>.Count", Contract,
            QBase.GetProperty("Count")!.GetMethod!, q, [], MBase.GetProperty("Count")!.GetMethod!, m, [],
            Show, Show, State, State);
    }

    [SkippableFact]
    public void Item_get_06000255()
    {
        Mathcad.Require();
        var (q, m) = Targets([1, 2, 3]);
        MethodComparison.Compare("ListResultNode<double>[1] get", Contract,
            QBase.GetProperty("Item")!.GetMethod!, q, [1], MBase.GetProperty("Item")!.GetMethod!, m, [1],
            Show, Show, State, State);
    }

    [SkippableFact]
    public void Item_set_06000256()
    {
        Mathcad.Require();
        var (q, m) = Targets([1, 2, 3]);
        MethodComparison.Compare("ListResultNode<double>[1] = 7.5", Contract,
            QBase.GetProperty("Item")!.SetMethod!, q, [1, 7.5], MBase.GetProperty("Item")!.SetMethod!, m, [1, 7.5],
            Show, Show, State, State);
    }

    [SkippableFact]
    public void List_get_06000257()
    {
        Mathcad.Require();
        var (q, m) = Targets([1, 2, 3]);
        MethodComparison.Compare("ListResultNode<double>.List", Contract,
            QBase.GetProperty("List")!.GetMethod!, q, [], MBase.GetProperty("List")!.GetMethod!, m, [],
            Show, Show, State, State);
    }

    private static (object Quire, object Original) Targets(double[] values)
    {
        return (Create(QDoubleList, values), Create(MDoubleList, values));
    }

    private static object Create(Type type, double[] values) =>
        type.GetConstructor([typeof(IList<double>)])!.Invoke([new List<double>(values)]);

    private static string State(object target)
    {
        object? data = Field(target, "_data");
        object? count = Field(target, "_meaningfulValuesCount");
        return $"data={ShowList(data)};meaningful={count}";
    }

    private static string Show(object? value) => value is IEnumerable and not string
        ? ShowList(value)
        : Reflect.Text(value);

    private static string ShowList(object? value) => value is null ? "null"
        : "[" + string.Join(",", ((IEnumerable)value).Cast<object?>().Select(item => item is null
            ? "null"
            : Convert.ToDouble(item, CultureInfo.InvariantCulture).ToString("R", CultureInfo.InvariantCulture))) + "]";

    private static object? Field(object target, string name)
    {
        for (Type? type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null) return field.GetValue(target);
        }

        throw new MissingFieldException(target.GetType().FullName, name);
    }
}

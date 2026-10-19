using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Quire.Calculation.Core.Trace2DResults;

namespace Quire.Differential.Tests;

/// <summary>Direct comparisons for ListResultNode's initialization, padding and lazy meaningful-value count.</summary>
public class PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Trace2DResults.ListResultNode`1";
    private const string DoubleListContract = "Ptc.Calculation.Core.Trace2DResults.DoubleListResultNode";
    private static Type Original => Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static Type QDoubleList => typeof(DoubleListResultNode);
    private static Type MDoubleList => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Trace2DResults.DoubleListResultNode");
    private static Type QDoubleBase => typeof(ListResultNode<double>);
    private static Type MDoubleBase => Original.MakeGenericType(typeof(double));

    [SkippableFact]
    public void ExtendList_06000251()
    {
        Mathcad.Require();
        MethodInfo qMethod = QDoubleBase.GetMethod("ExtendList")!;
        MethodInfo mMethod = MDoubleBase.GetMethod("ExtendList")!;
        CompareExtendList(qMethod, mMethod, [1, 2], 1, 9, "minimum already met");
        CompareExtendList(qMethod, mMethod, [], 3, 9, "empty input padded");
        CompareExtendList(qMethod, mMethod, [1], 3, -4, "short input padded");
        CompareExtendList(qMethod, mMethod, [1, 2], 0, 9, "zero minimum");
        CompareExtendList(qMethod, mMethod, null, 2, 9, "null input");
    }

    [SkippableFact]
    public void MeaningfulValuesCount_get_06000253()
    {
        Mathcad.Require();
        object q = CreateDoubleList(QDoubleList, []);
        object m = CreateDoubleList(MDoubleList, []);
        MethodInfo qGet = QDoubleBase.GetProperty("MeaningfulValuesCount")!.GetMethod!;
        MethodInfo mGet = MDoubleBase.GetProperty("MeaningfulValuesCount")!.GetMethod!;
        MethodComparison.Compare("lazy count for empty double list", Contract, qGet, q, [], mGet, m, [],
            quireState: State, originalState: State);
        MethodComparison.Compare("cached empty-list count", Contract, qGet, q, [], mGet, m, [],
            quireState: State, originalState: State);
    }

    [SkippableFact]
    public void ListResultNode_base_constructors()
    {
        Mathcad.Require();
        ConstructorInfo qValues = QDoubleList.GetConstructor([typeof(IList<double>)])!;
        ConstructorInfo mValues = MDoubleList.GetConstructor([typeof(IList<double>)])!;
        CompareConstructor(qValues, mValues, [1, double.NaN], "store list and leave count lazy");
        CompareConstructor(qValues, mValues, null, "store null list without eager validation");

        ConstructorInfo qPadded = QDoubleList.GetConstructor([typeof(IList<double>), typeof(double), typeof(int)])!;
        ConstructorInfo mPadded = MDoubleList.GetConstructor([typeof(IList<double>), typeof(double), typeof(int)])!;
        ComparePaddedConstructor(qPadded, mPadded, [1], -4, 3, "pad short list");
        ComparePaddedConstructor(qPadded, mPadded, [1, 2, 3], 9, 2, "keep long list");
        ComparePaddedConstructor(qPadded, mPadded, null, 9, 2, "null list");
    }

    private static void CompareExtendList(MethodInfo qMethod, MethodInfo mMethod, double[]? values,
        int minLength, double padding, string label)
    {
        object? qData = values is null ? null : new List<double>(values);
        object? mData = values is null ? null : new List<double>(values);
        MethodComparison.Compare("ExtendList " + label, Contract, qMethod, null, [qData, minLength, padding],
            mMethod, null, [mData, minLength, padding], ShowValue, ShowValue);
    }

    private static void CompareConstructor(ConstructorInfo qMethod, ConstructorInfo mMethod,
        double[]? values, string label)
    {
        object? qData = values is null ? null : new List<double>(values);
        object? mData = values is null ? null : new List<double>(values);
        MethodComparison.Compare("DoubleListResultNode(IList) " + label, DoubleListContract, qMethod, null, [qData],
            mMethod, null, [mData], ShowValue, ShowValue);
    }

    private static void ComparePaddedConstructor(ConstructorInfo qMethod, ConstructorInfo mMethod,
        double[]? values, double padding, int minLength, string label)
    {
        object? qData = values is null ? null : new List<double>(values);
        object? mData = values is null ? null : new List<double>(values);
        MethodComparison.Compare("DoubleListResultNode(IList, padding, minimum) " + label, DoubleListContract,
            qMethod, null, [qData, padding, minLength], mMethod, null, [mData, padding, minLength], ShowValue, ShowValue);
    }

    private static object CreateDoubleList(Type type, double[] values)
    {
        ConstructorInfo constructor = type.GetConstructor([typeof(IList<double>)])!;
        return constructor.Invoke([new List<double>(values)]);
    }

    private static string ShowValue(object? value)
    {
        if (value is null) return "null";
        if (value.GetType().Name == "DoubleListResultNode") return State(value);
        return value is IEnumerable ? ShowList(value) : Reflect.Text(value);
    }

    private static string State(object target)
    {
        object? data = Field(target, "_data");
        return $"data={ShowList(data)};meaningful={Field(target, "_meaningfulValuesCount")}";
    }

    private static object? Field(object target, string name)
    {
        for (Type? type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null) return field.GetValue(target);
        }

        throw new MissingFieldException(target.GetType().FullName, name);
    }

    private static string ShowList(object? value)
    {
        if (value is null) return "null";
        return "[" + string.Join(",", ((IEnumerable)value).Cast<object?>().Select(item => item is null
            ? "null"
            : Convert.ToDouble(item, CultureInfo.InvariantCulture).ToString("R", CultureInfo.InvariantCulture))) + "]";
    }
}

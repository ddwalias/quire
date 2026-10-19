using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.Trace2DResults.TraceDataPoints</c> members reconstructed for this mirror.
/// </summary>
public class PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Trace2DResults.TraceDataPoints";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public class VectorListProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) =>
            throw new NotSupportedException(targetMethod?.Name);
    }

    private static Type QType() => typeof(global::Quire.Calculation.Core.Trace2DResults.TraceDataPoints);
    private static Type MType() => Mathcad.Type("Ptc.Calculation.Core", Contract);

    private static object Proxy(Type interfaceType) => DispatchProxy.Create(interfaceType, typeof(VectorListProxy));

    private static Type QVectorListType() => typeof(global::Quire.Calculation.Core.IVectorListResultNode);
    private static Type MVectorListType() => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IVectorListResultNode");

    private static string Show(object? value)
    {
        if (value == null) return "null";
        Type type = value.GetType();
        if (type.Name.Contains("TraceDataPoints", StringComparison.Ordinal))
        {
            object? range = type.GetProperty("RangePoints")!.GetValue(value);
            object? result = type.GetProperty("ResultPoints")!.GetValue(value);
            return $"TraceDataPoints(range={Show(range)},result={Show(result)})";
        }
        return type.GetInterfaces().Any(i => i.Name == "IVectorListResultNode") ? "VectorList" : type.Name;
    }

    private static object Target(Type type, object? range, object? result)
    {
        object target = RuntimeHelpers.GetUninitializedObject(type);
        type.GetField("<RangePoints>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, range);
        type.GetField("<ResultPoints>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, result);
        return target;
    }

    private static string State(object target)
    {
        Type type = target.GetType();
        bool hasRange = type.GetProperty("RangePoints")!.GetValue(target) != null;
        bool hasResult = type.GetProperty("ResultPoints")!.GetValue(target) != null;
        return $"range={hasRange},result={hasResult}";
    }

    [SkippableFact]
    public void ctor_060002b0()
    {
        ConstructorInfo qCtor = QType().GetConstructor(Instance, null, [QVectorListType(), QVectorListType()], null)!;
        ConstructorInfo mCtor = (ConstructorInfo)MType().Module.ResolveMethod(0x060002b0)!;
        object qRange = Proxy(QVectorListType());
        object qResult = Proxy(QVectorListType());
        object mRange = Proxy(MVectorListType());
        object mResult = Proxy(MVectorListType());
        MethodComparison.Compare("stores both vector nodes", Contract, qCtor, null, [qRange, qResult],
            mCtor, null, [mRange, mResult], Show, Show);
        MethodComparison.Compare("null range throws ArgumentNullException", Contract, qCtor, null, [null, qResult],
            mCtor, null, [null, mResult], Show, Show);
        MethodComparison.Compare("null result throws ArgumentNullException", Contract, qCtor, null, [qRange, null],
            mCtor, null, [mRange, null], Show, Show);
    }

    [SkippableFact]
    public void IsEmpty_get_060002b6()
    {
        MethodInfo qGetter = QType().GetProperty("IsEmpty")!.GetMethod!;
        MethodInfo mGetter = MType().GetProperty("IsEmpty")!.GetMethod!;
        object? qNode = Proxy(QVectorListType());
        object? mNode = Proxy(MVectorListType());
        foreach ((object? qRange, object? qResult, object? mRange, object? mResult, string label) in new[]
        {
            ((object?)null, null, (object?)null, null, "both references null"),
            (qNode, null, mNode, null, "range only"),
            (null, qNode, null, mNode, "result only"),
            (qNode, qNode, mNode, mNode, "both references present"),
        })
        {
            object qTarget = Target(QType(), qRange, qResult);
            object mTarget = Target(MType(), mRange, mResult);
            MethodComparison.Compare(label, Contract, qGetter, qTarget, [], mGetter, mTarget, [],
                Reflect.Text, Reflect.Text, State, State);
        }
    }

    [SkippableFact]
    public void ctor_060002b1() => Mechanical.Compare(QType(), 0x060002b1);

    [SkippableFact]
    public void RangePoints_get_060002b2() => Mechanical.Compare(QType(), 0x060002b2);

    [SkippableFact]
    public void RangePoints_set_060002b3() => Mechanical.Compare(QType(), 0x060002b3);

    [SkippableFact]
    public void ResultPoints_get_060002b4() => Mechanical.Compare(QType(), 0x060002b4);

    [SkippableFact]
    public void ResultPoints_set_060002b5() => Mechanical.Compare(QType(), 0x060002b5);
}

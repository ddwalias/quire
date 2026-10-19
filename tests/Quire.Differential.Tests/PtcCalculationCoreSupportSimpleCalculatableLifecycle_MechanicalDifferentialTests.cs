using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for SimpleCalculatable's remaining Tier A lifecycle members.</summary>
public class PtcCalculationCoreSupportSimpleCalculatableLifecycle_MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Support.SimpleCalculatable";
    private static Type Original => Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static Type Mirrored => typeof(SimpleCalculatable);

    [SkippableFact]
    public void Constructor_0600031d()
    {
        Mathcad.Require();
        MethodComparison.Compare("new SimpleCalculatable()", Contract,
            Mirrored.GetConstructor(Type.EmptyTypes)!, null, [],
            Original.GetConstructor(Type.EmptyTypes)!, null, [], ShowObject, ShowObject);
    }

    [SkippableFact]
    public void Commit_06000310()
    {
        Mathcad.Require();
        object q = Activator.CreateInstance(Mirrored)!;
        object m = Activator.CreateInstance(Original)!;
        SetField(q, "_error", new EngineError());
        Type mError = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineError");
        SetField(m, "_error", Activator.CreateInstance(mError));
        SetField(q, "_evaluationResult", null);
        SetField(m, "_evaluationResult", null);
        var qCalls = new List<string>();
        var mCalls = new List<string>();
        MethodComparison.Compare("Commit without RequestEvaluation handler", Contract,
            Mirrored.GetMethod("Commit")!, q, [], Original.GetMethod("Commit")!, m, [],
            quireState: target => State(target, qCalls), originalState: target => State(target, mCalls));
        Subscribe(q, "RequestEvaluation", args => qCalls.Add(args.Length.ToString()));
        Subscribe(m, "RequestEvaluation", args => mCalls.Add(args.Length.ToString()));
        MethodComparison.Compare("Commit with RequestEvaluation handler", Contract,
            Mirrored.GetMethod("Commit")!, q, [], Original.GetMethod("Commit")!, m, [],
            quireState: target => State(target, qCalls), originalState: target => State(target, mCalls));
    }

    [SkippableFact]
    public void SetStatusSynchronized_06000311()
    {
        Mathcad.Require();
        object q = Activator.CreateInstance(Mirrored)!;
        object m = Activator.CreateInstance(Original)!;
        var qCalls = new List<string>();
        var mCalls = new List<string>();
        MethodComparison.Compare("SetStatusSynchronized without TreatAsUpToDate handler", Contract,
            Mirrored.GetMethod("SetStatusSynchronized")!, q, [], Original.GetMethod("SetStatusSynchronized")!, m, [],
            quireState: target => State(target, qCalls), originalState: target => State(target, mCalls));
        Subscribe(q, "TreatAsUpToDate", args => qCalls.Add(args.Length.ToString()));
        Subscribe(m, "TreatAsUpToDate", args => mCalls.Add(args.Length.ToString()));
        MethodComparison.Compare("SetStatusSynchronized with TreatAsUpToDate handler", Contract,
            Mirrored.GetMethod("SetStatusSynchronized")!, q, [], Original.GetMethod("SetStatusSynchronized")!, m, [],
            quireState: target => State(target, qCalls), originalState: target => State(target, mCalls));
    }

    [SkippableFact]
    public void SetSynchronizationStatus_06000312()
    {
        Mathcad.Require();
        object q = Activator.CreateInstance(Mirrored)!;
        object m = Activator.CreateInstance(Original)!;
        var qCalls = new List<string>();
        var mCalls = new List<string>();
        Type qStatus = Mirrored.GetMethod("SetSynchronizationStatus")!.GetParameters()[0].ParameterType;
        Type mStatus = Original.GetMethod("SetSynchronizationStatus")!.GetParameters()[0].ParameterType;
        MethodComparison.Compare("SetSynchronizationStatus(non-Synchronized)", Contract,
            Mirrored.GetMethod("SetSynchronizationStatus")!, q, [Enum.ToObject(qStatus, 0)],
            Original.GetMethod("SetSynchronizationStatus")!, m, [Enum.ToObject(mStatus, 0)],
            ShowEnum, ShowEnum, target => State(target, qCalls), target => State(target, mCalls));
        Subscribe(q, "Synchronized", args => qCalls.Add(args.Length.ToString()));
        Subscribe(m, "Synchronized", args => mCalls.Add(args.Length.ToString()));
        MethodComparison.Compare("SetSynchronizationStatus(Synchronized)", Contract,
            Mirrored.GetMethod("SetSynchronizationStatus")!, q, [Enum.ToObject(qStatus, 3)],
            Original.GetMethod("SetSynchronizationStatus")!, m, [Enum.ToObject(mStatus, 3)],
            ShowEnum, ShowEnum, target => State(target, qCalls), target => State(target, mCalls));
    }

    [SkippableFact]
    public void IsEngineWarningsEquals_0600031b()
    {
        Mathcad.Require();
        MethodInfo qMethod = Mirrored.GetMethod("IsEngineWarningsEquals")!;
        MethodInfo mMethod = Original.GetMethod("IsEngineWarningsEquals")!;
        CompareWarnings(qMethod, mMethod, null, null, "both null");
        CompareWarnings(qMethod, mMethod, null, [], "null and empty");
        CompareWarnings(qMethod, mMethod, [], [], "both empty");
        CompareWarnings(qMethod, mMethod, [("name", "loc", 1)], [("name", "loc", 1)], "matching warning");
        CompareWarnings(qMethod, mMethod, [("name", "loc", 1)], [("other", "loc", 1)], "different name");
        CompareWarnings(qMethod, mMethod, [("name", "loc", 1)], [("name", "other", 1)], "different old location");
        CompareWarnings(qMethod, mMethod, [("name", "loc", 1)], [("name", "loc", 2)], "different old type");
        CompareWarnings(qMethod, mMethod, [("one", "loc", 1)], [("one", "loc", 1), ("two", "loc", 1)], "different counts");
    }

    [SkippableFact]
    public void FireGetValueOfIdentifierBefore_0600031c()
    {
        Mathcad.Require();
        object q = Activator.CreateInstance(Mirrored)!;
        object m = Activator.CreateInstance(Original)!;
        var qCalls = new List<string>();
        var mCalls = new List<string>();
        MethodComparison.Compare("FireGetValueOfIdentifierBefore without handler", Contract,
            Mirrored.GetMethod("FireGetValueOfIdentifierBefore")!, q, ["x + 1"],
            Original.GetMethod("FireGetValueOfIdentifierBefore")!, m, ["x + 1"],
            ShowObject, ShowObject,
            quireState: target => State(target, qCalls), originalState: target => State(target, mCalls));
        Subscribe(q, "GetValueOfIdentifierBefore", args => qCalls.Add(args[1]?.ToString() ?? "null"));
        Subscribe(m, "GetValueOfIdentifierBefore", args => mCalls.Add(args[1]?.ToString() ?? "null"));
        MethodComparison.Compare("FireGetValueOfIdentifierBefore(" + Reflect.Text("x + 1") + ")", Contract,
            Mirrored.GetMethod("FireGetValueOfIdentifierBefore")!, q, ["x + 1"],
            Original.GetMethod("FireGetValueOfIdentifierBefore")!, m, ["x + 1"],
            ShowObject, ShowObject,
            quireState: target => State(target, qCalls), originalState: target => State(target, mCalls));
    }

    private static void CompareWarnings(MethodInfo qMethod, MethodInfo mMethod,
        (string Name, string Location, int Type)[]? first,
        (string Name, string Location, int Type)[]? second, string label)
    {
        object?[] qArgs = [CreateWarnings(qMethod, first), CreateWarnings(qMethod, second)];
        object?[] mArgs = [CreateWarnings(mMethod, first), CreateWarnings(mMethod, second)];
        MethodComparison.Compare("IsEngineWarningsEquals " + label, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            ShowWarnings, ShowWarnings);
    }

    private static object? CreateWarnings(MethodInfo method, (string Name, string Location, int Type)[]? values)
    {
        if (values is null) return null;
        Type itemType = method.GetParameters()[0].ParameterType.GetGenericArguments()[0];
        Type listType = typeof(List<>).MakeGenericType(itemType);
        IList list = (IList)Activator.CreateInstance(listType)!;
        foreach (var value in values)
        {
            object item = Activator.CreateInstance(itemType)!;
            itemType.GetProperty("Name")!.SetValue(item, value.Name);
            itemType.GetProperty("OldLocation")!.SetValue(item, value.Location);
            object oldType = Enum.ToObject(itemType.GetProperty("OldType")!.PropertyType, value.Type);
            itemType.GetProperty("OldType")!.SetValue(item, oldType);
            list.Add(item);
        }
        return list;
    }

    private static string ShowWarnings(object? value)
    {
        if (value is null) return "null";
        if (value is bool boolean) return boolean.ToString();
        return "[" + string.Join(";", ((IEnumerable)value).Cast<object>().Select(item =>
            $"{Read(item, "Name") ?? "null"}|{Read(item, "OldLocation") ?? "null"}|{Convert.ToInt32(Read(item, "OldType"))}")) + "]";
    }

    private static void Subscribe(object target, string eventName, Action<object?[]> callback)
    {
        EventInfo eventInfo = target.GetType().GetEvent(eventName)!;
        MethodInfo invoke = eventInfo.EventHandlerType!.GetMethod("Invoke")!;
        ParameterExpression[] parameters = invoke.GetParameters()
            .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name)).ToArray();
        NewArrayExpression args = Expression.NewArrayInit(typeof(object), parameters.Select(parameter => Expression.Convert(parameter, typeof(object))));
        MethodCallExpression call = Expression.Call(Expression.Constant(callback), typeof(Action<object?[]>).GetMethod("Invoke")!, args);
        Expression body = invoke.ReturnType == typeof(void) ? call : Expression.Block(call, Expression.Default(invoke.ReturnType));
        Delegate handler = Expression.Lambda(eventInfo.EventHandlerType, body, parameters).Compile();
        eventInfo.AddEventHandler(target, handler);
    }

    private static string State(object target, List<string> calls)
    {
        object? origin = Field(target, "_systemOrigin");
        object? result = Field(target, "_evaluationResult");
        return $"sync={ShowEnum(Field(target, "_syncStatus"))};calc={ShowEnum(Field(target, "_calculatingStatus"))};"
            + $"error={TypeName(Field(target, "_error"))};result={TypeName(result)};warnings={ShowWarnings(Field(target, "_warnings"))};"
            + $"origin={Read(origin!, "Value")};callbacks=[{string.Join(",", calls)}]";
    }

    private static string ShowObject(object? value) => value?.GetType().Name == "SimpleCalculatable"
        ? State(value, [])
        : Reflect.Text(value);

    private static object? Field(object target, string name) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target);

    private static void SetField(object target, string name, object? value) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);

    private static object? Read(object target, string name) =>
        target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(target);

    private static string TypeName(object? value) => value?.GetType().Name ?? "null";

    private static string ShowEnum(object? value) => value is null ? "null" : Convert.ToInt32(value).ToString();
}

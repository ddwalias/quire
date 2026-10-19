using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Calculation.Core.Support;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.Support.SimpleCalculatable</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.Support.SimpleCalculatable.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests
{
    [SkippableFact]
    public void SetCalculatingStatus_06000313() => Compare(0x06000313);

    [SkippableFact]
    public void SetResult_06000314() => Compare(0x06000314);

    [SkippableFact]
    public void SetError_06000315() => Compare(0x06000315);

    [SkippableFact]
    public void SetAutoSelectAlgorithm_06000316() => Compare(0x06000316);

    [SkippableFact]
    public void SetRedefinitionWarnings_06000317() => Compare(0x06000317);

    [SkippableFact]
    public void SetIdentifierLabels_06000318() => Compare(0x06000318);

    [SkippableFact]
    public void Result_get_06000301() => Compare(0x06000301);

    [SkippableFact]
    public void EngineWarnings_get_06000302() => Compare(0x06000302);

    [SkippableFact]
    public void SynchronizationStatus_get_06000303() => Compare(0x06000303);

    [SkippableFact]
    public void CalculatingStatus_get_06000304() => Compare(0x06000304);

    [SkippableFact]
    public void Error_get_06000305() => Compare(0x06000305);

    [SkippableFact]
    public void Rl_get_0600030e() => Compare(0x0600030e);

    [SkippableFact]
    public void Rl_set_0600030f() => Compare(0x0600030f);

    [SkippableFact]
    public void SystemOrigin_get_0600031a() => Compare(0x0600031a);

    [SkippableFact]
    public void Synchronized_add_06000306() => Compare(0x06000306);

    [SkippableFact]
    public void Synchronized_remove_06000307() => Compare(0x06000307);

    [SkippableFact]
    public void RequestEvaluation_add_06000308() => Compare(0x06000308);

    [SkippableFact]
    public void RequestEvaluation_remove_06000309() => Compare(0x06000309);

    [SkippableFact]
    public void GetValueOfIdentifierBefore_add_0600030a() => Compare(0x0600030a);

    [SkippableFact]
    public void GetValueOfIdentifierBefore_remove_0600030b() => Compare(0x0600030b);

    [SkippableFact]
    public void TreatAsUpToDate_add_0600030c() => Compare(0x0600030c);

    [SkippableFact]
    public void TreatAsUpToDate_remove_0600030d() => Compare(0x0600030d);

    private static void Compare(int token)
    {
        Mathcad.Require();
        Type originalType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Support.SimpleCalculatable");
        MethodInfo originalMethod = (MethodInfo)Mathcad.Load("Ptc.Calculation.Core").ManifestModule.ResolveMethod(token)!;
        Type quireType = typeof(SimpleCalculatable);
        MethodInfo quireMethod = quireType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Single(method => method.Name == originalMethod.Name);
        object? qTarget = RuntimeHelpers.GetUninitializedObject(quireType);
        object? mTarget = RuntimeHelpers.GetUninitializedObject(originalType);
        Initialize(qTarget);
        Initialize(mTarget);
        object?[] qArgs = Arguments(quireMethod);
        object?[] mArgs = Arguments(originalMethod);
        MethodComparison.Compare(originalMethod.Name, "Ptc.Calculation.Core.Support.SimpleCalculatable",
            quireMethod, qTarget, qArgs, originalMethod, mTarget, mArgs, Show, Show, State, State);
    }

    private static object?[] Arguments(MethodInfo method)
    {
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length == 0) return [];
        if (parameters.Length != 1) throw new InvalidOperationException($"Unexpected signature: {method}");
        Type type = parameters[0].ParameterType;
        if (type.IsEnum)
        {
            int value = method.Name == "SetSynchronizationStatus" ? 3 : 1;
            return [Enum.ToObject(type, value)];
        }

        return [null];
    }

    private static void Initialize(object target)
    {
        SetField(target, "_syncStatus", Enum.ToObject(FieldInfo(target, "_syncStatus").FieldType, 0));
        SetField(target, "_calculatingStatus", Enum.ToObject(FieldInfo(target, "_calculatingStatus").FieldType, 2));
        SetField(target, "_evaluationResult", null);
        SetField(target, "_error", null);
        SetField(target, "_warnings", null);
        SetField(target, "_systemOrigin", null);
    }

    private static string Show(object? value) => value switch
    {
        null => "null",
        Enum e => Convert.ToInt32(e).ToString(),
        string s => "\"" + s + "\"",
        _ when value.GetType().Name == "RealSimpleResultNode" => "Real(" + Read(value, "Value") + ")",
        _ => value.GetType().Name,
    };

    private static string State(object target)
    {
        string[] names = ["_syncStatus", "_calculatingStatus", "_evaluationResult", "_error", "_warnings", "_systemOrigin", "<Rl>k__BackingField"];
        return string.Join(";", names.Select(name =>
        {
            FieldInfo? field = FindField(target.GetType(), name);
            return name + "=" + (field == null ? "missing" : Show(field.GetValue(target)));
        }));
    }

    private static FieldInfo FieldInfo(object target, string name) => FindField(target.GetType(), name)
        ?? throw new MissingFieldException(target.GetType().FullName, name);

    private static FieldInfo? FindField(Type type, string name)
    {
        for (Type? current = type; current != null; current = current.BaseType)
        {
            FieldInfo? field = current.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null) return field;
        }

        return null;
    }

    private static void SetField(object target, string name, object? value) => FieldInfo(target, name).SetValue(target, value);

    private static object? Read(object target, string name) => target.GetType().GetProperty(name)!.GetValue(target);
}

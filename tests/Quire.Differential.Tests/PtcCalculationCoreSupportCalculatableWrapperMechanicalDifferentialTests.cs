using System.Linq;
using System.Reflection;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons for <c>Ptc.Calculation.Core.Support.CalculatableWrapper</c> recorded in its evidence file.
/// </summary>
public class PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests
{
    [SkippableFact]
    public void HandleRequestEvaluation_060002e3() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e3);

    [SkippableFact]
    public void RequestEvaluation_add_060002e7() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e7);

    [SkippableFact]
    public void RequestEvaluation_remove_060002e8() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e8);

    [SkippableFact]
    public void GetValueOfIdentifierBefore_add_060002e9() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e9);

    [SkippableFact]
    public void GetValueOfIdentifierBefore_remove_060002ea() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002ea);

    [SkippableFact]
    public void TreatAsUpToDate_add_060002eb() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002eb);

    [SkippableFact]
    public void TreatAsUpToDate_remove_060002ec() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002ec);

    [SkippableFact]
    public void WrappedCalculatable_get_060002e1() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e1);

    [SkippableFact]
    public void WrappedCalculatable_set_060002e2() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e2);

    [SkippableFact]
    public void Rl_get_060002ed() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002ed);

    [SkippableFact]
    public void ResultsProcessingPriority_get_060002fa() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002fa);

    [SkippableFact]
    public void ContainsSymbolicEvaluation_get_060002fb() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002fb);

    [SkippableFact]
    public void ctor_060002e0() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e0);

    [SkippableFact]
    public void HandleGetValueOfIdentifier_060002e4() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e4);

    [SkippableFact]
    public void HandleTreatAsUpToDate_060002e5() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e5);

    [SkippableFact]
    public void FireRequestEvaluation_060002e6() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002e6);

    [SkippableFact]
    public void Commit_060002ee() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002ee);

    [SkippableFact]
    public void SetStatusSynchronized_060002ef() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002ef);

    [SkippableFact]
    public void SetSynchronizationStatus_060002f0() => CompareForwarder("SetSynchronizationStatus",
        typeof(global::Quire.Calculation.Core.SynchronizationStatus), "Ptc.Calculation.Core.SynchronizationStatus", 2);

    [SkippableFact]
    public void SetCalculatingStatus_060002f1() => CompareForwarder("SetCalculatingStatus",
        typeof(global::Quire.Calculation.Core.CalculatingStatus), "Ptc.Calculation.Core.CalculatingStatus", 2);

    [SkippableFact]
    public void SetResult_060002f2() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f2);

    [SkippableFact]
    public void SetError_060002f3() => CompareForwarder("SetError",
        typeof(global::Quire.Calculation.Core.EngineError), "Ptc.Calculation.Core.EngineError", null);

    [SkippableFact]
    public void SetIdentifierLabels_060002f4() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f4);

    [SkippableFact]
    public void SetAutoSelectAlgorithm_060002f5() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f5);

    [SkippableFact]
    public void SetRedefinitionWarnings_060002f6() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f6);

    [SkippableFact]
    public void Dispose_060002f7() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f7);

    [SkippableFact]
    public void HookupCalculatableEvents_060002f8() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f8);

    [SkippableFact]
    public void UnhookCalculatableEvents_060002f9() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), 0x060002f9);

    private static void CompareForwarder(string methodName, Type quireArgumentType, string originalArgumentType,
        object? argumentValue)
    {
        Type originalWrapper = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Support.CalculatableWrapper");
        Type originalCalculatable = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculatable");
        var qProxy = CreateCalculatableProxy(typeof(global::Quire.Calculation.Core.ICalculatable));
        var mProxy = CreateCalculatableProxy(originalCalculatable);
        object qTarget = Activator.CreateInstance(typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper), qProxy)!;
        object mTarget = Activator.CreateInstance(originalWrapper, mProxy)!;
        Type mArgumentType = Mathcad.Type("Ptc.Calculation.Core", originalArgumentType);
        object? qArgument = argumentValue is int value ? Enum.ToObject(quireArgumentType, value) : null;
        object? mArgument = argumentValue is int originalValue ? Enum.ToObject(mArgumentType, originalValue) : null;
        MethodInfo qMethod = typeof(global::Quire.Calculation.Core.Support.CalculatableWrapper)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, [quireArgumentType])!;
        MethodInfo mMethod = originalWrapper.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance,
            [mArgumentType])!;
        MethodComparison.Compare(methodName + " forwarding", "Ptc.Calculation.Core.Support.CalculatableWrapper",
            qMethod, qTarget, [qArgument], mMethod, mTarget, [mArgument],
            ShowArgument, ShowArgument, _ => qProxy.LastCall, _ => mProxy.LastCall);
    }

    private static CalculatableDispatchProxy CreateCalculatableProxy(Type interfaceType)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, typeof(CalculatableDispatchProxy));
        return (CalculatableDispatchProxy)create.Invoke(null, null)!;
    }

    private static string ShowArgument(object? value) => value == null ? "null"
        : value is Enum ? Convert.ToInt32(value).ToString()
        : value.GetType().Name;

    public class CalculatableDispatchProxy : DispatchProxy
    {
        public string LastCall { get; private set; } = "";

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            LastCall = targetMethod!.Name + "|" + string.Join(",", (args ?? []).Select(ShowArgument));
            return targetMethod.ReturnType == typeof(void) ? null
                : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                : null;
        }
    }
}

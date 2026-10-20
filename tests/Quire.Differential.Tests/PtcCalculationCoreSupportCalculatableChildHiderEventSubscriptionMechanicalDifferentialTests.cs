using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreSupportCalculatableChildHiderEventSubscriptionMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Support.CalculatableChildHider";
    private const string OriginalContract = "Ptc.Calculation.Core.ICalculatableContainer";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly System.Type Original = Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static readonly System.Type OriginalContainer = Mathcad.Type("Ptc.Calculation.Core", OriginalContract);
    private static readonly System.Type OriginalAggregate = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculatableAggregate");
    private static readonly System.Type QuireType = typeof(CalculatableChildHider);
    private static readonly FieldInfo QuireAdopt = QuireType.GetField("_adoptCalculatableContainerChildren", Instance)!;
    private static readonly FieldInfo OriginalAdopt = Original.GetField("_adoptCalculatableContainerChildren", Instance)!;

    [SkippableFact]
    public void SubscribeToCalculatableContainerEvents_cases()
    {
        Mathcad.Require();
        MethodInfo qMethod = QuireType.GetMethod("SubscribeToCalculatableContainerEvents", Instance, null, [typeof(ICalculatableContainer)], null)!;
        MethodInfo mMethod = Original.GetMethod("SubscribeToCalculatableContainerEvents", Instance, null, [OriginalContainer], null)!;
        CheckOriginal(mMethod, 0x060002d2);

        CompareContainerMethod("subscribe while adoption disabled", qMethod, mMethod, false, aggregate: false, nullContainer: false);
        CompareContainerMethod("subscribe aggregate", qMethod, mMethod, true, aggregate: true, nullContainer: false);
        CompareContainerMethod("subscribe ordinary container", qMethod, mMethod, true, aggregate: false, nullContainer: false);
        CompareContainerMethod("subscribe null container after adoption enabled", qMethod, mMethod, true, aggregate: false, nullContainer: true);
    }

    [SkippableFact]
    public void UnsubscribeFromCalculatableContainerEvents_cases()
    {
        Mathcad.Require();
        MethodInfo qMethod = QuireType.GetMethod("UnsubscribeFromCalculatableContainerEvents", Instance, null, [typeof(ICalculatableContainer)], null)!;
        MethodInfo mMethod = Original.GetMethod("UnsubscribeFromCalculatableContainerEvents", Instance, null, [OriginalContainer], null)!;
        CheckOriginal(mMethod, 0x060002d3);

        CompareContainerMethod("unsubscribe while adoption disabled", qMethod, mMethod, false, aggregate: false, nullContainer: false);
        CompareContainerMethod("unsubscribe aggregate", qMethod, mMethod, true, aggregate: true, nullContainer: false);
        CompareContainerMethod("unsubscribe ordinary container", qMethod, mMethod, true, aggregate: false, nullContainer: false);
        CompareContainerMethod("unsubscribe null container after adoption enabled", qMethod, mMethod, true, aggregate: false, nullContainer: true);
    }

    private static void CompareContainerMethod(string caseId, MethodInfo qMethod, MethodInfo mMethod,
        bool adopt, bool aggregate, bool nullContainer)
    {
        object qTarget = NewHider(QuireType, QuireAdopt, adopt);
        object mTarget = NewHider(Original, OriginalAdopt, adopt);
        object? qContainer = nullContainer ? null : CreateProxy(aggregate ? typeof(ICalculatableAggregate) : typeof(ICalculatableContainer));
        object? mContainer = nullContainer ? null : CreateProxy(aggregate ? OriginalAggregate : OriginalContainer);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [qContainer], mMethod, mTarget, [mContainer],
            Show, Show, HiderState, HiderState);
    }

    private static object NewHider(System.Type type, FieldInfo adoptField, bool adopt)
    {
        object hider = Activator.CreateInstance(type, Instance, binder: null, args: [], culture: null)!;
        adoptField.SetValue(hider, adopt);
        return hider;
    }

    private static object CreateProxy(System.Type interfaceType)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, typeof(ContainerEventDispatchProxy));
        return create.Invoke(null, null)!;
    }

    public class ContainerEventDispatchProxy : DispatchProxy
    {
        private readonly Dictionary<string, Delegate?> _handlers = new();
        private readonly Dictionary<string, int> _adds = new();
        private readonly Dictionary<string, int> _removes = new();

        public string State => string.Join(";", new[] { "CalculatableAdded", "CalculatableRemoved", "CalculatableMoved" }
            .Select(name => $"{name}:{HandlerCount(name)}/{Count(_adds, name)}/{Count(_removes, name)}"));

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            string name = targetMethod!.Name;
            if (name.StartsWith("add_", StringComparison.Ordinal))
            {
                string eventName = name[4..];
                _adds[eventName] = Count(_adds, eventName) + 1;
                _handlers.TryGetValue(eventName, out Delegate? existing);
                _handlers[eventName] = Delegate.Combine(existing, (Delegate)args![0]!);
                return null;
            }
            if (name.StartsWith("remove_", StringComparison.Ordinal))
            {
                string eventName = name[7..];
                _removes[eventName] = Count(_removes, eventName) + 1;
                _handlers.TryGetValue(eventName, out Delegate? existing);
                _handlers[eventName] = Delegate.Remove(existing, (Delegate)args![0]!);
                return null;
            }
            return targetMethod.ReturnType == typeof(void) ? null
                : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                : null;
        }

        private int HandlerCount(string name) => _handlers.TryGetValue(name, out Delegate? handler)
            ? handler?.GetInvocationList().Length ?? 0
            : 0;

        private static int Count(Dictionary<string, int> counts, string name) => counts.TryGetValue(name, out int count) ? count : 0;
    }

    private static string HiderState(object hider) => (bool)(hider.GetType() == Original
        ? OriginalAdopt.GetValue(hider)! : QuireAdopt.GetValue(hider)!) ? "adopt" : "keep";

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is bool boolean) return boolean ? "true" : "false";
        if (value is ContainerEventDispatchProxy proxy) return proxy.State;
        return value.GetType().Name;
    }

    private static void CheckOriginal(MethodInfo method, int token)
    {
        Assert.Equal(Guid.Parse("402d6e7b-941e-48f3-b17b-e1cd1a92b615"), method.Module.ModuleVersionId);
        Assert.Equal(token, method.MetadataToken);
    }
}

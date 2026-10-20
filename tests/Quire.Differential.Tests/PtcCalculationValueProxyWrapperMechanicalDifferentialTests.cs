using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Automation;
using Quire.Calculation;

namespace Quire.Differential.Tests;

public class PtcCalculationValueProxyWrapperMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.ValueProxyWrapper";
    private const string QuireTypeName = "Quire.Calculation.ValueProxyWrapper";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly System.Type Original = Mathcad.Type("Ptc.Calculation", Contract);
    private static readonly System.Type Quire = typeof(CalculationController).Assembly.GetType(QuireTypeName, throwOnError: true)!;

    [SkippableFact]
    public void Constructor_cases()
    {
        Mathcad.Require();
        ConstructorInfo qCtor = Quire.GetConstructor(Instance, null, [typeof(IValueProxy)], null)!;
        ConstructorInfo mCtor = Original.GetConstructor(Instance, null, [Original.GetProperty("Proxy")!.PropertyType], null)!;
        CheckOriginal(mCtor, 0x0600006c);
        foreach (bool useProxy in new[] { false, true })
        {
            object? qProxy = useProxy ? CreateProxy(typeof(IValueProxy)) : null;
            object? mProxy = useProxy ? CreateProxy(Original.GetProperty("Proxy")!.PropertyType) : null;
            MethodComparison.Compare(useProxy ? "constructor with proxy" : "constructor with null",
                Contract, qCtor, null, [qProxy], mCtor, null, [mProxy], Show, Show);
        }
    }

    [SkippableFact]
    public void Proxy_get_cases()
    {
        Mathcad.Require();
        MethodInfo qGetter = Quire.GetProperty("Proxy", Instance)!.GetGetMethod()!;
        MethodInfo mGetter = Original.GetProperty("Proxy", Instance)!.GetGetMethod()!;
        CheckOriginal(mGetter, 0x0600006a);
        foreach (bool useProxy in new[] { false, true })
        {
            object? qProxy = useProxy ? CreateProxy(typeof(IValueProxy)) : null;
            object? mProxy = useProxy ? CreateProxy(Original.GetProperty("Proxy")!.PropertyType) : null;
            object qTarget = Activate(Quire, qProxy);
            object mTarget = Activate(Original, mProxy);
            MethodComparison.Compare(useProxy ? "getter with proxy" : "getter with null",
                Contract, qGetter, qTarget, [], mGetter, mTarget, [], Show, Show, WrapperState, WrapperState);
        }
    }

    [SkippableFact]
    public void Proxy_set_cases()
    {
        Mathcad.Require();
        MethodInfo qSetter = Quire.GetProperty("Proxy", Instance)!.GetSetMethod(nonPublic: true)!;
        MethodInfo mSetter = Original.GetProperty("Proxy", Instance)!.GetSetMethod(nonPublic: true)!;
        CheckOriginal(mSetter, 0x0600006b);
        foreach (bool useProxy in new[] { false, true })
        {
            object? qProxy = useProxy ? CreateProxy(typeof(IValueProxy)) : null;
            object? mProxy = useProxy ? CreateProxy(Original.GetProperty("Proxy")!.PropertyType) : null;
            object qTarget = Activate(Quire, null);
            object mTarget = Activate(Original, null);
            MethodComparison.Compare(useProxy ? "setter with proxy" : "setter with null",
                Contract, qSetter, qTarget, [qProxy], mSetter, mTarget, [mProxy], Show, Show, WrapperState, WrapperState);
        }
    }

    private static object Activate(System.Type type, object? proxy) =>
        Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null, args: [proxy], culture: null)!;

    private static object CreateProxy(System.Type interfaceType)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, typeof(ValueProxyDispatchProxy));
        return create.Invoke(null, null)!;
    }

    public class ValueProxyDispatchProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod!.Name == nameof(object.Equals)) return ReferenceEquals(this, args?[0]);
            if (targetMethod.Name == nameof(object.GetHashCode)) return RuntimeHelpers.GetHashCode(this);
            return targetMethod.ReturnType == typeof(void) ? null
                : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                : null;
        }
    }

    private static string WrapperState(object wrapper) => Show(wrapper.GetType().GetProperty("Proxy")!.GetValue(wrapper));

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is DispatchProxy) return "proxy";
        if (value.GetType().Name == "ValueProxyWrapper")
            return "ValueProxyWrapper(" + WrapperState(value) + ")";
        return value.GetType().Name;
    }

    private static void CheckOriginal(MethodBase method, int token)
    {
        Assert.True(method.Module.ModuleVersionId == Guid.Parse("82391b36-0d84-45c3-b21a-f7dccf288c68"),
            $"Expected Calculation MVID 82391b36-0d84-45c3-b21a-f7dccf288c68, got {method.Module.ModuleVersionId} at {method.Module.Assembly.Location}");
        Assert.True(method.MetadataToken == token,
            $"Expected Calculation token 0x{token:x8}, got 0x{method.MetadataToken:x8} for {method}");
    }
}

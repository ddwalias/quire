using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreSupportCalculatableChildHiderChildrenMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Support.CalculatableChildHider";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Type Original = Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static readonly Type OriginalCalculatable = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculatable");
    private static readonly Type OriginalContainer = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculatableContainer");
    private static readonly Type OriginalHideable = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculationHideableChild");
    private static readonly Type QuireType = typeof(CalculatableChildHider);
    private static readonly FieldInfo OriginalChildren = Original.GetField("_containerChildren", Instance)!;
    private static readonly FieldInfo QuireChildren = QuireType.GetField("_containerChildren", Instance)!;
    private static readonly MethodInfo QuireGetter = QuireType.GetProperty("CalculatableChildren", Instance)!.GetMethod!;
    private static readonly MethodInfo OriginalGetter = Original.GetProperty("CalculatableChildren", Instance)!.GetMethod!;
    private static readonly MethodInfo QuireHidden = QuireType.GetMethod("IsHiddenFromCalculation", BindingFlags.Static | BindingFlags.Public)!;
    private static readonly MethodInfo OriginalHidden = Original.GetMethod("IsHiddenFromCalculation", BindingFlags.Static | BindingFlags.Public)!;

    [SkippableFact]
    public void CalculatableChildren_cases()
    {
        Mathcad.Require();
        CheckOriginal();

        CompareHidden("null child", null);
        CompareHidden("ordinary child", new object());
        CompareHidden("visible hideable child", CreateHideable(QuireHideable, false));
        CompareHidden("hidden hideable child", CreateHideable(QuireHideable, true));
        Compare("ignores non-calculatable children", [new object(), null!, "text"]);
        Compare("yields direct calculatable children", [CreateHideable(QuireHideable, false), CreateHideable(QuireHideable, true), CreateCalculatable(QuireCalculatable, "direct"), new object()]);
        Compare("flattens ordinary container children", [CreateContainer(QuireContainer, new object[] { CreateCalculatable(QuireCalculatable, "nested") })]);
    }

    private static readonly Type QuireCalculatable = typeof(ICalculatable);
    private static readonly Type QuireContainer = typeof(ICalculatableContainer);
    private static readonly Type QuireHideable = typeof(ICalculationHideableChild);

    private static void CompareHidden(string caseId, object? quireChild)
    {
        object? originalChild = quireChild switch
        {
            HideableDispatchProxy proxy => CreateHideable(OriginalHideable, proxy.Hidden),
            _ => quireChild
        };
        MethodComparison.Compare(caseId, Contract, QuireHidden, null, [quireChild], OriginalHidden, null, [originalChild], Render, Render);
    }

    private static void Compare(string caseId, object?[] quireChildren)
    {
        object[] originalChildren = quireChildren.Select(ToOriginalChild).ToArray();
        object qTarget = NewHider(QuireType, QuireChildren, quireChildren);
        object mTarget = NewHider(Original, OriginalChildren, originalChildren);
        MethodComparison.Compare(caseId, Contract, QuireGetter, qTarget, [], OriginalGetter, mTarget, [],
            Render, Render, RenderTarget, RenderTarget);
    }

    private static object ToOriginalChild(object? child)
    {
        if (child == null || child is string || child.GetType() == typeof(object))
        {
            return child!;
        }
        if (child is CalculatableDispatchProxy proxy)
        {
            return CreateCalculatable(OriginalCalculatable, proxy.Marker);
        }
        if (child is HideableDispatchProxy hideable)
        {
            return CreateHideable(OriginalHideable, hideable.Hidden);
        }
        if (child is ContainerDispatchProxy container)
        {
            return CreateContainer(OriginalContainer, container.Children.Select(item =>
                CreateCalculatable(OriginalCalculatable, ((CalculatableDispatchProxy)item).Marker)).ToArray());
        }
        throw new InvalidOperationException($"Unsupported child case {child.GetType().Name}");
    }

    private static object NewHider(Type type, FieldInfo childrenField, IEnumerable<object?> children)
    {
        object hider = RuntimeHelpers.GetUninitializedObject(type);
        childrenField.SetValue(hider, children);
        return hider;
    }

    private static object CreateCalculatable(Type interfaceType, string marker)
    {
        object proxy = CreateProxy(interfaceType, typeof(CalculatableDispatchProxy));
        ((CalculatableDispatchProxy)proxy).Marker = marker;
        return proxy;
    }

    private static object CreateHideable(Type interfaceType, bool hidden)
    {
        object proxy = CreateProxy(interfaceType, typeof(HideableDispatchProxy));
        ((HideableDispatchProxy)proxy).Hidden = hidden;
        return proxy;
    }

    private static object CreateContainer(Type interfaceType, object children)
    {
        object proxy = CreateProxy(interfaceType, typeof(ContainerDispatchProxy));
        ((ContainerDispatchProxy)proxy).Children = ((System.Collections.IEnumerable)children).Cast<object>().ToArray();
        return proxy;
    }

    private static object CreateProxy(Type interfaceType, Type proxyType)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, proxyType);
        return create.Invoke(null, null)!;
    }

    public class CalculatableDispatchProxy : DispatchProxy
    {
        public string Marker { get; set; } = string.Empty;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => targetMethod!.Name switch
        {
            "get_Rl" => Marker,
            _ when targetMethod.ReturnType == typeof(void) => null,
            _ when targetMethod.ReturnType.IsValueType => Activator.CreateInstance(targetMethod.ReturnType),
            _ => null
        };
    }

    public class ContainerDispatchProxy : DispatchProxy
    {
        public object[] Children { get; set; } = [];

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod!.Name == "get_CalculatableChildren")
            {
                Type elementType = targetMethod.ReturnType.GetGenericArguments()[0];
                Array result = Array.CreateInstance(elementType, Children.Length);
                for (int index = 0; index < Children.Length; index++)
                {
                    result.SetValue(Children[index], index);
                }
                return result;
            }
            return targetMethod.ReturnType == typeof(void) ? null
                : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                : null;
        }
    }

    public class HideableDispatchProxy : DispatchProxy
    {
        public bool Hidden { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) =>
            targetMethod!.Name == "get_HideFromCalculationContext" ? Hidden
                : targetMethod.ReturnType == typeof(void) ? null
                : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                : null;
    }

    private static string Render(object? value)
    {
        if (value == null)
        {
            return "null";
        }
        Type[] interfaces = value.GetType().GetInterfaces();
        Type? hideableType = interfaces.FirstOrDefault(type => type.Name == "ICalculationHideableChild");
        if (hideableType != null)
        {
            return (bool)hideableType.GetProperty("HideFromCalculationContext")!.GetValue(value)! ? "hidden" : "visible";
        }
        Type? calculatableType = interfaces.FirstOrDefault(type => type.Name == "ICalculatable");
        if (calculatableType != null)
        {
            return (string?)calculatableType.GetProperty("Rl")!.GetValue(value) ?? "null-rl";
        }
        Type? containerType = interfaces.FirstOrDefault(type => type.Name == "ICalculatableContainer");
        if (containerType != null)
        {
            return $"container({Render(containerType.GetProperty("CalculatableChildren")!.GetValue(value))})";
        }
        if (value is System.Collections.IEnumerable sequence && value is not string)
        {
            return string.Join(",", sequence.Cast<object?>().Select(Render));
        }
        return value.ToString()!;
    }

    private static string RenderTarget(object target)
    {
        FieldInfo field = target.GetType() == Original ? OriginalChildren : QuireChildren;
        return Render(field.GetValue(target));
    }

    private static void CheckOriginal()
    {
        Assert.Equal(Guid.Parse("402d6e7b-941e-48f3-b17b-e1cd1a92b615"), OriginalGetter.Module.ModuleVersionId);
        Assert.Equal(0x060002cc, OriginalGetter.MetadataToken);
        Assert.Equal(0x060002cb, OriginalHidden.MetadataToken);
    }
}

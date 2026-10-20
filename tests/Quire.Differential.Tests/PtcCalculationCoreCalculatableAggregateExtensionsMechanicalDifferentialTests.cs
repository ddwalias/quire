using System.Reflection;
using Quire.Calculation.Core;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreCalculatableAggregateExtensionsMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.CalculatableAggregateExtensions";
    private static readonly System.Type OriginalAggregate = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculatableAggregate");
    private static readonly System.Type OriginalCalculatable = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ICalculatable");
    private static readonly MethodInfo OriginalMethod = (MethodInfo)Mathcad.Load("Ptc.Calculation.Core").ManifestModule.ResolveMethod(0x0600007f)!;
    private static readonly MethodInfo QuireMethod = typeof(CalculatableAggregateExtensions).GetMethod(nameof(CalculatableAggregateExtensions.GetAggregateRl))!;

    [SkippableFact]
    public void GetAggregateRl_0600007f()
    {
        Assert.Equal(Guid.Parse("402d6e7b-941e-48f3-b17b-e1cd1a92b615"), OriginalMethod.Module.ModuleVersionId);
        foreach (var (parent, childReads) in new[]
        {
            ("parent", new[] { Array.Empty<string>(), Array.Empty<string>() }),
            ("parent", new[] { new[] { "count-only" }, new[] { "first", "second" } }),
            ("", new[] { new[] { "same", "same" }, new[] { "same", "same" } }),
        })
        {
            object quireAggregate = CreateProxy(typeof(ICalculatableAggregate), typeof(ICalculatable), parent, childReads);
            object mathcadAggregate = CreateProxy(OriginalAggregate, OriginalCalculatable, parent, childReads);
            MethodComparison.Compare($"GetAggregateRl(parent={parent}, reads={Describe(childReads)})", Contract,
                QuireMethod, null, [quireAggregate], OriginalMethod, null, [mathcadAggregate], Render, Render);
        }
    }

    private static object CreateProxy(System.Type aggregateType, System.Type calculatableType, string parent, string[][] reads)
    {
        Array[] childSets = reads.Select(values =>
        {
            Array array = Array.CreateInstance(calculatableType, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array.SetValue(CreateCalculatable(calculatableType, values[i]), i);
            }
            return array;
        }).ToArray();
        return CreateProxy(aggregateType, parent, childSets);
    }

    private static object CreateCalculatable(System.Type calculatableType, string rl) =>
        CreateProxy(calculatableType, rl, Array.Empty<Array>());

    private static object CreateProxy(System.Type interfaceType, string rl, Array[] childSets)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, typeof(CalculatableDispatchProxy));
        var proxy = (CalculatableDispatchProxy)create.Invoke(null, null)!;
        proxy.Rl = rl;
        proxy.ChildSets = childSets;
        return proxy;
    }

    public class CalculatableDispatchProxy : DispatchProxy
    {
        public string? Rl { get; set; }
        public Array[] ChildSets { get; set; } = [];
        private int _reads;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            return targetMethod!.Name switch
            {
                "get_Rl" => Rl,
                "get_CalculatableChildren" => ChildSets[Math.Min(_reads++, ChildSets.Length - 1)],
                _ => targetMethod.ReturnType == typeof(void) ? null
                    : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                    : null,
            };
        }
    }

    private static string Describe(string[][] reads) => string.Join("/", reads.Select(read => "[" + string.Join(",", read) + "]"));

    private static string Render(object? value)
    {
        if (value == null) return "null";
        if (value is CalculatableDispatchProxy proxy)
            return $"rl={proxy.Rl};children={string.Join("/", proxy.ChildSets.Select(set => RenderEnumerable(set)))}";
        if (value is System.Collections.IEnumerable items && value is not string)
            return RenderEnumerable(items);
        return value.ToString()!;
    }

    private static string RenderEnumerable(System.Collections.IEnumerable items) =>
        "[" + string.Join(",", items.Cast<object?>().Select(Render)) + "]";
}

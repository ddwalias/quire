using System;
using System.Linq;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionDifferentialTests
{
    private static object? CallGeneric(Type target, string name, Type[] typeArgs, params object?[] args)
    {
        var method = target.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Single(m => m.Name == name && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == typeArgs.Length);
        return method.MakeGenericMethod(typeArgs).Invoke(null, args);
    }

    [SkippableFact]
    public void FunctionUtilities_array_operations_and_caches() => Scenario.Compare((s, log) =>
    {
        var u = s.T("Mpl.Run.FunctionUtilities");

        log.Log(s, "maxargs", () => Get(u, "maxargs"));
        log.Log(s, "empty_vec length", () => ((object[])Get(u, "empty_vec")!).Length);

        object[] a = [1, 2];
        object[] b = [3, 4, 5];
        log.Log(s, "concat", () => string.Join(",", (object[])Call(u, "concat", a, b)!));
        log.Log(s, "concat empty", () => string.Join(",", (object[])Call(u, "concat", a, Array.Empty<object>())!));

        int[] nums = [10, 20, 30, 40];
        log.Log(s, "take 2", () => string.Join(",", ((int[])CallGeneric(u, "take", [typeof(int)], 2, nums))!));
        log.Log(s, "take 10", () => string.Join(",", ((int[])CallGeneric(u, "take", [typeof(int)], 10, nums))!));
        log.Log(s, "but 2", () => string.Join(",", ((int[])CallGeneric(u, "but", [typeof(int)], 2, nums))!));
        log.Log(s, "but 10", () => string.Join(",", ((int[])CallGeneric(u, "but", [typeof(int)], 10, nums))!));

        log.Log(s, "PushFront 1", () => string.Join(",", (object[])Call(u, "PushFront", 0, a)!));
        log.Log(s, "PushFront 2", () => string.Join(",", (object[])Call(u, "PushFront", -1, 0, a)!));
        log.Log(s, "Push 1", () => string.Join(",", (object[])Call(u, "Push", a, 99)!));
        log.Log(s, "Push 2", () => string.Join(",", (object[])Call(u, "Push", a, 98, 99)!));
    });

    [SkippableFact]
    public void Delegates_projections_and_invocations() => Scenario.Compare((s, log) =>
    {
        var d = s.T("Mpl.Run.Delegates");

        var p21 = CallGeneric(d, "project21", [typeof(int), typeof(string)]) as Delegate;
        var p22 = CallGeneric(d, "project22", [typeof(int), typeof(string)]) as Delegate;
        log.Log(s, "project21", () => p21!.DynamicInvoke(42, "hello"));
        log.Log(s, "project22", () => p22!.DynamicInvoke(42, "hello"));

        var pi1 = CallGeneric(d, "pi_21", [typeof(int), typeof(int)]);
        var pi2 = CallGeneric(d, "pi_22", [typeof(int), typeof(int)]);
        log.Log(s, "pi_21 apply", () => Call(pi1!, "Apply", (object)new object[] { 10, 20 }));
        log.Log(s, "pi_22 apply", () => Call(pi2!, "Apply", (object)new object[] { 10, 20 }));
    });

    [SkippableFact]
    public void Function_helpers_formatting_and_composition() => Scenario.Compare((s, log) =>
    {
        var fType = s.T("Mpl.Run.Function");

        log.Log(s, "format_vector empty", () => Call(fType, "format_vector", (object)Array.Empty<object>()));
        log.Log(s, "format_vector null", () => Call(fType, "format_vector", (object?)null));
        log.Log(s, "format_vector values", () => Call(fType, "format_vector", (object)new object[] { 1, "abc", 3.5 }));

        log.Log(s, "canonical_name simple", () => Call(fType, "canonical_name", "simple"));
        log.Log(s, "canonical_name null", () => Call(fType, "canonical_name", (object?)null));

        var c = Call(fType, "ConstantFunction", 42)!;
        log.Log(s, "const format", () => c.ToString());
        log.Log(s, "const apply 0", () => Call(c, "Apply", (object)Array.Empty<object>()));
        log.Log(s, "const apply 2", () => Call(c, "Apply", (object)new object[] { 1, 2 }));
    });
}

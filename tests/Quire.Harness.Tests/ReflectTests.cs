using Quire.Differential.Tests;

namespace Quire.Harness.Tests;

public class ReflectTests
{
    private sealed class Probe
    {
        public static string Pick(string? value) => "string";
        public static string Pick(object? value) => "object";
        public static void Throw() => throw new ArgumentException("runtime error");
        public static int Value() => 3;
        public int Instance() => 4;
        public static void Ref(ref int value) { value++; }
        public static void Ref(ref string value) { value += "!"; }
    }

    private class BaseProbe { public virtual object Apply(object[] values) => "base"; }
    private class MiddleProbe : BaseProbe { public override object Apply(object[] values) => "middle"; }
    private sealed class DerivedProbe : MiddleProbe { }

    [Fact]
    public void Inherited_override_is_selected_once() =>
        Assert.Equal("middle", Reflect.CallSig(new DerivedProbe(), "Apply", ["Object[]"], (object)Array.Empty<object>()));

    [Fact]
    public void Missing_member_fails_instead_of_matching_errors() =>
        Assert.Throws<HarnessException>(() => Reflect.Outcome(() => Reflect.Call(typeof(Probe), "Missing")));

    [Fact]
    public void Ambiguous_overload_fails_and_explicit_signature_resolves()
    {
        Assert.Throws<HarnessException>(() => Reflect.Outcome(() => Reflect.Call(typeof(Probe), "Pick", [null])));
        Assert.Equal("string", Reflect.CallSig(typeof(Probe), "Pick", ["String"], [null]));
    }

    [Fact]
    public void Target_exception_is_still_a_comparable_outcome() =>
        Assert.Equal("throws ArgumentException: runtime error", Reflect.Outcome(() => Reflect.Call(typeof(Probe), "Throw")));

    [Fact]
    public void Renderer_failure_fails_test() =>
        Assert.Throws<InvalidOperationException>(() => Reflect.Outcome(() => Reflect.Call(typeof(Probe), "Value"),
            _ => throw new InvalidOperationException("broken renderer")));

    [Fact]
    public void Static_target_cannot_select_instance_method() =>
        Assert.Throws<HarnessException>(() => Reflect.Outcome(() => Reflect.Call(typeof(Probe), "Instance")));

    [Fact]
    public void Byref_arguments_select_by_element_type_and_keep_mutation()
    {
        object?[] args = [1];
        Reflect.Call(typeof(Probe), "Ref", args);
        Assert.Equal(2, args[0]);
    }

    private static IEnumerable<int> BrokenIterator()
    {
        yield return 1;
        throw new ArgumentException("iterator error");
    }

    [Fact]
    public void Deferred_runtime_error_is_compared() =>
        Assert.Equal("throws ArgumentException: iterator error", Reflect.Outcome(() => BrokenIterator()));

    [Fact]
    public void Typed_array_is_preserved_for_explicit_renderer() =>
        Assert.Equal("2", Reflect.Outcome(() => new int[] { 1, 2 }, value => ((int[])value!).Length.ToString()));

    [Fact]
    public void Bad_explicit_arguments_fail_instead_of_matching_binder_errors() =>
        Assert.Throws<HarnessException>(() => Reflect.Outcome(() => Reflect.CallSig(typeof(Probe), "Value", [], 1)));

    [Fact]
    public void Different_values_and_errors_fail_comparison()
    {
        var diffs = new List<string>();
        Reflect.Same(diffs, "value", () => 1, () => 2);
        Reflect.Same(diffs, "error", () => throw new ArgumentException("a"), () => throw new ArgumentException("b"));
        Assert.Equal(2, diffs.Count);
        Assert.ThrowsAny<Exception>(() => Reflect.NoDiffs(diffs));
    }
}

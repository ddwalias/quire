using System.Reflection;
using Quire.Differential.Tests;

namespace Quire.Harness.Tests;

public class MethodComparisonTests
{
    private interface IDefault { int Read() => 1; }
    private sealed class OverrideDefault : IDefault { public int Read() => 2; }
    private sealed class Counter { public int Value; public int Increment(int n) => Value += n; }
    private static int Echo(int value) => value;
    private static int Different(int value) => value + 1;
    private static int Throw(int value) => throw new ArgumentException("expected target error");
    private static MethodInfo M(string name) => typeof(MethodComparisonTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;

    [Fact]
    public void Private_methods_are_invoked_directly() =>
        MethodComparison.Compare("one", "fixture", M("Echo"), null, [1], M("Echo"), null, [1]);

    [Fact]
    public void Different_outcomes_fail() =>
        Assert.ThrowsAny<Exception>(() => MethodComparison.Compare("one", "fixture", M("Echo"), null, [1], M("Different"), null, [1]));

    [Fact]
    public void Different_inputs_fail_even_when_renderer_could_match_results() =>
        Assert.ThrowsAny<Exception>(() => MethodComparison.Compare("one", "fixture", M("Echo"), null, [1], M("Echo"), null, [2]));

    [Fact]
    public void Target_errors_can_be_compared() =>
        MethodComparison.Compare("error", "fixture", M("Throw"), null, [1], M("Throw"), null, [1]);

    [Fact]
    public void Instance_state_must_be_observed()
    {
        var method = typeof(Counter).GetMethod("Increment")!;
        Assert.Throws<HarnessException>(() => MethodComparison.Compare("increment", "fixture", method, new Counter(), [1], method, new Counter(), [1]));
        MethodComparison.Compare("increment", "fixture", method, new Counter(), [1], method, new Counter(), [1],
            quireState: value => ((Counter)value).Value.ToString(), originalState: value => ((Counter)value).Value.ToString());
    }

    [Fact]
    public void Interface_dispatch_cannot_credit_the_default_body()
    {
        var method = typeof(IDefault).GetMethod("Read")!;
        Assert.Throws<HarnessException>(() => MethodComparison.Compare("default", "fixture", method, new OverrideDefault(), [],
            method, new OverrideDefault(), [], quireState: _ => "state", originalState: _ => "state"));
    }

    [Fact]
    public void Reflection_binder_errors_do_not_count_as_equal() =>
        Assert.Throws<HarnessException>(() => MethodComparison.Compare("bad args", "fixture", M("Echo"), null, [], M("Echo"), null, []));
}

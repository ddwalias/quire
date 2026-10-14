using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValCollectionDifferentialTests
{
    [SkippableFact]
    public void Construction_push_shape_and_fill() => Scenario.Compare((s, log) =>
    {
        var t = s.T("Mcd.MatVal");
        void State(string label, object m) => log.Add(label + " " + MatValDifferentialTests.Storage(s, m));
        var vals = new object[] { s.Num(0, 0, "poly"), s.Num(2, 3, "L"), s.Num(double.NaN, 0, "identity") };
        foreach (string method in new[] { "MakeVector", "MakeVector0" })
        {
            log.Log(s, method, () => { var m = Call(t, method, (object)vals)!; State(method, m); return null; });
        }
        foreach (string dim in new[] { "poly", "identity", "L" })
        {
            var numeric = s.Num(3, 4, dim);
            var replicated = Call(t, "Replicate", numeric, 3)!;
            State("replicate " + dim, replicated);
        }
        var reserve = Call(t, "reserve", 4)!;
        State("reserve", reserve);
        log.Log(s, "PushRawDouble", () => CallSig(reserve, "PushRaw", ["Double"], 4.0));
        log.Log(s, "PushReal", () => Call(reserve, "PushReal", 5.0));
        log.Log(s, "PushZero", () => Call(reserve, "PushZero"));
        log.Log(s, "PushRawIndex", () => Call(reserve, "PushRawIndex", s.Num(6, 0, "poly")));
        State("pushed", reserve);
        Call(reserve, "Reshape", 2, 2);
        State("reshaped", reserve);
        var fill = New(t, 5, 1, true);
        Call(fill, "Fill", "pointer", 0, 2, 3);
        State("pointer fill", fill);
        Call(fill, "Fill", s.Num(7, 2, "L"), 4, -2, 3);
        State("numeric fill", fill);
        Call(fill, "Fill", s.Num(1, 0, "poly"), 0, 2, 3);
        State("real fill", fill);
        Set(fill, "Immutable", true);
        var copied = Call(fill, "Fill", s.Num(9, 0, "poly"), 6, 1, 2)!;
        State("immutable input", fill);
        State("immutable output", copied);
    });
}

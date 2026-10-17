namespace Quire.Differential.Tests;

using static Quire.Differential.Tests.MplRunSides;

/// <summary>
/// Compares mplrun's <c>Mpl.Run.Primitive</c> comparisons and cast helpers with Quire's using direct
/// <see cref="MethodComparison.Compare"/> calls; cases are listed in docs/mirror/evidence/Mpl.Run.Primitive.md.
/// </summary>
public class PrimitiveDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly string[] Comparisons = ["ceq", "cne", "clt", "cle", "cgt", "cge"];

    private static void CompareAll<T>(string suffix, T[] values)
    {
        var quire = typeof(Mpl.Run.Primitive);
        var mathcad = Mathcad.Type("mplrun", "Mpl.Run.Primitive");
        foreach (var name in Comparisons)
        {
            var q = quire.GetMethod(name + suffix, [typeof(T), typeof(T)])
                ?? throw new HarnessException($"Quire method not found: Primitive.{name}{suffix}");
            var m = mathcad.GetMethod(name + suffix, [typeof(T), typeof(T)])
                ?? throw new HarnessException($"Mathcad method not found: Primitive.{name}{suffix}");
            foreach (var x in values)
                foreach (var y in values)
                    MethodComparison.Compare($"{name}{suffix}({x}, {y})", Contract, q, null, [x, y], m, null, [x, y]);
        }
    }

    [SkippableFact]
    public void Int_comparisons() => CompareAll("_i4", [int.MinValue, -1, 0, 1, int.MaxValue]);

    [SkippableFact]
    public void Double_comparisons() => CompareAll("_f8",
        [double.NegativeInfinity, -1.0, -0.0, 0.0, 1.0, double.PositiveInfinity, double.NaN]);

    [SkippableFact]
    public void Cast_helpers_return_the_same_target_instance()
    {
        var qType = typeof(Mpl.Run.Primitive);
        var mType = M("Mpl.Run.Primitive");
        var qMat = Reflect.New(typeof(global::Quire.Mcd.MatVal), 1, 1);
        var mMat = Reflect.New(M("Mcd.MatVal"), 1, 1);
        var qNum = Side.Quire.Num(2.5, 0.0, "identity");
        var mNum = Side.Mathcad.Num(2.5, 0.0, "identity");
        const string qString = "cast target";
        const string mString = "cast target";
        Func<object, object> identity = value => value;
        var qFunction = global::Quire.Mpl.Run.DelegateFunction.make(identity, 1);
        var mFunction = Reflect.New(M("Mpl.Run.DelegateFunction"), identity, 1);
        var qRange = Reflect.New(typeof(global::Quire.Mcd.RngVal), 1.0, 2.0, 3, Side.Quire.D("identity"));
        var mRange = Reflect.New(M("Mcd.RngVal"), 1.0, 2.0, 3, Side.Mathcad.D("identity"));

        CompareCast("as_matrix", qType, mType, qMat, mMat);
        CompareCast("as_number", qType, mType, qNum, mNum);
        CompareCast("as_string", qType, mType, qString, mString);
        CompareCast("as_function", qType, mType, qFunction, mFunction);
        CompareCast("as_range", qType, mType, qRange, mRange);
    }

    private static void CompareCast(string name, System.Type qType, System.Type mType, object qTarget, object mTarget)
    {
        var qMethod = qType.GetMethod(name, [typeof(object)])!;
        var mMethod = mType.GetMethod(name, [typeof(object)])!;
        Func<object?, string> qShow = value => ReferenceEquals(value, qTarget) ? "same target" : Describe(value);
        Func<object?, string> mShow = value => ReferenceEquals(value, mTarget) ? "same target" : Describe(value);
        MethodComparison.Compare($"{name} returns its target instance", "Mpl.Run.Primitive",
            qMethod, null, [qTarget], mMethod, null, [mTarget], qShow, mShow);
    }
}

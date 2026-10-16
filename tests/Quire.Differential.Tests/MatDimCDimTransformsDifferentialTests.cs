using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimCDimTransformsDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const BindingFlags Declared = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly (string Name, int Token)[] Members =
    [
        ("transpose", 0x060007e4),
        ("det", 0x060007e5),
        ("invert", 0x060007e6)
    ];

    private static MethodInfo Method(Side side, string name, params System.Type[] args)
    {
        var type = Nested(side.T("Mcd.MatDim"), "c_dim");
        var method = type.GetMethod(name, Declared, null, args, null);
        return method ?? throw new HarnessException($"Missing {side.Name} c_dim.{name}");
    }

    private static Array Dims(Side side, params string[] names)
    {
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) values.SetValue(side.D(names[i]), i);
        return values;
    }

    private static object Column(Side side, string[] names, int rows) =>
        CallSig(Nested(side.T("Mcd.MatDim"), "c_dim"), "make", ["Dim[]", "Int32"], Dims(side, names), rows)!;

    private static string Axis(Side side, object? array) => array is Array values
        ? $"{array.GetType().GetElementType()?.Name}[{string.Join(",", values.Cast<object?>().Select(side.DimText))}]"
        : "null";

    private static string ReceiverState(Side side, object target, object? beforeAxis, object?[] beforeFactors)
    {
        var afterAxis = Get(target, "m_coldim") as Array;
        string factorRefs = afterAxis == null ? "null" : string.Join(",", Enumerable.Range(0, beforeFactors.Length)
            .Select(i => ReferenceEquals(beforeFactors[i], afterAxis.GetValue(i)) ? "same" : "changed"));
        return $"{target.GetType().Name} {Get(target, "Rows")}x{Get(target, "Cols")}; axis-ref={ReferenceEquals(beforeAxis, afterAxis)}; axis={Axis(side, afterAxis)}; factor-refs=[{factorRefs}]";
    }

    private static void Compare(string caseId, string name, int token, object qTarget, object mTarget, object?[] qArgs, object?[] mArgs,
        Func<Side, object?, object, string> qShow, Func<Side, object?, object, string> mShow)
    {
        var qMethod = Method(Side.Quire, name, qArgs.Length == 0 ? [] : [typeof(int)]);
        var mMethod = Method(Side.Mathcad, name, mArgs.Length == 0 ? [] : [typeof(int)]);
        Assert.Equal(token, mMethod.MetadataToken);
        var qAxis = Get(qTarget, "m_coldim");
        var mAxis = Get(mTarget, "m_coldim");
        var qFactors = ((Array)qAxis!).Cast<object?>().ToArray();
        var mFactors = ((Array)mAxis!).Cast<object?>().ToArray();
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs,
            quireShow: result => qShow(Side.Quire, result, qTarget),
            originalShow: result => mShow(Side.Mathcad, result, mTarget),
            quireState: target => ReceiverState(Side.Quire, target, qAxis, qFactors),
            originalState: target => ReceiverState(Side.Mathcad, target, mAxis, mFactors));
    }

    private static string TransposeResult(Side side, object? result, object target)
    {
        if (result == null) return "null";
        var axis = Get(target, "m_coldim");
        return $"{Describe(result)}; row-alias={ReferenceEquals(axis, Get(result, "m_rowdim"))}; row={Axis(side, Get(result, "m_rowdim"))}";
    }

    private static string DetResult(Side side, object? result, object target)
    {
        if (result is int argument) return argument.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var expected = CallSig(side.T("Mcd.MatDim"), "Product", ["Dim", "Dim[]"], side.Marker("identity"), Get(target, "m_coldim"));
        return $"{side.DimText(result)}; exact-product-ref={ReferenceEquals(result, expected)}";
    }

    private static string InvertResult(Side side, object? result, object target)
    {
        if (result == null) return "null";
        var source = (Array)Get(target, "m_coldim")!;
        var actual = (Array)Get(result, "m_rowdim")!;
        var expected = Array.CreateInstance(side.Dim, source.Length);
        bool exactPowers = true;
        for (int i = 0; i < source.Length; i++)
        {
            var power = CallSig(side.Dim, "Pwr", ["Dim", "Double"], source.GetValue(i), -1.0);
            expected.SetValue(power, i);
            exactPowers &= ReferenceEquals(power, actual.GetValue(i));
        }
        return $"{Describe(result)}; row-fresh={!ReferenceEquals(source, actual)}; row={Axis(side, actual)}; exact-powers={exactPowers}; expected={Axis(side, expected)}";
    }

    private static string Describe(object result) =>
        $"{result.GetType().Name} {Get(result, "Rows")}x{Get(result, "Cols")}";

    private static void CompareBoth(string caseId, string name, int token, string[] names, int rows, int arg,
        Func<Side, object?, object, string> show)
    {
        var q = Column(Side.Quire, names, rows);
        var m = Column(Side.Mathcad, names, rows);
        object?[] arguments = name == "det" ? [arg] : [];
        Compare(caseId, name, token, q, m, arguments, arguments, show, show);
    }

    [SkippableFact]
    public void Cdim_transpose_det_and_invert_match_the_pinned_bodies()
    {
        Runtime.Sync();
        foreach (var (name, token) in Members)
            Assert.Equal(token, Method(Side.Mathcad, name, name == "det" ? [typeof(int)] : []).MetadataToken);

        CompareBoth("c_dim.transpose maps input 5x3 to 3x5 and retains the column array", "transpose", 0x060007e4,
            ["L", "T", "N"], 5, int.MinValue, TransposeResult);
        CompareBoth("c_dim.transpose maps input 4x0 to 0x4 and retains the empty axis", "transpose", 0x060007e4,
            [], 4, int.MinValue, TransposeResult);

        foreach (int n in new[] { int.MinValue, -1, 0, 1, int.MaxValue })
            CompareBoth($"c_dim.det ignores n={n} and returns Product(identity, columns)", "det", 0x060007e5,
                ["L", "T", "N"], 2, n, DetResult);
        CompareBoth("c_dim.det of an empty axis returns the empty product", "det", 0x060007e5,
            [], 2, 0, DetResult);
        CompareBoth("c_dim.det passes a null factor through Product", "det", 0x060007e5,
            ["L", "poly", "T"], 2, 4, DetResult);

        CompareBoth("c_dim.invert creates an r_dim with exact inverses in column order", "invert", 0x060007e6,
            ["L", "T", "N"], 2, int.MinValue, InvertResult);
        CompareBoth("c_dim.invert creates a fresh empty row axis", "invert", 0x060007e6,
            [], 3, int.MinValue, InvertResult);
        CompareBoth("c_dim.invert passes a null factor through Invert and Dim.Pwr", "invert", 0x060007e6,
            ["L", "poly", "T"], 2, int.MinValue, InvertResult);
    }
}

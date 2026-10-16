using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimBaseDispatchDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const BindingFlags StaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static System.Type Parameter(Side side, string name) => name switch
    {
        "MatDim" => side.T("Mcd.MatDim"),
        "Dim" => side.Dim,
        "MatDim[]" => side.T("Mcd.MatDim").MakeArrayType(),
        "Dim[]" => side.Dim.MakeArrayType(),
        "Int32" => typeof(int),
        _ => throw new HarnessException($"Unknown MatDim parameter type {name}")
    };

    private static MethodInfo Method(Side side, string name, params string[] parameters) =>
        side.T("Mcd.MatDim").GetMethod(name, StaticPublic, null, parameters.Select(p => Parameter(side, p)).ToArray(), null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{name}({string.Join(", ", parameters)})");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Uniform(Side side, string dim = "L", int rows = 2, int cols = 3) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dim), rows, cols)!;

    private static object Row(Side side, string[] dims, int rows = 4) =>
        Call(Nested(side.T("Mcd.MatDim"), "r_dim"), "make", Dims(side, dims), rows)!;

    private static object Column(Side side, string[] dims, int cols = 4) =>
        Call(Nested(side.T("Mcd.MatDim"), "c_dim"), "make", Dims(side, dims), cols)!;

    private static object RowColumn(Side side, string[] rows, string[] cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "rc_dim"), "make_rc", Dims(side, rows), Dims(side, cols))!;

    private static object ScalarMatrix(Side side, string dim) =>
        Call(side.T("Mcd.MatDim"), "make", side.D(dim))!;

    private static object Crazy(Side side)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[4], new double[4], 2, 2, 2,
            new System.Collections.Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Set(matrix, "m_dim", side.D("L"));
        var type = Nested(side.T("Mcd.MatDim"), "crazy_dim");
        var make = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Single(method => method.Name == "make" && method.GetParameters() is [{ ParameterType: var parameter }] && parameter == side.T("Mcd.MatVal"));
        return make.Invoke(null, [matrix])!;
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(item => side.DimText(item))) + "]"
        : "null";

    private static object? MaybeGet(object target, string member)
    {
        try { return Get(target, member); }
        catch (HarnessException) { return null; }
    }

    private static string MatrixText(Side side, object value, object?[] args)
    {
        string Alias(object? candidate)
        {
            int index = Array.FindIndex(args, arg => ReferenceEquals(arg, candidate));
            if (index >= 0) return $"arg-{index}";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null || (args[i]!.GetType().Name != "MatDim" && args[i]!.GetType().BaseType?.Name != "MatDim")) continue;
                foreach (string member in new[] { "m_dim", "m_rowdim", "m_coldim" })
                    if (ReferenceEquals(MaybeGet(args[i]!, member), candidate)) return $"arg-{i}.{member}";
            }
            return "new";
        }
        string alias = Alias(value);
        object? row = MaybeGet(value, "m_rowdim"), col = MaybeGet(value, "m_coldim");
        object? uniform = MaybeGet(value, "m_dim");
        return $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")} {alias}" +
            $" uniform={side.DimText(uniform)}({Alias(uniform)})" +
            $" row={ArrayText(side, row)}({Alias(row)}) col={ArrayText(side, col)}({Alias(col)})";
    }

    private static string ValueText(Side side, object? value, object?[] args) => value switch
    {
        null => "null",
        bool or int or uint or string => Text(value),
        Array array when array.GetType().GetElementType() == side.Dim => ArrayText(side, array),
        _ when value.GetType().Name == "MatDim" || value.GetType().BaseType?.Name == "MatDim" => MatrixText(side, value, args),
        _ when value.GetType().Name == "Dim" || value.GetType().BaseType?.Name == "Dim" =>
            $"{side.DimText(value)}({Array.FindIndex(args, arg => ReferenceEquals(arg, value)) switch { -1 => "new", var i => $"arg-{i}" }})",
        _ => Text(value)
    };

    private static void Compare(string caseId, string name, int token, string[] parameters,
        object?[] qArgs, object?[] mArgs)
    {
        var qMethod = Method(Side.Quire, name, parameters);
        var mMethod = Method(Side.Mathcad, name, parameters);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => ValueText(Side.Quire, value, qArgs),
            originalShow: value => ValueText(Side.Mathcad, value, mArgs));
    }

    [SkippableFact]
    public void Equality_overloads_dispatch_to_dimension_kind_comparisons()
    {
        Runtime.Sync();
        const int matDimToken = 0x06000615;
        const int scalarToken = 0x06000616;

        Compare("Eq_b(MatDim,MatDim) uniform equality ignores shape", "Eq_b", matDimToken, ["MatDim", "MatDim"],
            [Uniform(Side.Quire, "L", 2, 3), Uniform(Side.Quire, "L", 7, 1)],
            [Uniform(Side.Mathcad, "L", 2, 3), Uniform(Side.Mathcad, "L", 7, 1)]);
        Compare("Eq_b(MatDim,MatDim) row dimensions compare axis values", "Eq_b", matDimToken, ["MatDim", "MatDim"],
            [Row(Side.Quire, ["L", "T"], 3), Row(Side.Quire, ["L", "T"], 8)],
            [Row(Side.Mathcad, ["L", "T"], 3), Row(Side.Mathcad, ["L", "T"], 8)]);
        Compare("Eq_b(MatDim,MatDim) row and column kinds differ", "Eq_b", matDimToken, ["MatDim", "MatDim"],
            [Row(Side.Quire, ["L", "T"]), Column(Side.Quire, ["L", "T"])],
            [Row(Side.Mathcad, ["L", "T"]), Column(Side.Mathcad, ["L", "T"])]);
        Compare("Eq_b(MatDim,MatDim) row-column factors compare independently", "Eq_b", matDimToken, ["MatDim", "MatDim"],
            [RowColumn(Side.Quire, ["L", "T"], ["T", "N"]), RowColumn(Side.Quire, ["L", "L"], ["T", "N"])],
            [RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), RowColumn(Side.Mathcad, ["L", "L"], ["T", "N"])]);
        Compare("Eq_b(MatDim,MatDim) matching row-column dimensions compare equal", "Eq_b", matDimToken, ["MatDim", "MatDim"],
            [RowColumn(Side.Quire, ["L", "T"], ["T", "N"]), RowColumn(Side.Quire, ["L", "T"], ["T", "N"])],
            [RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"])]);
        Compare("Eq_b(MatDim,Dim) accepts matching uniform units", "Eq_b", scalarToken, ["MatDim", "Dim"],
            [Uniform(Side.Quire, "L"), Side.Quire.D("L")], [Uniform(Side.Mathcad, "L"), Side.Mathcad.D("L")]);
        Compare("Eq_b(MatDim,Dim) rejects different uniform units", "Eq_b", scalarToken, ["MatDim", "Dim"],
            [Uniform(Side.Quire, "L"), Side.Quire.D("T")], [Uniform(Side.Mathcad, "L"), Side.Mathcad.D("T")]);
        Compare("Eq_b(MatDim,Dim) rejects row dimensions", "Eq_b", scalarToken, ["MatDim", "Dim"],
            [Row(Side.Quire, ["L", "T"]), Side.Quire.D("L")], [Row(Side.Mathcad, ["L", "T"]), Side.Mathcad.D("L")]);
        Compare("Eq_b(MatDim,MatDim) propagates null receiver", "Eq_b", matDimToken, ["MatDim", "MatDim"],
            [null, Uniform(Side.Quire)], [null, Uniform(Side.Mathcad)]);
    }

    [SkippableFact]
    public void Linear_algebra_entry_points_dispatch_to_dimension_kind_overrides()
    {
        Runtime.Sync();

        Compare("MMult uniform product", "MMult", 0x0600061a, ["MatDim", "MatDim"],
            [Uniform(Side.Quire, "L", 2, 3), Uniform(Side.Quire, "T", 3, 4)],
            [Uniform(Side.Mathcad, "L", 2, 3), Uniform(Side.Mathcad, "T", 3, 4)]);
        Compare("MMult row-column double dispatch", "MMult", 0x0600061a, ["MatDim", "MatDim"],
            [Row(Side.Quire, ["L", "T"], 4), Column(Side.Quire, ["L", "T"], 2)],
            [Row(Side.Mathcad, ["L", "T"], 4), Column(Side.Mathcad, ["L", "T"], 2)]);
        Compare("MMult unsupported crazy dimensions", "MMult", 0x0600061a, ["MatDim", "MatDim"],
            [Crazy(Side.Quire), Uniform(Side.Quire)], [Crazy(Side.Mathcad), Uniform(Side.Mathcad)]);
        Compare("MMult propagates null receiver", "MMult", 0x0600061a, ["MatDim", "MatDim"],
            [null, Uniform(Side.Quire)], [null, Uniform(Side.Mathcad)]);
        Compare("DMult uniform product", "DMult", 0x0600061b, ["MatDim", "MatDim"],
            [Uniform(Side.Quire, "L"), Uniform(Side.Quire, "T")],
            [Uniform(Side.Mathcad, "L"), Uniform(Side.Mathcad, "T")]);
        Compare("DMult row-column double dispatch", "DMult", 0x0600061b, ["MatDim", "MatDim"],
            [Row(Side.Quire, ["L", "L"]), Column(Side.Quire, ["T", "T"])],
            [Row(Side.Mathcad, ["L", "L"]), Column(Side.Mathcad, ["T", "T"])]);
        Compare("DMult unsupported crazy dimensions", "DMult", 0x0600061b, ["MatDim", "MatDim"],
            [Crazy(Side.Quire), Uniform(Side.Quire)], [Crazy(Side.Mathcad), Uniform(Side.Mathcad)]);
        Compare("DMult propagates null receiver", "DMult", 0x0600061b, ["MatDim", "MatDim"],
            [null, Uniform(Side.Quire)], [null, Uniform(Side.Mathcad)]);

        Compare("CMult null/null", "CMult", 0x0600061d, ["MatDim", "MatDim"],
            [null, null], [null, null]);
        Compare("CMult null/non-null", "CMult", 0x0600061d, ["MatDim", "MatDim"],
            [null, ScalarMatrix(Side.Quire, "L")], [null, ScalarMatrix(Side.Mathcad, "L")]);
        Compare("CMult non-null/null", "CMult", 0x0600061d, ["MatDim", "MatDim"],
            [ScalarMatrix(Side.Quire, "L"), null], [ScalarMatrix(Side.Mathcad, "L"), null]);
        Compare("CMult non-null/non-null", "CMult", 0x0600061d, ["MatDim", "MatDim"],
            [ScalarMatrix(Side.Quire, "L"), ScalarMatrix(Side.Quire, "T")],
            [ScalarMatrix(Side.Mathcad, "L"), ScalarMatrix(Side.Mathcad, "T")]);

        Compare("Transpose uniform preserves reference", "Transpose", 0x0600061c, ["MatDim"],
            [Uniform(Side.Quire, "L", 2, 3)], [Uniform(Side.Mathcad, "L", 2, 3)]);
        Compare("Transpose row becomes column", "Transpose", 0x0600061c, ["MatDim"],
            [Row(Side.Quire, ["L", "T"], 4)], [Row(Side.Mathcad, ["L", "T"], 4)]);
        Compare("Transpose row-column swaps factors", "Transpose", 0x0600061c, ["MatDim"],
            [RowColumn(Side.Quire, ["L", "T"], ["T", "N"])],
            [RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"])]);
        Compare("Transpose unsupported crazy dimensions", "Transpose", 0x0600061c, ["MatDim"],
            [Crazy(Side.Quire)], [Crazy(Side.Mathcad)]);
        Compare("Transpose propagates null receiver", "Transpose", 0x0600061c, ["MatDim"],
            [null], [null]);

        Compare("Det uniform raises dimension to supplied size", "Det", 0x0600061e, ["MatDim", "Int32"],
            [Uniform(Side.Quire, "L"), 3], [Uniform(Side.Mathcad, "L"), 3]);
        Compare("Det row dimensions fold axis", "Det", 0x0600061e, ["MatDim", "Int32"],
            [Row(Side.Quire, ["L", "T"]), 2], [Row(Side.Mathcad, ["L", "T"]), 2]);
        Compare("Det row-column dimensions fold both factors", "Det", 0x0600061e, ["MatDim", "Int32"],
            [RowColumn(Side.Quire, ["L", "T"], ["T", "N"]), 7],
            [RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), 7]);
        Compare("Det unsupported crazy dimensions", "Det", 0x0600061e, ["MatDim", "Int32"],
            [Crazy(Side.Quire), 2], [Crazy(Side.Mathcad), 2]);
        Compare("Det propagates null receiver", "Det", 0x0600061e, ["MatDim", "Int32"],
            [null, 2], [null, 2]);
    }

    [SkippableFact]
    public void Pwr_dispatches_positive_negative_and_zero_exponents()
    {
        Runtime.Sync();

        foreach (int exponent in new[] { 2, -1, -2, 0 })
            Compare($"Pwr uniform exponent {exponent}", "Pwr", 0x0600061f, ["MatDim", "Int32"],
                [Uniform(Side.Quire, "L", 2, 3), exponent], [Uniform(Side.Mathcad, "L", 2, 3), exponent]);
        foreach (int exponent in new[] { 2, -1, -2, 0 })
            Compare($"Pwr row exponent {exponent}", "Pwr", 0x0600061f, ["MatDim", "Int32"],
                [Row(Side.Quire, ["L", "T"]), exponent], [Row(Side.Mathcad, ["L", "T"]), exponent]);
        Compare("Pwr rc exponent uses constant factor", "Pwr", 0x0600061f, ["MatDim", "Int32"],
            [RowColumn(Side.Quire, ["L", "T"], ["L", "L"]), 3],
            [RowColumn(Side.Mathcad, ["L", "T"], ["L", "L"]), 3]);
        Compare("Pwr unsupported crazy dimensions", "Pwr", 0x0600061f, ["MatDim", "Int32"],
            [Crazy(Side.Quire), -1], [Crazy(Side.Mathcad), -1]);
        Compare("Pwr propagates null receiver", "Pwr", 0x0600061f, ["MatDim", "Int32"],
            [null, 0], [null, 0]);
    }
}

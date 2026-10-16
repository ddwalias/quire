using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimBaseGettersDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private static readonly (string Name, int Token)[] Getters =
    [
        ("get_UniformDim", 0x0600060e),
        ("get_RowDimensions", 0x06000611),
        ("get_ColDimensions", 0x06000612),
        ("get_IsLinear", 0x06000619),
    ];

    private sealed record Snapshot(object Target, object Matrix, object? MatrixDimension, object? Real,
        object? Imaginary, object? Table, IReadOnlyDictionary<int, object?> Entries,
        IReadOnlyDictionary<int, object?> PointerDimensions, IReadOnlyDictionary<int, (double? Real, double? Imag)> Numbers);

    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatDim").GetMethod(name, DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{name}()");

    private static object Matrix(Side side, int rows = 2, int cols = 2, int stride = 4, int offset = 3)
    {
        var real = new double[offset + stride * cols + 5];
        var imag = new double[real.Length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
        {
            real[offset + stride * col + row] = 10 + 10 * col + row;
            imag[offset + stride * col + row] = -10 - 10 * col - row;
        }
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static void AddPointer(Side side, object matrix, int row, int col, object value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static object Crazy(Side side, object matrix)
    {
        var type = Nested(side.T("Mcd.MatDim"), "crazy_dim");
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(candidate => candidate.Name == "make" && candidate.GetParameters() is [{ ParameterType: var parameter }] && parameter == matrix.GetType());
        return method.Invoke(null, [matrix])!;
    }

    private static Snapshot Capture(object target)
    {
        object matrix = Get(target, "m_matrix")!;
        object? table = Get(matrix, "m_parr");
        var entries = table is Hashtable pointers
            ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
            : new Dictionary<int, object?>();
        var pointerDimensions = entries.Where(pair => pair.Value?.GetType().Name == "NumVal")
            .ToDictionary(pair => pair.Key, pair => Get(pair.Value!, "m_dim"));
        var numbers = entries.Where(pair => pair.Value?.GetType().Name == "NumVal")
            .ToDictionary(pair => pair.Key, pair =>
            {
                object? data = Get(pair.Value!, "Data");
                return data == null ? ((double?, double?))(null, null) : ((double)Get(data, "rp")!, (double)Get(data, "ip")!);
            });
        return new(target, matrix, Get(matrix, "m_dim"), Get(matrix, "m_rarr"), Get(matrix, "m_iarr"), table,
            entries, pointerDimensions, numbers);
    }

    private static string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";
    private static string ArrayContents(object? value, Side side) => value is double[] array
        ? "[" + string.Join(",", array.Select(item => side.Show(item))) + "]"
        : value == null ? "null" : side.Show(value);

    private static string PointerValue(Side side, Snapshot before, int key, object? value)
    {
        if (value == null) return "null";
        string identity = before.Entries.TryGetValue(key, out var old) && ReferenceEquals(old, value) ? "retained" : "object";
        if (value.GetType().Name != "NumVal") return $"{value.GetType().Name}:{identity}";
        object? dimension = Get(value, "m_dim");
        bool sameDimension = before.PointerDimensions.TryGetValue(key, out var oldDimension) && ReferenceEquals(oldDimension, dimension);
        object? data = Get(value, "Data");
        string numbers = data == null ? "no-data" : $"real={Get(data, "rp")},imag={Get(data, "ip")}";
        bool sameValues = data != null && before.Numbers.TryGetValue(key, out var oldValues) &&
            oldValues.Real == (double)Get(data, "rp")! && oldValues.Imag == (double)Get(data, "ip")!;
        return $"NumVal:{identity}:{numbers}:dim={side.DimText(dimension)}:dim-ref={(sameDimension ? "same" : "changed")}:values={(sameValues ? "same" : "changed")}";
    }

    private static string State(Side side, Snapshot before, object target)
    {
        object matrix = before.Matrix;
        var table = Get(matrix, "m_parr") as Hashtable;
        string Entries(IEnumerable<DictionaryEntry> entries) => string.Join(";", entries.OrderBy(entry => (int)entry.Key)
            .Select(entry => $"{entry.Key}={PointerValue(side, before, (int)entry.Key, entry.Value)}"));
        object data = Get(matrix, "Data")!;
        string currentTable = table == null ? "null" : Entries(table.Cast<DictionaryEntry>());
        string retainedTable = before.Table is Hashtable oldTable ? Entries(oldTable.Cast<DictionaryEntry>()) : "null";
        return $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}:matrix-retained={ReferenceEquals(Get(target, "m_matrix"), matrix)}" +
            $" geometry={Get(data, "m_rows")}x{Get(data, "m_cols")}:stride={Get(data, "m_anr")}:offset={Get(matrix, "m_offset")}:flags={Get(data, "m_flags")}" +
            $" dim-ref={Ref(before.MatrixDimension, Get(matrix, "m_dim"))}:dim={side.DimText(Get(matrix, "m_dim"))}" +
            $" real-ref={Ref(before.Real, Get(matrix, "m_rarr"))}:real={ArrayContents(Get(matrix, "m_rarr"), side)}:retained-real={ArrayContents(before.Real, side)}" +
            $" imag-ref={Ref(before.Imaginary, Get(matrix, "m_iarr"))}:imag={ArrayContents(Get(matrix, "m_iarr"), side)}:retained-imag={ArrayContents(before.Imaginary, side)}" +
            $" table-ref={Ref(before.Table, table)}:table={currentTable}:retained-table-after={retainedTable}";
    }

    private static string Result(Side side, object? value) => value switch
    {
        null => "null",
        Array array => $"{value.GetType().Name}:[{string.Join(";", array.Cast<object?>().Select(item => side.DimText(item)))}]",
        _ when value.GetType().Name.EndsWith("Dim", StringComparison.Ordinal) => side.DimText(value),
        _ => Text(value),
    };

    private static void ComparePair(string caseId, string getter, int token, object q, object m)
    {
        var qBefore = Capture(q); var mBefore = Capture(m);
        var qMethod = Method(Side.Quire, getter); var mMethod = Method(Side.Mathcad, getter);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, q, [], mMethod, m, [],
            result => Result(Side.Quire, result), result => Result(Side.Mathcad, result),
            target => State(Side.Quire, qBefore, target), target => State(Side.Mathcad, mBefore, target));
    }

    [SkippableFact]
    public void Base_getters_on_crazy_dim_return_null_without_reading_or_mutating_matrix_state()
    {
        Runtime.Sync();
        var qLive = Matrix(Side.Quire); var mLive = Matrix(Side.Mathcad);
        AddPointer(Side.Quire, qLive, 0, 1, Side.Quire.Num(7, 2, "T"));
        AddPointer(Side.Mathcad, mLive, 0, 1, Side.Mathcad.Num(7, 2, "T"));
        AddPointer(Side.Quire, qLive, 1, 0, "stale"); AddPointer(Side.Mathcad, mLive, 1, 0, "stale");
        ((double[])Get(qLive, "m_rarr")!)[(int)Call(qLive, "Index", 1, 0)!] = 9;
        ((double[])Get(mLive, "m_rarr")!)[(int)Call(mLive, "Index", 1, 0)!] = 9;
        var qCrazy = Crazy(Side.Quire, qLive); var mCrazy = Crazy(Side.Mathcad, mLive);

        foreach (var (getter, token) in Getters)
            ComparePair($"{getter} base declaration on padded crazy_dim with live and stale pointers", getter, token, qCrazy, mCrazy);

        var qEmpty = Crazy(Side.Quire, Matrix(Side.Quire, 0, 0, 0, 0));
        var mEmpty = Crazy(Side.Mathcad, Matrix(Side.Mathcad, 0, 0, 0, 0));
        foreach (var (getter, token) in Getters)
            ComparePair($"{getter} base declaration on empty crazy_dim", getter, token, qEmpty, mEmpty);
    }

}

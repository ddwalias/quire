using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValMatrixDimensionDifferentialTests
{
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private static readonly System.Type QMatVal = typeof(global::Quire.Mcd.MatVal);
    private static readonly System.Type QMatDim = typeof(global::Quire.Mcd.MatDim);

    private sealed record Snapshot(object Matrix, object? Dimension, object? Table, object? Real, object? Imag,
        IReadOnlyDictionary<int, object?> Pointers, IReadOnlyDictionary<int, object?> TableEntries);

    private static System.Type MapType(Side side, System.Type type) => side == Side.Quire ? type
        : type == QMatVal ? side.T("Mcd.MatVal")
        : type == QMatDim ? side.T("Mcd.MatDim")
        : type;

    private static object? OptionalField(object target, string name)
    {
        for (var type = target.GetType(); type != null; type = type.BaseType)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null) return field.GetValue(target);
        }
        return null;
    }

    private static MethodInfo Method(Side side, string owner, string name, bool isStatic, params System.Type[] parameters)
    {
        var flags = Declared | (isStatic ? BindingFlags.Static : BindingFlags.Instance);
        return side.T(owner).GetMethod(name, flags, null, parameters.Select(parameter => MapType(side, parameter)).ToArray(), null)
            ?? throw new HarnessException($"Missing {side.Name} {owner}.{name}({string.Join(", ", parameters.Select(parameter => parameter.Name))})");
    }

    private static object Matrix(Side side, int rows = 2, int cols = 2, string dimension = "identity")
    {
        int stride = rows + 1;
        int offset = 1;
        var real = new double[offset + stride * cols];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            real[offset + row + stride * col] = 10 * col + row + 1;
        var matrix = New(side.T("Mcd.MatVal"), real, null, rows, cols, stride, null,
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, string dimension, double value = 1) =>
        Pointer(side, matrix, row, col, side.D(dimension), value);

    private static void Pointer(Side side, object matrix, int row, int col, object? dimension, double value)
    {
        object number = New(side.T("Mcd.NumVal"), value, 0.0, dimension);
        Call(matrix, "set_ptrs_2", row, col, number);
    }

    private static void Fill(Side side, object matrix, string[,] dimensions)
    {
        for (int row = 0; row < dimensions.GetLength(0); row++)
        for (int col = 0; col < dimensions.GetLength(1); col++)
            Pointer(side, matrix, row, col, dimensions[row, col], row * 10 + col + 1);
    }

    private static void StalePointer(Side side, object matrix, int row = 0, int col = 0)
    {
        int index = (int)Call(matrix, "Index", row, col)!;
        if (Get(matrix, "m_parr") is not Hashtable table)
        {
            table = new Hashtable();
            Set(matrix, "m_parr", table);
        }
        table[index] = side.Num(9, 0, "T");
        ((double[])Get(matrix, "m_rarr")!)[index] = 9;
    }

    private static Snapshot Capture(object matrix)
    {
        var table = Get(matrix, "m_parr") as Hashtable;
        var entries = table?.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
            ?? new Dictionary<int, object?>();
        return new(matrix, Get(matrix, "m_dim"), table, Get(matrix, "m_rarr"), Get(matrix, "m_iarr"), entries, entries);
    }

    private static string State(Side side, Snapshot before, object matrix)
    {
        object? tableAfter = Get(matrix, "m_parr");
        string Ref(object? first, object? second) => ReferenceEquals(first, second) ? "same" : "changed";
        string pointerRefs = tableAfter is Hashtable table
            ? string.Join(";", before.Pointers.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{(table.ContainsKey(pair.Key) ? Ref(pair.Value, table[pair.Key]) : "absent")}"))
            : string.Join(";", before.Pointers.OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}:absent"));
        string Entries(IEnumerable<KeyValuePair<int, object?>> entries, IReadOnlyDictionary<int, object?> reference) =>
            string.Join(";", entries.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{pair.Value?.GetType().Name ?? "null"}:{Text(pair.Value)}:" +
                (reference.TryGetValue(pair.Key, out var oldValue) && ReferenceEquals(oldValue, pair.Value) ? "same-before" : "changed")));
        string afterEntries = tableAfter is Hashtable afterTable
            ? Entries(afterTable.Cast<DictionaryEntry>().Select(entry => new KeyValuePair<int, object?>((int)entry.Key, entry.Value)), before.TableEntries)
            : "null-table";
        string aliasKeys = tableAfter is Hashtable current
            ? string.Join(",", current.Cast<DictionaryEntry>().Where(entry => ReferenceEquals(entry.Value, matrix)).Select(entry => entry.Key))
            : "";
        return $"{MatValDifferentialTests.Storage(side, matrix)} dim-ref {Ref(before.Dimension, Get(matrix, "m_dim"))}" +
            $" table-ref {Ref(before.Table, tableAfter)} real-ref {Ref(before.Real, Get(matrix, "m_rarr"))}" +
            $" imag-ref {Ref(before.Imag, Get(matrix, "m_iarr"))}" +
            $" table-before [{Entries(before.TableEntries, before.TableEntries)}] table-after [{afterEntries}]" +
            $" pointer-refs [{pointerRefs}] self-pointer-keys [{aliasKeys}]";
    }

    private static string ShowDimension(Side side, object? value, object source)
    {
        if (value == null) return "null";
        var type = value.GetType();
        int rows = (int)Get(value, "Rows")!; int cols = (int)Get(value, "Cols")!;
        var uniform = Get(value, "UniformDim");
        var linear = Get(value, "IsLinear");
        string linearText = linear == null ? "null" :
            $"{linear.GetType().Name} {Get(linear, "Rows")}x{Get(linear, "Cols")} same-result {ReferenceEquals(linear, value)}";
        object? rowStorage = OptionalField(value, "m_rowdim"); object? colStorage = OptionalField(value, "m_coldim");
        var rowDimensions = Get(value, "RowDimensions"); var colDimensions = Get(value, "ColDimensions");
        string ArrayText(object? array) => array is Array values
            ? $"{array.GetType().Name}[{string.Join(",", values.Cast<object?>().Select(side.DimText))}]"
            : "null";
        string arrayRefs = $"row-storage/property {ReferenceEquals(rowStorage, rowDimensions)}" +
            $" col-storage/property {ReferenceEquals(colStorage, colDimensions)}" +
            $" row-col-storage {ReferenceEquals(rowStorage, colStorage)}";
        string sourceDimensionRef = ReferenceEquals(uniform, Get(source, "m_dim")).ToString();
        string rowArray = ArrayText(rowDimensions); string colArray = ArrayText(colDimensions);
        var cells = new List<string>();
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
            cells.Add($"({row},{col})={side.DimText(Call(value, "dim", row, col))}");
        bool isCrazy = type.Name == "crazy_dim";
        bool sourceAlias = isCrazy && ReferenceEquals(Get(value, "m_matrix"), source);
        return $"{type.Name} {rows}x{cols} uniform {side.DimText(uniform)} uniform-source-dim-same {sourceDimensionRef}" +
            $" row-dimensions [{rowArray}] col-dimensions [{colArray}] array-refs [{arrayRefs}] linear [{linearText}]" +
            $" cells [{string.Join(" ", cells)}] crazy-source-same {sourceAlias}";
    }

    private static string Show(Side side, object? value, object source, Snapshot before)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
            return "MatVal " + State(side, before, value) + $" source-object-same {ReferenceEquals(source, value)}";
        if (value.GetType().FullName?.Contains("Mcd.MatDim", StringComparison.Ordinal) == true)
            return ShowDimension(side, value, source);
        return Text(value);
    }

    private static void Compare(string caseId, string owner, string methodName, int originalToken,
        bool isStatic, System.Type[] parameters, object qSource, object mSource,
        object?[] qArgs, object?[] mArgs)
    {
        var qBefore = Capture(qSource); var mBefore = Capture(mSource);
        var qMethod = Method(Side.Quire, owner, methodName, isStatic, parameters);
        var mMethod = Method(Side.Mathcad, owner, methodName, isStatic, parameters);
        Assert.Equal(originalToken, mMethod.MetadataToken);
        string contract = owner == "Mcd.MatVal" ? "Mcd.MatVal" : "Mcd.MatDim";
        Func<Side, object?, object, Snapshot, string> show = Show;
        MethodComparison.Compare(caseId, contract, qMethod, isStatic ? null : qSource, qArgs,
            mMethod, isStatic ? null : mSource, mArgs,
            value => show(Side.Quire, value, qSource, qBefore),
            value => show(Side.Mathcad, value, mSource, mBefore),
            isStatic ? null : target => State(Side.Quire, qBefore, target),
            isStatic ? null : target => State(Side.Mathcad, mBefore, target));
    }

    private static void CompareGetter(string caseId, object qMatrix, object mMatrix) =>
        Compare(caseId, "Mcd.MatVal", "get_MatrixDimension", 0x06000454, false, [], qMatrix, mMatrix, [], []);

    private static void CompareMake(string caseId, string methodName, int token,
        object qMatrix, object mMatrix, bool? linearOnly = null)
    {
        System.Type[] signature = linearOnly.HasValue ? [QMatVal, typeof(bool)] : [QMatVal];
        object?[] qArgs = linearOnly.HasValue ? [qMatrix, linearOnly.Value] : [qMatrix];
        object?[] mArgs = linearOnly.HasValue ? [mMatrix, linearOnly.Value] : [mMatrix];
        Compare(caseId, "Mcd.MatDim", methodName, token, true, signature, qMatrix, mMatrix, qArgs, mArgs);
    }

    private static (object Q, object M) UniformPair(int rows = 2, int cols = 2)
    {
        var q = Matrix(Side.Quire, rows, cols, "L"); var m = Matrix(Side.Mathcad, rows, cols, "L");
        return (q, m);
    }

    private static (object Q, object M) VectorPair(bool column)
    {
        int rows = column ? 3 : 1, cols = column ? 1 : 3;
        var q = Matrix(Side.Quire, rows, cols); var m = Matrix(Side.Mathcad, rows, cols);
        var qDims = new string[rows, cols]; var mDims = new string[rows, cols];
        for (int row = 0; row < rows; row++) for (int col = 0; col < cols; col++)
            qDims[row, col] = mDims[row, col] = row + col == 0 ? "L" : row + col == 1 ? "T" : "v";
        Fill(Side.Quire, q, qDims); Fill(Side.Mathcad, m, mDims);
        return (q, m);
    }

    private static (object Q, object M) ArrayPair(string kind)
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        string[,] dims = kind switch
        {
            "columns" => new[,] { { "L", "T" }, { "L", "T" } },
            "rows" => new[,] { { "L", "L" }, { "T", "T" } },
            "column-fallback" => new[,] { { "L", "L" }, { "L", "T" } },
            "row-fallback" => new[,] { { "L", "L" }, { "T", "v" } },
            "factors" => new[,] { { "L", "L" }, { "T", "L" } },
            "crazy" => new[,] { { "L", "v" }, { "T", "N" } },
            "null-rows" => new[,] { { "L", "T" }, { "L", "L" } },
            "null-cols" => new[,] { { "L", "L" }, { "T", "L" } },
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
        if (kind is "null-rows" or "null-cols")
        {
            if (kind == "null-rows")
            {
                Pointer(Side.Quire, q, 0, 1, "T"); Pointer(Side.Mathcad, m, 0, 1, "T");
                Pointer(Side.Quire, q, 1, 0, "mixed_matrix_dim"); Pointer(Side.Mathcad, m, 1, 0, "mixed_matrix_dim");
            }
            else
            {
                Pointer(Side.Quire, q, 1, 0, "T"); Pointer(Side.Mathcad, m, 1, 0, "T");
                Pointer(Side.Quire, q, 0, 1, "mixed_matrix_dim"); Pointer(Side.Mathcad, m, 0, 1, "mixed_matrix_dim");
            }
            return (q, m);
        }
        if (kind == "factors")
        {
            Pointer(Side.Quire, q, 0, 0, "L"); Pointer(Side.Mathcad, m, 0, 0, "L");
            Pointer(Side.Quire, q, 1, 0, "T"); Pointer(Side.Mathcad, m, 1, 0, "T");
            var qFactor = Call(Side.Quire.Dim, "Mult", Side.Quire.D("L"), Side.Quire.D("v"));
            var mFactor = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("L"), Side.Mathcad.D("v"));
            var qOther = Call(Side.Quire.Dim, "Mult", Side.Quire.D("T"), Side.Quire.D("v"));
            var mOther = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("T"), Side.Mathcad.D("v"));
            Pointer(Side.Quire, q, 0, 1, qFactor, 2); Pointer(Side.Mathcad, m, 0, 1, mFactor, 2);
            Pointer(Side.Quire, q, 1, 1, qOther, 3); Pointer(Side.Mathcad, m, 1, 1, mOther, 3);
            return (q, m);
        }
        Fill(Side.Quire, q, dims); Fill(Side.Mathcad, m, dims);
        return (q, m);
    }

    private static (object Q, object M) OneByOnePair()
    {
        var q = Matrix(Side.Quire, 1, 1); var m = Matrix(Side.Mathcad, 1, 1);
        Pointer(Side.Quire, q, 0, 0, "T"); Pointer(Side.Mathcad, m, 0, 0, "T");
        return (q, m);
    }

    [SkippableFact]
    public void Direct_MatVal_MatrixDimension_getter_cases()
    {
        Runtime.Sync();
        var (q, m) = UniformPair(); CompareGetter("get_MatrixDimension uniform precedence", q, m);
        (q, m) = VectorPair(column: true); CompareGetter("get_MatrixDimension column vector", q, m);
        (q, m) = VectorPair(column: false); CompareGetter("get_MatrixDimension row vector", q, m);
        (q, m) = OneByOnePair(); CompareGetter("get_MatrixDimension 1x1 uniform precedence", q, m);
        foreach (string kind in new[] { "columns", "rows", "column-fallback", "row-fallback", "factors", "crazy", "null-rows", "null-cols" })
        {
            (q, m) = ArrayPair(kind);
            CompareGetter("get_MatrixDimension array " + kind + " with padded stride and offset", q, m);
        }
        (q, m) = UniformPair(); StalePointer(Side.Quire, q); StalePointer(Side.Mathcad, m);
        CompareGetter("get_MatrixDimension clears stale-only table", q, m);
    }

    [SkippableFact]
    public void Direct_MatDim_make_MatVal_overloads()
    {
        Runtime.Sync();
        foreach (int token in new[] { 0x06000621, 0x06000620 })
        foreach (bool? linear in token == 0x06000621 ? new bool?[] { null } : new bool?[] { false, true })
        {
            string method = linear.HasValue ? "make" : "make";
            (object q, object m) pair = UniformPair();
            CompareMake($"{method} uniform precedence linear {linear}", method, token, pair.q, pair.m, linear);
            pair = VectorPair(column: true);
            CompareMake($"{method} column vector linear {linear}", method, token, pair.q, pair.m, linear);
            pair = VectorPair(column: false);
            CompareMake($"{method} row vector linear {linear}", method, token, pair.q, pair.m, linear);
            pair = OneByOnePair();
            CompareMake($"{method} 1x1 precedence linear {linear}", method, token, pair.q, pair.m, linear);
            foreach (string kind in new[] { "columns", "rows", "column-fallback", "row-fallback", "factors", "crazy", "null-rows", "null-cols" })
            {
                pair = ArrayPair(kind);
                CompareMake($"{method} {kind} factorization linear {linear}", method, token, pair.q, pair.m, linear);
            }
        }
    }

    [SkippableFact]
    public void Direct_MatDim_makeArrayDim_overloads()
    {
        Runtime.Sync();
        foreach (string kind in new[] { "columns", "rows", "column-fallback", "row-fallback", "factors", "crazy", "null-rows", "null-cols" })
        {
            var (q, m) = ArrayPair(kind);
            Compare($"makeArrayDim {kind}", "Mcd.MatDim", "makeArrayDim", 0x0600064b, true,
                [QMatVal], q, m, [q], [m]);
            Compare($"makeArrayDim {kind} linear only false", "Mcd.MatDim", "makeArrayDim", 0x0600064a, true,
                [QMatVal, typeof(bool)], q, m, [q, false], [m, false]);
            Compare($"makeArrayDim {kind} linear only true", "Mcd.MatDim", "makeArrayDim", 0x0600064a, true,
                [QMatVal, typeof(bool)], q, m, [q, true], [m, true]);
        }
        var (qStale, mStale) = UniformPair(); StalePointer(Side.Quire, qStale); StalePointer(Side.Mathcad, mStale);
        Compare("makeArrayDim stale-only table", "Mcd.MatDim", "makeArrayDim", 0x0600064b, true,
            [QMatVal], qStale, mStale, [qStale], [mStale]);
    }

    [SkippableFact]
    public void Direct_MatDim_vector_dimension_helpers()
    {
        Runtime.Sync();
        var (qCol, mCol) = VectorPair(column: true);
        Compare("makeColumnVectorDim column dimensions", "Mcd.MatDim", "makeColumnVectorDim", 0x0600064c, true,
            [QMatVal], qCol, mCol, [qCol], [mCol]);
        (qCol, mCol) = (Matrix(Side.Quire, 2, 1), Matrix(Side.Mathcad, 2, 1));
        Pointer(Side.Quire, qCol, 0, 0, "mixed_matrix_dim"); Pointer(Side.Mathcad, mCol, 0, 0, "mixed_matrix_dim");
        Compare("makeColumnVectorDim crazy fallback preserves source", "Mcd.MatDim", "makeColumnVectorDim", 0x0600064c, true,
            [QMatVal], qCol, mCol, [qCol], [mCol]);

        var (qRow, mRow) = VectorPair(column: false);
        Compare("makeRowVectorDim row dimensions", "Mcd.MatDim", "makeRowVectorDim", 0x0600064d, true,
            [QMatVal], qRow, mRow, [qRow], [mRow]);
        (qRow, mRow) = (Matrix(Side.Quire, 1, 2), Matrix(Side.Mathcad, 1, 2));
        Pointer(Side.Quire, qRow, 0, 0, "mixed_matrix_dim"); Pointer(Side.Mathcad, mRow, 0, 0, "mixed_matrix_dim");
        Compare("makeRowVectorDim crazy fallback preserves source", "Mcd.MatDim", "makeRowVectorDim", 0x0600064d, true,
            [QMatVal], qRow, mRow, [qRow], [mRow]);
    }
}

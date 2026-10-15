using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatValContainsDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const int ContainsToken = 0x0600051a;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side) => side.T("Mcd.MatVal").GetMethod("contains", DeclaredInstance,
        null, [typeof(object)], null) ?? throw new HarnessException($"Missing {side.Name} MatVal.contains(object)");

    private static object Matrix(Side side)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[] { 10, 11, 12, 13 }, new double[] { 20, 21, 22, 23 },
            2, 2, 2, new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static string State(Side side, object target)
    {
        var identities = new Dictionary<object, int>(ReferenceEqualityComparer.Instance);
        int Id(object? value)
        {
            if (value == null) return 0;
            if (!identities.TryGetValue(value, out int id)) identities.Add(value, id = identities.Count + 1);
            return id;
        }
        string ArrayText(object? value) => value is double[] values
            ? "[" + string.Join(",", values.Select(Side.Bits)) + "]"
            : "null";
        string KeyText(object? key) => key == null ? "null" : side.Show(key);
        string Value(object? value)
        {
            if (value == null) return "null";
            if (value.GetType().Name == "MatVal") return Node(value);
            int id = Id(value);
            if (value.GetType().Name == "NumVal") return $"NumVal#{id}({side.Show(value)})";
            if (value.GetType().Name == "StrVal") return $"StrVal#{id}({side.Show(value)})";
            return $"{value.GetType().Name}#{id}({side.Show(value)})";
        }
        string Node(object matrix)
        {
            int id = Id(matrix);
            if (matrix.GetType().Name != "MatVal") return $"ref#{id}";
            object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr"), dim = Get(matrix, "m_dim");
            var data = Get(matrix, "Data");
            string pointers = table is Hashtable hash
                ? string.Join(";", hash.Cast<DictionaryEntry>().OrderBy(entry => KeyText(entry.Key), StringComparer.Ordinal)
                    .Select(entry => $"{KeyText(entry.Key)}={Value(entry.Value)}"))
                : "null";
            return $"MatVal#{id}(shape={Get(matrix, "Rows")}x{Get(matrix, "Cols")}/{Get(matrix, "Anr")}/{Get(matrix, "Offset")}," +
                $"dim#{Id(dim)}:{side.DimText(dim)},flags={Convert.ToInt64(Get(data!, "m_flags"))},immutable={Get(matrix, "Immutable")}," +
                $"real#{Id(real)}={ArrayText(real)},imag#{Id(imag)}={ArrayText(imag)},table#{Id(table)}=[{pointers}])";
        }
        return Node(target);
    }

    private sealed record MatValSnapshot(object Matrix, object? Real, object? Imag, object? Table, object? Dimension,
        int Rows, int Cols, int Stride, int Offset, object? Flags, object? Immutable,
        double[]? RealValues, double[]? ImagValues, IReadOnlyList<DictionaryEntry> Entries);

    private static IReadOnlyList<MatValSnapshot> Snapshot(object root)
    {
        var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var result = new List<MatValSnapshot>();
        void Visit(object? value)
        {
            if (value?.GetType().Name != "MatVal" || !seen.Add(value)) return;
            object? table = Get(value, "m_parr"), real = Get(value, "m_rarr"), imag = Get(value, "m_iarr");
            var entries = table is Hashtable hash ? hash.Cast<DictionaryEntry>().ToArray() : Array.Empty<DictionaryEntry>();
            result.Add(new MatValSnapshot(value, real, imag, table, Get(value, "m_dim"),
                (int)Get(value, "Rows")!, (int)Get(value, "Cols")!, (int)Get(value, "Anr")!, (int)Get(value, "Offset")!,
                Get(Get(value, "Data")!, "m_flags"), Get(value, "Immutable"),
                real is double[] ra ? ra.ToArray() : null, imag is double[] ia ? ia.ToArray() : null, entries));
            foreach (var entry in entries) Visit(entry.Value);
        }
        Visit(root);
        return result;
    }

    private static void AssertUnchanged(IReadOnlyList<MatValSnapshot> snapshots)
    {
        foreach (var before in snapshots)
        {
            object value = before.Matrix;
            Assert.Same(before.Real, Get(value, "m_rarr"));
            Assert.Same(before.Imag, Get(value, "m_iarr"));
            Assert.Same(before.Table, Get(value, "m_parr"));
            Assert.Same(before.Dimension, Get(value, "m_dim"));
            Assert.Equal(before.Rows, Get(value, "Rows"));
            Assert.Equal(before.Cols, Get(value, "Cols"));
            Assert.Equal(before.Stride, Get(value, "Anr"));
            Assert.Equal(before.Offset, Get(value, "Offset"));
            Assert.Equal(before.Flags, Get(Get(value, "Data")!, "m_flags"));
            Assert.Equal(before.Immutable, Get(value, "Immutable"));
            if (before.RealValues != null)
                Assert.Equal(before.RealValues.Select(Side.Bits), ((double[])Get(value, "m_rarr")!).Select(Side.Bits));
            if (before.ImagValues != null)
                Assert.Equal(before.ImagValues.Select(Side.Bits), ((double[])Get(value, "m_iarr")!).Select(Side.Bits));
            if (before.Table is Hashtable oldTable)
            {
                var current = (Hashtable)Get(value, "m_parr")!;
                Assert.Equal(oldTable.Count, current.Count);
                foreach (DictionaryEntry entry in before.Entries)
                {
                    Assert.True(current.ContainsKey(entry.Key));
                    Assert.Same(entry.Value, current[entry.Key]);
                }
            }
        }
    }

    private static void Compare(string caseId, Action<Side, object> setup,
        Func<Side, object, object?> query)
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        object? qQuery = query(Side.Quire, q), mQuery = query(Side.Mathcad, m);
        var qSnapshot = Snapshot(q); var mSnapshot = Snapshot(m);
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(ContainsToken, mMethod.MetadataToken);
        string qBefore = State(Side.Quire, q), mBefore = State(Side.Mathcad, m);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, q, [qQuery], mMethod, m, [mQuery],
            quireShow: value => Side.Quire.Show(value), originalShow: value => Side.Mathcad.Show(value),
            quireState: target => State(Side.Quire, target), originalState: target => State(Side.Mathcad, target));
        string qAfter = State(Side.Quire, q), mAfter = State(Side.Mathcad, m);
        Assert.Equal(qBefore, qAfter);
        Assert.Equal(mBefore, mAfter);
        Assert.Equal(mAfter, qAfter);
        AssertUnchanged(qSnapshot);
        AssertUnchanged(mSnapshot);
    }

    [SkippableFact]
    public void Contains_uses_reference_identity_and_recurses_through_nested_pointer_values_without_mutation()
    {
        Runtime.Sync();

        Compare("contains null table and null query", (_, matrix) => Set(matrix, "m_parr", null), (_, _) => null);
        Compare("contains empty table and non-null query", (_, _) => { }, (side, _) => side.Num(1, 0, "L"));
        Compare("contains null stored pointer with null query", (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["null-value"] = null, (_, _) => null);

        Compare("contains same-reference NumVal", (side, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["num"] = side.Num(3, 4, "L"),
            (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["num"]!);
        Compare("contains rejects equal but distinct NumVal", (side, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["num"] = side.Num(3, 4, "L"),
            (side, _) => side.Num(3, 4, "L"));
        Compare("contains same-reference StrVal", (side, matrix) => ((Hashtable)Get(matrix, "m_parr")!)[17] = side.Str("same"),
            (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)[17]!);
        Compare("contains rejects equal but distinct StrVal", (side, matrix) => ((Hashtable)Get(matrix, "m_parr")!)[17] = side.Str("same"),
            (side, _) => side.Str("same"));
        Compare("contains same-reference arbitrary object", (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["object-key"] = new object(),
            (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["object-key"]!);

        Compare("contains rejects equal but distinct strings with arbitrary key", (_, matrix) =>
        {
            ((Hashtable)Get(matrix, "m_parr")!)["string-key"] = new string("equal".ToCharArray());
        }, (_, _) => new string("equal".ToCharArray()));

        Compare("contains finds stale table value independently of PtrNan", (side, matrix) =>
        {
            ((Hashtable)Get(matrix, "m_parr")!)["stale-key"] = side.Num(8, 1, "T");
        }, (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["stale-key"]!);
        Compare("contains does not infer a match from an unrelated PtrNan slot", (side, matrix) =>
        {
            Call(matrix, "set_ptrs_at", 1, side.Num(9, 0, "L"));
        }, (side, _) => side.Str("absent"));

        Compare("contains matches a nested matrix at one level", (side, matrix) =>
        {
            var child = Matrix(side);
            ((Hashtable)Get(matrix, "m_parr")!)["child"] = child;
        }, (_, matrix) => ((Hashtable)Get(matrix, "m_parr")!)["child"]!);
        Compare("contains finds a value two nested matrices deep", (side, matrix) =>
        {
            var child = Matrix(side); var grandchild = Matrix(side); var needle = side.Str("deep");
            ((Hashtable)Get(matrix, "m_parr")!)["child"] = child;
            ((Hashtable)Get(child, "m_parr")!)[31] = grandchild;
            ((Hashtable)Get(grandchild, "m_parr")!)["needle"] = needle;
        }, (_, matrix) =>
        {
            var child = ((Hashtable)Get(matrix, "m_parr")!)["child"]!;
            var grandchild = ((Hashtable)Get(child, "m_parr")!)[31]!;
            return ((Hashtable)Get(grandchild, "m_parr")!)["needle"]!;
        });
        Compare("contains rejects absent value through nested matrices", (side, matrix) =>
        {
            var child = Matrix(side); var grandchild = Matrix(side);
            ((Hashtable)Get(matrix, "m_parr")!)["child"] = child;
            ((Hashtable)Get(child, "m_parr")!)[31] = grandchild;
            ((Hashtable)Get(grandchild, "m_parr")!)["other"] = side.Str("present");
        }, (side, _) => side.Str("absent"));
        Compare("contains recurses into stale nested entries", (side, matrix) =>
        {
            var child = Matrix(side); var needle = side.Num(11, 2, "N");
            ((Hashtable)Get(matrix, "m_parr")!)["child"] = child;
            ((Hashtable)Get(child, "m_parr")!)["stale-arbitrary-key"] = needle;
        }, (_, matrix) =>
        {
            var child = ((Hashtable)Get(matrix, "m_parr")!)["child"]!;
            return ((Hashtable)Get(child, "m_parr")!)["stale-arbitrary-key"]!;
        });
    }
}

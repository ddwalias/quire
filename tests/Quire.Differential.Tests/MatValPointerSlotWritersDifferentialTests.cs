using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValPointerSlotWritersDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly int[] Tokens = [0x06000517, 0x06000518, 0x06000519, 0x0600051b, 0x06000521];

    private static MethodInfo Method(Side side, string name, params System.Type[] parameters)
    {
        System.Type Map(System.Type type) => type;
        return side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, parameters.Select(Map).ToArray(), null)
            ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}");
    }

    private static object Matrix(Side side, bool shared = false, int rows = 2, int cols = 2, int stride = 3, int offset = 1)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        var imag = new double[length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
        {
            int index = offset + row + stride * col;
            real[index] = 10 + row + 3 * col;
            imag[index] = 20 + row + 3 * col;
        }
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        Set(matrix, "Shared", shared);
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static void StalePointer(Side side, object matrix, int row, int col, object? value = null)
    {
        int index = (int)Call(matrix, "Index", row, col)!;
        ((Hashtable)Get(matrix, "m_parr")!)[index] = value ?? "stale";
        ((double[])Get(matrix, "m_rarr")!)[index] = 77;
    }

    private sealed record PtrState(object? Value, object? Dim, double? Re, double? Im);

    private sealed record MatValEntry(int Index, object? Value, object? Dim, double? Re, double? Im);

    private sealed record MatValState(object Value, object? Dim, object? Real, object? Imag, object? Table,
        double[]? RealValues, double[]? ImagValues, int Rows, int Cols, int Stride, int Offset,
        object? Flags, object? Immutable, IReadOnlyList<MatValEntry> Entries);

    private static MatValState CaptureMatVal(object value)
    {
        object? real = Get(value, "m_rarr"), imag = Get(value, "m_iarr"), table = Get(value, "m_parr");
        var entries = table is Hashtable hash
            ? hash.Cast<DictionaryEntry>().OrderBy(e => (int)e.Key).Select(e =>
            {
                object? item = e.Value;
                if (item?.GetType().Name != "NumVal") return new MatValEntry((int)e.Key, item, null, null, null);
                object? data = Get(item, "Data");
                return new MatValEntry((int)e.Key, item, Get(item, "m_dim"),
                    data == null ? null : (double)Get(data, "rp")!, data == null ? null : (double)Get(data, "ip")!);
            }).ToArray()
            : Array.Empty<MatValEntry>();
        return new MatValState(value, Get(value, "m_dim"), real, imag, table,
            real is double[] ra ? ra.ToArray() : null, imag is double[] ia ? ia.ToArray() : null,
            (int)Get(value, "Rows")!, (int)Get(value, "Cols")!, (int)Get(value, "Anr")!,
            (int)Get(value, "Offset")!, Get(Get(value, "Data")!, "m_flags"), Get(value, "Immutable"), entries);
    }

    private static IReadOnlyList<MatValState> CaptureMatVals(object target, object?[] args)
    {
        var values = args.Append(target).Where(v => v?.GetType().Name == "MatVal").Cast<object>()
            .Distinct(ReferenceEqualityComparer.Instance);
        return values.Select(CaptureMatVal).ToArray();
    }

    private static IReadOnlyDictionary<int, PtrState> Snapshot(object? table) => table is Hashtable hash
        ? hash.Cast<DictionaryEntry>().ToDictionary(e => (int)e.Key, e =>
        {
            object? value = e.Value;
            if (value?.GetType().Name != "NumVal") return new PtrState(value, null, null, null);
            object? data = Get(value, "Data");
            return new PtrState(value, Get(value, "m_dim"),
                data == null ? null : (double)Get(data, "rp")!, data == null ? null : (double)Get(data, "ip")!);
        })
        : new Dictionary<int, PtrState>();

    private static string ArrayText(Side side, object? value) => value is double[] array
        ? $"[{string.Join(",", array.Select(x => side.Show(x)))}]"
        : value == null ? "null" : side.Show(value);

    private static string PtrText(Side side, int index, object? value, PtrState? before,
        IReadOnlyList<MatValState> matValBefore, MatValState? cloneSource = null)
    {
        if (value == null) return $"{index}:null";
        string identity = before != null && ReferenceEquals(value, before.Value) ? "same-object" : "new-object";
        if (value.GetType().Name == "NumVal")
        {
            object? data = Get(value, "Data");
            object? dim = Get(value, "m_dim");
            string dimRef = before == null ? "new-dim" : ReferenceEquals(before.Dim, dim) ? "same-dim-ref" : "changed-dim-ref";
            string nums = data == null ? "no-data" : $"re={Get(data, "rp")},im={Get(data, "ip")}";
            string numRef = before == null ? "new-values" :
                before.Re == (double)Get(data!, "rp")! && before.Im == (double)Get(data!, "ip")! ? "same-values" : "changed-values";
            return $"{index}:NumVal:{identity}:{nums}:{side.DimText(dim)}:{dimRef}:{numRef}";
        }
        if (value.GetType().Name == "MatVal")
        {
            MatValState? source = cloneSource ?? matValBefore.FirstOrDefault(s => ReferenceEquals(s.Value, value));
            string matIdentity = before != null && ReferenceEquals(value, before.Value) ? "same-object" :
                cloneSource != null && ReferenceEquals(value, cloneSource.Value) ? "argument-object" : "new-object";
            return $"{index}:MatVal:{matIdentity}:" + MatValText(side, value, source, matValBefore);
        }
        return $"{index}:{value.GetType().Name}:{identity}:{side.Show(value)}";
    }

    private static string MatValText(Side side, object value, MatValState? before,
        IReadOnlyList<MatValState> matValBefore)
    {
        object? dim = Get(value, "m_dim"), real = Get(value, "m_rarr"), imag = Get(value, "m_iarr"), table = Get(value, "m_parr");
        int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!, stride = (int)Get(value, "Anr")!, offset = (int)Get(value, "Offset")!;
        string Ref(object? a, object? b) => ReferenceEquals(a, b) ? "same" : "changed";
        string Logical(object? array) => array is double[] values
            ? string.Join(",", Enumerable.Range(0, cols).SelectMany(c => Enumerable.Range(0, rows)
                .Select(r => side.Show(values[offset + r + stride * c]))))
            : "null";
        string sourceRelation = before == null ? "no-source" :
            $"source-ref={Ref(before.Value, value)} source-dim-ref={Ref(before.Dim, dim)} " +
            $"source-real-ref={Ref(before.Real, real)} source-imag-ref={Ref(before.Imag, imag)} source-table-ref={Ref(before.Table, table)} " +
            $"source-shape={before.Rows}x{before.Cols}/{before.Stride}/{before.Offset} " +
            $"source-flags={side.Show(before.Flags)} source-immutable={side.Show(before.Immutable)} " +
            $"source-real=[{(before.RealValues == null ? "null" : string.Join(",", before.RealValues.Select(x => side.Show(x))))}] " +
            $"source-imag=[{(before.ImagValues == null ? "null" : string.Join(",", before.ImagValues.Select(x => side.Show(x))))}] " +
            $"source-entries=[{MatValEntries(side, before.Entries, matValBefore)}]";
        string currentEntries = table is Hashtable hash
            ? string.Join(";", hash.Cast<DictionaryEntry>().OrderBy(e => (int)e.Key).Select(e =>
                ShallowEntryText(side, (int)e.Key, e.Value, matValBefore)))
            : "null";
        string logicalMatch = before == null ? "no-source" :
            $"logical-real-match={Logical(before.Real) == Logical(real)} logical-imag-match={Logical(before.Imag) == Logical(imag)}";
        return $"shape={rows}x{cols} stride={stride} offset={offset} " +
            $"dim={side.DimText(dim)} flags={side.Show(Get(Get(value, "Data")!, "m_flags"))} immutable={side.Show(Get(value, "Immutable"))} " +
            $"real=[{ArrayText(side, real)}] imag=[{ArrayText(side, imag)}] logical-real=[{Logical(real)}] logical-imag=[{Logical(imag)}] " +
            $"table-ref={(before == null ? "n/a" : Ref(before.Table, table))} entries=[{currentEntries}] {logicalMatch} {sourceRelation}";
    }

    private static string ShallowEntryText(Side side, int index, object? value, IReadOnlyList<MatValState> snapshots)
    {
        if (value == null) return $"{index}:null";
        string sourceIdentity = snapshots.SelectMany(s => s.Entries).Any(e => ReferenceEquals(e.Value, value)) ? "retained-source-entry" : "independent-entry";
        if (value.GetType().Name == "NumVal")
        {
            object? data = Get(value, "Data");
            return $"{index}:NumVal:{sourceIdentity}:re={Get(data!, "rp")}:im={Get(data!, "ip")}:dim={side.DimText(Get(value, "m_dim"))}";
        }
        if (value.GetType().Name == "MatVal")
        {
            object? dim = Get(value, "m_dim");
            return $"{index}:MatVal:{sourceIdentity}:source-ref={snapshots.Any(s => ReferenceEquals(s.Value, value))}:" +
                $"{Get(value, "Rows")}x{Get(value, "Cols")}:stride={Get(value, "Anr")}:offset={Get(value, "Offset")}:" +
                $"dim={side.DimText(dim)}:flags={side.Show(Get(Get(value, "Data")!, "m_flags"))}:immutable={side.Show(Get(value, "Immutable"))}";
        }
        return $"{index}:{value.GetType().Name}:{sourceIdentity}";
    }

    private static string MatValEntries(Side side, IReadOnlyList<MatValEntry> entries, IReadOnlyList<MatValState> matValBefore) =>
        string.Join(";", entries.Select(e =>
        {
            if (e.Value == null) return $"{e.Index}:null";
            string sourceIdentity = matValBefore.SelectMany(s => s.Entries).Any(entry => ReferenceEquals(entry.Value, e.Value))
                ? "retained-source-entry" : "independent-entry";
            if (e.Value.GetType().Name == "NumVal")
                return $"{e.Index}:NumVal:{sourceIdentity}:re={e.Re}:im={e.Im}:dim={side.DimText(e.Dim)}";
            if (e.Value.GetType().Name == "MatVal")
                return $"{e.Index}:MatVal:{sourceIdentity}:source-ref={matValBefore.Any(s => ReferenceEquals(s.Value, e.Value))}";
            return $"{e.Index}:{e.Value.GetType().Name}:{sourceIdentity}";
        }));

    private static string SourceAfterText(Side side, MatValState before, IReadOnlyList<MatValState> snapshots)
    {
        object value = before.Value;
        object? dim = Get(value, "m_dim"), real = Get(value, "m_rarr"), imag = Get(value, "m_iarr"), table = Get(value, "m_parr");
        int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!, stride = (int)Get(value, "Anr")!, offset = (int)Get(value, "Offset")!;
        object? flags = Get(Get(value, "Data")!, "m_flags"), immutable = Get(value, "Immutable");
        string Ref(object? a, object? b) => ReferenceEquals(a, b) ? "same" : "changed";
        string entries = table is Hashtable hash
            ? string.Join(";", hash.Cast<DictionaryEntry>().OrderBy(e => (int)e.Key).Select(e =>
            {
                int key = (int)e.Key;
                MatValEntry? old = before.Entries.FirstOrDefault(entry => entry.Index == key);
                string identity = old == null ? "new-entry" : ReferenceEquals(old.Value, e.Value) ? "same-entry-object" : "changed-entry-object";
                return ShallowEntryText(side, key, e.Value, snapshots) + ":" + identity;
            }))
            : "null";
        return $"source-after shape={rows}x{cols} stride={stride} offset={offset} dim={side.DimText(dim)} " +
            $"flags={side.Show(flags)} immutable={side.Show(immutable)} dim-ref={Ref(before.Dim, dim)} " +
            $"real-ref={Ref(before.Real, real)} real=[{ArrayText(side, real)}] " +
            $"imag-ref={Ref(before.Imag, imag)} imag=[{ArrayText(side, imag)}] " +
            $"table-ref={Ref(before.Table, table)} entries=[{entries}] " +
            $"pre-shape={before.Rows}x{before.Cols} stride={before.Stride} offset={before.Offset} " +
            $"pre-flags={side.Show(before.Flags)} pre-immutable={side.Show(before.Immutable)} " +
            $"pre-real=[{(before.RealValues == null ? "null" : string.Join(",", before.RealValues.Select(x => side.Show(x))))}] " +
            $"pre-imag=[{(before.ImagValues == null ? "null" : string.Join(",", before.ImagValues.Select(x => side.Show(x))))}] " +
            $"pre-entries=[{MatValEntries(side, before.Entries, snapshots)}]";
    }

    private static string State(Side side, object? oldTable, object? oldReal, object? oldImag, object? oldDim,
        IReadOnlyDictionary<int, PtrState> oldPointers, IReadOnlyList<MatValState> matValBefore,
        MatValState? cloneSource, object matrix)
    {
        object? nowTable = Get(matrix, "m_parr");
        string Ref(object? a, object? b) => ReferenceEquals(a, b) ? "same" : "changed";
        string Before = oldTable is Hashtable beforeHash
            ? string.Join(";", beforeHash.Cast<DictionaryEntry>().OrderBy(e => (int)e.Key).Select(e =>
                PtrText(side, (int)e.Key, e.Value, oldPointers.TryGetValue((int)e.Key, out var prior) ? prior : null, matValBefore, cloneSource)))
            : "null";
        string After = nowTable is Hashtable currentHash
            ? string.Join(";", currentHash.Cast<DictionaryEntry>().OrderBy(e => (int)e.Key).Select(e =>
                PtrText(side, (int)e.Key, e.Value, oldPointers.TryGetValue((int)e.Key, out var prior) ? prior : null, matValBefore, cloneSource)))
            : "null";
        return $"{MatValDifferentialTests.Storage(side, matrix)} dim-ref={Ref(oldDim, Get(matrix, "m_dim"))}" +
            $" table-ref={Ref(oldTable, nowTable)} old-table-after=[{Before}] current-table=[{After}]" +
            $" real-ref={Ref(oldReal, Get(matrix, "m_rarr"))} real-old-after={ArrayText(side, oldReal)}" +
            $" imag-ref={Ref(oldImag, Get(matrix, "m_iarr"))} imag-old-after={ArrayText(side, oldImag)}" +
            $" retained-matval-sources=[{string.Join(" || ", matValBefore.Select(source => SourceAfterText(side, source, matValBefore)))}]";
    }

    private static void Compare(string caseId, string name, int token, object q, object m,
        object?[] qArgs, object?[] mArgs, params System.Type[] parameters)
    {
        var qMethod = Method(Side.Quire, name, parameters); var mMethod = Method(Side.Mathcad, name, parameters);
        Assert.Equal(token, mMethod.MetadataToken);
        var qTable = Get(q, "m_parr"); var mTable = Get(m, "m_parr");
        var qReal = Get(q, "m_rarr"); var mReal = Get(m, "m_rarr");
        var qImag = Get(q, "m_iarr"); var mImag = Get(m, "m_iarr");
        var qDim = Get(q, "m_dim"); var mDim = Get(m, "m_dim");
        var qPtrs = Snapshot(qTable); var mPtrs = Snapshot(mTable);
        var qMatVals = CaptureMatVals(q, qArgs); var mMatVals = CaptureMatVals(m, mArgs);
        var qCloneSource = qArgs.LastOrDefault()?.GetType().Name == "MatVal" ? qMatVals.FirstOrDefault(s => ReferenceEquals(s.Value, qArgs.Last())) : null;
        var mCloneSource = mArgs.LastOrDefault()?.GetType().Name == "MatVal" ? mMatVals.FirstOrDefault(s => ReferenceEquals(s.Value, mArgs.Last())) : null;
        MethodComparison.Compare(caseId, Contract, qMethod, q, qArgs, mMethod, m, mArgs,
            quireShow: value => Side.Quire.Show(value), originalShow: value => Side.Mathcad.Show(value),
            quireState: target => State(Side.Quire, qTable, qReal, qImag, qDim, qPtrs, qMatVals, qCloneSource, target),
            originalState: target => State(Side.Mathcad, mTable, mReal, mImag, mDim, mPtrs, mMatVals, mCloneSource, target));
    }

    private static void CompareBoth(string caseId, string name, int token, Action<Side, object> setup,
        object? qValue, object? mValue, int index = 4, int row = 1, int col = 1, bool shared = false,
        bool qNoValue = false, bool mNoValue = false)
    {
        var q = Matrix(Side.Quire, shared); var m = Matrix(Side.Mathcad, shared);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        object? Q(object? v) => v is ValueSentinel.QNoValue ? Get(Side.Quire.T("Mcd.NumVal"), "NoValue") : v;
        object? M(object? v) => v is ValueSentinel.MNoValue ? Get(Side.Mathcad.T("Mcd.NumVal"), "NoValue") : v;
        object? qArg = qNoValue ? Get(Side.Quire.T("Mcd.NumVal"), "NoValue") : Q(qValue);
        object? mArg = mNoValue ? Get(Side.Mathcad.T("Mcd.NumVal"), "NoValue") : M(mValue);
        switch (name)
        {
            case "set_ptrs_1":
                Compare(caseId, name, token, q, m, [index, qArg], [index, mArg], typeof(int), typeof(object));
                break;
            case "set_ptrs_2":
                Compare(caseId, name, token, q, m, [row, col, qArg], [row, col, mArg], typeof(int), typeof(int), typeof(object));
                break;
            case "set_ptrs":
            case "set_ptrs_raw":
            case "set_ptrs_at":
                Compare(caseId, name, token, q, m, [index, qArg], [index, mArg], typeof(int), typeof(object));
                break;
        }
    }

    private static class ValueSentinel
    {
        public sealed class QNoValue { }
        public sealed class MNoValue { }
    }

    [SkippableFact]
    public void Public_pointer_writers_use_their_observed_indexing_contracts()
    {
        Runtime.Sync();
        Assert.Equal(Tokens[0], Method(Side.Mathcad, "set_ptrs_1", typeof(int), typeof(object)).MetadataToken);
        Assert.Equal(Tokens[1], Method(Side.Mathcad, "set_ptrs_2", typeof(int), typeof(int), typeof(object)).MetadataToken);

        CompareBoth("set_ptrs_1 applies raw storage offset", "set_ptrs_1", Tokens[0], (_, _) => { }, "text", "text", index: 4);
        CompareBoth("set_ptrs_1 inserts null at offset index", "set_ptrs_1", Tokens[0], (_, _) => { }, null, null, index: 1);
        CompareBoth("set_ptrs_2 maps logical coordinates through padded stride and offset", "set_ptrs_2", Tokens[1], (_, _) => { },
            Side.Quire.Num(4, 5, "T"), Side.Mathcad.Num(4, 5, "T"), row: 1, col: 1);
        CompareBoth("set_ptrs_2 overwrites a stale table slot", "set_ptrs_2", Tokens[1], (side, matrix) => StalePointer(side, matrix, 0, 1),
            "replacement", "replacement", row: 0, col: 1);
    }

    [SkippableFact]
    public void Private_set_ptrs_wraps_strings_and_clones_matrix_cycles()
    {
        Runtime.Sync();
        Assert.Equal(Tokens[2], Method(Side.Mathcad, "set_ptrs", typeof(int), typeof(object)).MetadataToken);

        CompareBoth("set_ptrs wraps string as StrVal", "set_ptrs", Tokens[2], (_, _) => { }, "text", "text", index: 4);
        CompareBoth("set_ptrs stores null as a live null pointer", "set_ptrs", Tokens[2], (_, _) => { }, null, null, index: 5);
        CompareBoth("set_ptrs keeps generic object pointer type", "set_ptrs", Tokens[2], (_, _) => { }, 17, 17, index: 2);

        var qSelf = Matrix(Side.Quire); var mSelf = Matrix(Side.Mathcad);
        Compare("set_ptrs clones self-reference", "set_ptrs", Tokens[2], qSelf, mSelf,
            [2, qSelf], [2, mSelf], typeof(int), typeof(object));

        var qTarget = Matrix(Side.Quire); var mTarget = Matrix(Side.Mathcad);
        var qNested = Matrix(Side.Quire); var mNested = Matrix(Side.Mathcad);
        Pointer(Side.Quire, qNested, 0, 0, qTarget);
        Pointer(Side.Mathcad, mNested, 0, 0, mTarget);
        Compare("set_ptrs clones matrix whose graph contains destination", "set_ptrs", Tokens[2], qTarget, mTarget,
            [3, qNested], [3, mNested], typeof(int), typeof(object));

        var qIndependentTarget = Matrix(Side.Quire); var mIndependentTarget = Matrix(Side.Mathcad);
        var qIndependent = Matrix(Side.Quire); var mIndependent = Matrix(Side.Mathcad);
        Compare("set_ptrs retains independent matrix and marks it immutable", "set_ptrs", Tokens[2],
            qIndependentTarget, mIndependentTarget, [3, qIndependent], [3, mIndependent], typeof(int), typeof(object));
    }

    [SkippableFact]
    public void Raw_pointer_writer_applies_NotNoValue_before_slot_write()
    {
        Runtime.Sync();
        Assert.Equal(Tokens[3], Method(Side.Mathcad, "set_ptrs_raw", typeof(int), typeof(object)).MetadataToken);

        CompareBoth("set_ptrs_raw stores raw string without StrVal wrapping", "set_ptrs_raw", Tokens[3], (_, _) => { }, "text", "text", index: 4);
        CompareBoth("set_ptrs_raw preserves ordinary NumVal pointer", "set_ptrs_raw", Tokens[3], (_, _) => { },
            Side.Quire.Num(0, 3, "T"), Side.Mathcad.Num(0, 3, "T"), index: 2);
        CompareBoth("set_ptrs_raw inserts table value before invalid backing exception", "set_ptrs_raw", Tokens[3], (_, matrix) => { },
            "invalid", "invalid", index: 7);
        CompareBoth("set_ptrs_raw allocates table before NoValue exception", "set_ptrs_raw", Tokens[3], (_, matrix) => Set(matrix, "m_parr", null),
            null, null, index: 1, qNoValue: true, mNoValue: true);
    }

    [SkippableFact]
    public void Direct_pointer_slot_writer_inserts_overwrites_and_mutates_table_before_array_failure()
    {
        Runtime.Sync();
        Assert.Equal(Tokens[4], Method(Side.Mathcad, "set_ptrs_at", typeof(int), typeof(object)).MetadataToken);

        CompareBoth("set_ptrs_at inserts NumVal without transforming identity", "set_ptrs_at", Tokens[4], (_, _) => { },
            Side.Quire.Num(0, 0, "mcd_zero"), Side.Mathcad.Num(0, 0, "mcd_zero"), index: 3);
        CompareBoth("set_ptrs_at overwrites existing table value", "set_ptrs_at", Tokens[4], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 1, side.Num(1, 0, "L"));
        }, "replacement", "replacement", index: 5);
        CompareBoth("set_ptrs_at stores null pointer object", "set_ptrs_at", Tokens[4], (_, _) => { }, null, null, index: 2);
        CompareBoth("set_ptrs_at table insertion precedes invalid backing write", "set_ptrs_at", Tokens[4], (_, _) => { },
            "invalid", "invalid", index: 7);
    }
}

using System.Collections;
using System.Reflection;
using Quire.Mcd;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DimValApplyMatValDifferentialTests
{
    private const string Contract = "Mcd.DimVal";
    private const int OriginalToken = 0x060006b0;
    private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static System.Type QDimVal => typeof(global::Quire.Mcd.DimVal);
    private static System.Type MDimVal => Mathcad.Type("mplrun", "Mcd.DimVal");

    private sealed class Invocation
    {
        public int Calls;
        public object[]? Arguments;
        public object? Result;
    }

    private sealed record MatrixSnapshot(object Matrix, object? Dimension, object? Table, object? Real, object? Imag,
        double[] RealValues, double[]? ImagValues, IReadOnlyDictionary<int, object?> Pointers, int Rows, int Cols,
        int Stride, int Offset, long Flags);

    private sealed record Context(Side Side, object? Function, Invocation? Invocation, object? Matrix, MatrixSnapshot? Snapshot);

    private static (object Q, object M) MatrixPair(bool livePointer = false, bool staleEntry = false, bool missingPointerMarker = false)
    {
        object Create(Side side)
        {
            const int rows = 2, cols = 3, stride = 3, offset = 1;
            var real = Enumerable.Repeat(-91.0, offset + stride * cols).ToArray();
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++) real[offset + col * stride + row] = col * 10 + row + 1;
                real[offset + col * stride + rows] = -20 - col;
            }
            var matrix = New(side.T("Mcd.MatVal"), real, null, rows, cols, stride, new Hashtable(),
                Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
            Set(matrix, "m_dim", side.D("L"));
            if (livePointer)
            {
                var pointer = New(side.T("Mcd.NumVal"), 777.0, 0.0, side.D("T"));
                Call(matrix, "set_ptrs_2", 1, 1, pointer);
            }
            if (staleEntry)
            {
                var pointer = New(side.T("Mcd.NumVal"), 888.0, 0.0, side.D("T"));
                int index = (int)Call(matrix, "Index", 0, 2)!;
                ((Hashtable)Get(matrix, "m_parr")!)[index] = pointer;
            }
            if (missingPointerMarker)
            {
                int index = (int)Call(matrix, "Index", 0, 2)!;
                double marker = (double)Get(side.T("Mcd.MatVal"), "PtrNan")!;
                ((double[])Get(matrix, "m_rarr")!)[index] = marker;
            }
            return matrix;
        }
        return (Create(Side.Quire), Create(Side.Mathcad));
    }

    private static MatrixSnapshot Capture(object matrix)
    {
        var data = Get(matrix, "Data")!;
        var table = Get(matrix, "m_parr") as Hashtable;
        var pointers = table?.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
            ?? new Dictionary<int, object?>();
        var real = (double[])Get(matrix, "m_rarr")!;
        var imag = Get(matrix, "m_iarr") as double[];
        return new(matrix, Get(matrix, "m_dim"), table, real, imag, (double[])real.Clone(), imag == null ? null : (double[])imag.Clone(),
            pointers, (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!, (int)Get(matrix, "Anr")!, (int)Get(matrix, "Offset")!,
            Convert.ToInt64(Get(data, "m_flags")));
    }

    private static string MatrixState(Side side, MatrixSnapshot before, object matrix)
    {
        string Ref(object? a, object? b) => ReferenceEquals(a, b) ? "same" : "changed";
        string Entries(IEnumerable<KeyValuePair<int, object?>> values, IReadOnlyDictionary<int, object?> old) =>
            string.Join(";", values.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{side.Show(pair.Value)}:" +
                (old.TryGetValue(pair.Key, out var previous) && ReferenceEquals(previous, pair.Value) ? "same-before" : "changed")));
        var table = Get(matrix, "m_parr") as Hashtable;
        var after = table?.Cast<DictionaryEntry>().Select(entry => new KeyValuePair<int, object?>((int)entry.Key, entry.Value))
            ?? Enumerable.Empty<KeyValuePair<int, object?>>();
        var data = Get(matrix, "Data")!;
        var real = (double[])Get(matrix, "m_rarr")!;
        var imag = Get(matrix, "m_iarr") as double[];
        return $"{Get(matrix, "Rows")}x{Get(matrix, "Cols")} stride {Get(matrix, "Anr")} offset {Get(matrix, "Offset")}" +
            $" flags {Convert.ToInt64(Get(data, "m_flags"))} Shared {Get(matrix, "Shared")}" +
            $" dim {side.DimText(Get(matrix, "m_dim"))} dim-ref {Ref(before.Dimension, Get(matrix, "m_dim"))}" +
            $" real-ref {Ref(before.Real, real)} real-before [{side.Show(before.RealValues)}] real-after [{side.Show(real)}]" +
            $" imag-ref {Ref(before.Imag, imag)} imag-before [{side.Show(before.ImagValues)}] imag-after [{side.Show(imag)}]" +
            $" table-ref {Ref(before.Table, table)} table-before [{Entries(before.Pointers, before.Pointers)}]" +
            $" table-after [{(table == null ? "null" : Entries(after, before.Pointers))}]";
    }

    private static string InvocationState(Side side, Invocation invocation, MatrixSnapshot? source)
    {
        string args = invocation.Arguments == null ? "null" : string.Join(" | ", invocation.Arguments.Select(side.Show));
        string aliases = invocation.Arguments == null || source == null ? "none" : string.Join(",", source.Pointers
            .Where(pair => invocation.Arguments.Any(argument => ReferenceEquals(argument, pair.Value)))
            .Select(pair => pair.Key));
        return $"calls {invocation.Calls} args [{args}] source-pointer-keys [{aliases}] result-same-as-args {ReferenceEquals(invocation.Result, invocation.Arguments)}";
    }

    private static string Render(Side side, object? value, Context context)
    {
        if (ReferenceEquals(value, context.Function) && context.Invocation != null)
            return "function " + InvocationState(side, context.Invocation, context.Snapshot);
        if (ReferenceEquals(value, context.Matrix) && context.Snapshot != null)
            return "matrix " + MatrixState(side, context.Snapshot, value!);
        if (value is object[] result)
        {
            string aliases = context.Snapshot == null ? "none" : string.Join(",", context.Snapshot.Pointers
                .Where(pair => result.Any(element => ReferenceEquals(element, pair.Value))).Select(pair => pair.Key));
            return $"object[{result.Length}] same-callback-result {ReferenceEquals(result, context.Invocation?.Result)}" +
                $" source-pointer-keys [{aliases}] values [{string.Join(" | ", result.Select(side.Show))}]";
        }
        return side.Show(value);
    }

    private static (object Q, object M, Invocation QState, Invocation MState) FunctionPair()
    {
        var qState = new Invocation(); var mState = new Invocation();
        Func<object, object, object, object, object, object, object> qDelegate = (a, b, c, d, e, f) =>
        {
            qState.Calls++;
            qState.Arguments = [a, b, c, d, e, f];
            qState.Result = qState.Arguments;
            return qState.Result;
        };
        Func<object, object, object, object, object, object, object> mDelegate = (a, b, c, d, e, f) =>
        {
            mState.Calls++;
            mState.Arguments = [a, b, c, d, e, f];
            mState.Result = mState.Arguments;
            return mState.Result;
        };
        var qCtor = typeof(DelegateFunction).GetConstructor(All, null, [typeof(Delegate), typeof(int)], null)!;
        var mType = Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");
        var mCtor = mType.GetConstructor(All, null, [typeof(Delegate), typeof(int)], null)!;
        return (qCtor.Invoke([qDelegate, 6]), mCtor.Invoke([mDelegate, 6]), qState, mState);
    }

    private static void Compare(string caseId, object qFunction, object qMatrix, Invocation? qInvocation, object mFunction,
        object mMatrix, Invocation? mInvocation)
    {
        var qSource = qMatrix.GetType().Name == "MatVal" ? qMatrix : null;
        var mSource = mMatrix.GetType().Name == "MatVal" ? mMatrix : null;
        var qSnapshot = qSource == null ? null : Capture(qSource);
        var mSnapshot = mSource == null ? null : Capture(mSource);
        var qContext = new Context(Side.Quire, qFunction, qInvocation, qSource, qSnapshot);
        var mContext = new Context(Side.Mathcad, mFunction, mInvocation, mSource, mSnapshot);
        var qMethod = QDimVal.GetMethod("ApplyMatVal", BindingFlags.Public | BindingFlags.Static, null, [QDimVal, QDimVal], null)!;
        var mMethod = MDimVal.GetMethod("ApplyMatVal", BindingFlags.Public | BindingFlags.Static, null, [MDimVal, MDimVal], null)!;
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qFunction, qMatrix], mMethod, null, [mFunction, mMatrix],
            value => Render(Side.Quire, value, qContext), value => Render(Side.Mathcad, value, mContext));
    }

    [SkippableFact]
    public void ApplyMatVal_packs_nonsymmetric_matrix_column_major_and_preserves_live_pointer()
    {
        Runtime.Sync();
        var (qMatrix, mMatrix) = MatrixPair(livePointer: true);
        var (qFunction, mFunction, qState, mState) = FunctionPair();
        Compare("2x3 padded matrix packs column-major values and live pointer identity", qFunction, qMatrix, qState,
            mFunction, mMatrix, mState);
        Assert.Equal(1, qState.Calls); Assert.Equal(1, mState.Calls);
        Assert.Equal(777.0, Get(qState.Arguments![3]!, "Real"));
        Assert.Equal(777.0, Get(mState.Arguments![3]!, "Real"));
    }

    [SkippableFact]
    public void ApplyMatVal_preserves_stale_table_and_resolves_missing_pointer_marker()
    {
        Runtime.Sync();
        var (qStale, mStale) = MatrixPair(staleEntry: true);
        var (qStaleFn, mStaleFn, qStaleState, mStaleState) = FunctionPair();
        Compare("stale table entry outside a pointer slot is retained", qStaleFn, qStale, qStaleState,
            mStaleFn, mStale, mStaleState);

        var (qMissing, mMissing) = MatrixPair(missingPointerMarker: true);
        var (qMissingFn, mMissingFn, qMissingState, mMissingState) = FunctionPair();
        Compare("missing pointer marker packs as NaN and updates storage", qMissingFn, qMissing, qMissingState,
            mMissingFn, mMissing, mMissingState);
        Assert.Equal(1, qStaleState.Calls); Assert.Equal(1, mStaleState.Calls);
        Assert.Equal(1, qMissingState.Calls); Assert.Equal(1, mMissingState.Calls);
        Assert.True(double.IsNaN((double)Get(Get(qMissingState.Arguments![4]!, "Data")!, "rp")!));
        Assert.True(double.IsNaN((double)Get(Get(mMissingState.Arguments![4]!, "Data")!, "rp")!));
    }

    [SkippableFact]
    public void ApplyMatVal_propagates_apply_and_matrix_conversion_exceptions()
    {
        Runtime.Sync();
        var (qMatrix, mMatrix) = MatrixPair();
        Compare("base Apply exception after matrix packing", Side.Quire.NotAVal()!, qMatrix, null,
            Side.Mathcad.NotAVal()!, mMatrix, null);

        var (qFunction, mFunction, qState, mState) = FunctionPair();
        var qScalar = Side.Quire.Num(5, 0, "L"); var mScalar = Side.Mathcad.Num(5, 0, "L");
        Compare("asMatrix conversion exception occurs before Apply callback", qFunction, qScalar, qState,
            mFunction, mScalar, mState);
        Assert.Equal(0, qState.Calls); Assert.Equal(0, mState.Calls);
    }
}

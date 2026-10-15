using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValViewDifferentialTests
{
    private const string Contract = "Mcd.MatVal.Views";
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static System.Type QMat => typeof(global::Quire.Mcd.MatVal);
    private static System.Type MMat => Mathcad.Type("mplrun", "Mcd.MatVal");

    private static System.Type MapType(Side side, System.Type type)
    {
        if (side == Side.Quire) return type;
        if (type == QMat) return MMat;
        if (type == typeof(global::Quire.Mcd.DimVal)) return side.T("Mcd.DimVal");
        if (type == typeof(global::Quire.Mcd.MatVal)) return side.T("Mcd.MatVal");
        return type;
    }

    private static MethodInfo Method(Side side, string name, bool isStatic, params System.Type[] parameters)
    {
        var flags = Declared | (isStatic ? BindingFlags.Static : BindingFlags.Instance);
        var method = (side == Side.Quire ? QMat : MMat).GetMethod(name, flags, null,
            parameters.Select(type => MapType(side, type)).ToArray(), null);
        return method ?? throw new HarnessException($"Missing {side.Name} method {name}({string.Join(", ", parameters.Select(type => type.Name))})");
    }

    private static ConstructorInfo Constructor(Side side, params System.Type[] parameters)
    {
        var type = side == Side.Quire ? QMat : MMat;
        return type.GetConstructor(Declared | BindingFlags.Instance, null,
            parameters.Select(parameter => MapType(side, parameter)).ToArray(), null)
            ?? throw new HarnessException($"Missing {side.Name} MatVal constructor ({string.Join(", ", parameters.Select(parameter => parameter.Name))})");
    }

    private static object Matrix(Side side, int kind = 0)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[] { 99, 1, 2, 99, 3, 4, 99 },
            new double[7], 2, 2, 3, null, Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
        Set(matrix, "m_dim", side.D("L"));
        if (kind > 0) Call(matrix, "set_ptrs_2", 0, 0, "outside");
        if (kind > 1) CallSig(matrix, "set_2", ["Int32", "Int32", "Object"], 1, 1, side.Num(5, 2, "T"));
        if (kind > 2) Call(matrix, "set_ptrs_2", 0, 1, New(side.T("Mcd.MatVal"), new double[] { 6 }));
        return matrix;
    }

    private static object Shape(Side side, int rows, int cols, int stride, int offset = 0)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        var imag = new double[length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
        {
            real[offset + row + stride * col] = row + 1 + 10 * col;
            imag[offset + row + stride * col] = row + 0.5 + col;
        }
        return New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
    }

    private static string Storage(Side side, object matrix) => MatValDifferentialTests.Storage(side, matrix);
    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
            return Storage(side, value);
        if (value is Hashtable table)
            return "{" + string.Join(";", table.Keys.Cast<int>().Order().Select(index => index + ":" + Show(side, table[index]))) + "}";
        if (value is IEnumerable values && value is not string)
            return "[" + string.Join(", ", values.Cast<object?>().Select(item => Show(side, item))) + "]";
        return side.Show(value);
    }

    private static void Compare(string caseId, string name, System.Type[] parameters, bool isStatic,
        object? qTarget, object? mTarget, object?[] qArgs, object?[] mArgs,
        Func<Side, object?, string>? render = null,
        Func<Side, object, string>? qState = null, Func<Side, object, string>? mState = null)
    {
        render ??= Show;
        MethodComparison.Compare(caseId, Contract,
            Method(Side.Quire, name, isStatic, parameters), qTarget, qArgs,
            Method(Side.Mathcad, name, isStatic, parameters), mTarget, mArgs,
            value => render(Side.Quire, value), value => render(Side.Mathcad, value),
            qState == null ? null : target => qState(Side.Quire, target),
            mState == null ? null : target => mState(Side.Mathcad, target));
    }

    private static void CompareInstance(string caseId, string name, System.Type[] parameters,
        object qTarget, object mTarget, object?[] qArgs, object?[] mArgs,
        Func<Side, object?, string>? render = null)
        => Compare(caseId, name, parameters, false, qTarget, mTarget, qArgs, mArgs, render,
            (side, target) => Storage(side, target), (side, target) => Storage(side, target));

    private static void CompareConstructor(string caseId, System.Type[] parameters, object?[] qArgs, object?[] mArgs,
        Func<Side, object?, string> render)
    {
        MethodComparison.Compare(caseId, Contract,
            Constructor(Side.Quire, parameters), null, qArgs,
            Constructor(Side.Mathcad, parameters), null, mArgs,
            value => render(Side.Quire, value), value => render(Side.Mathcad, value));
    }

    [SkippableFact]
    public void Direct_views_and_private_helpers()
    {
        Runtime.Sync();
        var qSource = Matrix(Side.Quire, 1);
        var mSource = Matrix(Side.Mathcad, 1);
        CompareConstructor("private column-view constructor trims pointer entries", [typeof(global::Quire.Mcd.MatVal), typeof(int)],
            [qSource, 0], [mSource, 0], (side, result) => ShowWithSource(side, result, side == Side.Quire ? qSource : mSource));

        var qTrim = Matrix(Side.Quire, 1);
        var mTrim = Matrix(Side.Mathcad, 1);
        CompareInstance("TrimPtrs copies only live pointer NaNs", "TrimPtrs", [typeof(Hashtable)], qTrim, mTrim,
            [new Hashtable((Hashtable)Get(qTrim, "m_parr")!)], [new Hashtable((Hashtable)Get(mTrim, "m_parr")!)]);

        var qColumn = Matrix(Side.Quire, 1);
        var mColumn = Matrix(Side.Mathcad, 1);
        CompareInstance("Column packs mutable mixed storage", "Column", [typeof(int)], qColumn, mColumn, [0], [0],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qColumn : mColumn));
        qColumn = Shape(Side.Quire, 1, 1, 1); mColumn = Shape(Side.Mathcad, 1, 1, 1);
        CompareInstance("Column uniform mutable path", "Column", [typeof(int)], qColumn, mColumn, [0], [0],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qColumn : mColumn));
        qColumn = Matrix(Side.Quire, 1); mColumn = Matrix(Side.Mathcad, 1);
        Set(qColumn, "Immutable", true); Set(mColumn, "Immutable", true);
        CompareInstance("Column mixed immutable path", "Column", [typeof(int)], qColumn, mColumn, [0], [0],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qColumn : mColumn));
        qColumn = Matrix(Side.Quire); mColumn = Matrix(Side.Mathcad);
        CompareInstance("Column raw out-of-range path", "Column", [typeof(int)], qColumn, mColumn, [2], [2]);
        var qScalar = Shape(Side.Quire, 1, 1, 1);
        var mScalar = Shape(Side.Mathcad, 1, 1, 1);
        Set(qScalar, "Immutable", true); Set(mScalar, "Immutable", true);
        CompareInstance("Column aliases immutable uniform scalar", "Column", [typeof(int)], qScalar, mScalar, [0], [0],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qScalar : mScalar));
        var qReal = Matrix(Side.Quire, 2);
        var mReal = Matrix(Side.Mathcad, 2);
        CompareInstance("real_column mixed rows", "real_column", [typeof(int)], qReal, mReal, [1], [1]);
        var qUniformReal = Shape(Side.Quire, 1, 2, 1);
        var mUniformReal = Shape(Side.Mathcad, 1, 2, 1);
        CompareInstance("real_column uniform scalar rows", "real_column", [typeof(int)], qUniformReal, mUniformReal, [0], [0]);

        foreach (var wrapper in new[] { "mcd_sym_column", "mcd_column" })
        {
            var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
            Compare("wrapper successful column " + wrapper, wrapper, [typeof(object), typeof(int)], true,
                null, null, [q, 1], [m, 1]);
            q = Matrix(Side.Quire); m = Matrix(Side.Mathcad);
            Compare("wrapper first column " + wrapper, wrapper, [typeof(object), typeof(int)], true,
                null, null, [q, 0], [m, 0]);
            Compare("wrapper invalid type " + wrapper, wrapper, [typeof(object), typeof(int)], true,
                null, null, ["text", 0], ["text", 0]);
            foreach (int index in new[] { -1, 2 })
            {
                q = Matrix(Side.Quire); m = Matrix(Side.Mathcad);
                Compare("wrapper invalid index " + wrapper + " " + index, wrapper, [typeof(object), typeof(int)], true,
                    null, null, [q, index], [m, index]);
            }
        }
        foreach (var wrapper in new[] { "mcd_sym_row", "mcd_row" })
        {
            var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
            Compare("wrapper successful row " + wrapper, wrapper, [typeof(object), typeof(int)], true,
                null, null, [q, 1], [m, 1]);
            q = Matrix(Side.Quire); m = Matrix(Side.Mathcad);
            Compare("wrapper first row " + wrapper, wrapper, [typeof(object), typeof(int)], true,
                null, null, [q, 0], [m, 0]);
            Compare("wrapper invalid type " + wrapper, wrapper, [typeof(object), typeof(int)], true,
                null, null, ["text", 0], ["text", 0]);
            foreach (int index in new[] { -1, 2 })
            {
                q = Matrix(Side.Quire); m = Matrix(Side.Mathcad);
                Compare("wrapper invalid index " + wrapper + " " + index, wrapper, [typeof(object), typeof(int)], true,
                    null, null, [q, index], [m, index]);
            }
        }

        var qRow = Matrix(Side.Quire, 1); var mRow = Matrix(Side.Mathcad, 1);
        CompareInstance("Row delegates to half-open submatrix", "Row", [typeof(int)], qRow, mRow, [1], [1]);
        var qSub = Matrix(Side.Quire, 1); var mSub = Matrix(Side.Mathcad, 1);
        CompareInstance("SubMatrix aliases and marks source immutable", "SubMatrix",
            [typeof(uint), typeof(uint), typeof(uint), typeof(uint)], qSub, mSub, [0u, 2u, 1u, 2u], [0u, 2u, 1u, 2u],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qSub : mSub));
        qSub = Matrix(Side.Quire, 1); mSub = Matrix(Side.Mathcad, 1);
        CompareInstance("SubMatrix rejects empty bounds", "SubMatrix",
            [typeof(uint), typeof(uint), typeof(uint), typeof(uint)], qSub, mSub, [1u, 1u, 0u, 1u], [1u, 1u, 0u, 1u]);
        foreach (uint[] bounds in new uint[][]
        {
            [0, 1, 1, 1], [0, 3, 0, 1], [0, 1, 0, 3], [2, 1, 0, 1]
        })
        {
            qSub = Matrix(Side.Quire); mSub = Matrix(Side.Mathcad);
            CompareInstance("SubMatrix rejects bounds " + string.Join(",", bounds), "SubMatrix",
                [typeof(uint), typeof(uint), typeof(uint), typeof(uint)], qSub, mSub,
                bounds.Cast<object?>().ToArray(), bounds.Cast<object?>().ToArray());
        }
        qSub = Matrix(Side.Quire, 1); mSub = Matrix(Side.Mathcad, 1);
        CompareConstructor("private half-open submatrix constructor", [typeof(global::Quire.Mcd.MatVal), typeof(int), typeof(int), typeof(int), typeof(int)],
            [qSub, 0, 1, 0, 1], [mSub, 0, 1, 0, 1], (side, value) => ShowWithSource(side, value, side == Side.Quire ? qSub : mSub));

        var qNestedType = typeof(global::Quire.Mcd.MatVal).GetNestedType("TrimSubMatrixPtrs", BindingFlags.NonPublic)!;
        var mNestedType = Mathcad.Type("mplrun", "Mcd.MatVal+TrimSubMatrixPtrs");
        var qDestination = Shape(Side.Quire, 2, 2, 3); var mDestination = Shape(Side.Mathcad, 2, 2, 3);
        MethodComparison.Compare("TrimSubMatrixPtrs constructor stores destination and bounds", Contract,
            qNestedType.GetConstructor(Declared | BindingFlags.Instance, null,
                [QMat, typeof(int), typeof(int), typeof(int), typeof(int)], null)!, null,
            [qDestination, 0, 1, 1, 2],
            mNestedType.GetConstructor(Declared | BindingFlags.Instance, null,
                [MMat, typeof(int), typeof(int), typeof(int), typeof(int)], null)!, null,
            [mDestination, 0, 1, 1, 2],
            value => ShowNested(Side.Quire, value), value => ShowNested(Side.Mathcad, value));
        var qCallback = New(qNestedType, qDestination, 0, 1, 1, 2);
        var mCallback = New(mNestedType, mDestination, 0, 1, 1, 2);
        var qNestedMethod = qNestedType.GetMethod("set_submatrix_ptrs", Declared | BindingFlags.Instance)!;
        var mNestedMethod = mNestedType.GetMethod("set_submatrix_ptrs", Declared | BindingFlags.Instance)!;
        MethodComparison.Compare("TrimSubMatrixPtrs in-range callback writes shifted pointer", Contract,
            qNestedMethod, qCallback, [0, 1, "inside"], mNestedMethod, mCallback, [0, 1, "inside"],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            value => NestedState(Side.Quire, value), value => NestedState(Side.Mathcad, value));
        qCallback = New(qNestedType, qDestination, 0, 1, 1, 2);
        mCallback = New(mNestedType, mDestination, 0, 1, 1, 2);
        MethodComparison.Compare("TrimSubMatrixPtrs ignores outside callback", Contract,
            qNestedMethod, qCallback, [1, 0, "outside"], mNestedMethod, mCallback, [1, 0, "outside"],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            value => NestedState(Side.Quire, value), value => NestedState(Side.Mathcad, value));
        qDestination = Shape(Side.Quire, 4, 4, 5); mDestination = Shape(Side.Mathcad, 4, 4, 5);
        qCallback = New(qNestedType, qDestination, 1, 3, 1, 3);
        mCallback = New(mNestedType, mDestination, 1, 3, 1, 3);
        MethodComparison.Compare("TrimSubMatrixPtrs shifts a nonzero source row and column", Contract,
            qNestedMethod, qCallback, [2, 2, "shifted"], mNestedMethod, mCallback, [2, 2, "shifted"],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            value => NestedState(Side.Quire, value), value => NestedState(Side.Mathcad, value));
        qSub = Matrix(Side.Quire, 1); mSub = Matrix(Side.Mathcad, 1);
        var qPtrTarget = Shape(Side.Quire, 2, 1, 2); var mPtrTarget = Shape(Side.Mathcad, 2, 1, 2);
        CompareInstance("private SubMatrixPtrs transforms selected pointer coordinates", "SubMatrixPtrs",
            [typeof(global::Quire.Mcd.MatVal), typeof(int), typeof(int), typeof(int), typeof(int)], qPtrTarget, mPtrTarget,
            [qSub, 0, 1, 0, 1], [mSub, 0, 1, 0, 1]);

        var qTranspose = Matrix(Side.Quire); var mTranspose = Matrix(Side.Mathcad);
        CompareInstance("TransposeArr uses logical rows and columns", "TransposeArr", [typeof(double[])], qTranspose, mTranspose,
            [Get(qTranspose, "m_rarr")!], [Get(mTranspose, "m_rarr")!], (side, value) => Show(side, value));
        qTranspose = Matrix(Side.Quire); mTranspose = Matrix(Side.Mathcad);
        CompareInstance("TransposeReal transposes backing", "TransposeReal", [], qTranspose, mTranspose, [], [], (side, value) => Show(side, value));
        CompareInstance("TransposeImag transposes backing", "TransposeImag", [], qTranspose, mTranspose, [], [], (side, value) => Show(side, value));
        Set(qTranspose, "m_iarr", null); Set(mTranspose, "m_iarr", null);
        CompareInstance("TransposeImag preserves absent backing", "TransposeImag", [], qTranspose, mTranspose, [], [], (side, value) => Show(side, value));
        qTranspose = Matrix(Side.Quire, 1); mTranspose = Matrix(Side.Mathcad, 1);
        CompareInstance("Transpose returns packed matrix and transposes pointers", "Transpose", [], qTranspose, mTranspose, [], [],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qTranspose : mTranspose));
        qTranspose = Shape(Side.Quire, 2, 2, 3); mTranspose = Shape(Side.Mathcad, 2, 2, 3);
        CompareInstance("set_ptrs_2_transpose maps row to column", "set_ptrs_2_transpose",
            [typeof(int), typeof(int), typeof(object)], qTranspose, mTranspose, [0, 1, "ptr"], [0, 1, "ptr"]);
        qTranspose = Shape(Side.Quire, 2, 2, 3); mTranspose = Shape(Side.Mathcad, 2, 2, 3);
        var qPtrSource = Matrix(Side.Quire, 1); var mPtrSource = Matrix(Side.Mathcad, 1);
        CompareInstance("TransposePtrs maps source pointers", "TransposePtrs", [typeof(global::Quire.Mcd.MatVal)], qTranspose, mTranspose,
            [qPtrSource], [mPtrSource]);
        var qReverse = Shape(Side.Quire, 3, 2, 4); var mReverse = Shape(Side.Mathcad, 3, 2, 4);
        CompareInstance("set_ptrs_2_reverse uses original Rows minus row", "set_ptrs_2_reverse",
            [typeof(int), typeof(int), typeof(object)], qReverse, mReverse, [0, 1, "ptr"], [0, 1, "ptr"]);
        qReverse = Shape(Side.Quire, 3, 2, 4); mReverse = Shape(Side.Mathcad, 3, 2, 4);
        CompareInstance("set_ptrs_2_reverse offsets a nonzero row", "set_ptrs_2_reverse",
            [typeof(int), typeof(int), typeof(object)], qReverse, mReverse, [1, 1, "ptr"], [1, 1, "ptr"]);
        qPtrSource = Matrix(Side.Quire, 1); mPtrSource = Matrix(Side.Mathcad, 1);
        CompareInstance("ReversePtrs maps source pointers using original coordinate", "ReversePtrs",
            [typeof(global::Quire.Mcd.MatVal)], qReverse, mReverse, [qPtrSource], [mPtrSource]);

        var qTuple = Matrix(Side.Quire); var mTuple = Matrix(Side.Mathcad);
        CompareInstance("asTuple returns column-major requested shape", "asTuple",
            [typeof(uint), typeof(uint), typeof(uint)], qTuple, mTuple, [1u, 2u, 7u], [1u, 2u, 7u], (side, value) => Show(side, value));
        qTuple = Matrix(Side.Quire); mTuple = Matrix(Side.Mathcad);
        CompareInstance("asTuple rejects oversized shape with handle", "asTuple",
            [typeof(uint), typeof(uint), typeof(uint)], qTuple, mTuple, [3u, 2u, 9u], [3u, 2u, 9u], (side, value) => Show(side, value));
        qTuple = Matrix(Side.Quire); mTuple = Matrix(Side.Mathcad);
        CompareInstance("asTuple rejects oversized columns with handle", "asTuple",
            [typeof(uint), typeof(uint), typeof(uint)], qTuple, mTuple, [2u, 3u, 9u], [2u, 3u, 9u], (side, value) => Show(side, value));
        qTuple = Matrix(Side.Quire); mTuple = Matrix(Side.Mathcad);
        CompareInstance("get_elements requested prefix", "get_elements", [typeof(uint), typeof(uint)], qTuple, mTuple,
            [1u, 2u], [1u, 2u], (side, value) => Show(side, value));
        qTuple = Matrix(Side.Quire); mTuple = Matrix(Side.Mathcad);
        CompareInstance("get_elements logical shape", "get_elements", [], qTuple, mTuple, [], [], (side, value) => Show(side, value));

        var qEquals = Matrix(Side.Quire); var mEquals = Matrix(Side.Mathcad);
        CompareInstance("Equals same values", "Equals", [typeof(object)], qEquals, mEquals, [Matrix(Side.Quire)], [Matrix(Side.Mathcad)]);
        CompareInstance("Equals rejects other type", "Equals", [typeof(object)], qEquals, mEquals, ["text"], ["text"]);
        CompareInstance("Equals rejects null", "Equals", [typeof(object)], qEquals, mEquals, [null], [null]);
        CompareInstance("Equals rejects different shape", "Equals", [typeof(object)], qEquals, mEquals,
            [Shape(Side.Quire, 1, 1, 1)], [Shape(Side.Mathcad, 1, 1, 1)]);
        var qUnequal = Matrix(Side.Quire); var mUnequal = Matrix(Side.Mathcad);
        ((double[])Get(qUnequal, "m_rarr")!)[1] = 0;
        ((double[])Get(mUnequal, "m_rarr")!)[1] = 0;
        CompareInstance("Equals detects element mismatch", "Equals", [typeof(object)], qEquals, mEquals, [qUnequal], [mUnequal]);
        var qNear = Matrix(Side.Quire); var mNear = Matrix(Side.Mathcad);
        CompareInstance("nearly_equal rejects nonmatrix", "nearly_equal", [typeof(global::Quire.Mcd.DimVal), typeof(double)], qNear, mNear,
            [Side.Quire.Num(1, 0, "L"), 0.001], [Side.Mathcad.Num(1, 0, "L"), 0.001]);
        CompareInstance("nearly_equal same shape and values", "nearly_equal", [typeof(global::Quire.Mcd.DimVal), typeof(double)], qNear, mNear,
            [Matrix(Side.Quire), 0.001], [Matrix(Side.Mathcad), 0.001]);
        var qNearValue = Matrix(Side.Quire); var mNearValue = Matrix(Side.Mathcad);
        var qNearArg = Matrix(Side.Quire); var mNearArg = Matrix(Side.Mathcad);
        ((double[])Get(qNearArg, "m_rarr")!)[1] += 2;
        ((double[])Get(mNearArg, "m_rarr")!)[1] += 2;
        CompareInstance("nearly_equal detects value outside fuzz", "nearly_equal", [typeof(global::Quire.Mcd.DimVal), typeof(double)], qNearValue, mNearValue,
            [qNearArg, 0.001], [mNearArg, 0.001]);
        CompareInstance("nearly_equal rejects different shape", "nearly_equal", [typeof(global::Quire.Mcd.DimVal), typeof(double)], qNear, mNear,
            [Shape(Side.Quire, 1, 1, 1), 0.001], [Shape(Side.Mathcad, 1, 1, 1), 0.001]);
        CompareInstance("ToString rows and columns", "ToString", [], Matrix(Side.Quire), Matrix(Side.Mathcad), [], []);
        CompareInstance("ToString truncates dimensions", "ToString", [], Shape(Side.Quire, 11, 4, 12), Shape(Side.Mathcad, 11, 4, 12), [], []);
        CompareInstance("ToString skips rows for zero columns", "ToString", [], Shape(Side.Quire, 2, 0, 1), Shape(Side.Mathcad, 2, 0, 1), [], []);

        var qImmutable = Matrix(Side.Quire); var mImmutable = Matrix(Side.Mathcad);
        CompareInstance("asImmutable sets flag and returns self", "asImmutable", [], qImmutable, mImmutable, [], [],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qImmutable : mImmutable));
        qImmutable = Matrix(Side.Quire); mImmutable = Matrix(Side.Mathcad);
        CompareInstance("Fix sets flag and returns self", "Fix", [], qImmutable, mImmutable, [], [],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qImmutable : mImmutable));
        qImmutable = Matrix(Side.Quire); mImmutable = Matrix(Side.Mathcad);
        Compare("Fix(object) matrix", "Fix", [typeof(object)], true, null, null, [qImmutable], [mImmutable],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qImmutable : mImmutable));
        Compare("Fix(object) null", "Fix", [typeof(object)], true, null, null, [null], [null]);
        Compare("Fix(object) nonmatrix", "Fix", [typeof(object)], true, null, null, ["text"], ["text"]);
        var qResult = Matrix(Side.Quire); var mResult = Matrix(Side.Mathcad);
        CompareInstance("asResult packs unpacked matrices", "asResult", [], qResult, mResult, [], [],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qResult : mResult));
        qResult = Shape(Side.Quire, 2, 2, 2); mResult = Shape(Side.Mathcad, 2, 2, 2);
        CompareInstance("asResult retains packed matrices", "asResult", [], qResult, mResult, [], [],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qResult : mResult));
        qResult = Shape(Side.Quire, 2, 2, 2); mResult = Shape(Side.Mathcad, 2, 2, 2);
        Set(qResult, "m_dim", null); Set(mResult, "m_dim", null);
        CompareInstance("asResult fixes null dimension", "asResult", [], qResult, mResult, [], [],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qResult : mResult));
        Compare("FixForExport wraps string", "FixForExport", [typeof(object)], true, null, null, ["text"], ["text"]);
        Compare("FixForExport passes plain object", "FixForExport", [typeof(object)], true, null, null, [new object()], [new object()]);
        var qExport = Matrix(Side.Quire); var mExport = Matrix(Side.Mathcad);
        Set(qExport, "m_dim", null); Set(mExport, "m_dim", null);
        Compare("FixForExport fixes matrix", "FixForExport", [typeof(object)], true, null, null, [qExport], [mExport],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qExport : mExport));
        qExport = Matrix(Side.Quire); mExport = Matrix(Side.Mathcad);
        Compare("FixForExport keeps dimensioned matrix", "FixForExport", [typeof(object)], true, null, null, [qExport], [mExport],
            (side, value) => ShowWithSource(side, value, side == Side.Quire ? qExport : mExport));
        Compare("FixForExport passes null", "FixForExport", [typeof(object)], true, null, null, [null], [null]);
        var qNumber = Side.Quire.Num(2, 0, "poly"); var mNumber = Side.Mathcad.Num(2, 0, "poly");
        Compare("FixForExport fixes dimensional value", "FixForExport", [typeof(object)], true, null, null, [qNumber], [mNumber]);
        qNumber = Side.Quire.Num(2, 0, "L"); mNumber = Side.Mathcad.Num(2, 0, "L");
        Compare("FixForExport dimensional value with dimension", "FixForExport", [typeof(object)], true, null, null, [qNumber], [mNumber]);
    }

    private static string ShowWithSource(Side side, object? result, object source)
    {
        string aliases = "same=" + ReferenceEquals(result, source);
        if (result?.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true &&
            source.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
        {
            aliases += " real=" + ReferenceEquals(Get(result, "m_rarr"), Get(source, "m_rarr"));
            aliases += " imag=" + ReferenceEquals(Get(result, "m_iarr"), Get(source, "m_iarr"));
            aliases += " ptrs=" + ReferenceEquals(Get(result, "m_parr"), Get(source, "m_parr"));
        }
        return Show(side, result) + " " + aliases + " source=" + Storage(side, source);
    }

    private static string ShowNested(Side side, object? value) =>
        value?.GetType().Name == "TrimSubMatrixPtrs" ? NestedState(side, value) : Show(side, value);

    private static string NestedState(Side side, object callback) =>
        $"dest={Storage(side, Get(callback, "m_m")!)} bounds={Get(callback, "m_r1")},{Get(callback, "m_r2")},{Get(callback, "m_c1")},{Get(callback, "m_c2")}";
}

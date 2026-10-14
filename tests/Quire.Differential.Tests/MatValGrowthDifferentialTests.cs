using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValGrowthDifferentialTests
{
    private const string Contract = "Mcd.MatVal.Growth";
    private const BindingFlags PublicDeclared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags StaticDeclared = PublicDeclared | BindingFlags.Static;
    private const BindingFlags InstanceDeclared = PublicDeclared | BindingFlags.Instance;
    private static System.Type QMat => typeof(global::Quire.Mcd.MatVal);
    private static System.Type MMat => Mathcad.Type("mplrun", "Mcd.MatVal");

    private static System.Type MapToM(System.Type type)
    {
        if (type == QMat) return MMat;
        if (type == typeof(global::Quire.Mcd.NumVal)) return Mathcad.Type("mplrun", "Mcd.NumVal");
        if (type == typeof(global::Quire.Mcd.DimVal)) return Mathcad.Type("mplrun", "Mcd.DimVal");
        return type;
    }

    private static MethodInfo Method(Side side, string name, bool isStatic, params System.Type[] parameters)
    {
        var type = side == Side.Quire ? QMat : MMat;
        var flags = isStatic ? StaticDeclared : InstanceDeclared;
        var exactParameters = side == Side.Quire ? parameters : parameters.Select(MapToM).ToArray();
        var method = type.GetMethod(name, flags, exactParameters);
        return method ?? throw new HarnessException($"Missing {side.Name} method {name}({string.Join(", ", parameters.Select(p => p.Name))})");
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
            return MatValDifferentialTests.Storage(side, value);
        return side.Show(value);
    }

    private static Func<object?, string> Result(Side side) => value => Show(side, value);
    private static Func<object, string> State(Side side) => value => MatValDifferentialTests.Storage(side, value);

    private static void CompareStatic(string caseId, string name, System.Type[] parameterTypes,
        object?[] qArgs, object?[] mArgs)
    {
        MethodComparison.Compare(caseId, Contract, Method(Side.Quire, name, true, parameterTypes), null, qArgs,
            Method(Side.Mathcad, name, true, parameterTypes), null, mArgs,
            Result(Side.Quire), Result(Side.Mathcad));
    }

    private static void CompareInstance(string caseId, string name, System.Type[] parameterTypes,
        object qTarget, object mTarget, object?[] qArgs, object?[] mArgs)
    {
        MethodComparison.Compare(caseId, Contract, Method(Side.Quire, name, false, parameterTypes), qTarget, qArgs,
            Method(Side.Mathcad, name, false, parameterTypes), mTarget, mArgs,
            Result(Side.Quire), Result(Side.Mathcad), State(Side.Quire), State(Side.Mathcad));
    }

    private static object Matrix(Side side, params object?[] args) => New(side == Side.Quire ? QMat : MMat, args);
    private static object Number(Side side, double real, double imag = 0) => New(side.T("Mcd.NumVal"), real, imag);
    private static void SetFlagPair(object q, object m, string name, bool value)
    {
        Set(q, name, value);
        Set(m, name, value);
    }

    [SkippableFact]
    public void Growth_capacity_and_copy_on_write()
    {
        Runtime.Sync();
        var qVector = Matrix(Side.Quire, new double[] { 1, 2 });
        var mVector = Matrix(Side.Mathcad, new double[] { 1, 2 });
        CompareInstance("mcd_set_1 vector growth", "mcd_set_1", [typeof(int), typeof(object)], qVector, mVector,
            [2, Number(Side.Quire, 3)], [2, Number(Side.Mathcad, 3)]);

        var qShared = Matrix(Side.Quire, new double[] { 1, 2 });
        var mShared = Matrix(Side.Mathcad, new double[] { 1, 2 });
        SetFlagPair(qShared, mShared, "Immutable", true);
        SetFlagPair(qShared, mShared, "Shared", true);
        CompareInstance("mcd_set_1 immutable shared growth", "mcd_set_1", [typeof(int), typeof(object)], qShared, mShared,
            [4, Number(Side.Quire, 5)], [4, Number(Side.Mathcad, 5)]);

        var qUpdate = Matrix(Side.Quire, new double[] { 1, 2 });
        var mUpdate = Matrix(Side.Mathcad, new double[] { 1, 2 });
        SetFlagPair(qUpdate, mUpdate, "Immutable", true);
        CompareInstance("mcd_upd_1 double ignores immutable", "mcd_upd_1", [typeof(int), typeof(double)], qUpdate, mUpdate,
            [0, 9.0], [0, 9.0]);

        var qRows = Matrix(Side.Quire, 2, 2);
        var mRows = Matrix(Side.Mathcad, 2, 2);
        Call(qRows, "set_ptrs_2", 1, 1, "pointer");
        Call(mRows, "set_ptrs_2", 1, 1, "pointer");
        CompareInstance("mcd_set_2 row growth", "mcd_set_2", [typeof(int), typeof(int), typeof(object)], qRows, mRows,
            [2, 1, Number(Side.Quire, 7)], [2, 1, Number(Side.Mathcad, 7)]);
        CompareInstance("mcd_set_2 column growth", "mcd_set_2", [typeof(int), typeof(int), typeof(object)], qRows, mRows,
            [3, 2, Number(Side.Quire, 8)], [3, 2, Number(Side.Mathcad, 8)]);

        var qCapacity = Matrix(Side.Quire, 2, 2);
        var mCapacity = Matrix(Side.Mathcad, 2, 2);
        CompareStatic("realloc existing matrix", "realloc", [typeof(object), typeof(int), typeof(int)], [qCapacity, 5, 4], [mCapacity, 5, 4]);
        CompareStatic("realloc null input", "realloc", [typeof(object), typeof(int), typeof(int)], [null, -1, -1], [null, -1, -1]);

        foreach (var dimension in Side.DimNames)
        {
            var q = Matrix(Side.Quire, 1, 1);
            var m = Matrix(Side.Mathcad, 1, 1);
            Set(q, "m_dim", Side.Quire.D(dimension));
            Set(m, "m_dim", Side.Mathcad.D(dimension));
            Call(q, "set_ptrs_2", 0, 0, "pointer");
            Call(m, "set_ptrs_2", 0, 0, "pointer");
            CompareInstance("extend_2 unit growth " + dimension, "extend_2", [typeof(int), typeof(int)], q, m, [1, 1], [1, 1]);

            var qBadRow = Matrix(Side.Quire, 1, 1);
            var mBadRow = Matrix(Side.Mathcad, 1, 1);
            CompareInstance("extend_2 rejects negative row " + dimension, "extend_2", [typeof(int), typeof(int)], qBadRow, mBadRow,
                [-1, 0], [-1, 0]);
            CompareInstance("extend_2 rejects negative column " + dimension, "extend_2", [typeof(int), typeof(int)], qBadRow, mBadRow,
                [0, -1], [0, -1]);
            var qImmutable = Matrix(Side.Quire, 1, 1);
            var mImmutable = Matrix(Side.Mathcad, 1, 1);
            SetFlagPair(qImmutable, mImmutable, "Immutable", true);
            CompareInstance("extend_2 immutable copy " + dimension, "extend_2", [typeof(int), typeof(int)], qImmutable, mImmutable,
                [1, 1], [1, 1]);
        }
    }

    [SkippableFact]
    public void Assignment_blocks_columns_and_errors()
    {
        Runtime.Sync();
        foreach (var dimension in Side.DimNames)
        {
            var qSource = Matrix(Side.Quire, new double[] { 1, 2, 3, 4 }, new double[] { 5, 6, 7, 8 }, 2, 2);
            var mSource = Matrix(Side.Mathcad, new double[] { 1, 2, 3, 4 }, new double[] { 5, 6, 7, 8 }, 2, 2);
            Set(qSource, "m_dim", Side.Quire.D(dimension));
            Set(mSource, "m_dim", Side.Mathcad.D(dimension));
            Call(qSource, "set_ptrs_2", 1, 1, "nested");
            Call(mSource, "set_ptrs_2", 1, 1, "nested");

            var qDestination = Matrix(Side.Quire, 1, 1);
            var mDestination = Matrix(Side.Mathcad, 1, 1);
            CompareInstance("setblock " + dimension, "setblock", [typeof(int), typeof(int), typeof(object)], qDestination, mDestination,
                [1, 1, qSource], [1, 1, mSource]);
            CompareInstance("setcolumn " + dimension, "setcolumn", [typeof(int), typeof(object)], qDestination, mDestination,
                [1, Matrix(Side.Quire, new double[] { 10, 11 })], [1, Matrix(Side.Mathcad, new double[] { 10, 11 })]);
            CompareStatic("mcd_setrow " + dimension, "mcd_setrow", [typeof(object), typeof(int), typeof(object)],
                [null, 1, qSource], [null, 1, mSource]);
        }

        var qScalar = Number(Side.Quire, 6);
        var mScalar = Number(Side.Mathcad, 6);
        var qImmutable = Matrix(Side.Quire, 1, 1);
        var mImmutable = Matrix(Side.Mathcad, 1, 1);
        SetFlagPair(qImmutable, mImmutable, "Immutable", true);
        CompareStatic("mcd_setcolumn immutable scalar destination", "mcd_setcolumn", [typeof(object), typeof(int), typeof(object)],
            [qImmutable, 2, qScalar], [mImmutable, 2, mScalar]);

        var qFill = Matrix(Side.Quire, new double[] { 2, 3 });
        var mFill = Matrix(Side.Mathcad, new double[] { 2, 3 });
        CompareInstance("StretchWithColumnFill fills new columns", "StretchWithColumnFill", [typeof(int), typeof(int), typeof(global::Quire.Mcd.DimVal)],
            qFill, mFill, [4, 3, qScalar], [4, 3, mScalar]);

        var qBadVector = Matrix(Side.Quire, 2, 2);
        var mBadVector = Matrix(Side.Mathcad, 2, 2);
        CompareInstance("extend_1 rejects multicolumn matrix", "extend_1", [typeof(int)], qBadVector, mBadVector, [0], [0]);
        var qBadRow = Matrix(Side.Quire, 1, 1);
        var mBadRow = Matrix(Side.Mathcad, 1, 1);
        CompareInstance("mcd_set_1 rejects negative index", "mcd_set_1", [typeof(int), typeof(object)], qBadRow, mBadRow,
            [-1, qScalar], [-1, mScalar]);
        var qBadColumn = Matrix(Side.Quire, 1, 1);
        var mBadColumn = Matrix(Side.Mathcad, 1, 1);
        CompareInstance("mcd_set_2 rejects negative index", "mcd_set_2", [typeof(int), typeof(int), typeof(object)], qBadColumn, mBadColumn,
            [0, -1, qScalar], [0, -1, mScalar]);
        CompareInstance("mcd_set_2 rejects negative row", "mcd_set_2", [typeof(int), typeof(int), typeof(object)], qBadColumn, mBadColumn,
            [-1, 0, qScalar], [-1, 0, mScalar]);
        var qImmutableMatrix = Matrix(Side.Quire, 1, 1);
        var mImmutableMatrix = Matrix(Side.Mathcad, 1, 1);
        SetFlagPair(qImmutableMatrix, mImmutableMatrix, "Immutable", true);
        CompareInstance("mcd_set_2 immutable copy", "mcd_set_2", [typeof(int), typeof(int), typeof(object)], qImmutableMatrix, mImmutableMatrix,
            [1, 1, qScalar], [1, 1, mScalar]);
        CompareInstance("setcolumn rejects multicolumn source", "setcolumn", [typeof(int), typeof(object)], qBadColumn, mBadColumn,
            [0, Matrix(Side.Quire, 1, 2)], [0, Matrix(Side.Mathcad, 1, 2)]);
    }
}

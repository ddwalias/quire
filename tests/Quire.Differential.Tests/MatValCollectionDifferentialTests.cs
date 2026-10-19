using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValCollectionDifferentialTests
{
    private const string Contract = "Mcd.MatVal.Collections";
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
        if (type == typeof(global::Quire.Mcd.Dim)) return Mathcad.Type("mplrun", "Mcd.Dim");
        if (type == typeof(global::Quire.Mpl.Run.Function)) return Mathcad.Type("mplrun", "Mpl.Run.Function");
        return type;
    }

    private static (object q, object m) FunctionPair(Delegate fn, int arity)
    {
        var qCtor = typeof(global::Quire.Mpl.Run.DelegateFunction).GetConstructor(PublicDeclared | BindingFlags.Instance, null, [typeof(Delegate), typeof(int)], null)!;
        var mType = Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");
        var mCtor = mType.GetConstructor(PublicDeclared | BindingFlags.Instance, null, [typeof(Delegate), typeof(int)], null)!;
        return (qCtor.Invoke([fn, arity]), mCtor.Invoke([fn, arity]));
    }

    private static (object q, object m) ConsPair(object? qHead, object? qTail, object? mHead, object? mTail)
    {
        var qCons = new global::Quire.Mpl.Run.Cons(qHead, qTail);
        var mCons = Activator.CreateInstance(Mathcad.Type("mplrun", "Mpl.Run.Cons"), mHead, mTail)!;
        return (qCons, mCons);
    }

    private static MethodInfo Method(Side side, string name, bool isStatic, params System.Type[] parameters)
    {
        var type = side == Side.Quire ? QMat : MMat;
        var flags = isStatic ? StaticDeclared : InstanceDeclared;
        var paramTypes = side == Side.Quire ? parameters : parameters.Select(MapToM).ToArray();
        var method = type.GetMethod(name, flags, paramTypes);
        return method ?? throw new HarnessException($"Missing {side.Name} method {name}({string.Join(", ", parameters.Select(p => p.Name))})");
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
            return MatValDifferentialTests.Storage(side, value);
        if (value is object[] arr)
            return "[" + string.Join(", ", arr.Select(x => Show(side, x))) + "]";
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

    private static object Reserve(Side side, int capacity) => Call(side.T("Mcd.MatVal"), "reserve", capacity)!;

    [SkippableFact]
    public void Construction_push_shape_and_fill()
    {
        Runtime.Sync();
        foreach (string method in new[] { "MakeVector", "MakeVector0" })
        {
            var qValues = new object[] { Side.Quire.Num(0, 0, "poly"), Side.Quire.Num(2, 3, "L"), Side.Quire.Num(double.NaN, 0, "identity") };
            var mValues = new object[] { Side.Mathcad.Num(0, 0, "poly"), Side.Mathcad.Num(2, 3, "L"), Side.Mathcad.Num(double.NaN, 0, "identity") };
            CompareStatic(method + " object[]", method, [typeof(object[])], [qValues], [mValues]);
        }
        CompareStatic("MakeVector0 exact Zero singleton", "MakeVector0", [typeof(object[])],
            [new object?[] { Get(Side.Quire.T("Mcd.NumVal"), "Zero") }],
            [new object?[] { Get(Side.Mathcad.T("Mcd.NumVal"), "Zero") }]);

        foreach (string dimension in new[] { "poly", "identity", "L" })
            CompareStatic("Replicate " + dimension, "Replicate", [typeof(object), typeof(int)],
                [Side.Quire.Num(3, 4, dimension), 3], [Side.Mathcad.Num(3, 4, dimension), 3]);

        CompareStatic("reserve(int)", "reserve", [typeof(int)], [4], [4]);
        var q = Reserve(Side.Quire, 4);
        var m = Reserve(Side.Mathcad, 4);
        CompareInstance("PushRaw(double)", "PushRaw", [typeof(double)], q, m, [4.0], [4.0]);
        CompareInstance("PushReal(double)", "PushReal", [typeof(double)], q, m, [5.0], [5.0]);
        CompareInstance("PushZero()", "PushZero", [], q, m, [], []);
        CompareInstance("PushRawIndex(object)", "PushRawIndex", [typeof(object)], q, m,
            [Side.Quire.Num(6, 0, "poly")], [Side.Mathcad.Num(6, 0, "poly")]);
        CompareInstance("Reshape(int,int)", "Reshape", [typeof(int), typeof(int)], q, m, [2, 2], [2, 2]);

        var qFill = New(QMat, 5, 1, true);
        var mFill = New(MMat, 5, 1, true);
        CompareInstance("Fill pointer", "Fill", [typeof(object), typeof(int), typeof(int), typeof(int)], qFill, mFill,
            ["pointer", 0, 2, 3], ["pointer", 0, 2, 3]);
        CompareInstance("Fill numeric dimensioned value", "Fill", [typeof(object), typeof(int), typeof(int), typeof(int)], qFill, mFill,
            [Side.Quire.Num(7, 2, "L"), 4, -2, 3], [Side.Mathcad.Num(7, 2, "L"), 4, -2, 3]);
        CompareInstance("Fill polymorphic real", "Fill", [typeof(object), typeof(int), typeof(int), typeof(int)], qFill, mFill,
            [Side.Quire.Num(1, 0, "poly"), 0, 2, 3], [Side.Mathcad.Num(1, 0, "poly"), 0, 2, 3]);

        var qProductExtent = New(QMat, new double[2], new double[2], 2, 1, 2,
            new System.Collections.Hashtable(), Enum.ToObject(Side.Quire.T("Mcd.Flags"), 2146697216), 0);
        var mProductExtent = New(MMat, new double[2], new double[2], 2, 1, 2,
            new System.Collections.Hashtable(), Enum.ToObject(Side.Mathcad.T("Mcd.Flags"), 2146697216), 0);
        // The final write is at index 8. Division instead of multiplication computes extent 3.
        CompareInstance("Fill product determines extent", "Fill", [typeof(object), typeof(int), typeof(int), typeof(int)],
            qProductExtent, mProductExtent, ["fill", 0, 4, 3], ["fill", 0, 4, 3]);

        var qImmutable = New(QMat, 5, 1, true);
        var mImmutable = New(MMat, 5, 1, true);
        Set(qImmutable, "Immutable", true);
        Set(mImmutable, "Immutable", true);
        CompareInstance("Fill immutable target returns copy", "Fill", [typeof(object), typeof(int), typeof(int), typeof(int)],
            qImmutable, mImmutable, [Side.Quire.Num(9, 0, "poly"), 6, 1, 2], [Side.Mathcad.Num(9, 0, "poly"), 6, 1, 2]);
    }

    [SkippableFact]
    public void Collection_factories_push_and_shape()
    {
        Runtime.Sync();
        CompareStatic("reserve(2, 3)", "reserve", [typeof(int), typeof(int)], [2, 3], [2, 3]);

        CompareStatic("Singleton string", "Singleton", [typeof(object)], ["item"], ["item"]);
        CompareStatic("Singleton num", "Singleton", [typeof(object)], [Side.Quire.Num(3, 0, "poly")], [Side.Mathcad.Num(3, 0, "poly")]);

        CompareStatic("Pair strings", "Pair", [typeof(object), typeof(object)], ["a", "b"], ["a", "b"]);
        CompareStatic("Pair nums", "Pair", [typeof(object), typeof(object)],
            [Side.Quire.Num(1, 0, "poly"), Side.Quire.Num(2, 0, "poly")],
            [Side.Mathcad.Num(1, 0, "poly"), Side.Mathcad.Num(2, 0, "poly")]);

        CompareStatic("Triple strings", "Triple", [typeof(object), typeof(object), typeof(object)], ["a", "b", "c"], ["a", "b", "c"]);

        var q = Reserve(Side.Quire, 4);
        var m = Reserve(Side.Mathcad, 4);
        CompareInstance("Push(object) 1", "Push", [typeof(object)], q, m, ["first"], ["first"]);
        CompareInstance("Push(object) 2", "Push", [typeof(object)], q, m,
            [Side.Quire.Num(10, 0, "poly")], [Side.Mathcad.Num(10, 0, "poly")]);
        CompareInstance("PushRaw1", "PushRaw1", [typeof(object)], q, m, ["raw1"], ["raw1"]);

        var q2 = Reserve(Side.Quire, 3);
        var m2 = Reserve(Side.Mathcad, 3);
        CompareInstance("PushRaw(object)", "PushRaw", [typeof(object)], q2, m2, ["raw_obj"], ["raw_obj"]);
        CompareInstance("PushRaw(int)", "PushRaw", [typeof(int)], q2, m2, [42], [42]);
        CompareInstance("SetShape1", "SetShape1", [], q2, m2, [], []);

        var (qCons1, mCons1) = ConsPair(Side.Quire.Num(1, 0, "poly"), null, Side.Mathcad.Num(1, 0, "poly"), null);
        var (qCons2, mCons2) = ConsPair(Side.Quire.Num(2, 0, "poly"), qCons1, Side.Mathcad.Num(2, 0, "poly"), mCons1);
        var qVec = Reserve(Side.Quire, 2);
        var mVec = Reserve(Side.Mathcad, 2);
        CompareInstance("PushVectorArg valid", "PushVectorArg", [typeof(object)], qVec, mVec, [qCons2], [mCons2]);
        CompareInstance("SetShape", "SetShape", [], qVec, mVec, [], []);

        var qVecErr = Reserve(Side.Quire, 2);
        var mVecErr = Reserve(Side.Mathcad, 2);
        CompareInstance("PushVectorArg non-cons raises", "PushVectorArg", [typeof(object)], qVecErr, mVecErr, ["not-a-cons"], ["not-a-cons"]);
    }

    [SkippableFact]
    public void Ranges_map_and_vectorize()
    {
        Runtime.Sync();
        CompareStatic("mkRange2 scalars", "mkRange2", [typeof(object), typeof(object)],
            [Side.Quire.Num(1, 0, "poly"), Side.Quire.Num(5, 0, "poly")],
            [Side.Mathcad.Num(1, 0, "poly"), Side.Mathcad.Num(5, 0, "poly")]);
        CompareStatic("mkRange2 dimensioned", "mkRange2", [typeof(object), typeof(object)],
            [Side.Quire.Num(2, 0, "L"), Side.Quire.Num(10, 0, "L")],
            [Side.Mathcad.Num(2, 0, "L"), Side.Mathcad.Num(10, 0, "L")]);

        CompareStatic("mkRange3 scalars", "mkRange3", [typeof(object), typeof(object), typeof(object)],
            [Side.Quire.Num(1, 0, "poly"), Side.Quire.Num(3, 0, "poly"), Side.Quire.Num(9, 0, "poly")],
            [Side.Mathcad.Num(1, 0, "poly"), Side.Mathcad.Num(3, 0, "poly"), Side.Mathcad.Num(9, 0, "poly")]);

        var qVecObj = Call(Side.Quire.T("Mcd.MatVal"), "MakeVector", [new object[] { Side.Quire.Num(1, 0, "poly"), Side.Quire.Num(2, 0, "poly") }])!;
        var mVecObj = Call(Side.Mathcad.T("Mcd.MatVal"), "MakeVector", [new object[] { Side.Mathcad.Num(1, 0, "poly"), Side.Mathcad.Num(2, 0, "poly") }])!;
        CompareStatic("as_object_vector", "as_object_vector", [typeof(global::Quire.Mcd.DimVal)], [qVecObj], [mVecObj]);

        Func<object, object> doubleFn = x => $"{x}_mapped";
        var (qFn1, mFn1) = FunctionPair(doubleFn, 1);
        CompareStatic("map unary Function", "map", [typeof(global::Quire.Mpl.Run.Function), typeof(global::Quire.Mcd.DimVal)],
            [qFn1, qVecObj], [mFn1, mVecObj]);

        Func<object, object, object> pairFn = (x, y) => $"{x}+{y}";
        var (qFn2, mFn2) = FunctionPair(pairFn, 2);
        CompareStatic("map binary Function", "map",
            [typeof(global::Quire.Mpl.Run.Function), typeof(global::Quire.Mcd.DimVal), typeof(global::Quire.Mcd.DimVal)],
            [qFn2, qVecObj, qVecObj], [mFn2, mVecObj, mVecObj]);

        // vectorize tests
        // Case A: scalar only
        CompareStatic("vectorize scalar only", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFn2, new object[] { "hello", "world" }], [mFn2, new object[] { "hello", "world" }]);

        // Case B: matching 2x2 matrices
        var qMat1 = New(QMat, 2, 2, true);
        var mMat1 = New(MMat, 2, 2, true);
        Call(qMat1, "Fill", "qA", 0, 1, 4);
        Call(mMat1, "Fill", "qA", 0, 1, 4);
        var qMat2 = New(QMat, 2, 2, true);
        var mMat2 = New(MMat, 2, 2, true);
        Call(qMat2, "Fill", "qB", 0, 1, 4);
        Call(mMat2, "Fill", "qB", 0, 1, 4);
        CompareStatic("vectorize matching 2x2", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFn2, new object[] { qMat1, qMat2 }], [mFn2, new object[] { mMat1, mMat2 }]);

        // Case C: unequal matrix dimensions (2x2 vs 3x2) -> throws "unequal matrix dimensions"
        var qMat3 = New(QMat, 3, 2, true);
        var mMat3 = New(MMat, 3, 2, true);
        Call(qMat3, "Fill", "qC", 0, 1, 6);
        Call(mMat3, "Fill", "qC", 0, 1, 6);
        CompareStatic("vectorize unequal dimensions raises", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFn2, new object[] { qMat1, qMat3 }], [mFn2, new object[] { mMat1, mMat3 }]);

        // Case D: matrix and scalar mixed
        CompareStatic("vectorize matrix and scalar", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFn2, new object[] { qMat1, "scalar" }], [mFn2, new object[] { mMat1, "scalar" }]);

        // Case E: arity mismatch
        CompareStatic("vectorize arity mismatch raises", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFn2, new object[] { qMat1 }], [mFn2, new object[] { mMat1 }]);

        // Case F: non-polymorphic input promoted to polymorphic result with identity dim
        var qReal = New(QMat, 2, 2, false);
        var mReal = New(MMat, 2, 2, false);
        Set(qReal, "m_dim", Get(Side.Quire.T("Mcd.Dim"), "identity"));
        Set(mReal, "m_dim", Get(Side.Mathcad.T("Mcd.Dim"), "identity"));
        CallSig(qReal, "set_2", ["Int32", "Int32", "Double"], 0, 0, 1.0); CallSig(mReal, "set_2", ["Int32", "Int32", "Double"], 0, 0, 1.0);
        CallSig(qReal, "set_2", ["Int32", "Int32", "Double"], 0, 1, 2.0); CallSig(mReal, "set_2", ["Int32", "Int32", "Double"], 0, 1, 2.0);
        CallSig(qReal, "set_2", ["Int32", "Int32", "Double"], 1, 0, 3.0); CallSig(mReal, "set_2", ["Int32", "Int32", "Double"], 1, 0, 3.0);
        CallSig(qReal, "set_2", ["Int32", "Int32", "Double"], 1, 1, 4.0); CallSig(mReal, "set_2", ["Int32", "Int32", "Double"], 1, 1, 4.0);
        Func<object, object> toStr = x => $"{x}_str";
        var (qFnStr, mFnStr) = FunctionPair(toStr, 1);
        CompareStatic("vectorize non-polymorphic to polymorphic", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFnStr, new object[] { qReal }], [mFnStr, new object[] { mReal }]);

        // Case G: non-polymorphic input to non-polymorphic result
        Func<object, object> addOne = x => Convert.ToDouble(x) + 1.0;
        var (qFnNum, mFnNum) = FunctionPair(addOne, 1);
        CompareStatic("vectorize non-polymorphic to non-polymorphic", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFnNum, new object[] { qReal }], [mFnNum, new object[] { mReal }]);

        // Case H: empty 0x0 matrix
        var qEmpty = New(QMat, 0, 0, true);
        var mEmpty = New(MMat, 0, 0, true);
        CompareStatic("vectorize empty 0x0 matrix", "vectorize",
            [typeof(global::Quire.Mpl.Run.Function), typeof(object[])],
            [qFnStr, new object[] { qEmpty }], [mFnStr, new object[] { mEmpty }]);
    }
}

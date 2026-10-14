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
        if (type == typeof(global::Quire.Mcd.Dim)) return Mathcad.Type("mplrun", "Mcd.Dim");
        return type;
    }

    private static MethodInfo Method(Side side, string name, bool isStatic, params System.Type[] parameters)
    {
        var type = side == Side.Quire ? QMat : MMat;
        var flags = isStatic ? StaticDeclared : InstanceDeclared;
        var method = type.GetMethod(name, flags, parameters.Select(MapToM).ToArray());
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
}

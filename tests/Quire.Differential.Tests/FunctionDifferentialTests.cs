using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class FunctionDifferentialTests
{
    private const string FunctionContract = "Mpl.Run.Function";
    private const string UtilityContract = "Mpl.Run.FunctionUtilities";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static Type QFunction => typeof(Function);
    private static Type MFunction => Mathcad.Type("mplrun", "Mpl.Run.Function");
    private static Type QUtilities => typeof(FunctionUtilities);
    private static Type MUtilities => Mathcad.Type("mplrun", "Mpl.Run.FunctionUtilities");
    private static string Show(object? value) => MplRunSides.Describe(value);

    private static (MethodInfo q, MethodInfo m) StaticMethod(Type qt, Type mt, string name, params Type[] parameters) =>
        (qt.GetMethod(name, Static, null, parameters, null) ?? throw new HarnessException($"Missing Quire {qt.Name}.{name}"),
         mt.GetMethod(name, Static, null, parameters, null) ?? throw new HarnessException($"Missing Mathcad {mt.Name}.{name}"));

    private static (MethodInfo q, MethodInfo m) InstanceMethod(Type qt, Type mt, string name, params Type[] parameters) =>
        (qt.GetMethod(name, Instance, null, parameters, null) ?? throw new HarnessException($"Missing Quire {qt.Name}.{name}"),
         mt.GetMethod(name, Instance, null, parameters, null) ?? throw new HarnessException($"Missing Mathcad {mt.Name}.{name}"));

    private static (object q, object m) FunctionPair(int purity)
    {
        var qCtor = typeof(DelegateFunction).GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)!;
        var mType = Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");
        var mCtor = mType.GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)!;
        Func<object, object> unary = Identity;
        Func<object, object, object> binary = Pair;
        Delegate qDelegate = purity == 2 ? binary : unary;
        Delegate mDelegate = purity == 2 ? binary : unary;
        return (qCtor.Invoke([qDelegate, purity]), mCtor.Invoke([mDelegate, purity]));
    }

    private static object Identity(object value) => value;
    private static object Pair(object first, object second) => $"{first ?? "null"}|{second ?? "null"}";

    [SkippableFact]
    public void FunctionUtilities_array_operations_and_caches()
    {
        Assert.Equal(16, FunctionUtilities.maxargs);
        Assert.Empty(FunctionUtilities.empty_vec);
        var qs = Show;
        var ms = Show;

        var (qConcat, mConcat) = StaticMethod(QUtilities, MUtilities, "concat", typeof(object[]), typeof(object[]));
        foreach (var pair in new[] { (new object[] { 1, 2 }, new object[] { 3, 4, 5 }), (new object[] { 1, 2 }, Array.Empty<object>()) })
            MethodComparison.Compare("concat", UtilityContract, qConcat, null, [pair.Item1, pair.Item2], mConcat, null, [pair.Item1, pair.Item2], qs, ms);

        var qTake = QUtilities.GetMethod("take", Static)!.MakeGenericMethod(typeof(int));
        var mTake = MUtilities.GetMethod("take", Static)!.MakeGenericMethod(typeof(int));
        var qBut = QUtilities.GetMethod("but", Static)!.MakeGenericMethod(typeof(int));
        var mBut = MUtilities.GetMethod("but", Static)!.MakeGenericMethod(typeof(int));
        int[] numbers = [10, 20, 30, 40];
        foreach (int count in new[] { 2, 10 })
        {
            MethodComparison.Compare($"take({count})", UtilityContract, qTake, null, [count, numbers], mTake, null, [count, numbers], qs, ms);
            MethodComparison.Compare($"but({count})", UtilityContract, qBut, null, [count, numbers], mBut, null, [count, numbers], qs, ms);
        }

        foreach (string name in new[] { "PushFront", "Push" })
        {
            Type[][] signatures = name == "PushFront"
                ? new[] { new[] { typeof(object), typeof(object[]) }, new[] { typeof(object), typeof(object), typeof(object[]) } }
                : new[] { new[] { typeof(object[]), typeof(object) }, new[] { typeof(object[]), typeof(object), typeof(object) } };
            foreach (Type[] signature in signatures)
            {
                var (q, m) = StaticMethod(QUtilities, MUtilities, name, signature);
                object?[] args = name == "PushFront"
                    ? signature.Length == 2 ? [0, new object[] { 1, 2 }] : [-1, 0, new object[] { 1, 2 }]
                    : signature.Length == 2 ? [new object[] { 1, 2 }, 99] : [new object[] { 1, 2 }, 98, 99];
                MethodComparison.Compare($"{name}/{signature.Length}", UtilityContract, q, null, args, m, null, args, qs, ms);
            }
        }

        var (qCloCtor, mCloCtor) = StaticMethod(QUtilities, MUtilities, "CloCtor", typeof(System.Type));
        MethodComparison.Compare("CloCtor(typeof(int))", UtilityContract, qCloCtor, null, [typeof(int)], mCloCtor, null, [typeof(int)], qs, ms);
        var (qInstMethCtor, mInstMethCtor) = StaticMethod(QUtilities, MUtilities, "InstMethCtor", typeof(System.Type));
        MethodComparison.Compare("InstMethCtor(typeof(object))", UtilityContract, qInstMethCtor, null, [typeof(object)], mInstMethCtor, null, [typeof(object)], qs, ms);
        var (qGetCall, mGetCall) = StaticMethod(QUtilities, MUtilities, "GetCallMethod", typeof(System.Type), typeof(System.Type[]));
        MethodComparison.Compare("GetCallMethod(arity 1)", UtilityContract, qGetCall, null, [typeof(object), new[] { typeof(object) }], mGetCall, null, [typeof(object), new[] { typeof(object) }], qs, ms);
        MethodComparison.Compare("GetCallMethod(arity 17 vector fallback)", UtilityContract, qGetCall, null, [typeof(object), Enumerable.Repeat(typeof(object), 17).ToArray()], mGetCall, null, [typeof(object), Enumerable.Repeat(typeof(object), 17).ToArray()], qs, ms);
    }

    [SkippableFact]
    public void Function_formatting_identity_and_inverse_links()
    {
        var qs = Show;
        var ms = Show;
        var (qFormat, mFormat) = StaticMethod(QFunction, MFunction, "format_vector", typeof(object[]));
        foreach (object[]? values in new object[]?[] { [], null, [42], [1, "abc", 3.5], [null, "x"], [1, null] })
            MethodComparison.Compare("format_vector", FunctionContract, qFormat, null, [values], mFormat, null, [values], qs, ms);

        var (qCanonical, mCanonical) = StaticMethod(QFunction, MFunction, "canonical_name", typeof(string));
        foreach (string? name in new string?[] { "simple", "mc_foo", "ui_button", "f#x#y", null })
            MethodComparison.Compare($"canonical_name({name ?? "null"})", FunctionContract, qCanonical, null, [name], mCanonical, null, [name], qs, ms);

        var (qToString, mToString) = InstanceMethod(QFunction, MFunction, "ToString");
        var (qName, mName) = InstanceMethod(QFunction, MFunction, "set_Name", typeof(string));
        var (qGetName, mGetName) = InstanceMethod(typeof(DelegateFunction), Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction"), "get_Name");
        var (qNamedFunction, mNamedFunction) = FunctionPair(1);
        MethodComparison.Compare("DelegateFunction.get_Name canonical fallback", FunctionContract,
            qGetName, qNamedFunction, [], mGetName, mNamedFunction, [], qs, ms, qs, ms);
        MethodComparison.Compare("ToString canonical name", FunctionContract,
            qToString, qNamedFunction, [], mToString, mNamedFunction, [], qs, ms, qs, ms);
        MethodComparison.Compare("set_Name", FunctionContract, qName, qNamedFunction, ["named"], mName, mNamedFunction, ["named"], qs, ms, qs, ms);
        MethodComparison.Compare("DelegateFunction.get_Name explicit value", FunctionContract,
            qGetName, qNamedFunction, [], mGetName, mNamedFunction, [], qs, ms, qs, ms);
        MethodComparison.Compare("ToString explicit value", FunctionContract,
            qToString, qNamedFunction, [], mToString, mNamedFunction, [], qs, ms, qs, ms);

        var qDeclare = QFunction.GetMethod("DeclareInverse", Instance, null, [QFunction], null)!;
        var mDeclare = MFunction.GetMethod("DeclareInverse", Instance, null, [MFunction], null)!;
        var (qOriginal, mOriginal) = FunctionPair(1);
        var (qInverse, mInverse) = FunctionPair(1);
        MethodComparison.Compare("DeclareInverse reciprocal links", FunctionContract,
            qDeclare, qOriginal, [qInverse], mDeclare, mOriginal, [mInverse], qs, ms, qs, ms);
    }

    [SkippableFact]
    public void Function_application_optional_padding_and_delegate_projection()
    {
        var qs = Show;
        var ms = Show;
        var (qIgnore, mIgnore) = InstanceMethod(QFunction, MFunction, "IgnoreVectorization", typeof(bool));
        var (qGetPure, mGetPure) = InstanceMethod(QFunction, MFunction, "get_Purity");
        var (qGetMin, mGetMin) = InstanceMethod(QFunction, MFunction, "get_MinArgs");
        var (qGetVarargs, mGetVarargs) = InstanceMethod(QFunction, MFunction, "get_Varargs");
        var (qOptFunc, mOptFunc) = InstanceMethod(QFunction, MFunction, "OptFunc", typeof(int));
        var (qIsFully, mIsFully) = InstanceMethod(QFunction, MFunction, "IsFullyApplied", typeof(int));
        var (qAssert, mAssert) = InstanceMethod(QFunction, MFunction, "AssertFullApp", typeof(int));
        var (qApply, mApply) = InstanceMethod(QFunction, MFunction, "Apply", typeof(object[]));
        var (qVec, mVec) = InstanceMethod(QFunction, MFunction, "VecApply", typeof(object[]));
        var (qDelegate1, mDelegate1) = InstanceMethod(QFunction, MFunction, "AsDelegate_1");
        var (qDelegate2, mDelegate2) = InstanceMethod(QFunction, MFunction, "AsDelegate_2");

        var (qFn, mFn) = FunctionPair(2);
        MethodComparison.Compare("get_Purity", FunctionContract, qGetPure, qFn, [], mGetPure, mFn, [], qs, ms, qs, ms);
        MethodComparison.Compare("get_MinArgs", FunctionContract, qGetMin, qFn, [], mGetMin, mFn, [], qs, ms, qs, ms);
        MethodComparison.Compare("get_Varargs", FunctionContract, qGetVarargs, qFn, [], mGetVarargs, mFn, [], qs, ms, qs, ms);
        MethodComparison.Compare("IgnoreVectorization(true)", FunctionContract, qIgnore, qFn, [true], mIgnore, mFn, [true], qs, ms, qs, ms);
        MethodComparison.Compare("OptFunc(1)", FunctionContract, qOptFunc, qFn, [1], mOptFunc, mFn, [1], qs, ms, qs, ms);
        foreach (int count in new[] { 0, 1, 2, 3 })
        {
            MethodComparison.Compare($"IsFullyApplied({count})", FunctionContract, qIsFully, qFn, [count], mIsFully, mFn, [count], qs, ms, qs, ms);
            MethodComparison.Compare($"AssertFullApp({count})", FunctionContract, qAssert, qFn, [count], mAssert, mFn, [count], qs, ms, qs, ms);
        }
        MethodComparison.Compare("Apply optional padded argument", FunctionContract, qApply, qFn, [new object[] { "x" }], mApply, mFn, [new object[] { "x" }], qs, ms, qs, ms);
        MethodComparison.Compare("Apply full argument vector", FunctionContract, qApply, qFn, [new object[] { "x", "y" }], mApply, mFn, [new object[] { "x", "y" }], qs, ms, qs, ms);
        MethodComparison.Compare("AsDelegate_1 full application", FunctionContract, qDelegate1, qFn, [], mDelegate1, mFn, [], qs, ms, qs, ms);
        MethodComparison.Compare("AsDelegate_2", FunctionContract, qDelegate2, qFn, [], mDelegate2, mFn, [], qs, ms, qs, ms);

        var (qUnary, mUnary) = FunctionPair(1);
        MethodComparison.Compare("VecApply bypass when vectorization ignored", FunctionContract,
            qVec, qUnary, [new object[] { 7 }], mVec, mUnary, [new object[] { 7 }], qs, ms, qs, ms);
    }
}

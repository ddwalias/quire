using System.Reflection;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class FunctionMatrixDifferentialTests
{
    private const string Contract = "Mpl.Run.Function.Matrix";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static System.Type QFunction => typeof(Function);
    private static System.Type MFunction => Mathcad.Type("mplrun", "Mpl.Run.Function");
    private static string Show(object? value) => MplRunSides.Describe(value);

    private static (MethodInfo q, MethodInfo m) Method(string name, params System.Type[] parameters) =>
        (QFunction.GetMethod(name, Instance, null, parameters, null) ?? throw new HarnessException($"Missing Quire Function.{name}"),
         MFunction.GetMethod(name, Instance, null, parameters, null) ?? throw new HarnessException($"Missing Mathcad Function.{name}"));

    private static void SetVarargs(System.Type type, object target, bool value) =>
        type.GetField("m_varargs", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(target, value);

    private static (object q, object m) FunctionPair(int purity)
    {
        var qCtor = typeof(DelegateFunction).GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)!;
        var mType = Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");
        var mCtor = mType.GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)!;
        Delegate d = purity switch
        {
            1 => (Func<object, object>)Identity,
            2 => (Func<object, object, object>)Pair,
            3 => (Func<object, object, object, object>)Triple,
            _ => throw new ArgumentOutOfRangeException(nameof(purity)),
        };
        return (qCtor.Invoke([d, purity]), mCtor.Invoke([d, purity]));
    }

    private static object Identity(object value) => value;
    private static object Pair(object first, object second) => $"{first ?? "null"}|{second ?? "null"}";
    private static object Triple(object first, object second, object third) => $"{first}|{second}|{third}";

    private static (object q, object m) MatrixPair()
    {
        var q = new MatVal(2, 1);
        var mType = Mathcad.Type("mplrun", "Mcd.MatVal");
        var mCtor = mType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int), typeof(int)], null)!;
        return (q, mCtor.Invoke([2, 1]));
    }

    [SkippableFact]
    public void Function_arity_checks_apply_and_delegate_projection()
    {
        var (qFully, mFully) = Method("IsFullyApplied", typeof(int));
        var (qAssert, mAssert) = Method("AssertFullApp", typeof(int));
        var (qApply, mApply) = Method("Apply", typeof(object[]));
        var (qAsOne, mAsOne) = Method("AsDelegate_1");
        var (qAsTwo, mAsTwo) = Method("AsDelegate_2");
        var (qFn, mFn) = FunctionPair(2);
        var qs = Show;
        var ms = Show;

        foreach (int n in new[] { 0, 1, 2, 3 })
        {
            MethodComparison.Compare($"IsFullyApplied({n})", Contract, qFully, qFn, [n], mFully, mFn, [n], qs, ms, qs, ms);
            MethodComparison.Compare($"AssertFullApp({n})", Contract, qAssert, qFn, [n], mAssert, mFn, [n], qs, ms, qs, ms);
        }

        var (qOpt, mOpt) = Method("OptFunc", typeof(int));
        MethodComparison.Compare("OptFunc(1)", Contract, qOpt, qFn, [1], mOpt, mFn, [1], qs, ms, qs, ms);
        MethodComparison.Compare("Apply optional trailing argument", Contract,
            qApply, qFn, [new object[] { "left" }], mApply, mFn, [new object[] { "left" }], qs, ms, qs, ms);
        MethodComparison.Compare("Apply exact arity", Contract,
            qApply, qFn, [new object[] { "left", "right" }], mApply, mFn, [new object[] { "left", "right" }], qs, ms, qs, ms);
        MethodComparison.Compare("AsDelegate_1", Contract, qAsOne, qFn, [], mAsOne, mFn, [], qs, ms, qs, ms);
        MethodComparison.Compare("AsDelegate_2", Contract, qAsTwo, qFn, [], mAsTwo, mFn, [], qs, ms, qs, ms);

        var (qVarargs, mVarargs) = FunctionPair(1);
        SetVarargs(QFunction, qVarargs, true);
        SetVarargs(MFunction, mVarargs, true);
        var (qGetVarargs, mGetVarargs) = Method("get_Varargs");
        MethodComparison.Compare("get_Varargs enabled", Contract,
            qGetVarargs, qVarargs, [], mGetVarargs, mVarargs, [], qs, ms, qs, ms);
        foreach (int count in new[] { -1, 17 })
        {
            MethodComparison.Compare($"IsFullyApplied({count}) varargs", Contract,
                qFully, qVarargs, [count], mFully, mVarargs, [count], qs, ms, qs, ms);
            MethodComparison.Compare($"AssertFullApp({count}) varargs", Contract,
                qAssert, qVarargs, [count], mAssert, mVarargs, [count], qs, ms, qs, ms);
        }
        MethodComparison.Compare("Apply varargs with one value", Contract,
            qApply, qVarargs, [new object[] { "any arity" }], mApply, mVarargs, [new object[] { "any arity" }], qs, ms, qs, ms);
    }

    [SkippableFact]
    public void Function_matrix_call_bridge_vectorizes_only_matrix_inputs()
    {
        var (qVecOne, mVecOne) = Method("VecCall", typeof(object));
        var (qVecTwo, mVecTwo) = Method("VecCall", typeof(object), typeof(object));
        var (qVecThree, mVecThree) = Method("VecCall", typeof(object), typeof(object), typeof(object));
        var (qVecApply, mVecApply) = Method("VecApply", typeof(object[]));
        var (qIgnore, mIgnore) = Method("IgnoreVectorization", typeof(bool));
        var qs = Show;
        var ms = Show;

        var (qUnary, mUnary) = FunctionPair(1);
        var (qMatrix, mMatrix) = MatrixPair();
        MethodComparison.Compare("VecCall unary matrix bridge", Contract,
            qVecOne, qUnary, [qMatrix], mVecOne, mUnary, [mMatrix], qs, ms, qs, ms);
        MethodComparison.Compare("VecCall unary scalar branch", Contract,
            qVecOne, qUnary, ["scalar"], mVecOne, mUnary, ["scalar"], qs, ms, qs, ms);
        MethodComparison.Compare("VecApply matrix bridge", Contract,
            qVecApply, qUnary, [new[] { qMatrix }], mVecApply, mUnary, [new[] { mMatrix }], qs, ms, qs, ms);

        var (qBinary, mBinary) = FunctionPair(2);
        var (qMatrix2, mMatrix2) = MatrixPair();
        MethodComparison.Compare("VecCall binary bridge with second matrix", Contract,
            qVecTwo, qBinary, ["left", qMatrix2], mVecTwo, mBinary, ["left", mMatrix2], qs, ms, qs, ms);
        MethodComparison.Compare("VecCall binary scalar branch", Contract,
            qVecTwo, qBinary, ["left", "right"], mVecTwo, mBinary, ["left", "right"], qs, ms, qs, ms);

        var (qTernary, mTernary) = FunctionPair(3);
        var (qMatrix3, mMatrix3) = MatrixPair();
        MethodComparison.Compare("VecCall ternary bridge with third matrix", Contract,
            qVecThree, qTernary, ["a", "b", qMatrix3], mVecThree, mTernary, ["a", "b", mMatrix3], qs, ms, qs, ms);
        MethodComparison.Compare("VecCall ternary scalar branch", Contract,
            qVecThree, qTernary, ["a", "b", "c"], mVecThree, mTernary, ["a", "b", "c"], qs, ms, qs, ms);

        var (qIgnored, mIgnored) = FunctionPair(1);
        MethodComparison.Compare("IgnoreVectorization(true)", Contract,
            qIgnore, qIgnored, [true], mIgnore, mIgnored, [true], qs, ms, qs, ms);
        var (qIgnoredMatrix, mIgnoredMatrix) = MatrixPair();
        MethodComparison.Compare("VecCall unary matrix bypass after IgnoreVectorization", Contract,
            qVecOne, qIgnored, [qIgnoredMatrix], mVecOne, mIgnored, [mIgnoredMatrix], qs, ms, qs, ms);
        var (qIgnoredApplyMatrix, mIgnoredApplyMatrix) = MatrixPair();
        MethodComparison.Compare("VecApply bypasses matrix bridge after IgnoreVectorization", Contract,
            qVecApply, qIgnored, [new[] { qIgnoredApplyMatrix }], mVecApply, mIgnored, [new[] { mIgnoredApplyMatrix }], qs, ms, qs, ms);

        var (qBinaryIgnored, mBinaryIgnored) = FunctionPair(2);
        MethodComparison.Compare("IgnoreVectorization(true) binary", Contract,
            qIgnore, qBinaryIgnored, [true], mIgnore, mBinaryIgnored, [true], qs, ms, qs, ms);
        var (qIgnoredMatrix2, mIgnoredMatrix2) = MatrixPair();
        MethodComparison.Compare("VecCall binary matrix bypass after IgnoreVectorization", Contract,
            qVecTwo, qBinaryIgnored, ["left", qIgnoredMatrix2], mVecTwo, mBinaryIgnored, ["left", mIgnoredMatrix2], qs, ms, qs, ms);

        var (qTernaryIgnored, mTernaryIgnored) = FunctionPair(3);
        MethodComparison.Compare("IgnoreVectorization(true) ternary", Contract,
            qIgnore, qTernaryIgnored, [true], mIgnore, mTernaryIgnored, [true], qs, ms, qs, ms);
        var (qIgnoredMatrix3, mIgnoredMatrix3) = MatrixPair();
        MethodComparison.Compare("VecCall ternary matrix bypass after IgnoreVectorization", Contract,
            qVecThree, qTernaryIgnored, ["a", "b", qIgnoredMatrix3], mVecThree, mTernaryIgnored, ["a", "b", mIgnoredMatrix3], qs, ms, qs, ms);
    }
}

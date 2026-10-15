using System.Globalization;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;
namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.DimVal</c> base behaviour with Quire's through <c>NotAVal</c>,
/// covering all capability queries, conversion methods, call errors, dynamic dispatch,
/// iteration and matrix methods, and static helpers using direct <see cref="MethodComparison.Compare"/> calls.
/// Cases are listed in docs/mirror/evidence/Mcd.DimVal.md.
/// </summary>
public class DimValDifferentialTests
{
    private const string Contract = "Mcd.DimVal";
    private const BindingFlags DeclaredPublic = BindingFlags.Public | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type M => Mathcad.Type("mplrun", "Mcd.DimVal");
    private static System.Type QVal => typeof(global::Quire.Mcd.DimVal);
    private static System.Type MDim => Mathcad.Type("mplrun", "Mcd.Dim");
    private static System.Type QDim => typeof(global::Quire.Mcd.Dim);

    private static System.Type MapToM(System.Type t)
    {
        if (t == QVal) return M;
        if (t == QDim) return MDim;
        if (t == typeof(global::Quire.Mcd.MatVal)) return Mathcad.Type("mplrun", "Mcd.MatVal");
        if (t == typeof(global::Quire.Mcd.MatDim)) return Mathcad.Type("mplrun", "Mcd.MatDim");
        if (t == typeof(global::Quire.Mcd.Units)) return Mathcad.Type("mplrun", "Mcd.Units");
        if (t == typeof(global::Quire.Mcd.RngVal)) return Mathcad.Type("mplrun", "Mcd.RngVal");
        if (t == typeof(global::Quire.Mpl.Run.Function)) return Mathcad.Type("mplrun", "Mpl.Run.Function");
        return t;
    }

    private static (MethodInfo q, MethodInfo m) Method(string name, BindingFlags flags, params System.Type[] qParamTypes)
    {
        var mParamTypes = qParamTypes.Select(MapToM).ToArray();
        var q = QVal.GetMethod(name, flags, qParamTypes)
            ?? throw new HarnessException($"Quire method not found: {name}({string.Join(", ", qParamTypes.Select(t => t.Name))})");
        var m = M.GetMethod(name, flags, mParamTypes)
            ?? throw new HarnessException($"Mathcad method not found: {name}({string.Join(", ", mParamTypes.Select(t => t.Name))})");
        return (q, m);
    }

    private static string ShowVal(Side side, object? v)
    {
        if (v == null) return "null";
        if (v is Pointer) return "ptr";
        if (v is string or int or uint or double or bool or long or byte or short) return Text(v);
        if (v is int[] a) return $"[{string.Join(",", a)}]";
        if (v is object[] arr) return $"[{string.Join(", ", arr.Select(x => ShowVal(side, x)))}]";
        if (v.GetType().Name == "Units") return Text(v);
        if (v.GetType().Name == "MatVal" || v.GetType().FullName!.Contains("MatVal")) return MatValDifferentialTests.Storage(side, v);
        if (v.GetType().Name == "MatDim" || v.GetType().FullName!.Contains("MatDim")) return $"MatDim[{Get(v, "Rows")}x{Get(v, "Cols")}]";
        if (v.GetType().Name == "Dim") return side.DimText(v);
        if (v.GetType().Name is "NotAVal" or "StrVal" or "NumVal" || v.GetType().IsSubclassOf(side.T("Mcd.DimVal")))
        {
            return $"{v.GetType().Name}[dim={side.DimText(Get(v, "m_dim"))}]";
        }
        return Text(v);
    }

    private static Func<object?, string> Show(Side side) => v => ShowVal(side, v);

    private static Func<object, string> State(Side side) => v => $"{v.GetType().Name}[dim={side.DimText(Get(v, "m_dim"))}]";

    [SkippableFact]
    public void Properties_and_conversions()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var propertyNames = new[]
        {
            "IsPolymorphic", "PhysicalUnit", "PhysicalDimension", "UniformDimension", "IsString", "IsReal",
            "IsDimless", "IsDimlessReal", "IsMatrix", "IsFunction", "Rsize", "Rfinal", "Isize"
        };
        var propMethods = propertyNames.Select(p => (Name: p, Methods: Method("get_" + p, DeclaredInstancePublic))).ToArray();

        var (qToString, mToString) = Method("ToString", DeclaredInstancePublic, typeof(IFormatProvider));
        var (qGetUnits, mGetUnits) = Method("GetUnits", DeclaredInstancePublic, typeof(bool));

        var (qAsString0, mAsString0) = Method("asString", DeclaredInstancePublic);
        var (qAsString1, mAsString1) = Method("asString", DeclaredInstancePublic, typeof(uint));
        var (qAsString2, mAsString2) = Method("asString", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsReal0, mAsReal0) = Method("asReal", DeclaredInstancePublic);
        var (qAsReal1, mAsReal1) = Method("asReal", DeclaredInstancePublic, typeof(uint));
        var (qAsReal2, mAsReal2) = Method("asReal", DeclaredInstancePublic, typeof(string), typeof(uint));

        unsafe
        {
            var (qIsInt, mIsInt) = Method("isInt", DeclaredInstancePublic, typeof(int*));
            int qResult = -1, mResult = -1;
            var qTarget0 = Side.Quire.NotAVal();
            var mTarget0 = Side.Mathcad.NotAVal();
            MethodComparison.Compare("NotAVal.isInt(&result)", Contract,
                qIsInt, qTarget0, [Pointer.Box(&qResult, typeof(int*))],
                mIsInt, mTarget0, [Pointer.Box(&mResult, typeof(int*))],
                qs, ms, qState, mState);
        }

        var (qAsInt0, mAsInt0) = Method("asInt", DeclaredInstancePublic);
        var (qAsInt1, mAsInt1) = Method("asInt", DeclaredInstancePublic, typeof(uint));
        var (qAsInt2, mAsInt2) = Method("asInt", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsDimlessInt0, mAsDimlessInt0) = Method("asDimlessInt", DeclaredInstancePublic);
        var (qAsDimlessInt1, mAsDimlessInt1) = Method("asDimlessInt", DeclaredInstancePublic, typeof(uint));
        var (qAsDimlessInt2, mAsDimlessInt2) = Method("asDimlessInt", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsDimless0, mAsDimless0) = Method("asDimless", DeclaredInstancePublic);
        var (qAsDimless1, mAsDimless1) = Method("asDimless", DeclaredInstancePublic, typeof(uint));

        var (qAsDimlessReal1, mAsDimlessReal1) = Method("asDimlessReal", DeclaredInstancePublic, typeof(uint));
        var (qAsDimlessReal2, mAsDimlessReal2) = Method("asDimlessReal", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsVector0, mAsVector0) = Method("asVector", DeclaredInstancePublic);
        var (qAsVector1, mAsVector1) = Method("asVector", DeclaredInstancePublic, typeof(uint));
        var (qAsVector2, mAsVector2) = Method("asVector", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsRealVector0, mAsRealVector0) = Method("asRealVector", DeclaredInstancePublic);
        var (qAsRealVector1, mAsRealVector1) = Method("asRealVector", DeclaredInstancePublic, typeof(uint));
        var (qAsRealVector2, mAsRealVector2) = Method("asRealVector", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsRealMatrix0, mAsRealMatrix0) = Method("asRealMatrix", DeclaredInstancePublic);
        var (qAsRealMatrix1, mAsRealMatrix1) = Method("asRealMatrix", DeclaredInstancePublic, typeof(uint));
        var (qAsRealMatrix2, mAsRealMatrix2) = Method("asRealMatrix", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsMatrix0, mAsMatrix0) = Method("asMatrix", DeclaredInstancePublic);
        var (qAsMatrix1, mAsMatrix1) = Method("asMatrix", DeclaredInstancePublic, typeof(uint));
        var (qAsMatrix2, mAsMatrix2) = Method("asMatrix", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsTuple, mAsTuple) = Method("asTuple", DeclaredInstancePublic, typeof(uint), typeof(uint), typeof(uint));

        var (qAsRange0, mAsRange0) = Method("asRange", DeclaredInstancePublic);
        var (qAsRange1, mAsRange1) = Method("asRange", DeclaredInstancePublic, typeof(uint));
        var (qAsRange2, mAsRange2) = Method("asRange", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qAsFunc0, mAsFunc0) = Method("asFunction", DeclaredInstancePublic);
        var (qAsFunc1, mAsFunc1) = Method("asFunction", DeclaredInstancePublic, typeof(uint));
        var (qAsFunc2, mAsFunc2) = Method("asFunction", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qIsFuncN, mIsFuncN) = Method("IsFunctionN", DeclaredInstancePublic, typeof(int));
        var (qAsFuncN1, mAsFuncN1) = Method("asFunctionN", DeclaredInstancePublic, typeof(int));
        var (qAsFuncN2, mAsFuncN2) = Method("asFunctionN", DeclaredInstancePublic, typeof(int), typeof(uint));

        foreach (var dim in Side.DimNames)
        {
            var qTarget = Call(Side.Quire.NotAVal(), "dv", Side.Quire.D(dim))!;
            var mTarget = Call(Side.Mathcad.NotAVal(), "dv", Side.Mathcad.D(dim))!;

            foreach (var (propName, (qProp, mProp)) in propMethods)
            {
                MethodComparison.Compare($"NotAVal_{dim}.{propName}", Contract,
                    qProp, qTarget, [], mProp, mTarget, [], qs, ms, qState, mState);
            }

            MethodComparison.Compare($"NotAVal_{dim}.ToString(InvariantCulture)", Contract,
                qToString, qTarget, [CultureInfo.InvariantCulture],
                mToString, mTarget, [CultureInfo.InvariantCulture],
                qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.GetUnits(true)", Contract,
                qGetUnits, qTarget, [true], mGetUnits, mTarget, [true],
                qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.GetUnits(false)", Contract,
                qGetUnits, qTarget, [false], mGetUnits, mTarget, [false],
                qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asString()", Contract,
                qAsString0, qTarget, [], mAsString0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asString(3)", Contract,
                qAsString1, qTarget, [3u], mAsString1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asString(m, 4)", Contract,
                qAsString2, qTarget, ["custom key", 4u], mAsString2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asReal()", Contract,
                qAsReal0, qTarget, [], mAsReal0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asReal(3)", Contract,
                qAsReal1, qTarget, [3u], mAsReal1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asReal(m, 4)", Contract,
                qAsReal2, qTarget, ["custom key", 4u], mAsReal2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asInt()", Contract,
                qAsInt0, qTarget, [], mAsInt0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asInt(3)", Contract,
                qAsInt1, qTarget, [3u], mAsInt1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asInt(m, 4)", Contract,
                qAsInt2, qTarget, ["custom key", 4u], mAsInt2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asDimlessInt()", Contract,
                qAsDimlessInt0, qTarget, [], mAsDimlessInt0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asDimlessInt(3)", Contract,
                qAsDimlessInt1, qTarget, [3u], mAsDimlessInt1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asDimlessInt(m, 4)", Contract,
                qAsDimlessInt2, qTarget, ["custom key", 4u], mAsDimlessInt2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asDimless()", Contract,
                qAsDimless0, qTarget, [], mAsDimless0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asDimless(5)", Contract,
                qAsDimless1, qTarget, [5u], mAsDimless1, mTarget, [5u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asDimlessReal(5)", Contract,
                qAsDimlessReal1, qTarget, [5u], mAsDimlessReal1, mTarget, [5u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asDimlessReal(m, 5)", Contract,
                qAsDimlessReal2, qTarget, ["custom key", 5u], mAsDimlessReal2, mTarget, ["custom key", 5u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asVector()", Contract,
                qAsVector0, qTarget, [], mAsVector0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asVector(3)", Contract,
                qAsVector1, qTarget, [3u], mAsVector1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asVector(m, 4)", Contract,
                qAsVector2, qTarget, ["custom key", 4u], mAsVector2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asRealVector()", Contract,
                qAsRealVector0, qTarget, [], mAsRealVector0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asRealVector(3)", Contract,
                qAsRealVector1, qTarget, [3u], mAsRealVector1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asRealVector(m, 4)", Contract,
                qAsRealVector2, qTarget, ["custom key", 4u], mAsRealVector2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asRealMatrix()", Contract,
                qAsRealMatrix0, qTarget, [], mAsRealMatrix0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asRealMatrix(3)", Contract,
                qAsRealMatrix1, qTarget, [3u], mAsRealMatrix1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asRealMatrix(m, 4)", Contract,
                qAsRealMatrix2, qTarget, ["custom key", 4u], mAsRealMatrix2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asMatrix()", Contract,
                qAsMatrix0, qTarget, [], mAsMatrix0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asMatrix(3)", Contract,
                qAsMatrix1, qTarget, [3u], mAsMatrix1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asMatrix(m, 4)", Contract,
                qAsMatrix2, qTarget, ["custom key", 4u], mAsMatrix2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asTuple(1, 2, 7)", Contract,
                qAsTuple, qTarget, [1u, 2u, 7u], mAsTuple, mTarget, [1u, 2u, 7u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asRange()", Contract,
                qAsRange0, qTarget, [], mAsRange0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asRange(3)", Contract,
                qAsRange1, qTarget, [3u], mAsRange1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asRange(m, 4)", Contract,
                qAsRange2, qTarget, ["custom key", 4u], mAsRange2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.asFunction()", Contract,
                qAsFunc0, qTarget, [], mAsFunc0, mTarget, [], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asFunction(3)", Contract,
                qAsFunc1, qTarget, [3u], mAsFunc1, mTarget, [3u], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asFunction(m, 4)", Contract,
                qAsFunc2, qTarget, ["custom key", 4u], mAsFunc2, mTarget, ["custom key", 4u], qs, ms, qState, mState);

            MethodComparison.Compare($"NotAVal_{dim}.IsFunctionN(2)", Contract,
                qIsFuncN, qTarget, [2], mIsFuncN, mTarget, [2], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asFunctionN(2)", Contract,
                qAsFuncN1, qTarget, [2], mAsFuncN1, mTarget, [2], qs, ms, qState, mState);
            MethodComparison.Compare($"NotAVal_{dim}.asFunctionN(2, 6)", Contract,
                qAsFuncN2, qTarget, [2, 6u], mAsFuncN2, mTarget, [2, 6u], qs, ms, qState, mState);
        }

        // Direct comparisons on NumVal and StrVal for IsDimlessReal and asDimlessReal (caller sensitivity)
        var (qGetIsDimlessReal, mGetIsDimlessReal) = Method("get_IsDimlessReal", DeclaredInstancePublic);
        var testTargets = new (string label, object q, object m)[]
        {
            ("NumVal_dimless", Side.Quire.Num(42.0, 0.0, "identity"), Side.Mathcad.Num(42.0, 0.0, "identity")),
            ("NumVal_dim", Side.Quire.Num(42.0, 0.0, "L"), Side.Mathcad.Num(42.0, 0.0, "L")),
            ("NumVal_complex", Side.Quire.Num(1.0, 2.0, "identity"), Side.Mathcad.Num(1.0, 2.0, "identity")),
            ("StrVal_dimless", Side.Quire.Str("hello"), Side.Mathcad.Str("hello")),
        };
        foreach (var (label, qT, mT) in testTargets)
        {
            MethodComparison.Compare($"{label}.IsDimlessReal", Contract,
                qGetIsDimlessReal, qT, [], mGetIsDimlessReal, mT, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{label}.asDimlessReal(5)", Contract,
                qAsDimlessReal1, qT, [5u], mAsDimlessReal1, mT, [5u], qs, ms, qState, mState);
            MethodComparison.Compare($"{label}.asDimlessReal(m, 5)", Contract,
                qAsDimlessReal2, qT, ["custom key", 5u], mAsDimlessReal2, mT, ["custom key", 5u], qs, ms, qState, mState);
        }
    }

    [SkippableFact]
    public void Matrix_and_iteration_members()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var (qRangeGet, mRangeGet) = Method("range_get", DeclaredInstancePublic, typeof(int));
        var (qIteratorGet, mIteratorGet) = Method("iterator_get", DeclaredInstancePublic, typeof(int));
        var (qAsIterator, mAsIterator) = Method("asIterator", DeclaredInstancePublic);

        var (qMatrixGet1, mMatrixGet1) = Method("matrix_get_1", DeclaredInstancePublic, typeof(int));
        var (qSymMatrixGet1, mSymMatrixGet1) = Method("sym_matrix_get_1", DeclaredInstancePublic, typeof(int));
        var (qMatrixGet2, mMatrixGet2) = Method("matrix_get_2", DeclaredInstancePublic, typeof(int), typeof(int));
        var (qSymMatrixGet2, mSymMatrixGet2) = Method("sym_matrix_get_2", DeclaredInstancePublic, typeof(int), typeof(int));

        var (qAsImmutable, mAsImmutable) = Method("asImmutable", DeclaredInstancePublic);
        var (qAsResult, mAsResult) = Method("asResult", DeclaredInstancePublic);
        var (qApply, mApply) = Method("Apply", DeclaredInstancePublic, typeof(object[]));

        var (qMatDim, mMatDim) = Method("get_MatrixDimension", DeclaredInstancePublic);
        var (qExtend1, mExtend1) = Method("extend_1", DeclaredInstancePublic, typeof(int));
        var (qExtend2, mExtend2) = Method("extend_2", DeclaredInstancePublic, typeof(int), typeof(int));
        var (qExtend1u, mExtend1u) = Method("extend_1u", DeclaredInstancePublic, typeof(int));
        var (qExtend2u, mExtend2u) = Method("extend_2u", DeclaredInstancePublic, typeof(int), typeof(int));

        var (qMatrixSet0_1, mMatrixSet0_1) = Method("matrix_set0_1", DeclaredInstancePublic, typeof(int));
        var (qMatrixSet0_2, mMatrixSet0_2) = Method("matrix_set0_2", DeclaredInstancePublic, typeof(int), typeof(int));
        var (qMatrixSet1_d, mMatrixSet1_d) = Method("matrix_set_1", DeclaredInstancePublic, typeof(int), typeof(double));
        var (qMatrixSet1_o, mMatrixSet1_o) = Method("matrix_set_1", DeclaredInstancePublic, typeof(int), typeof(object));
        var (qMatrixSet1_v, mMatrixSet1_v) = Method("matrix_set_1", DeclaredInstancePublic, typeof(int), QVal);
        var (qMatrixSet1_ob, mMatrixSet1_ob) = Method("matrix_set_1_ob", DeclaredInstancePublic, typeof(int), typeof(object));

        var (qMatrixSet2_d, mMatrixSet2_d) = Method("matrix_set_2", DeclaredInstancePublic, typeof(int), typeof(int), typeof(double));
        var (qMatrixSet2_dd, mMatrixSet2_dd) = Method("matrix_set_2", DeclaredInstancePublic, typeof(int), typeof(int), typeof(double), typeof(double));
        var (qMatrixSet2i, mMatrixSet2i) = Method("matrix_set_2i", DeclaredInstancePublic, typeof(int), typeof(int), typeof(double));
        var (qMatrixSet2_o, mMatrixSet2_o) = Method("matrix_set_2", DeclaredInstancePublic, typeof(int), typeof(int), typeof(object));
        var (qMatrixSet2_v, mMatrixSet2_v) = Method("matrix_set_2", DeclaredInstancePublic, typeof(int), typeof(int), QVal);
        var (qMatrixSet2_ob, mMatrixSet2_ob) = Method("matrix_set_2_ob", DeclaredInstancePublic, typeof(int), typeof(int), typeof(object));

        var qTarget = Side.Quire.NotAVal();
        var mTarget = Side.Mathcad.NotAVal();

        MethodComparison.Compare("NotAVal.range_get(5)", Contract,
            qRangeGet, qTarget, [5], mRangeGet, mTarget, [5], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.iterator_get(5)", Contract,
            qIteratorGet, qTarget, [5], mIteratorGet, mTarget, [5], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.asIterator()", Contract,
            qAsIterator, qTarget, [], mAsIterator, mTarget, [], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.matrix_get_1(0)", Contract,
            qMatrixGet1, qTarget, [0], mMatrixGet1, mTarget, [0], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.sym_matrix_get_1(0)", Contract,
            qSymMatrixGet1, qTarget, [0], mSymMatrixGet1, mTarget, [0], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_get_2(0, 0)", Contract,
            qMatrixGet2, qTarget, [0, 0], mMatrixGet2, mTarget, [0, 0], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.sym_matrix_get_2(0, 0)", Contract,
            qSymMatrixGet2, qTarget, [0, 0], mSymMatrixGet2, mTarget, [0, 0], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.asImmutable()", Contract,
            qAsImmutable, qTarget, [], mAsImmutable, mTarget, [], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.asResult()", Contract,
            qAsResult, qTarget, [], mAsResult, mTarget, [], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.Apply([])", Contract,
            qApply, qTarget, [Array.Empty<object>()], mApply, mTarget, [Array.Empty<object>()], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.MatrixDimension", Contract,
            qMatDim, qTarget, [], mMatDim, mTarget, [], qs, ms, qState, mState);

        // Matrix extension & setters
        MethodComparison.Compare("NotAVal.extend_1(-1): out of bounds", Contract,
            qExtend1, qTarget, [-1], mExtend1, mTarget, [-1], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.extend_1(2)", Contract,
            qExtend1, qTarget, [2], mExtend1, mTarget, [2], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.extend_2(-1, 0): out of bounds i", Contract,
            qExtend2, qTarget, [-1, 0], mExtend2, mTarget, [-1, 0], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.extend_2(0, -1): out of bounds j", Contract,
            qExtend2, qTarget, [0, -1], mExtend2, mTarget, [0, -1], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.extend_2(1, 2)", Contract,
            qExtend2, qTarget, [1, 2], mExtend2, mTarget, [1, 2], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.extend_1u(2)", Contract,
            qExtend1u, qTarget, [2], mExtend1u, mTarget, [2], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.extend_2u(1, 2)", Contract,
            qExtend2u, qTarget, [1, 2], mExtend2u, mTarget, [1, 2], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.matrix_set0_1(2)", Contract,
            qMatrixSet0_1, qTarget, [2], mMatrixSet0_1, mTarget, [2], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set0_2(1, 2)", Contract,
            qMatrixSet0_2, qTarget, [1, 2], mMatrixSet0_2, mTarget, [1, 2], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.matrix_set_1(1, double)", Contract,
            qMatrixSet1_d, qTarget, [1, 3.5], mMatrixSet1_d, mTarget, [1, 3.5], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_1(1, object)", Contract,
            qMatrixSet1_o, qTarget, [1, "text"], mMatrixSet1_o, mTarget, [1, "text"], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_1(1, DimVal)", Contract,
            qMatrixSet1_v, qTarget, [1, Side.Quire.NotAVal()], mMatrixSet1_v, mTarget, [1, Side.Mathcad.NotAVal()], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_1_ob(1, object)", Contract,
            qMatrixSet1_ob, qTarget, [1, "text"], mMatrixSet1_ob, mTarget, [1, "text"], qs, ms, qState, mState);

        MethodComparison.Compare("NotAVal.matrix_set_2(1, 2, double)", Contract,
            qMatrixSet2_d, qTarget, [1, 2, 3.5], mMatrixSet2_d, mTarget, [1, 2, 3.5], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_2(1, 2, double, double)", Contract,
            qMatrixSet2_dd, qTarget, [1, 2, 3.5, -2.0], mMatrixSet2_dd, mTarget, [1, 2, 3.5, -2.0], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_2i(1, 2, double)", Contract,
            qMatrixSet2i, qTarget, [1, 2, 4.2], mMatrixSet2i, mTarget, [1, 2, 4.2], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_2(1, 2, object)", Contract,
            qMatrixSet2_o, qTarget, [1, 2, "text"], mMatrixSet2_o, mTarget, [1, 2, "text"], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_2(1, 2, DimVal)", Contract,
            qMatrixSet2_v, qTarget, [1, 2, Side.Quire.NotAVal()], mMatrixSet2_v, mTarget, [1, 2, Side.Mathcad.NotAVal()], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set_2_ob(1, 2, object)", Contract,
            qMatrixSet2_ob, qTarget, [1, 2, "text"], mMatrixSet2_ob, mTarget, [1, 2, "text"], qs, ms, qState, mState);
    }

    [SkippableFact]
    public void Call_errors_and_dynamic_dispatch()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var qTarget = Side.Quire.NotAVal();
        var mTarget = Side.Mathcad.NotAVal();

        var (qDynCall0, mDynCall0) = Method("DynCall", DeclaredInstancePublic);
        MethodComparison.Compare("DynCall()", Contract,
            qDynCall0, qTarget, [], mDynCall0, mTarget, [], qs, ms, qState, mState);

        for (int n = 1; n <= 16; n++)
        {
            var paramTypes = Enumerable.Repeat(typeof(object), n).ToArray();
            var (qDynCallN, mDynCallN) = Method("DynCall", DeclaredInstancePublic, paramTypes);
            var args = Enumerable.Repeat<object?>("x", n).ToArray();
            MethodComparison.Compare($"DynCall({n})", Contract,
                qDynCallN, qTarget, args, mDynCallN, mTarget, args, qs, ms, qState, mState);

            if (n <= 3)
            {
                var (qVecCallN, mVecCallN) = Method("VecCall", DeclaredInstancePublic, paramTypes);
                MethodComparison.Compare($"VecCall({n})", Contract,
                    qVecCallN, qTarget, args, mVecCallN, mTarget, args, qs, ms, qState, mState);
            }
        }

        var (qDynApply, mDynApply) = Method("DynApply", DeclaredInstancePublic, typeof(object[]));
        MethodComparison.Compare("DynApply(3)", Contract,
            qDynApply, qTarget, [new object[3]], mDynApply, mTarget, [new object[3]], qs, ms, qState, mState);

        var (qVecApply, mVecApply) = Method("VecApply", DeclaredInstancePublic, typeof(object[]));
        MethodComparison.Compare("VecApply(2)", Contract,
            qVecApply, qTarget, [new object[2]], mVecApply, mTarget, [new object[2]], qs, ms, qState, mState);

        var (qBadCall2, mBadCall2) = Method("BadCall", DeclaredInstancePublic, typeof(int), typeof(int));
        var (qBadCall3, mBadCall3) = Method("BadCall", DeclaredInstancePublic, typeof(int), typeof(int), typeof(uint));
        var (qWrongArgs2, mWrongArgs2) = Method("WrongNumberOfArguments", DeclaredStaticPublic, typeof(int), typeof(int));
        var (qWrongArgs3, mWrongArgs3) = Method("WrongNumberOfArguments", DeclaredStaticPublic, typeof(int), typeof(int), typeof(uint));

        foreach (int required in new[] { -2, -1, 0, 1, 2, 3 })
        {
            foreach (int actual in new[] { 0, 1, 2, 3, 4 })
            {
                MethodComparison.Compare($"BadCall({required}, {actual})", Contract,
                    qBadCall2, qTarget, [required, actual], mBadCall2, mTarget, [required, actual],
                    qs, ms, qState, mState);
                MethodComparison.Compare($"BadCall({required}, {actual}, 9)", Contract,
                    qBadCall3, qTarget, [required, actual, 9u], mBadCall3, mTarget, [required, actual, 9u],
                    qs, ms, qState, mState);

                MethodComparison.Compare($"WrongNumberOfArguments({required}, {actual})", Contract,
                    qWrongArgs2, null, [required, actual], mWrongArgs2, null, [required, actual],
                    qs, ms);
                MethodComparison.Compare($"WrongNumberOfArguments({required}, {actual}, 8)", Contract,
                    qWrongArgs3, null, [required, actual, 8u], mWrongArgs3, null, [required, actual, 8u],
                    qs, ms);
            }
        }

        var (qOnError4, mOnError4) = Method("OnError", DeclaredStaticPublic, typeof(uint), typeof(string), typeof(int), typeof(int));
        var (qOnError3_i, mOnError3_i) = Method("OnError", DeclaredStaticPublic, typeof(uint), typeof(string), typeof(int));
        var (qOnError3_o, mOnError3_o) = Method("OnError", DeclaredStaticPublic, typeof(uint), typeof(string), typeof(object));

        MethodComparison.Compare("OnError(3, key, 1, 2)", Contract,
            qOnError4, null, [3u, "key", 1, 2], mOnError4, null, [3u, "key", 1, 2], qs, ms);
        MethodComparison.Compare("OnError(3, key, 1)", Contract,
            qOnError3_i, null, [3u, "key", 1], mOnError3_i, null, [3u, "key", 1], qs, ms);
        MethodComparison.Compare("OnError(MaxValue, key@4, 1)", Contract,
            qOnError3_i, null, [uint.MaxValue, "key@4", 1], mOnError3_i, null, [uint.MaxValue, "key@4", 1], qs, ms);

        foreach (var thing in new object?[] { null, "text", 2.5, "12>tagged" })
        {
            MethodComparison.Compare($"OnError(3, key, {Text(thing)})", Contract,
                qOnError3_o, null, [3u, "key", thing], mOnError3_o, null, [3u, "key", thing], qs, ms);
        }
        MethodComparison.Compare("OnError(3, key, NotAVal)", Contract,
            qOnError3_o, null, [3u, "key", Side.Quire.NotAVal()], mOnError3_o, null, [3u, "key", Side.Mathcad.NotAVal()], qs, ms);
    }

    [SkippableFact]
    public void Static_and_dimension_helpers()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var (qDyn, mDyn) = Method("dyn", DeclaredStaticPublic, typeof(object), QDim);
        var (qDv, mDv) = Method("dv", DeclaredInstancePublic, QDim);
        var (qNearlyEqual, mNearlyEqual) = Method("NearlyEqual", DeclaredStaticPublic, typeof(object), typeof(object), typeof(double));
        var (qNearlyEqualInst, mNearlyEqualInst) = Method("nearly_equal", DeclaredInstancePublic, QVal, typeof(double));
        var (qGetUnitsStatic, mGetUnitsStatic) = Method("GetUnits", DeclaredStaticPublic, QDim, typeof(bool));
        var (qDimOf, mDimOf) = Method("DimOf", DeclaredStaticPublic, typeof(object));
        var (qIsDimOf, mIsDimOf) = Method("IsDimOf", DeclaredStaticPublic, typeof(object), QDim);
        var (qDimCast, mDimCast) = Method("DimCast", DeclaredStaticPublic, typeof(object), QDim);
        var (qSetDim, mSetDim) = Method("SetDim", DeclaredInstancePublic, QDim);
        var (qIsUniformDimOf, mIsUniformDimOf) = Method("IsUniformDimOf", DeclaredInstancePublic, QDim);
        var (qAsResultStatic, mAsResultStatic) = Method("as_result", DeclaredStaticPublic, typeof(object));
        var (qMatDimOf, mMatDimOf) = Method("MatDimOf", DeclaredStaticPublic, typeof(object));

        foreach (var dim in Side.DimNames)
        {
            var qD = Side.Quire.D(dim);
            var mD = Side.Mathcad.D(dim);

            MethodComparison.Compare($"GetUnits({dim}, true)", Contract,
                qGetUnitsStatic, null, [qD, true], mGetUnitsStatic, null, [mD, true], qs, ms);
            MethodComparison.Compare($"GetUnits({dim}, false)", Contract,
                qGetUnitsStatic, null, [qD, false], mGetUnitsStatic, null, [mD, false], qs, ms);

            var qTargetL = Call(Side.Quire.NotAVal(), "dv", Side.Quire.D("L"))!;
            var mTargetL = Call(Side.Mathcad.NotAVal(), "dv", Side.Mathcad.D("L"))!;

            MethodComparison.Compare($"IsDimOf(NotAVal_L, {dim})", Contract,
                qIsDimOf, null, [qTargetL, qD], mIsDimOf, null, [mTargetL, mD], qs, ms);
            MethodComparison.Compare($"NotAVal_L.IsUniformDimOf({dim})", Contract,
                qIsUniformDimOf, qTargetL, [qD], mIsUniformDimOf, mTargetL, [mD],
                qs, ms, qState, mState);

            var qTargetDyn = Side.Quire.NotAVal();
            var mTargetDyn = Side.Mathcad.NotAVal();
            MethodComparison.Compare($"dyn(NotAVal, {dim})", Contract,
                qDyn, null, [qTargetDyn, qD], mDyn, null, [mTargetDyn, mD], qs, ms);

            var qTargetCast = Side.Quire.NotAVal();
            var mTargetCast = Side.Mathcad.NotAVal();
            MethodComparison.Compare($"DimCast(NotAVal, {dim})", Contract,
                qDimCast, null, [qTargetCast, qD], mDimCast, null, [mTargetCast, mD], qs, ms);

            var qTargetSetDim = Side.Quire.NotAVal();
            var mTargetSetDim = Side.Mathcad.NotAVal();
            MethodComparison.Compare($"NotAVal.SetDim({dim})", Contract,
                qSetDim, qTargetSetDim, [qD], mSetDim, mTargetSetDim, [mD],
                qs, ms, qState, mState);

            var qTargetDv = Side.Quire.NotAVal();
            var mTargetDv = Side.Mathcad.NotAVal();
            MethodComparison.Compare($"NotAVal.dv({dim})", Contract,
                qDv, qTargetDv, [qD], mDv, mTargetDv, [mD],
                qs, ms, qState, mState);

            var qTargetDimOf = Call(Side.Quire.NotAVal(), "dv", qD)!;
            var mTargetDimOf = Call(Side.Mathcad.NotAVal(), "dv", mD)!;
            MethodComparison.Compare($"DimOf(NotAVal_{dim})", Contract,
                qDimOf, null, [qTargetDimOf], mDimOf, null, [mTargetDimOf], qs, ms);

            MethodComparison.Compare($"MatDimOf(NotAVal_{dim})", Contract,
                qMatDimOf, null, [qTargetDimOf], mMatDimOf, null, [mTargetDimOf], qs, ms);
        }

        foreach (var o in new object?[] { "plain", 3, null })
        {
            var qDL = Side.Quire.D("L");
            var mDL = Side.Mathcad.D("L");

            MethodComparison.Compare($"dyn({Text(o)}, L)", Contract,
                qDyn, null, [o, qDL], mDyn, null, [o, mDL], qs, ms);
            MethodComparison.Compare($"DimCast({Text(o)}, L)", Contract,
                qDimCast, null, [o, qDL], mDimCast, null, [o, mDL], qs, ms);
            MethodComparison.Compare($"IsDimOf({Text(o)}, L)", Contract,
                qIsDimOf, null, [o, qDL], mIsDimOf, null, [o, mDL], qs, ms);
            MethodComparison.Compare($"as_result({Text(o)})", Contract,
                qAsResultStatic, null, [o], mAsResultStatic, null, [o], qs, ms);
            MethodComparison.Compare($"DimOf({Text(o)})", Contract,
                qDimOf, null, [o], mDimOf, null, [o], qs, ms);
            MethodComparison.Compare($"MatDimOf({Text(o)})", Contract,
                qMatDimOf, null, [o], mMatDimOf, null, [o], qs, ms);
        }

        MethodComparison.Compare("as_result(NotAVal)", Contract,
            qAsResultStatic, null, [Side.Quire.NotAVal()], mAsResultStatic, null, [Side.Mathcad.NotAVal()], qs, ms);

        // NearlyEqual
        object?[] xsQ = [Side.Quire.NotAVal(), "a", 1.0, null];
        object?[] xsM = [Side.Mathcad.NotAVal(), "a", 1.0, null];
        for (int i = 0; i < xsQ.Length; i++)
        {
            for (int j = 0; j < xsQ.Length; j++)
            {
                MethodComparison.Compare($"NearlyEqual({i}, {j})", Contract,
                    qNearlyEqual, null, [xsQ[i], xsQ[j], 0.1],
                    mNearlyEqual, null, [xsM[i], xsM[j], 0.1],
                    qs, ms);
            }
        }

        var qT1 = Side.Quire.NotAVal();
        var mT1 = Side.Mathcad.NotAVal();
        var qT2 = Side.Quire.NotAVal();
        var mT2 = Side.Mathcad.NotAVal();
        MethodComparison.Compare("NotAVal.nearly_equal(NotAVal, 0.1)", Contract,
            qNearlyEqualInst, qT1, [qT2, 0.1],
            mNearlyEqualInst, mT1, [mT2, 0.1],
            qs, ms, qState, mState);
    }
}

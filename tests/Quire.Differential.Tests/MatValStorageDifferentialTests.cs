using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValStorageDifferentialTests
{
    private const string Contract = "Mcd.MatVal.Storage";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name, params Type[] parameterTypes) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, parameterTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}({string.Join(", ", parameterTypes.Select(t => t.Name))})");

    private static MethodInfo StaticMethod(Side side, string name, params Type[] parameterTypes) =>
        side.T("Mcd.MatVal").GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} static MatVal.{name}({string.Join(", ", parameterTypes.Select(t => t.Name))})");

    private static ConstructorInfo Ctor(Side side, params Type[] parameterTypes) =>
        side.T("Mcd.MatVal").GetConstructor(DeclaredInstance, null, parameterTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.ctor({string.Join(", ", parameterTypes.Select(t => t.Name))})");

    private static Delegate CreatePtrDelegate(Side side, object target, string methodName = "set_ptrs_2")
    {
        var method = target.GetType().GetMethod(methodName, DeclaredInstance, null, [typeof(int), typeof(int), typeof(object)], null)
            ?? throw new HarnessException($"Missing {side.Name} {methodName} for PtrDelegate");
        return Delegate.CreateDelegate(side.T("Mcd.PtrDelegate"), target, method);
    }

    private static object Matrix(Side side, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = false)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            real[offset + row + stride * col] = (row + 1) * 10 + (col + 1);
        double[]? imag = imaginary ? new double[length] : null;
        if (imag != null)
        {
            for (int col = 0; col < cols; col++)
            for (int row = 0; row < rows; row++)
                imag[offset + row + stride * col] = (row + 1) * 100 + (col + 1);
        }
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static string Show(Side side, object? target, object? value)
    {
        if (value == null) return "null";
        if (target != null && ReferenceEquals(target, value)) return "same-instance";
        if (value is double[] doubles) return "[" + string.Join(", ", doubles) + "]";
        if (side.T("Mcd.MatVal").IsInstanceOfType(value)) return MatValDifferentialTests.Storage(side, value);
        if (side.Dim.IsInstanceOfType(value)) return (string)Reflect.Call(value, "ToString")!;
        if (side.T("Mcd.MatDim").IsInstanceOfType(value)) return MatDimDifferentialTests.DescribeDimension(side, value);
        if (value is Hashtable table)
            return "{" + string.Join(";", table.Keys.Cast<int>().Order().Select(index => index + ":" + Show(side, target, table[index]))) + "}";
        return side.Show(value);
    }

    private static string State(Side side, object target) => MatValDifferentialTests.Storage(side, target);

    private static void Compare(string caseId, MethodBase qMethod, object? qTarget, object?[] qArgs,
        MethodBase mMethod, object? mTarget, object?[] mArgs, int expectedToken)
    {
        Assert.Equal(expectedToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs,
            quireShow: value => Show(Side.Quire, qTarget, value),
            originalShow: value => Show(Side.Mathcad, mTarget, value),
            quireState: target => target == null ? "null" : State(Side.Quire, target),
            originalState: target => target == null ? "null" : State(Side.Mathcad, target));
    }

    private static void CompareBoth(string caseId, string methodName, Type[] parameterTypes, int token,
        object?[] args, Action<Side, object>? setup = null, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = false)
    {
        var qTarget = Matrix(Side.Quire, rows, cols, stride, offset, imaginary);
        var mTarget = Matrix(Side.Mathcad, rows, cols, stride, offset, imaginary);
        setup?.Invoke(Side.Quire, qTarget);
        setup?.Invoke(Side.Mathcad, mTarget);
        var qMethod = Method(Side.Quire, methodName, parameterTypes);
        var mMethod = Method(Side.Mathcad, methodName, parameterTypes);
        Compare(caseId, qMethod, qTarget, args, mMethod, mTarget, args, token);
    }

    [SkippableFact]
    public void Storage_pack_and_zero_helpers_match_mathcad()
    {
        // 0x06000534: Copy(double[], int, double[], int, int)
        {
            var qCopy = StaticMethod(Side.Quire, "Copy", typeof(double[]), typeof(int), typeof(double[]), typeof(int), typeof(int));
            var mCopy = StaticMethod(Side.Mathcad, "Copy", typeof(double[]), typeof(int), typeof(double[]), typeof(int), typeof(int));

            var qDst1 = new double[] { 0, 0, 0, 0, 0 };
            var mDst1 = new double[] { 0, 0, 0, 0, 0 };
            var src = new double[] { 10, 20, 30, 40 };
            Compare("Copy.positive", qCopy, null, [src, 1, qDst1, 2, 2], mCopy, null, [src, 1, mDst1, 2, 2], 0x06000534);

            var qDst2 = new double[] { 1, 2, 3 };
            var mDst2 = new double[] { 1, 2, 3 };
            Compare("Copy.zero", qCopy, null, [src, 0, qDst2, 0, 0], mCopy, null, [src, 0, mDst2, 0, 0], 0x06000534);

            var qDst3 = new double[] { 1, 2, 3 };
            var mDst3 = new double[] { 1, 2, 3 };
            Compare("Copy.negative", qCopy, null, [src, 0, qDst3, 0, -1], mCopy, null, [src, 0, mDst3, 0, -1], 0x06000534);
        }

        // 0x06000542: PackReal()
        CompareBoth("PackReal.unpacked", "PackReal", [], 0x06000542, [], rows: 2, cols: 2, stride: 3, offset: 1);
        CompareBoth("PackReal.packed", "PackReal", [], 0x06000542, [], rows: 2, cols: 2, stride: 2, offset: 0);

        // 0x06000543: PackReal(int)
        CompareBoth("PackReal.col0", "PackReal", [typeof(int)], 0x06000543, [0], rows: 2, cols: 2, stride: 3, offset: 1);
        CompareBoth("PackReal.col1", "PackReal", [typeof(int)], 0x06000543, [1], rows: 2, cols: 2, stride: 3, offset: 1);

        // 0x06000544: PackReal(int, int, int, int)
        CompareBoth("PackReal.sub_0_1_0_1", "PackReal", [typeof(int), typeof(int), typeof(int), typeof(int)],
            0x06000544, [0, 1, 0, 1], rows: 2, cols: 2, stride: 3, offset: 1);

        // 0x06000545: PackImag()
        CompareBoth("PackImag.none", "PackImag", [], 0x06000545, [], imaginary: false);
        CompareBoth("PackImag.present", "PackImag", [], 0x06000545, [], imaginary: true);

        // 0x06000546: PackImag(int)
        CompareBoth("PackImag.col0", "PackImag", [typeof(int)], 0x06000546, [0], imaginary: true);

        // 0x06000547: PackImag(int, int, int, int)
        CompareBoth("PackImag.sub", "PackImag", [typeof(int), typeof(int), typeof(int), typeof(int)],
            0x06000547, [0, 2, 0, 1], imaginary: true);

        // 0x06000541: PackZero(double[])
        CompareBoth("PackZero.all_zero", "PackZero", [typeof(double[])], 0x06000541, [new double[] { 0, 0, 0 }]);
        CompareBoth("PackZero.nonzero", "PackZero", [typeof(double[])], 0x06000541, [new double[] { 0, 1.5, 0 }]);

        // 0x06000535: Pack(double[])
        CompareBoth("Pack.arr", "Pack", [typeof(double[])], 0x06000535, [new double[] { 1, 2, 3, 4, 5, 6, 7 }]);
        // 0x06000537: Pack(double[], int)
        CompareBoth("Pack.arr_col", "Pack", [typeof(double[]), typeof(int)], 0x06000537, [new double[] { 1, 2, 3, 4, 5, 6, 7 }, 1]);
        // 0x06000536: Pack(double[], int, int, int, int)
        CompareBoth("Pack.arr_sub", "Pack", [typeof(double[]), typeof(int), typeof(int), typeof(int), typeof(int)],
            0x06000536, [new double[] { 1, 2, 3, 4, 5, 6, 7 }, 0, 1, 0, 1]);
    }

    [SkippableFact]
    public void Storage_pointer_packing_and_copy_ctor_match_mathcad()
    {
        // 0x0600054a: PackPtrs(int)
        CompareBoth("PackPtrs.none", "PackPtrs", [typeof(int)], 0x0600054a, [0]);
        CompareBoth("PackPtrs.present", "PackPtrs", [typeof(int)], 0x0600054a, [0],
            setup: (s, m) => Call(m, "set_ptrs_2", 0, 0, s.Str("ptr")));

        // 0x0600054b: PackPtrs(int, int, int, int)
        CompareBoth("PackPtrs.sub_present", "PackPtrs", [typeof(int), typeof(int), typeof(int), typeof(int)],
            0x0600054b, [0, 1, 0, 1], setup: (s, m) => Call(m, "set_ptrs_2", 0, 0, s.Str("ptr")));

        // 0x0600048d: .ctor(MatVal) copy constructor
        {
            var qSource = Matrix(Side.Quire, rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);
            var mSource = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);
            Call(qSource, "set_ptrs_2", 0, 1, Side.Quire.Num(5, 6, "T"));
            Call(mSource, "set_ptrs_2", 0, 1, Side.Mathcad.Num(5, 6, "T"));

            var qCtor = Ctor(Side.Quire, Side.Quire.T("Mcd.MatVal"));
            var mCtor = Ctor(Side.Mathcad, Side.Mathcad.T("Mcd.MatVal"));
            Compare("ctor.copy", qCtor, null, [qSource], mCtor, null, [mSource], 0x0600048d);
        }

        // 0x0600048e: InitPtrs(MatVal)
        {
            var qSrcPacked = Matrix(Side.Quire, rows: 2, cols: 2, stride: 2, offset: 0);
            var mSrcPacked = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 2, offset: 0);
            Call(qSrcPacked, "set_ptrs_2", 0, 0, Side.Quire.Str("ptr"));
            Call(mSrcPacked, "set_ptrs_2", 0, 0, Side.Mathcad.Str("ptr"));

            var qTarget1 = Matrix(Side.Quire, rows: 2, cols: 2, stride: 2, offset: 0);
            var mTarget1 = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 2, offset: 0);

            var qMethod = Method(Side.Quire, "InitPtrs", Side.Quire.T("Mcd.MatVal"));
            var mMethod = Method(Side.Mathcad, "InitPtrs", Side.Mathcad.T("Mcd.MatVal"));
            Compare("InitPtrs.packed_same_stride", qMethod, qTarget1, [qSrcPacked], mMethod, mTarget1, [mSrcPacked], 0x0600048e);

            var qSrcUnpacked = Matrix(Side.Quire, rows: 2, cols: 2, stride: 3, offset: 1);
            var mSrcUnpacked = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 3, offset: 1);
            Call(qSrcUnpacked, "set_ptrs_2", 0, 0, Side.Quire.Str("ptr2"));
            Call(mSrcUnpacked, "set_ptrs_2", 0, 0, Side.Mathcad.Str("ptr2"));

            var qTarget2 = Matrix(Side.Quire, rows: 2, cols: 2, stride: 2, offset: 0);
            var mTarget2 = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 2, offset: 0);
            Compare("InitPtrs.unpacked_fallback", qMethod, qTarget2, [qSrcUnpacked], mMethod, mTarget2, [mSrcUnpacked], 0x0600048e);

            var qSrcCol1 = Matrix(Side.Quire, rows: 3, cols: 1, stride: 3, offset: 0);
            var mSrcCol1 = Matrix(Side.Mathcad, rows: 3, cols: 1, stride: 3, offset: 0);
            Call(qSrcCol1, "set_ptrs_2", 0, 0, Side.Quire.Str("ptr3"));
            Call(mSrcCol1, "set_ptrs_2", 0, 0, Side.Mathcad.Str("ptr3"));

            var qTarget3 = Matrix(Side.Quire, rows: 3, cols: 1, stride: 5, offset: 0);
            var mTarget3 = Matrix(Side.Mathcad, rows: 3, cols: 1, stride: 5, offset: 0);
            Compare("InitPtrs.col1_packed", qMethod, qTarget3, [qSrcCol1], mMethod, mTarget3, [mSrcCol1], 0x0600048e);
        }

        // 0x0600054c: IteratePtrs(PtrDelegate)
        {
            var qSrc = Matrix(Side.Quire);
            var mSrc = Matrix(Side.Mathcad);
            Call(qSrc, "set_ptrs_2", 0, 0, Side.Quire.Str("p0"));
            Call(qSrc, "set_ptrs_2", 1, 1, Side.Quire.Num(7, 8, "T"));
            Call(mSrc, "set_ptrs_2", 0, 0, Side.Mathcad.Str("p0"));
            Call(mSrc, "set_ptrs_2", 1, 1, Side.Mathcad.Num(7, 8, "T"));

            var qDest = Matrix(Side.Quire);
            var mDest = Matrix(Side.Mathcad);
            var qDel = CreatePtrDelegate(Side.Quire, qDest);
            var mDel = CreatePtrDelegate(Side.Mathcad, mDest);

            var qMethod = Method(Side.Quire, "IteratePtrs", Side.Quire.T("Mcd.PtrDelegate"));
            var mMethod = Method(Side.Mathcad, "IteratePtrs", Side.Mathcad.T("Mcd.PtrDelegate"));
            Compare("IteratePtrs.present", qMethod, qSrc, [qDel], mMethod, mSrc, [mDel], 0x0600054c);
            Assert.Equal(State(Side.Mathcad, mDest), State(Side.Quire, qDest));

            var qEmpty = Matrix(Side.Quire);
            var mEmpty = Matrix(Side.Mathcad);
            Compare("IteratePtrs.empty", qMethod, qEmpty, [qDel], mMethod, mEmpty, [mDel], 0x0600054c);
        }

        // 0x0600054d: TransformPtrs(MatVal, PtrDelegate)
        {
            var qSrc = Matrix(Side.Quire);
            var mSrc = Matrix(Side.Mathcad);
            Call(qSrc, "set_ptrs_2", 0, 1, Side.Quire.Str("p1"));
            Call(mSrc, "set_ptrs_2", 0, 1, Side.Mathcad.Str("p1"));

            var qTarget = Matrix(Side.Quire);
            var mTarget = Matrix(Side.Mathcad);
            var qDel = CreatePtrDelegate(Side.Quire, qTarget);
            var mDel = CreatePtrDelegate(Side.Mathcad, mTarget);

            var qMethod = Method(Side.Quire, "TransformPtrs", Side.Quire.T("Mcd.MatVal"), Side.Quire.T("Mcd.PtrDelegate"));
            var mMethod = Method(Side.Mathcad, "TransformPtrs", Side.Mathcad.T("Mcd.MatVal"), Side.Mathcad.T("Mcd.PtrDelegate"));
            Compare("TransformPtrs.present", qMethod, qTarget, [qSrc, qDel], mMethod, mTarget, [mSrc, mDel], 0x0600054d);

            var qTarget2 = Matrix(Side.Quire);
            var mTarget2 = Matrix(Side.Mathcad);
            var qEmpty = Matrix(Side.Quire);
            var mEmpty = Matrix(Side.Mathcad);
            var qDel2 = CreatePtrDelegate(Side.Quire, qTarget2);
            var mDel2 = CreatePtrDelegate(Side.Mathcad, mTarget2);
            Compare("TransformPtrs.no_ptrs", qMethod, qTarget2, [qEmpty, qDel2], mMethod, mTarget2, [mEmpty, mDel2], 0x0600054d);
        }

        // 0x06000555: CopyPtrs(MatVal)
        {
            var qTarget = Matrix(Side.Quire);
            var mTarget = Matrix(Side.Mathcad);
            var qSrc = Matrix(Side.Quire);
            var mSrc = Matrix(Side.Mathcad);
            Call(qSrc, "set_ptrs_2", 0, 0, Side.Quire.Str("copied"));
            Call(mSrc, "set_ptrs_2", 0, 0, Side.Mathcad.Str("copied"));

            var qMethod = Method(Side.Quire, "CopyPtrs", Side.Quire.T("Mcd.MatVal"));
            var mMethod = Method(Side.Mathcad, "CopyPtrs", Side.Mathcad.T("Mcd.MatVal"));
            Compare("CopyPtrs.present", qMethod, qTarget, [qSrc], mMethod, mTarget, [mSrc], 0x06000555);
        }
    }
}

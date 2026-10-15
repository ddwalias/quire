using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type Q => typeof(global::Quire.Mcd.MatVal);
    private static System.Type M => Mathcad.Type("mplrun", "Mcd.MatVal");

    private static System.Type OriginalType(System.Type type)
    {
        if (type.IsArray) return OriginalType(type.GetElementType()!).MakeArrayType();
        if (type.Assembly == typeof(global::Quire.Mcd.MatVal).Assembly)
            return Mathcad.Type("mplrun", type.FullName!.Replace("Quire.", "", StringComparison.Ordinal));
        return type;
    }

    private static (MethodInfo Q, MethodInfo M) Method(string name, BindingFlags flags, params System.Type[] types)
    {
        var q = Q.GetMethod(name, flags, types) ?? throw new HarnessException($"Quire MatVal method not found: {name}");
        var m = M.GetMethod(name, flags, types.Select(OriginalType).ToArray()) ?? throw new HarnessException($"Mathcad MatVal method not found: {name}");
        return (q, m);
    }

    private static (ConstructorInfo Q, ConstructorInfo M) Constructor(params System.Type[] types)
    {
        var q = Q.GetConstructor(types)
            ?? throw new HarnessException("Quire MatVal constructor not found");
        var m = M.GetConstructor(types.Select(OriginalType).ToArray())
            ?? throw new HarnessException("Mathcad MatVal constructor not found");
        return (q, m);
    }

    private static object New(Side side, params object?[] args)
    {
        System.Type[] types = args.Length switch
        {
            1 when args[0] is int => [typeof(int)],
            1 => [typeof(double[])],
            2 when args[0] is int => [typeof(int), typeof(int)],
            2 => [typeof(double[]), typeof(double[])],
            8 => [typeof(double[]), typeof(double[]), typeof(int), typeof(int), typeof(int), typeof(Hashtable), typeof(global::Quire.Mcd.Flags), typeof(int)],
            _ => throw new HarnessException($"Unsupported MatVal test fixture constructor with {args.Length} arguments")
        };
        bool isQuire = side.Name == "Quire";
        var ctor = (isQuire ? Q : M).GetConstructor(types.Select(t => isQuire ? t : OriginalType(t)).ToArray())
            ?? throw new HarnessException("MatVal fixture constructor not found");
        object?[] callArgs = args.ToArray();
        if (!isQuire && types.Length == 8)
            callArgs[6] = Enum.ToObject(Mathcad.Type("mplrun", "Mcd.Flags"), Convert.ToInt64(args[6]));
        return ctor.Invoke(callArgs);
    }

    private static string State(Side side, object value)
    {
        var data = Get(value, "Data")!;
        var pointers = Get(value, "m_parr") as Hashtable;
        string pointerState = pointers == null ? "null" : string.Join(";", pointers.Keys.Cast<int>().Order().Select(k => $"{k}:{side.Show(pointers[k])}"));
        string packed = Outcome(() => Get(value, "Packed"), side.Show);
        string isize = Outcome(() => Get(value, "Isize"), side.Show);
        return $"{Get(value, "Rows")}x{Get(value, "Cols")} stride {Get(value, "Anr")} offset {Get(value, "Offset")} isize {isize} packed {packed} flags {Convert.ToInt64(Get(data, "m_flags"))} dim {side.DimText(Get(value, "m_dim"))} real {side.Show(Get(value, "m_rarr"))} imag {side.Show(Get(value, "m_iarr"))} ptrs {pointerState}";
    }

    public static string Storage(Side side, object value) => State(side, value);

    private static Func<object, string> Observe(Side side) => value => State(side, value);
    private static Func<object?, string> Show(Side side) => value => value == null ? "null" : value is global::Quire.Mcd.MatVal || value.GetType().FullName == "Mcd.MatVal" ? State(side, value) : side.Show(value);

    private static string ShowConstructor(Side side, object? value, object?[] args)
    {
        if (value == null || !side.T("Mcd.MatVal").IsInstanceOfType(value)) return Show(side)(value);
        var aliases = new List<string>();
        int arrayIndex = 0;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] is double[])
            {
                string field = arrayIndex++ == 0 ? "m_rarr" : "m_iarr";
                aliases.Add($"arg{i}-{field}={ReferenceEquals(args[i], Get(value, field))}");
            }
            else if (args[i] is Hashtable)
                aliases.Add($"arg{i}-m_parr={ReferenceEquals(args[i], Get(value, "m_parr"))}");
        }
        return State(side, value) + " aliases[" + string.Join(",", aliases) + "]";
    }

    private static object[] MatrixArgs(int rows = 2, int cols = 2, int stride = 3, int offset = 1, long flags = 2146697216) =>
        [new double[offset + stride * cols], new double[offset + stride * cols], rows, cols, stride,
            new Hashtable(), Enum.ToObject(typeof(global::Quire.Mcd.Flags), flags), offset];

    private static void Compare(string id, MethodInfo q, MethodInfo m, object? qt, object? mt, object?[] args,
        Func<object?, string>? qShow = null, Func<object?, string>? mShow = null, Side? side = null)
    {
        MethodComparison.Compare(id, Contract, q, qt, args, m, mt, args.ToArray(), qShow, mShow,
            qt == null ? null : Observe(Side.Quire), mt == null ? null : Observe(Side.Mathcad));
    }

    [SkippableFact]
    public void Constructors_and_storage_queries()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var constructorCases = new (System.Type[] Types, object?[] Args)[]
        {
            ([], []), ([typeof(int)], [2]), ([typeof(int), typeof(int)], [2, 3]),
            ([typeof(int), typeof(int), typeof(bool)], [2, 3, true]), ([typeof(int), typeof(int), typeof(bool), typeof(int)], [2, 3, true, 4]),
            ([typeof(double[])], [new double[] { 1, 2, 3 }]), ([typeof(double[]), typeof(double[])], [new double[] { 1, 2 }, new double[] { 3, 4 }]),
            ([typeof(double[]), typeof(double[]), typeof(int)], [new double[] { 1, 2, 3 }, null, 2]),
            ([typeof(double[]), typeof(double[]), typeof(int), typeof(int)], [new double[] { 1, 2, 3 }, null, 2, 1]),
            ([typeof(double[]), typeof(double[]), typeof(int), typeof(int), typeof(int)], [new double[] { 1, 2, 3 }, null, 2, 1, 3]),
            ([typeof(double[]), typeof(double[]), typeof(int), typeof(int), typeof(int), typeof(Hashtable)], [new double[] { 1, 2, 3 }, null, 2, 1, 3, new Hashtable()]),
            ([typeof(double[]), typeof(double[]), typeof(int), typeof(int), typeof(int), typeof(Hashtable), typeof(global::Quire.Mcd.Flags), typeof(int)], MatrixArgs())
        };
        foreach (var (types, args) in constructorCases)
        {
            var (q, m) = Constructor(types);
            object?[] originalArgs = args.ToArray();
            if (types.Length == 8)
                originalArgs[6] = Enum.ToObject(Mathcad.Type("mplrun", "Mcd.Flags"), 2146697216);
            MethodComparison.Compare($"MatVal constructor ({string.Join(",", types.Select(t => t.Name))})", Contract,
                q, null, args, m, null, originalArgs,
                value => ShowConstructor(Side.Quire, value, args), value => ShowConstructor(Side.Mathcad, value, originalArgs));
        }

        var (qRows, mRows) = Method("get_Rows", PublicInstance);
        var (qCols, mCols) = Method("get_Cols", PublicInstance);
        var (qAnr, mAnr) = Method("get_Anr", PublicInstance);
        var (qOffset, mOffset) = Method("get_Offset", PublicInstance);
        var (qIsize, mIsize) = Method("get_Isize", PublicInstance);
        var (qPacked, mPacked) = Method("get_Packed", PublicInstance);
        foreach (var (name, q, m) in new[] { ("Rows", qRows, mRows), ("Cols", qCols, mCols), ("Anr", qAnr, mAnr), ("Offset", qOffset, mOffset), ("Isize", qIsize, mIsize), ("Packed", qPacked, mPacked) })
        {
            var qa = MatrixArgs();
            var ma = MatrixArgs();
            var qt = New(Side.Quire, qa);
            var mt = New(Side.Mathcad, ma);
            Compare($"get_{name} on strided matrix", q, m, qt, mt, []);
        }

        var (qHash, mHash) = Method("GetHashCode", PublicInstance);
        var qHashTarget = New(Side.Quire, 2, 3);
        var mHashTarget = New(Side.Mathcad, 2, 3);
        Compare("GetHashCode logical shape", qHash, mHash, qHashTarget, mHashTarget, []);

        var qIsizeTarget = New(Side.Quire, MatrixArgs(rows: 3, cols: 2, stride: 4, offset: 1));
        var mIsizeTarget = New(Side.Mathcad, MatrixArgs(rows: 3, cols: 2, stride: 4, offset: 1));
        Compare("get_Isize on non-square matrix", qIsize, mIsize, qIsizeTarget, mIsizeTarget, []);
    }

    [SkippableFact]
    public void Indexing_and_axis_arithmetic()
    {
        Runtime.Sync();
        var (qIndex1, mIndex1) = Method("Index", PublicInstance, typeof(int));
        var (qIndex2, mIndex2) = Method("Index", PublicInstance, typeof(int), typeof(int));
        var (qRow, mRow) = Method("RowOf", PublicInstance, typeof(int));
        var (qCol, mCol) = Method("ColOf", PublicInstance, typeof(int));
        foreach (int stride in new[] { 0, 3 })
        foreach (int index in new[] { -8, -1, 0, 1, 7 })
        {
            var qa = MatrixArgs(stride: stride);
            var ma = MatrixArgs(stride: stride);
            var qt = New(Side.Quire, qa);
            var mt = New(Side.Mathcad, ma);
            if (stride != 0)
            {
                Compare($"Index({index})", qIndex1, mIndex1, qt, mt, [index]);
                Compare($"Index({index},2)", qIndex2, mIndex2, qt, mt, [index, 2]);
                Compare($"RowOf({index})", qRow, mRow, qt, mt, [index]);
                Compare($"ColOf({index})", qCol, mCol, qt, mt, [index]);
            }
            else
            {
                Compare("RowOf with zero stride", qRow, mRow, qt, mt, [index]);
                Compare("ColOf with zero stride", qCol, mCol, qt, mt, [index]);
            }
        }
    }

    [SkippableFact]
    public void Fill_uses_product_for_extent()
    {
        Runtime.Sync();
        var (qFill, mFill) = Method("Fill", PublicInstance, typeof(object), typeof(int), typeof(int), typeof(int));
        var qa = MatrixArgs(rows: 2, cols: 1, stride: 2, offset: 0);
        var ma = MatrixArgs(rows: 2, cols: 1, stride: 2, offset: 0);
        var qt = New(Side.Quire, qa);
        var mt = New(Side.Mathcad, ma);

        // init + incr * (n - 1) is 8. Replacing multiplication with division
        // yields extent 3, which cannot hold the final fill index at 8.
        Compare("Fill product determines extent", qFill, mFill, qt, mt, ["fill", 0, 4, 3],
            Show(Side.Quire), Show(Side.Mathcad));

        var qUnitIncrement = New(Side.Quire, MatrixArgs(rows: 2, cols: 1, stride: 2, offset: 0));
        var mUnitIncrement = New(Side.Mathcad, MatrixArgs(rows: 2, cols: 1, stride: 2, offset: 0));
        // The positive-increment arm includes 1; its IL boundary is strictly greater than zero.
        Compare("Fill positive unit increment", qFill, mFill, qUnitIncrement, mUnitIncrement, ["fill", 0, 1, 3],
            Show(Side.Quire), Show(Side.Mathcad));
    }

    [SkippableFact]
    public void Imaginary_storage_flags_and_pointer_nan()
    {
        Runtime.Sync();
        var (qHasImag, mHasImag) = Method("get_HasImag", PublicInstance);
        var (qAlloc, mAlloc) = Method("AllocImaginary", PublicInstance);
        var (qImmutableGet, mImmutableGet) = Method("get_Immutable", PublicInstance);
        var (qImmutableSet, mImmutableSet) = Method("set_Immutable", PublicInstance, typeof(bool));
        var (qSharedGet, mSharedGet) = Method("get_Shared", PublicInstance);
        var (qSharedSet, mSharedSet) = Method("set_Shared", PublicInstance, typeof(bool));
        foreach (double[]? imaginary in new double[]?[] { null, [0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, double.NaN, 0, 0], [0, 0, 0, 0, 2, 0, 0] })
        {
            var qa = MatrixArgs();
            var ma = MatrixArgs();
            qa[1] = imaginary;
            ma[1] = imaginary?.ToArray();
            var qt = New(Side.Quire, qa);
            var mt = New(Side.Mathcad, ma);
            Compare("HasImag scans backing", qHasImag, mHasImag, qt, mt, []);
        }

        var qAllocTarget = New(Side.Quire, new double[] { 1, 2, 3 });
        var mAllocTarget = New(Side.Mathcad, new double[] { 1, 2, 3 });
        Compare("AllocImaginary allocates backing", qAlloc, mAlloc, qAllocTarget, mAllocTarget, []);
        foreach (var (getQ, getM, setQ, setM, name) in new[]
        {
            (qImmutableGet, mImmutableGet, qImmutableSet, mImmutableSet, "Immutable"),
            (qSharedGet, mSharedGet, qSharedSet, mSharedSet, "Shared")
        })
        {
            foreach (bool enabled in new[] { true, false })
            {
                var qt = New(Side.Quire, MatrixArgs());
                var mt = New(Side.Mathcad, MatrixArgs());
                Compare($"set_{name}({enabled})", setQ, setM, qt, mt, [enabled]);
                Compare($"get_{name} after set", getQ, getM, qt, mt, []);
            }
        }

        var qSharedBit = New(Side.Quire, MatrixArgs(flags: 4));
        var mSharedBit = New(Side.Mathcad, MatrixArgs(flags: 4));
        Compare("get_Shared exact bit", qSharedGet, mSharedGet, qSharedBit, mSharedBit, []);
        var qUnrelatedBit = New(Side.Quire, MatrixArgs(flags: 1));
        var mUnrelatedBit = New(Side.Mathcad, MatrixArgs(flags: 1));
        Compare("get_Shared ignores unrelated low bit", qSharedGet, mSharedGet, qUnrelatedBit, mUnrelatedBit, []);

        var (qPtrNan, mPtrNan) = Method("IsPtrNan", PublicStatic, typeof(double).MakeByRefType());
        foreach (long bits in new[] { -1L, unchecked((long)0xFFFFFFFF00000000UL), unchecked((long)0x7FF8000000000000UL), 0L })
        {
            object?[] qa = [BitConverter.Int64BitsToDouble(bits)];
            object?[] ma = [BitConverter.Int64BitsToDouble(bits)];
            MethodComparison.Compare($"IsPtrNan bits 0x{bits:X16}", Contract, qPtrNan, null, qa, mPtrNan, null, ma);
        }

        var (qPtrAt, mPtrAt) = Method("IsPtrNan", PrivateInstance, typeof(int), typeof(int));
        foreach (double payload in new[] { double.NaN, BitConverter.Int64BitsToDouble(-1L), 1.0 })
        {
            var qa = MatrixArgs();
            var ma = MatrixArgs();
            ((double[])qa[0]!)[1] = payload;
            ((double[])ma[0]!)[1] = payload;
            var qt = New(Side.Quire, qa);
            var mt = New(Side.Mathcad, ma);
            Compare("private IsPtrNan(row,column)", qPtrAt, mPtrAt, qt, mt, [0, 0]);
        }

        var (qThePtrNan, mThePtrNan) = Method("ThePtrNan", PublicStatic);
        MethodComparison.Compare("ThePtrNan sentinel", Contract, qThePtrNan, null, [], mThePtrNan, null, []);
    }
}

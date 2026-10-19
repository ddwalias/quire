#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    private partial struct arg_desc
    {
        [MirrorAnalyzed("Mcd.MatVal.Collections")] public int m_pos;
        [MirrorAnalyzed("Mcd.MatVal.Collections")] public MatVal m_mat;
        [MirrorAnalyzed("Mcd.MatVal.Collections")]
        public void FetchInto(object[] args, int i, int j) => args[m_pos] = m_mat.get_2(i, j);
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal(RngVal r) : this(r.Rsize)
    {
        IsRange = true;
        IsIntegral = r.Integral;
        m_dim = r.m_dim;
        for (int i = 0; i < r.Rsize; ++i) m_rarr[i] = r.get(i);
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal(DimVal first, DimVal last) : this(RngVal.mkRange2(first, last)) { }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal(DimVal first, DimVal next, DimVal last) : this(RngVal.mkRange3(first, next, last)) { }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal(object[] vals) : this(vals.Length, 1)
    {
        Data.m_rows = 0;
        foreach (object value in vals) PushRaw(value);
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal MakeVector(object[] vals) => new(vals);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal MakeVector0(object[] vals)
    {
        int num = vals.Length;
        MatVal matVal = reserve(num);
        for (int i = 0; i < num; i++)
        {
            object obj = vals[i];
            matVal.PushRaw(obj == NumVal.Zero ? NumVal.ZeroId : obj);
        }
        return matVal;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal reserve(int n) { var result = new MatVal(n); result.Data.m_rows = 0; return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal reserve(int r, int c) { var result = new MatVal(r, c); result.Data.m_rows = 0; return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal Singleton(object o) { var result = new MatVal(1); result.set(0, o); return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal Pair(object o1, object o2) { var result = reserve(2); result.PushRaw(o1); result.PushRaw(o2); return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal Triple(object o1, object o2, object o3) { var result = reserve(3); result.PushRaw(o1); result.PushRaw(o2); result.PushRaw(o3); return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal Push(object o) => mcd_set_1(Data.m_rows, o);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushRaw1(object o) { set(Data.m_rows++, o); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushRaw(object o) { setNaN(Data.m_rows++, o); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushRaw(double re) { set(Data.m_rows++, re, Dim.identity); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushRaw(int re) { set(Data.m_rows++, re, Dim.identity); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushReal(double re) { set(Data.m_rows++, re); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushZero() { set(Data.m_rows++, 0.0, Dim.mcd_zero); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public int PushRawIndex(object o) { set(Data.m_rows++, o); return Data.m_rows; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushVectorArg(object o)
    {
        Cons cons = o as Cons;
        for (;;)
        {
            if (cons == null) AppException.Raise("must be vector", 1u);
            setspec(Data.m_rows++, cons.Field1);
            if (cons.Field2 == null)
            {
                break;
            }
            cons = cons.Field2 as Cons;
        }
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal PromoteDim() { m_dim ??= Dim.identity; return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal PromoteDim(Dim d) { m_dim ??= Dim.intern(d); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal Replicate(object o, int n) => new MatVal(n).FillThis(o, 0, 1, n).PromoteDim();
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal Reshape(int r, int c) { Data.m_rows = r; Data.m_anr = r; Data.m_cols = c; return PromoteDim(); }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal SetShape() { Data.m_rows = Data.m_anr; if (m_parr != null) Canonicalize(); return PromoteDim(); }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal SetShape1() { Data.m_rows = Data.m_anr; return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal map(F<object, object> f, DimVal collection)
    {
        int isize = collection.Isize;
        MatVal matVal = reserve(isize);
        for (int i = 0; i < isize; i++)
        {
            matVal.Push(f(collection.iterator_get(i)));
        }
        if (collection is MatVal matVal2)
        {
            matVal = matVal.Reshape(matVal2.Rows, matVal2.Cols);
        }
        return matVal;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal map(Function func, DimVal collection) => map(func.AsDelegate_1(), collection);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal map(F<object, object, object> f, DimVal rvals, DimVal cvals)
    {
        int rows = rvals.Isize, cols = cvals.Isize;
        var result = reserve(rows * cols);
        for (int c = 0; c < cols; ++c)
        {
            var columnValue = cvals.iterator_get(c);
            for (int r = 0; r < rows; ++r) result.Push(f(rvals.iterator_get(r), columnValue));
        }
        return result.Reshape(rows, cols);
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal map(Function func, DimVal rvals, DimVal cvals) => map(func.AsDelegate_2(), rvals, cvals);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static object[] mapv(F<object, object> f, DimVal collection)
    {
        int length = collection.Isize;
        var result = new object[length];
        for (int i = 0; i < length; ++i) result[i] = f(collection.iterator_get(i));
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object[] as_object_vector(DimVal collection)
    {
        int length = collection.Isize;
        var result = new object[length];
        for (int i = 0; i < length; ++i) result[i] = collection.iterator_get(i);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object mkRange2(object first, object last) => new MatVal(first as DimVal, last as DimVal);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object mkRange3(object first, object next, object last) => new MatVal(first as DimVal, next as DimVal, last as DimVal);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object vectorize(Function f, object[] args)
    {
        int argc = args.Length;
        arg_desc[] descriptors = null;
        int mat_count = 0;
        int mat_rows = 0;
        int mat_cols = 0;
        bool all_polymorphic = true;
        f.AssertFullApp(argc);
        for (int i = 0; i < argc; i++)
        {
            if (args[i] is MatVal matVal)
            {
                if (mat_count == 0)
                {
                    descriptors = new arg_desc[argc - i];
                    mat_rows = matVal.Rows;
                    mat_cols = matVal.Cols;
                }
                else if (mat_rows != matVal.Rows || mat_cols != matVal.Cols) AppException.Raise("unequal matrix dimensions");
                descriptors[mat_count].m_mat = matVal;
                descriptors[mat_count].m_pos = i;
                mat_count++;
                if (!matVal.IsPolymorphic)
                {
                    all_polymorphic = false;
                }
            }
        }
        if (mat_count == 0)
        {
            return f.Apply(args);
        }
        MatVal result = new MatVal(mat_rows, mat_cols);
        for (int j = 0; j < mat_cols; j++)
        {
            for (int k = 0; k < mat_rows; k++)
            {
                for (int l = 0; l < mat_count; l++)
                {
                    descriptors[l].FetchInto(args, k, j);
                }
                result.set_2(k, j, f.Apply(args));
            }
        }
        if (result.IsPolymorphic && result.m_dim == null && !all_polymorphic)
        {
            result.m_dim = Dim.identity;
        }
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal Fill(object v, int init, int incr, int n)
    {
        int extent = (incr > 0 ? init + incr * (n - 1) : init) + 1;
        if (Immutable) return ReAllocate(extent).FillThis(v, init, incr, n);
        Stretch(extent);
        return FillThis(v, init, incr, n);
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal FillThis(object o, int init, int incr, int n)
    {
        if (o is NumVal x)
        {
            return FillThis(x, init, incr, n);
        }
        return FillThisPtrs(o, init, incr, n);
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal FillThis(NumVal x, int init, int incr, int n)
    {
        int start = init + m_offset;
        if (m_iarr == null)
        {
            if (x.Data.ip == 0) { FillArr(m_rarr, x.Data.rp, start, incr, n); return this; }
            AllocImaginary();
        }
        FillArr(m_rarr, x.Data.rp, start, incr, n);
        FillArr(m_iarr, x.Data.ip, start, incr, n);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal FillThisPtrs(object p, int init, int incr, int n)
    {
        int num = init + m_offset;
        for (int i = 0; i < n; i++)
        {
            set_ptrs_at(num, p);
            num += incr;
        }
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private static void FillArr(double[] arr, double v, int init, int incr, int n)
    {
        if (incr == 1)
        {
            int end = init + n;
            for (int i = init; i < end; i++)
            {
                arr[i] = v;
            }
            return;
        }
        int idx = init;
        for (int j = 0; j < n; j++)
        {
            arr[idx] = v;
            idx += incr;
        }
    }
}

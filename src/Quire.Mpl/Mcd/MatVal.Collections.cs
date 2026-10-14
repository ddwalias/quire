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
        var result = reserve(vals.Length);
        foreach (object value in vals) result.PushRaw(ReferenceEquals(value, NumVal.Zero) ? NumVal.ZeroId : value);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal reserve(int n) { var result = new MatVal(n); result.Data.m_rows = 0; return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal reserve(int r, int c) { var result = new MatVal(r, c); result.Data.m_rows = 0; return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal Singleton(object o) { var result = new MatVal(1); result.set(0, o); return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal Pair(object o1, object o2) { var result = reserve(2); result.PushRaw(o1); result.PushRaw(o2); return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal Triple(object o1, object o2, object o3) { var result = reserve(3); result.PushRaw(o1); result.PushRaw(o2); result.PushRaw(o3); return result; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal Push(object o) => mcd_set_1(Data.m_rows, o);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushRaw1(object o) { set(Data.m_rows++, o); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushRaw(object o) { setNaN(Data.m_rows++, o); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushRaw(double re) { set(Data.m_rows++, re, Dim.identity); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushRaw(int re) { set(Data.m_rows++, re, Dim.identity); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushReal(double re) { set(Data.m_rows++, re); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushZero() { set(Data.m_rows++, 0.0, Dim.mcd_zero); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public int PushRawIndex(object o) { set(Data.m_rows++, o); return Data.m_rows; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushVectorArg(object o)
    {
        var node = o as Cons;
        while (true)
        {
            if (node == null) AppException.Raise("must be vector", 1u);
            setspec(Data.m_rows++, node.Field1);
            if (node.Field2 == null) return this;
            node = node.Field2 as Cons;
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal PromoteDim() { if (m_dim == null) m_dim = Dim.identity; return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal PromoteDim(Dim d) { if (m_dim == null) m_dim = Dim.intern(d); return this; }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal Replicate(object o, int n) => new MatVal(n).FillThis(o, 0, 1, n).PromoteDim();
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal Reshape(int r, int c) { Data.m_rows = r; Data.m_anr = r; Data.m_cols = c; return PromoteDim(); }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal SetShape() { Data.m_rows = Data.m_anr; if (m_parr != null) Canonicalize(); return PromoteDim(); }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal SetShape1() { Data.m_rows = Data.m_anr; return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal map(F<object, object> f, DimVal collection)
    {
        int length = collection.Isize;
        var result = reserve(length);
        for (int i = 0; i < length; ++i) result.Push(f(collection.iterator_get(i)));
        return collection is MatVal matrix ? result.Reshape(matrix.Rows, matrix.Cols) : result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
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
    public static object[] as_object_vector(DimVal collection)
    {
        int length = collection.Isize;
        var result = new object[length];
        for (int i = 0; i < length; ++i) result[i] = collection.iterator_get(i);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static object mkRange2(object first, object last) => new MatVal(first as DimVal, last as DimVal);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static object mkRange3(object first, object next, object last) => new MatVal(first as DimVal, next as DimVal, last as DimVal);
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static object vectorize(Function f, object[] args)
    {
        f.AssertFullApp(args.Length);
        arg_desc[] matrices = null;
        int count = 0, rows = 0, cols = 0;
        bool allPolymorphic = true;
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i] is not MatVal matrix) continue;
            if (count == 0) { matrices = new arg_desc[args.Length - i]; rows = matrix.Rows; cols = matrix.Cols; }
            else if (rows != matrix.Rows || cols != matrix.Cols) AppException.Raise("unequal matrix dimensions");
            matrices[count++] = new arg_desc { m_pos = i, m_mat = matrix };
            if (!matrix.IsPolymorphic) allPolymorphic = false;
        }
        if (count == 0) return f.Apply(args);
        var result = new MatVal(rows, cols);
        for (int c = 0; c < cols; ++c)
            for (int r = 0; r < rows; ++r)
            {
                for (int i = 0; i < count; ++i) matrices[i].FetchInto(args, r, c);
                result.set_2(r, c, f.Apply(args));
            }
        if (result.IsPolymorphic && result.m_dim == null && !allPolymorphic) result.m_dim = Dim.identity;
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
    private MatVal FillThis(object o, int init, int incr, int n) => o is NumVal number ? FillThis(number, init, incr, n) : FillThisPtrs(o, init, incr, n);
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
        int index = init + m_offset;
        for (int i = 0; i < n; ++i, index += incr) set_ptrs_at(index, p);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private static void FillArr(double[] arr, double v, int init, int incr, int n)
    {
        if (incr == 1)
        {
            int end = init + n;
            for (int i = init; i < end; ++i) arr[i] = v;
        }
        else for (int i = 0, index = init; i < n; ++i, index += incr) arr[index] = v;
    }
}

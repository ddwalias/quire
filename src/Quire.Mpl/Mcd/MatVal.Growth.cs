#nullable disable
using System.Collections;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private MatVal(MatVal source, int rows, int cols)
        : this(source.CopyResize(source.m_rarr, rows, cols), source.m_iarr == null ? null : source.CopyResize(source.m_iarr, rows, cols), Math.Max(rows, source.Rows), Math.Max(cols, source.Cols))
    {
        m_dim = source.m_dim;
        if (source.RepHasPtrs) InitPtrs(source);
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal realloc(object o, int i, int j)
    {
        MatVal matVal = o as MatVal;
        i = Math.Max(0, i);
        j = Math.Max(0, j);
        if (matVal == null)
        {
            return new MatVal(0, j + 1, false, i + 1);
        }
        MatVal result = new MatVal(matVal, i + 1, j + 1);
        int cols = result.Data.m_cols;
        int rows = result.Data.m_rows;
        result.Data.m_rows = matVal.Data.m_rows;
        result.Data.m_cols = matVal.Data.m_cols;
        result.TidyZeros(matVal.Data.m_rows, matVal.Data.m_cols, rows, cols);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal ReAlloc(object o, int r)
    {
        MatVal matVal = o as MatVal;
        if (matVal != null)
        {
            return matVal.ReAllocate(r);
        }
        return new MatVal(r);
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal ReAlloc(object o, int r, int c)
    {
        MatVal matVal = o as MatVal;
        if (matVal != null)
        {
            return matVal.ReAllocate(r, c);
        }
        return new MatVal(r, c);
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal ReAllocate(int r) => new(this, Math.Max(r, Rows), 1);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal ReAllocate(int r, int c) => new(this, Math.Max(r, Rows), Math.Max(c, Cols));

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private double[] Resize(double[] arr, int nr)
    {
        var result = new double[nr];
        Copy(arr, Offset, result, 0, Rows);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private double[] CopyResize(double[] arr, int nr, int nc) => Resize(arr, Math.Max(nr, Rows), Math.Max(nc, Cols));
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private double[] Resize(double[] arr, int nr, int nc)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        int offset = Offset;
        double[] array = new double[nr * nc];
        if (nr == anr)
        {
            Copy(arr, offset, array, 0, nr * cols);
        }
        else
        {
            for (int i = 0; i < cols; i++)
            {
                Copy(arr, offset + anr * i, array, nr * i, rows);
            }
        }
        return array;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void Stretch(int n)
    {
        if (n > Data.m_rows)
        {
            if (n > Data.m_anr)
            {
                int newAnr = Math.Max(n, 2 * Data.m_anr);
                m_rarr = Resize(m_rarr, newAnr);
                m_iarr = (m_iarr == null) ? null : Resize(m_iarr, newAnr);
                if (RepHasPtrs)
                {
                    int offset = m_offset;
                    int limit = offset + n;
                    for (int i = offset; i < limit; i++)
                    {
                        if (i - offset >= Data.m_rows && !IsPtrNan(ref m_rarr[i]) && m_rarr[i] == 0.0)
                        {
                            if (m_parr == null)
                            {
                                m_parr = new Hashtable();
                            }
                            m_rarr[i] = ThePtrNan();
                            m_parr[i] = NumVal.ZeroId;
                        }
                    }
                }
                Data.m_anr = newAnr;
            }
            Data.m_rows = n;
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void TidyZeros(int oldr, int oldc, int newr, int newc)
    {
        if (RepHasPtrs)
        {
            for (int i = 0; i < newc; i++)
            {
                int start = i * Data.m_anr + m_offset;
                int limit = start + newr;
                for (int j = start; j < limit; j++)
                {
                    if ((i >= oldc || j - start >= oldr) && !IsPtrNan(ref m_rarr[j]) && m_rarr[j] == 0.0)
                    {
                        if (m_dim != Dim.identity && m_dim != null)
                        {
                            if (m_parr == null)
                            {
                                m_parr = new Hashtable();
                            }
                            m_rarr[j] = ThePtrNan();
                            m_parr[j] = NumVal.ZeroId;
                        }
                        else if (m_dim == null)
                        {
                            m_dim = Dim.identity;
                        }
                    }
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void Stretch(int r, int c)
    {
        int oldCols = Data.m_cols;
        int oldRows = Data.m_rows;
        if (c > Data.m_cols)
        {
            if (m_rarr == null || c * Data.m_anr <= m_rarr.Length)
            {
                Data.m_cols = c;
            }
            else
            {
                int capacity = Math.Max(r, Rows);
                if (m_parr != null && capacity != Data.m_anr)
                {
                    m_parr = ResizePtrs(capacity, m_offset);
                }
                m_rarr = Resize(m_rarr, capacity, c);
                m_iarr = (m_iarr == null) ? null : Resize(m_iarr, capacity, c);
                Data.m_rows = capacity;
                Data.m_cols = c;
                Data.m_anr = capacity;
                TidyZeros(oldRows, oldCols, Data.m_rows, Data.m_cols);
                return;
            }
        }
        if (r > Data.m_rows)
        {
            if (r > Data.m_anr)
            {
                int newCols = Math.Max(c, Cols);
                int capacity = Math.Max(r, Math.Min(2 * Data.m_anr, 16777216 / (newCols == 0 ? 1 : newCols)));
                if (m_parr != null)
                {
                    m_parr = ResizePtrs(capacity, m_offset);
                }
                m_rarr = Resize(m_rarr, capacity, newCols);
                m_iarr = (m_iarr == null) ? null : Resize(m_iarr, capacity, newCols);
                Data.m_cols = newCols;
                Data.m_anr = capacity;
                TidyZeros(oldRows, oldCols, r, Data.m_cols);
            }
            Data.m_rows = r;
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public void StretchWithColumnFill(int r, int c, DimVal fill)
    {
        int oldCols = Cols;
        int rows = Rows;
        Stretch(r, c);
        for (int i = oldCols; i < c; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                set_2(j, i, fill);
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public override MatVal extend_1(int i)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (Data.m_cols > 1) AppException.Raise("must be vector", 1u);
        if (Immutable) return ReAllocate(i + 1);
        Stretch(i + 1);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public override MatVal extend_2(int i, int j)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        if (Immutable) return ReAllocate(i + 1, j + 1);
        Stretch(i + 1, j + 1);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setm_1(object o, int i, object v) => v is DimVal ? (o as DimVal).matrix_set_1(i, v) : (o as DimVal).matrix_set_1_ob(i, v);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setm_2(object o, int i, int j, object v) => v is DimVal ? (o as DimVal).matrix_set_2(i, j, v) : (o as DimVal).matrix_set_2_ob(i, j, v);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_set_1(int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (Data.m_cols > 1) AppException.Raise("must be vector", 1u);
        if (Immutable) { var copy = ReAllocate(i + 1); copy.set_1(i, v); return copy; }
        Stretch(i + 1);
        set(i + m_offset, v);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_set_2(int i, int j, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        if (Immutable) { var copy = ReAllocate(i + 1, j + 1); copy.set_2(i, j, v); return copy; }
        Stretch(i + 1, j + 1);
        set(Index(i, j), v);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_1(int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        Stretch(i + 1);
        set(i + m_offset, v);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_1(int i, double re)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        Stretch(i + 1);
        set(i + m_offset, re, Dim.identity);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_2(int i, int j, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        Stretch(i + 1, j + 1);
        set(Index(i, j), v);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_2(int i, int j, double re)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        Stretch(i + 1, j + 1);
        set(Index(i, j), re, Dim.identity);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setcolumn(object o, int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        MatVal matVal = o as MatVal;
        if (matVal == null)
        {
            MatVal matVal2 = v as MatVal;
            if (matVal2 == null)
            {
                matVal2 = Singleton(v);
            }
            int rows = matVal2.Rows;
            MatVal matVal3 = new MatVal(rows, i + 1);
            matVal3.setcolumn(i, matVal2);
            return matVal3;
        }
        if (matVal.Immutable)
        {
            MatVal matVal4 = v as MatVal;
            if (matVal4 == null)
            {
                matVal4 = Singleton(v);
            }
            int rows2 = matVal4.Rows;
            MatVal matVal5 = matVal.ReAllocate(rows2, i + 1);
            matVal5.setcolumn(i, matVal4);
            return matVal5;
        }
        matVal.setcolumn(i, v);
        return matVal;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setcolumn(int i, object o)
    {
        MatVal matVal = o as MatVal;
        if (matVal == null)
        {
            matVal = Singleton(o);
        }
        int rows = matVal.Rows;
        if (!matVal.IsVector)
        {
            AppException.Raise("must be vector", 2u);
        }
        Stretch(rows, i + 1);
        setcol(i, matVal);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public void setcol(int i, MatVal v)
    {
        if (v.m_dim != m_dim)
        {
            if (m_dim == null)
            {
                m_dim = v.m_dim;
            }
            else
            {
                setmatrix_ptrs(0, i, v);
                return;
            }
        }
        int rows = v.Rows;
        int destOffset = m_offset + i * Data.m_anr;
        Copy(v.m_rarr, v.m_offset, m_rarr, destOffset, rows);
        if (v.m_iarr == null)
        {
            if (m_iarr != null)
            {
                FillArr(m_iarr, 0.0, destOffset, 1, rows);
            }
        }
        else
        {
            if (m_iarr == null)
            {
                AllocImaginary();
            }
            Copy(v.m_iarr, v.m_offset, m_iarr, destOffset, rows);
        }
        if (v.RepHasPtrs)
        {
            if (m_parr == null)
            {
                m_parr = new Hashtable();
            }
            for (int j = 0; j < rows; j++)
            {
                if (IsPtrNan(ref m_rarr[j + destOffset]))
                {
                    m_parr[destOffset + j] = v.m_parr[j + v.m_offset];
                }
            }
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setrow(object o, int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        MatVal matVal = o as MatVal;
        if (matVal == null)
        {
            MatVal matVal2 = v as MatVal;
            if (matVal2 == null)
            {
                matVal2 = Singleton(v);
            }
            int cols = matVal2.Cols;
            MatVal matVal3 = new MatVal(i + 1, cols);
            matVal3.setrow(i, matVal2);
            return matVal3;
        }
        if (matVal.Immutable)
        {
            MatVal matVal4 = v as MatVal;
            if (matVal4 == null)
            {
                matVal4 = Singleton(v);
            }
            int cols2 = matVal4.Cols;
            MatVal matVal5 = matVal.ReAllocate(i + 1, cols2);
            matVal5.setrow(i, matVal4);
            return matVal5;
        }
        matVal.setrow(i, v);
        return matVal;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setrow(int i, object o) => setblock(i, 0, o);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setblock(int i, int j, object v)
    {
        MatVal matVal = v as MatVal;
        if (matVal == null)
        {
            Stretch(i + 1, j + 1);
            set_2(i, j, v);
        }
        else
        {
            Stretch(i + matVal.Rows, j + matVal.Cols);
            setmatrix(i, j, matVal);
        }
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void setmatrix(int i, int j, MatVal v)
    {
        if (v.m_dim != m_dim)
        {
            if (m_dim == null)
            {
                m_dim = v.m_dim;
            }
            else
            {
                setmatrix_ptrs(i, j, v);
                return;
            }
        }
        int rows = v.Rows;
        int cols = v.Cols;
        int anr = Anr;
        int anr2 = v.Anr;
        int destOffset = m_offset + i + j * anr;
        int srcOffset = v.m_offset;
        if (v.m_iarr != null && m_iarr == null)
        {
            AllocImaginary();
        }
        for (int k = 0; k < cols; k++)
        {
            Copy(v.m_rarr, srcOffset, m_rarr, destOffset, rows);
            if (v.m_iarr == null)
            {
                if (m_iarr != null)
                {
                    FillArr(m_iarr, 0.0, destOffset, 1, rows);
                }
            }
            else
            {
                Copy(v.m_iarr, srcOffset, m_iarr, destOffset, rows);
            }
            srcOffset += anr2;
            destOffset += anr;
        }
        if (v.RepHasPtrs)
        {
            AssignPtrs(v, i, j);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void setmatrix_ptrs(int ii, int jj, MatVal v)
    {
        int rows = v.Rows;
        int cols = v.Cols;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                set_2(i + ii, j + jj, v.get_2(i, j));
            }
        }
    }

    private partial class AssignMatrixPtrs
    {
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private MatVal m_m;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_r;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_c;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public AssignMatrixPtrs(MatVal m, int r, int c) { m_m = m; m_r = r; m_c = c; }
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public void assign_matrix_ptrs(int i, int j, object v) => m_m.set_ptrs_2(i + m_r, j + m_c, v);
    }
    private partial class ResizeMatrixPtrs
    {
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private Hashtable m_parr;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_anr;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_off;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public ResizeMatrixPtrs(Hashtable parr, int anr, int off) { m_parr = parr; m_anr = anr; m_off = off; }
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public void resize_matrix_ptrs(int i, int j, object v) => m_parr[m_off + i + j * m_anr] = v;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void AssignPtrs(MatVal m, int r, int c) => TransformPtrs(m, new AssignMatrixPtrs(this, r, c).assign_matrix_ptrs);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private Hashtable ResizePtrs(int anr, int off)
    {
        Hashtable hashtable = new Hashtable();
        ResizeMatrixPtrs resizeMatrixPtrs = new ResizeMatrixPtrs(hashtable, anr, off);
        IteratePtrs(new PtrDelegate(resizeMatrixPtrs.resize_matrix_ptrs));
        if (hashtable.Count <= 0)
        {
            return null;
        }
        return hashtable;
    }
}

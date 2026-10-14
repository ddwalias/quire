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
        i = Math.Max(0, i);
        j = Math.Max(0, j);
        if (o is not MatVal source) return new MatVal(0, j + 1, false, i + 1);
        var result = new MatVal(source, i + 1, j + 1);
        int allocatedRows = result.Rows, allocatedCols = result.Cols;
        result.Data.m_rows = source.Rows;
        result.Data.m_cols = source.Cols;
        result.TidyZeros(source.Rows, source.Cols, allocatedRows, allocatedCols);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal ReAlloc(object o, int r) => o is MatVal m ? m.ReAllocate(r) : new MatVal(r);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal ReAlloc(object o, int r, int c) => o is MatVal m ? m.ReAllocate(r, c) : new MatVal(r, c);
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
        var result = new double[nr * nc];
        if (nr == Anr) Copy(arr, Offset, result, 0, nr * Cols);
        else for (int col = 0; col < Cols; col++) Copy(arr, Offset + col * Anr, result, col * nr, Rows);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void Stretch(int n)
    {
        if (n <= Rows) return;
        if (n > Anr)
        {
            int capacity = Math.Max(n, 2 * Anr);
            m_rarr = Resize(m_rarr, capacity);
            if (m_iarr != null) m_iarr = Resize(m_iarr, capacity);
            if (RepHasPtrs)
            {
                for (int index = Offset; index < Offset + n; index++)
                {
                    if (index - Offset < Rows || IsPtrNan(ref m_rarr[index]) || m_rarr[index] != 0) continue;
                    m_parr ??= new Hashtable();
                    m_rarr[index] = ThePtrNan();
                    m_parr[index] = NumVal.ZeroId;
                }
            }
            Data.m_anr = capacity;
        }
        Data.m_rows = n;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void TidyZeros(int oldr, int oldc, int newr, int newc)
    {
        if (!RepHasPtrs) return;
        for (int col = 0; col < newc; col++)
        for (int row = 0; row < newr; row++)
        {
            int index = Index(row, col);
            if (col < oldc && row < oldr || IsPtrNan(ref m_rarr[index]) || m_rarr[index] != 0) continue;
            if (m_dim != null && m_dim != Dim.identity)
            {
                m_parr ??= new Hashtable();
                m_rarr[index] = ThePtrNan();
                m_parr[index] = NumVal.ZeroId;
            }
            else if (m_dim == null) m_dim = Dim.identity;
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void Stretch(int r, int c)
    {
        int oldRows = Rows, oldCols = Cols;
        if (c > Cols)
        {
            if (m_rarr != null && c * Anr > m_rarr.Length)
            {
                int capacity = Math.Max(r, Rows);
                if (m_parr != null && capacity != Anr) m_parr = ResizePtrs(capacity, Offset);
                m_rarr = Resize(m_rarr, capacity, c);
                if (m_iarr != null) m_iarr = Resize(m_iarr, capacity, c);
                Data.m_rows = capacity;
                Data.m_cols = c;
                Data.m_anr = capacity;
                TidyZeros(oldRows, oldCols, Rows, Cols);
                return;
            }
            Data.m_cols = c;
        }
        if (r <= Rows) return;
        if (r > Anr)
        {
            int newCols = Math.Max(c, Cols);
            int capacity = Math.Max(r, Math.Min(2 * Anr, 16777216 / (newCols == 0 ? 1 : newCols)));
            if (m_parr != null) m_parr = ResizePtrs(capacity, Offset);
            m_rarr = Resize(m_rarr, capacity, newCols);
            if (m_iarr != null) m_iarr = Resize(m_iarr, capacity, newCols);
            Data.m_cols = newCols;
            Data.m_anr = capacity;
            TidyZeros(oldRows, oldCols, r, Cols);
        }
        Data.m_rows = r;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public void StretchWithColumnFill(int r, int c, DimVal fill)
    {
        int oldRows = Rows, oldCols = Cols;
        Stretch(r, c);
        for (int col = oldCols; col < c; col++)
        for (int row = 0; row < oldRows; row++) set_2(row, col, fill);
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public override MatVal extend_1(int i)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (Cols > 1) AppException.Raise("must be vector", 1u);
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
        if (Cols > 1) AppException.Raise("must be vector", 1u);
        if (Immutable) { var copy = ReAllocate(i + 1); copy.set_1(i, v); return copy; }
        Stretch(i + 1);
        set(i + Offset, v);
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
        set(i + Offset, v);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_1(int i, double re)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        Stretch(i + 1);
        set(i + Offset, re, Dim.identity);
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
        if (o is not MatVal matrix)
        {
            var value = v as MatVal ?? Singleton(v);
            return new MatVal(value.Rows, i + 1).setcolumn(i, value);
        }
        if (matrix.Immutable)
        {
            var value = v as MatVal ?? Singleton(v);
            return matrix.ReAllocate(value.Rows, i + 1).setcolumn(i, value);
        }
        matrix.setcolumn(i, v);
        return matrix;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setcolumn(int i, object o)
    {
        var value = o as MatVal ?? Singleton(o);
        if (!value.IsVector) AppException.Raise("must be vector", 2u);
        Stretch(value.Rows, i + 1);
        setcol(i, value);
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public void setcol(int i, MatVal v)
    {
        if (v.m_dim != m_dim)
        {
            if (m_dim != null) { setmatrix_ptrs(0, i, v); return; }
            m_dim = v.m_dim;
        }
        int start = Offset + i * Anr;
        Copy(v.m_rarr, v.Offset, m_rarr, start, v.Rows);
        if (v.m_iarr == null)
        {
            if (m_iarr != null) FillArr(m_iarr, 0, start, 1, v.Rows);
        }
        else
        {
            if (m_iarr == null) AllocImaginary();
            Copy(v.m_iarr, v.Offset, m_iarr, start, v.Rows);
        }
        if (!v.RepHasPtrs) return;
        m_parr ??= new Hashtable();
        for (int row = 0; row < v.Rows; row++)
            if (IsPtrNan(ref m_rarr[start + row])) m_parr[start + row] = v.m_parr[v.Offset + row];
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setrow(object o, int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (o is not MatVal matrix)
        {
            var value = v as MatVal ?? Singleton(v);
            return new MatVal(i + 1, value.Cols).setrow(i, value);
        }
        if (matrix.Immutable)
        {
            var value = v as MatVal ?? Singleton(v);
            return matrix.ReAllocate(i + 1, value.Cols).setrow(i, value);
        }
        matrix.setrow(i, v);
        return matrix;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setrow(int i, object o) => setblock(i, 0, o);
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setblock(int i, int j, object v)
    {
        if (v is MatVal matrix)
        {
            Stretch(i + matrix.Rows, j + matrix.Cols);
            setmatrix(i, j, matrix);
        }
        else
        {
            Stretch(i + 1, j + 1);
            set_2(i, j, v);
        }
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void setmatrix(int i, int j, MatVal v)
    {
        if (v.m_dim != m_dim)
        {
            if (m_dim != null) { setmatrix_ptrs(i, j, v); return; }
            m_dim = v.m_dim;
        }
        if (v.m_iarr != null && m_iarr == null) AllocImaginary();
        for (int col = 0; col < v.Cols; col++)
        {
            int destination = Index(i, j + col), source = v.Index(0, col);
            Copy(v.m_rarr, source, m_rarr, destination, v.Rows);
            if (v.m_iarr != null) Copy(v.m_iarr, source, m_iarr, destination, v.Rows);
            else if (m_iarr != null) FillArr(m_iarr, 0, destination, 1, v.Rows);
        }
        if (v.RepHasPtrs) AssignPtrs(v, i, j);
    }
    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void setmatrix_ptrs(int ii, int jj, MatVal v)
    {
        for (int row = 0; row < v.Rows; row++)
        for (int col = 0; col < v.Cols; col++) set_2(row + ii, col + jj, v.get_2(row, col));
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
        var result = new Hashtable();
        IteratePtrs(new ResizeMatrixPtrs(result, anr, off).resize_matrix_ptrs);
        return result.Count == 0 ? null : result;
    }
}

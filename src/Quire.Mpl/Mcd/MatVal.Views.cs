#nullable disable
using System.Collections;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private MatVal(MatVal source, int column)
        : this(source.m_rarr, source.m_iarr, source.Rows, 1, source.Anr, null, Flags.StringTag, source.Index(0, column))
    {
        m_dim = source.m_dim;
        source.Shared = Shared = true;
        if (source.m_parr != null) TrimPtrs(source.m_parr);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void TrimPtrs(Hashtable pointers)
    {
        for (int col = 0; col < Cols; col++)
        for (int row = 0; row < Rows; row++)
        {
            int index = Index(row, col);
            if (IsPtrNan(ref m_rarr[index])) (m_parr ??= new Hashtable())[index] = pointers[index];
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Column(int column) => IsUniformScalar & Immutable
        ? new MatVal(this, column)
        : new MatVal(PackReal(column), PackImag(column), Rows, 1, Rows, PackPtrs(column)).dvm(m_dim);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public double[] real_column(int column)
    {
        if (IsUniformScalar) return PackReal(column);
        var result = new double[Rows];
        for (int row = 0; row < Rows; row++) result[row] = get_real_mixed(row, column);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_sym_column(object value, int column)
    {
        if (value is not MatVal matrix) { AppException.Raise("must be array"); return null; }
        if (column < 0 || column >= matrix.Cols) AppException.Raise("index out of bounds");
        return matrix.Column(column);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_column(object value, int column)
    {
        if (value is not MatVal matrix) { AppException.Raise("must be array", 1u); return null; }
        if (column < 0 || column >= matrix.Cols) AppException.Raise("index out of bounds", 2u);
        return matrix.Column(column);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_sym_row(object value, int row)
    {
        if (value is not MatVal matrix) { AppException.Raise("must be array"); return null; }
        if (row < 0 || row >= matrix.Rows) AppException.Raise("index out of bounds");
        return matrix.Row(row);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_row(object value, int row)
    {
        if (value is not MatVal matrix) { AppException.Raise("must be array", 1u); return null; }
        if (row < 0 || row >= matrix.Rows) AppException.Raise("index out of bounds", 2u);
        return matrix.Row(row);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Row(int row) => SubMatrix((uint)row, (uint)(row + 1), 0, (uint)Cols);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private MatVal(MatVal source, int rowStart, int rowEnd, int colStart, int colEnd)
        : this(source.m_rarr, source.m_iarr, rowEnd - rowStart, colEnd - colStart, source.Anr, null,
            Flags.StringTag | Flags.Shared, source.Index(rowStart, colStart))
    {
        m_dim = source.m_dim;
        if (source.m_parr != null) SubMatrixPtrs(source, rowStart, rowEnd, colStart, colEnd);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal SubMatrix(uint rowStart, uint rowEnd, uint colStart, uint colEnd)
    {
        if (rowStart >= rowEnd || colStart >= colEnd || rowEnd > Rows || colEnd > Cols)
        { AppException.Raise("index out of bounds", 1u); return null; }
        Immutable = true;
        return new MatVal(this, (int)rowStart, (int)rowEnd, (int)colStart, (int)colEnd);
    }
    private partial class TrimSubMatrixPtrs
    {
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private MatVal m_m;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_r1;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_r2;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_c1;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_c2;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        public TrimSubMatrixPtrs(MatVal matrix, int rowStart, int rowEnd, int colStart, int colEnd)
        { m_m = matrix; m_r1 = rowStart; m_r2 = rowEnd; m_c1 = colStart; m_c2 = colEnd; }
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        public void set_submatrix_ptrs(int row, int col, object value)
        {
            if (row >= m_r1 && row < m_r2 && col >= m_c1 && col < m_c2)
                m_m.set_ptrs_2(row - m_r1, col - m_c1, value);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void SubMatrixPtrs(MatVal source, int rowStart, int rowEnd, int colStart, int colEnd) =>
        TransformPtrs(source, new TrimSubMatrixPtrs(this, rowStart, rowEnd, colStart, colEnd).set_submatrix_ptrs);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private double[] TransposeArr(double[] array)
    {
        var result = new double[Rows * Cols];
        for (int col = 0; col < Cols; col++)
        for (int row = 0; row < Rows; row++) result[col + row * Cols] = array[Index(row, col)];
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public double[] TransposeReal() => TransposeArr(m_rarr);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public double[] TransposeImag() => m_iarr == null ? null : TransposeArr(m_iarr);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Transpose()
    {
        var result = new MatVal(TransposeReal(), TransposeImag(), Cols, Rows) { m_dim = m_dim };
        result.TransposePtrs(this);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void set_ptrs_2_transpose(int row, int col, object value) => set_ptrs_2(col, row, value);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public void TransposePtrs(MatVal source) => TransformPtrs(source, set_ptrs_2_transpose);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void set_ptrs_2_reverse(int row, int col, object value) => set_ptrs_2(Rows - row, col, value);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public void ReversePtrs(MatVal source) => TransformPtrs(source, set_ptrs_2_reverse);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override object[] asTuple(uint rows, uint cols, uint handle)
    {
        if (rows > Rows || cols > Cols) AppException.Raise("incompatible shape", handle);
        return get_elements(rows, cols);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public object[] get_elements(uint rows, uint cols)
    {
        var result = new object[rows * cols];
        int index = 0;
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++) result[index++] = get_2(row, col);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public object[] get_elements() => get_elements((uint)Rows, (uint)Cols);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override bool Equals(object value)
    {
        if (value == null || value.GetType() != GetType()) return false;
        var matrix = (MatVal)value;
        int rows = Rows, cols = Cols;
        if (matrix.Rows != rows || matrix.Cols != cols) return false;
        var first = new NumVal();
        var second = new NumVal();
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            if (!get_2_na(first, row, col).Equals(matrix.get_2_na(second, row, col))) return false;
        return true;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override bool nearly_equal(DimVal value, double fuzz)
    {
        if (value is not MatVal matrix) return false;
        int rows = Rows, cols = Cols;
        if (matrix.Rows != rows || matrix.Cols != cols) return false;
        var first = new NumVal();
        var second = new NumVal();
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            if (!DimVal.NearlyEqual(get_2_na(first, row, col), matrix.get_2_na(second, row, col), fuzz)) return false;
        return true;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override string ToString()
    {
        string result = "MatVal[" + Rows + "," + Cols + "]";
        for (int row = 0; row < Math.Min(Rows, 10); row++)
        {
            if (Cols == 0) continue;
            result += "\n{ " + get_2(row, 0);
            for (int col = 1; col < Math.Min(Cols, 3); col++) result += ", " + get_2(row, col);
            result += Cols > 3 ? ", ... }" : " }";
        }
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override DimVal asImmutable() { Immutable = true; return this; }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Fix() { Immutable = true; return this; }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static MatVal Fix(object value) => (value as MatVal)?.Fix();
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override DimVal asResult()
    {
        var result = (Packed ? this : new MatVal(this)).Canonicalize();
        if (result.m_dim == null) result.m_dim = Dim.FixDimension(result.m_dim);
        return result.Fix();
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object FixForExport(object value)
    {
        if (value is not DimVal dimensional) return value is string text ? new StrVal(text) : value;
        if (dimensional.m_dim == null) dimensional.m_dim = Dim.FixDimension(dimensional.m_dim);
        return value is MatVal matrix ? matrix.Fix() : dimensional.asResult();
    }
}

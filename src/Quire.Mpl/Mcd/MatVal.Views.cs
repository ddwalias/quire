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
        source.Shared = true;
        Shared = true;
        if (source.m_parr != null) TrimPtrs(source.m_parr);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void TrimPtrs(Hashtable pointers)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        for (int i = 0; i < cols; i++)
        {
            int num = i * anr + m_offset;
            int num2 = num + rows;
            for (int j = num; j < num2; j++)
            {
                if (IsPtrNan(ref m_rarr[j]))
                {
                    if (m_parr == null) m_parr = new Hashtable();
                    m_parr[j] = pointers[j];
                }
            }
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
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array");
        if (column < 0 || column >= matrix.Cols) AppException.Raise("index out of bounds");
        return matrix.Column(column);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_column(object value, int column)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array", 1u);
        if (column < 0 || column >= matrix.Cols) AppException.Raise("index out of bounds", 2u);
        return matrix.Column(column);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_sym_row(object value, int row)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array");
        if (row < 0 || row >= matrix.Rows) AppException.Raise("index out of bounds");
        return matrix.Row(row);
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_row(object value, int row)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array", 1u);
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
        if (rowStart < rowEnd && colStart < colEnd && rowEnd <= Rows && colEnd <= Cols)
        {
            Immutable = true;
            return new MatVal(this, (int)rowStart, (int)rowEnd, (int)colStart, (int)colEnd);
        }
        AppException.Raise("index out of bounds", 1u);
        return null;
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
            if (m_r1 <= row && row < m_r2 && m_c1 <= col && col < m_c2)
                m_m.set_ptrs_2(row - m_r1, col - m_c1, value);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void SubMatrixPtrs(MatVal source, int rowStart, int rowEnd, int colStart, int colEnd) =>
        TransformPtrs(source, new TrimSubMatrixPtrs(this, rowStart, rowEnd, colStart, colEnd).set_submatrix_ptrs);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private double[] TransposeArr(double[] array)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        int offset = Offset;
        var result = new double[rows * cols];
        for (int col = 0; col < cols; col++)
        {
            int dstIndex = col;
            int srcIndex = offset + col * anr;
            for (int row = 0; row < rows; row++)
            {
                result[dstIndex] = array[srcIndex++];
                dstIndex += cols;
            }
        }
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
        int index = 0;
        var result = new object[rows * cols];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++) result[index++] = get_2(row, col);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public object[] get_elements() => get_elements((uint)Rows, (uint)Cols);
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override bool Equals(object value)
    {
        if (value == null || GetType() != value.GetType()) return false;
        var matrix = (MatVal)value;
        int rows = Data.m_rows;
        if (rows != matrix.Data.m_rows) return false;
        int cols = Data.m_cols;
        if (cols != matrix.Data.m_cols) return false;
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
        var matrix = value as MatVal;
        if (matrix == null) return false;
        int rows = Data.m_rows;
        if (rows != matrix.Data.m_rows) return false;
        int cols = Data.m_cols;
        if (cols != matrix.Data.m_cols) return false;
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
        int maxRows = Math.Min(Rows, 10);
        for (int row = 0; row < maxRows; row++)
        {
            switch (Cols)
            {
                case 0:
                    break;
                case 1:
                    result = string.Concat(result, "\n{ ", get_2(row, 0), " }");
                    break;
                case 2:
                    result = string.Concat(new object[] { result, "\n{ ", get_2(row, 0), ", ", get_2(row, 1), " }" });
                    break;
                case 3:
                    result = string.Concat(new object[] { result, "\n{ ", get_2(row, 0), ", ", get_2(row, 1), ", ", get_2(row, 2), " }" });
                    break;
                default:
                    result = string.Concat(new object[] { result, "\n{ ", get_2(row, 0), ", ", get_2(row, 1), ", ", get_2(row, 2), ", ... }" });
                    break;
            }
        }
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override DimVal asImmutable() { Immutable = true; return this; }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Fix() { Immutable = true; return this; }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static MatVal Fix(object value)
    {
        var matVal = value as MatVal;
        if (matVal != null) return matVal.Fix();
        return null;
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override DimVal asResult()
    {
        var result = Packed ? this : new MatVal(this);
        result = result.Canonicalize();
        if (result.m_dim == null) result.m_dim = Dim.FixDimension(result.m_dim);
        return result.Fix();
    }
    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object FixForExport(object value)
    {
        if (value is not DimVal dimensional) return value is string text ? new StrVal(text) : value;
        if (dimensional.m_dim == null) dimensional.m_dim = Dim.FixDimension(dimensional.m_dim);
        var matVal = value as MatVal;
        if (matVal != null) return matVal.Fix();
        return dimensional.asResult();
    }
}

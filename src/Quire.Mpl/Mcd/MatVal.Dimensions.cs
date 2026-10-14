#nullable disable
using System.Collections;
using Quire.Mirror;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    private MatVal(MatVal source, Dim dimension)
        : this(source.PackReal(), source.PackImag(), source.Rows, source.Cols, source.Rows, null, Flags.ComplexSnan, 0)
    {
        m_dim = dimension;
        if (source.RepHasPtrs) CastPtrs(source, dimension);
    }

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    private void CastPtrs(MatVal source, Dim dimension) => TransformPtrs(source, (row, col, value) =>
    {
        if (value is NumVal number) set(Index(row, col), number.Real, number.Imag, dimension);
        else set_ptrs(Index(row, col), DimVal.DimCast(value, dimension));
    });

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public Dim DivideByDefaultDimension() => DivideByDimension(m_dim);

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public Dim DivideByDimension(Dim dimension)
    {
        Canonicalize();
        var quotient = Dim.Div(m_dim, dimension);
        if (!Dim.Id_b(dimension) && m_parr != null)
        {
            var pointers = new Hashtable();
            foreach (DictionaryEntry entry in m_parr)
            {
                int index = (int)entry.Key;
                if (!IsPtrNan(ref m_rarr[index])) continue;
                if (entry.Value is NumVal number)
                    pointers[index] = new NumVal(number, Dim.Div(number.m_dim, dimension));
                else if (entry.Value is MatVal matrix)
                    pointers[index] = new MatVal(matrix).DivideByDimension(dimension);
                else if (entry.Value != null) pointers[index] = entry.Value;
            }
            m_parr = pointers;
        }
        m_dim = quotient;
        return dimension;
    }

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public static MatVal StripDimension(MatVal matrix) => new(matrix, Dim.identity);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public static MatVal AsUniform(MatVal matrix) => matrix.IsUniform ? matrix : new(matrix, matrix.m_dim);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public static MatVal AsDimless(MatVal matrix) => matrix.IsDimless ? matrix : StripDimension(matrix);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public static MatVal AsPolymorphic(MatVal matrix) => matrix.IsUniformScalar & matrix.IsPolymorphic ? matrix : new(matrix, null);

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public MatVal ApplyLinearDimension(MatDim dimension)
    {
        var uniform = dimension.UniformDim;
        if (uniform != null) { m_dim = uniform; return this; }
        m_dim = Dim.identity;
        int rows = Rows, cols = Cols;
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
        {
            var unit = dimension.dim(row, col);
            if (unit == Dim.identity) continue;
            int index = Index(row, col);
            set_ptrs_at(index, new NumVal(m_rarr[index], m_iarr == null ? 0 : m_iarr[index], unit));
        }
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public MatVal ApplyDimension(MatDim dimension) => ApplyLinearDimension(dimension);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    private MatVal ApplyCrazyDimension(MatDim dimension) => ApplyLinearDimension(dimension);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public MatVal ApplyRowDimensions(Dim[] dimensions) => ApplyLinearDimension(MatDim.make(MatDim.RC.ROW, dimensions, Cols));
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public MatVal ApplyRowDimensions(Dim first, Dim second) => ApplyRowDimensions([first, second]);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public MatVal ApplyColDimensions(Dim[] dimensions) => ApplyLinearDimension(MatDim.make(MatDim.RC.COL, dimensions, Rows));
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public MatVal ApplyColDimensions(Dim first, Dim second) => ApplyColDimensions([first, second]);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public override DimVal SetDim(Dim dimension) => new MatVal(this, dimension);
    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public static object MDimCast(object value, MatDim dimension) => value is MatVal matrix && dimension != null ? StripDimension(matrix).ApplyLinearDimension(dimension) : value;

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    public static object MatCreateDim(object order, object point, object value)
    {
        var pointDimension = DimVal.DimOf(point);
        if (pointDimension == null || order is not MatVal orderMatrix || value is not MatVal matrix) return value;
        if (matrix.IsDimless) return value;
        var units = matrix.ColDimArray(0);
        var first = units[0];
        if (first == null) return value;
        for (int row = 1; row < orderMatrix.Data.m_rows; row++)
        {
            int previous = (int)orderMatrix.m_rarr[row];
            if (previous >= row) continue;
            if (units[previous] != units[row])
            {
                units[row] = first;
                return StripDimension(matrix).ApplyRowDimensions(units);
            }
            units[row] = Dim.Div(first, Dim.Pwr(pointDimension, row));
        }
        return StripDimension(matrix).ApplyRowDimensions(units);
    }
}

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
    public Dim DivideByDimension(Dim d)
    {
        Canonicalize();
        Dim dim = Dim.Div(m_dim, d);
        if (!Dim.Id_b(d) && m_parr != null)
        {
            Hashtable hashtable = new Hashtable();
            foreach (DictionaryEntry item in m_parr)
            {
                int num = (int)item.Key;
                if (IsPtrNan(ref m_rarr[num]))
                {
                    if (item.Value is NumVal numVal)
                    {
                        Dim d2 = Dim.Div(numVal.m_dim, d);
                        hashtable[num] = new NumVal(numVal, d2);
                    }
                    else if (item.Value is MatVal m)
                    {
                        hashtable[num] = new MatVal(m).DivideByDimension(d);
                    }
                    else if (item.Value != null)
                    {
                        hashtable[num] = item.Value;
                    }
                }
            }
            m_parr = hashtable;
        }
        m_dim = dim;
        return d;
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
    public MatVal ApplyLinearDimension(MatDim d)
    {
        Dim uniformDim = d.UniformDim;
        if (uniformDim != null)
        {
            m_dim = uniformDim;
            return this;
        }
        m_dim = Dim.identity;
        int rows = Rows;
        int cols = Cols;
        if (m_iarr == null)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    uniformDim = d.dim(i, j);
                    if (uniformDim != Dim.identity)
                    {
                        int num = Index(i, j);
                        set_ptrs_at(num, new NumVal(get_real_raw(num), 0.0, uniformDim));
                    }
                }
            }
        }
        else
        {
            for (int k = 0; k < rows; k++)
            {
                for (int l = 0; l < cols; l++)
                {
                    uniformDim = d.dim(k, l);
                    if (uniformDim != Dim.identity)
                    {
                        int num2 = Index(k, l);
                        set_ptrs_at(num2, new NumVal(get_real_raw(num2), get_imag_raw(num2), uniformDim));
                    }
                }
            }
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
    public static object MatCreateDim(object icord, object icpnt, object icval)
    {
        Dim dim = DimVal.DimOf(icpnt);
        if (dim == null)
        {
            return icval;
        }
        MatVal matVal = icord as MatVal;
        MatVal matVal2 = icval as MatVal;
        if (matVal == null || matVal2 == null)
        {
            return icval;
        }
        if (matVal2.IsDimless)
        {
            return icval;
        }
        int rows = matVal.Data.m_rows;
        Dim[] array = matVal2.ColDimArray(0);
        Dim dim2 = array[0];
        if (dim2 == null)
        {
            return icval;
        }
        for (int i = 1; i < rows; i++)
        {
            int num = (int)matVal.m_rarr[i];
            if (num < i)
            {
                if (array[num] == array[i])
                {
                    array[i] = Dim.Div(dim2, Dim.Pwr(dim, (double)i));
                }
                else
                {
                    array[i] = dim2;
                    return StripDimension(matVal2).ApplyRowDimensions(array);
                }
            }
        }
        return StripDimension(matVal2).ApplyRowDimensions(array);
    }
}

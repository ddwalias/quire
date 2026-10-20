#nullable disable
using System.Collections;
using Quire.Mirror;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorVerified("MatValDimensionDifferentialTests")]
    private MatVal(MatVal m, Dim d)
        : this(m.PackReal(), m.PackImag(), m.Rows, m.Cols, m.Rows, null, Flags.ComplexSnan, 0)
    {
        m_dim = d;
        if (m.RepHasPtrs)
        {
            CastPtrs(m, d);
        }
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
    private void CastPtrs(MatVal m, Dim d)
    {
        TransformPtrs(m, (r, c, val) =>
        {
            NumVal numVal = val as NumVal;
            int num = Index(r, c);
            if (numVal != null)
            {
                set(num, numVal.Data.rp, numVal.Data.ip, d);
            }
            else
            {
                set_ptrs(num, DimVal.DimCast(val, d));
            }
        });
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public Dim DivideByDefaultDimension() => DivideByDimension(m_dim);

    [MirrorVerified("MatValDimensionDifferentialTests")]
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

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public static MatVal StripDimension(MatVal m) => new MatVal(m, Dim.identity);

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public static MatVal AsUniform(MatVal m)
    {
        if (!m.IsUniform)
        {
            return new MatVal(m, m.m_dim);
        }
        return m;
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public static MatVal AsDimless(MatVal m)
    {
        if (!m.IsDimless)
        {
            return StripDimension(m);
        }
        return m;
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public static MatVal AsPolymorphic(MatVal m)
    {
        if (!(m.IsUniformScalar & m.IsPolymorphic))
        {
            return new MatVal(m, null);
        }
        return m;
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
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
                        int cellIndex = Index(k, l);
                        set_ptrs_at(cellIndex, new NumVal(get_real_raw(cellIndex), get_imag_raw(cellIndex), uniformDim));
                    }
                }
            }
        }
        return this;
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public MatVal ApplyDimension(MatDim d) => ApplyLinearDimension(d);

    [MirrorVerified("MatValDimensionDifferentialTests")]
    private MatVal ApplyCrazyDimension(MatDim d) => ApplyLinearDimension(d);

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public MatVal ApplyRowDimensions(Dim[] dims) => ApplyLinearDimension(MatDim.make(MatDim.RC.ROW, dims, Cols));

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public MatVal ApplyRowDimensions(Dim d1, Dim d2) => ApplyRowDimensions(new Dim[2] { d1, d2 });

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public MatVal ApplyColDimensions(Dim[] dims) => ApplyLinearDimension(MatDim.make(MatDim.RC.COL, dims, Rows));

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public MatVal ApplyColDimensions(Dim d1, Dim d2) => ApplyColDimensions(new Dim[2] { d1, d2 });

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public override DimVal SetDim(Dim d) => new MatVal(this, d);

    [MirrorVerified("MatValDimensionDifferentialTests")]
    public static object MDimCast(object o, MatDim md)
    {
        MatVal matVal = o as MatVal;
        if (matVal == null || md == null)
        {
            return o;
        }
        return StripDimension(matVal).ApplyLinearDimension(md);
    }

    [MirrorVerified("MatValDimensionDifferentialTests")]
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
                    array[i] = Dim.Div(dim2, Dim.Pwr(dim, i));
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

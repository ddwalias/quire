#nullable disable
using Quire.Mirror;

namespace Quire.Mcd;

/// <summary>Matrix dimension shape and dimension-array operations. Contracts: Mcd.MatDim evidence record.</summary>
public abstract partial class MatDim
{
    [MirrorTested("Mcd.MatDim")]
    public readonly int m_row;
    [MirrorTested("Mcd.MatDim")]
    public readonly int m_col;
    [MirrorTested("Mcd.MatDim")]
    protected MatDim(int r, int c) { m_row = r; m_col = c; }
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public int Rows => m_row;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public int Cols => m_col;
    [MirrorVerified("MatDimBaseGettersDifferentialTests")]
    public virtual Dim UniformDim => null;
    [MirrorVerified("MatDimBaseGettersDifferentialTests")]
    public virtual Dim[] RowDimensions => null;
    [MirrorVerified("MatDimBaseGettersDifferentialTests")]
    public virtual Dim[] ColDimensions => null;
    [MirrorVerified("MatDimBaseGettersDifferentialTests")]
    public virtual MatDim IsLinear => null;
    public abstract Dim dim(int i, int j);
    public abstract MatDim matdim(int i, int j);

    [MirrorVerified("MatDimDimensionArraysDifferentialTests")]
    public virtual Dim[] RowDimensionArray(int r)
    {
        int cols = Cols;
        var result = new Dim[cols];
        for (int col = 0; col < cols; col++) result[col] = dim(r, col);
        return result;
    }
    [MirrorVerified("MatDimDimensionArraysDifferentialTests")]
    public virtual Dim[] ColDimensionArray(int c)
    {
        int rows = Rows;
        var result = new Dim[rows];
        for (int row = 0; row < rows; row++) result[row] = dim(row, c);
        return result;
    }

    [MirrorVerified("MatDimDifferentialTests")]
    public static MatDim CMult(MatDim m1, MatDim m2) => null;
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim Uniform(Dim[] dims)
    {
        if (dims == null)
        {
            return null;
        }
        Dim dim = dims[0];
        foreach (Dim dim2 in dims)
        {
            if (dim != dim2)
            {
                return null;
            }
        }
        return dim;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static bool Eq_b(Dim[] d1, Dim[] d2)
    {
        int num = d1.Length;
        if (num != d2.Length)
        {
            return false;
        }
        for (int i = 0; i < num; i++)
        {
            if (!Dim.Eq_b(d1[i], d2[i]))
            {
                return false;
            }
        }
        return true;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim IsScalarMultiple(Dim[] d1, Dim[] d2)
    {
        Dim dim = Dim.Div(d2[0], d1[0]);
        int num = d1.Length;
        for (int i = 1; i < num; i++)
        {
            if (dim != Dim.Div(d2[i], d1[i]))
            {
                return null;
            }
        }
        return dim;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim Mult(Dim[] d1, Dim[] d2)
    {
        Dim dim = Dim.Mult(d2[0], d1[0]);
        int num = d1.Length;
        for (int i = 1; i < num; i++)
        {
            if (dim != Dim.Mult(d2[i], d1[i]))
            {
                return null;
            }
        }
        return dim;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim[] Mult(Dim d, Dim[] dv)
    {
        int num = dv.Length;
        Dim[] array = new Dim[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = Dim.Mult(d, dv[i]);
        }
        return array;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim[] Invert(Dim[] d1)
    {
        int num = d1.Length;
        Dim[] array = new Dim[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = Dim.Pwr(d1[i], -1.0);
        }
        return array;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim Product(Dim scalar, Dim[] dv)
    {
        int num = dv.Length;
        Dim dim = scalar;
        for (int i = 0; i < num; i++)
        {
            dim = Dim.Mult(dim, dv[i]);
        }
        return dim;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim[] Push(Dim d1, Dim[] d2)
    {
        var result = new Dim[d2.Length + 1];
        result[0] = d1;
        Array.Copy(d2, 0, result, 1, d2.Length);
        return result;
    }
}

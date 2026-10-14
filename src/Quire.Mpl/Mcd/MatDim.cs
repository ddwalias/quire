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
    [MirrorTested("Mcd.MatDim")]
    public int Rows => m_row;
    [MirrorTested("Mcd.MatDim")]
    public int Cols => m_col;
    [MirrorTested("Mcd.MatDim")]
    public virtual Dim UniformDim => null;
    [MirrorTested("Mcd.MatDim")]
    public virtual Dim[] RowDimensions => null;
    [MirrorTested("Mcd.MatDim")]
    public virtual Dim[] ColDimensions => null;
    [MirrorTested("Mcd.MatDim")]
    public virtual MatDim IsLinear => null;
    public abstract Dim dim(int i, int j);
    public abstract MatDim matdim(int i, int j);

    [MirrorTested("Mcd.MatDim")]
    public virtual Dim[] RowDimensionArray(int r)
    {
        var result = new Dim[Cols];
        for (int col = 0; col < result.Length; col++) result[col] = dim(r, col);
        return result;
    }
    [MirrorTested("Mcd.MatDim")]
    public virtual Dim[] ColDimensionArray(int c)
    {
        var result = new Dim[Rows];
        for (int row = 0; row < result.Length; row++) result[row] = dim(row, c);
        return result;
    }

    [MirrorVerified("MatDimDifferentialTests")]
    public static MatDim CMult(MatDim m1, MatDim m2) => null;
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim Uniform(Dim[] dims)
    {
        if (dims == null) return null;
        Dim candidate = dims[0];
        return Array.TrueForAll(dims, d => ReferenceEquals(d, candidate)) ? candidate : null;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static bool Eq_b(Dim[] d1, Dim[] d2)
    {
        if (d1.Length != d2.Length) return false;
        for (int i = 0; i < d1.Length; i++) if (!Dim.Eq_b(d1[i], d2[i])) return false;
        return true;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim IsScalarMultiple(Dim[] d1, Dim[] d2)
    {
        var factor = Dim.Div(d2[0], d1[0]);
        for (int i = 1; i < d1.Length; i++) if (!ReferenceEquals(factor, Dim.Div(d2[i], d1[i]))) return null;
        return factor;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim Mult(Dim[] d1, Dim[] d2)
    {
        var product = Dim.Mult(d2[0], d1[0]);
        for (int i = 1; i < d1.Length; i++) if (!ReferenceEquals(product, Dim.Mult(d2[i], d1[i]))) return null;
        return product;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim[] Mult(Dim d, Dim[] dv)
    {
        var result = new Dim[dv.Length];
        for (int i = 0; i < result.Length; i++) result[i] = Dim.Mult(d, dv[i]);
        return result;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim[] Invert(Dim[] d1)
    {
        var result = new Dim[d1.Length];
        for (int i = 0; i < result.Length; i++) result[i] = Dim.Pwr(d1[i], -1.0);
        return result;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim Product(Dim scalar, Dim[] dv)
    {
        foreach (var value in dv) scalar = Dim.Mult(scalar, value);
        return scalar;
    }
    [MirrorVerified("MatDimDifferentialTests")]
    public static Dim[] Push(Dim d1, Dim[] d2)
    {
        var result = new Dim[d2.Length + 1];
        result[0] = d1;
        d2.CopyTo(result, 1);
        return result;
    }
}

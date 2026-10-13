#nullable disable
using System.Collections;

namespace Quire.Mcd;

/// <summary>The shape of a matrix: <see cref="m_anr"/> is the allocated row count (the column stride).</summary>
public partial struct mat_val
{
    public int m_anr;
    public int m_rows;
    public int m_cols;
}

/// <summary>
/// A matrix, stored column by column in <see cref="m_rarr"/> (and <see cref="m_iarr"/> when it has imaginary
/// parts). Plain numbers share the matrix's dimension <c>m_dim</c>. Any other element (a number with different
/// units, a string, a nested matrix) is a pointer: it lives in <see cref="m_parr"/> under its offset, and its slot
/// in <see cref="m_rarr"/> holds <see cref="PtrNan"/>.
/// </summary>
public sealed partial class MatVal : DimVal
{
    /// <summary>The NaN pattern that marks a pointer slot.</summary>
    public static readonly double PtrNan = BitConverter.Int64BitsToDouble(0x7FF8_0000_5054_524EL);

    public mat_val Data;
    public int m_offset;
    public double[] m_rarr;
    public double[] m_iarr;
    public Hashtable m_parr;

    public MatVal() : this(0, 0) { }

    public MatVal(int r) : this(r, 1) { }

    public MatVal(int r, int c) : this(r, c, false) { }

    public MatVal(int r, int c, bool imag) : this(r, c, imag, r) { }

    public MatVal(int r, int c, bool imag, int anr)
    {
        if (r < 0 || c < 0 || anr < r) throw new ArgumentOutOfRangeException(nameof(r), $"Invalid matrix shape {r}×{c}");
        Data.m_rows = r;
        Data.m_cols = c;
        Data.m_anr = anr;
        m_rarr = new double[checked(anr * c)];
        m_iarr = imag ? new double[m_rarr.Length] : null;
        m_dim = Dim.identity;
    }

    public MatVal(double[] rarr, double[] iarr, int r, int c)
    {
        if (rarr.Length < r * c || (iarr != null && iarr.Length != rarr.Length))
            throw new ArgumentException("Matrix data doesn't fit its shape");
        Data.m_rows = r;
        Data.m_cols = c;
        Data.m_anr = r;
        m_rarr = rarr;
        m_iarr = iarr;
        m_dim = Dim.identity;
    }

    /// <summary>A column vector of <paramref name="vals"/>; its dimension is that of the first number.</summary>
    public MatVal(object[] vals) : this(vals.Length, 1)
    {
        m_dim = vals.OfType<NumVal>().FirstOrDefault()?.m_dim ?? Dim.identity;
        for (int i = 0; i < vals.Length; i++) set_1(i, vals[i]);
    }

    public int Rows => Data.m_rows;
    public int Cols => Data.m_cols;
    public int Anr => Data.m_anr;
    public int Offset => m_offset;
    public bool HasPtrs => m_parr != null && m_parr.Count > 0;
    public bool HasImag => m_iarr != null;
    public bool IsUniform => !HasPtrs;
    public bool IsVector => Rows == 1 || Cols == 1;

    public override MatVal IsMatrix => this;

    /// <summary>The shared dimension, or <see cref="Dim.mixed_matrix_dim"/> when elements differ.</summary>
    public override Dim PhysicalDimension => HasPtrs ? Dim.mixed_matrix_dim : m_dim;

    public static MatVal MakeVector(object[] vals) => new(vals);

    public int Index(int r, int c) => m_offset + c * Anr + r;

    public int RowOf(int idx) => (idx - m_offset) % Anr;

    public int ColOf(int idx) => (idx - m_offset) / Anr;

    private void Check(int i, int j)
    {
        if (i < 0 || i >= Rows || j < 0 || j >= Cols)
            throw new IndexOutOfRangeException($"Element ({i}, {j}) is outside a {Rows}×{Cols} matrix");
    }

    /// <summary>Element <paramref name="i"/> in column-major order.</summary>
    public object get_1(int i) => Rows == 0 ? throw new IndexOutOfRangeException("The matrix is empty") : get_2(i % Rows, i / Rows);

    public object get_2(int i, int j)
    {
        Check(i, j);
        int off = Index(i, j);
        if (m_parr != null && m_parr.ContainsKey(off)) return m_parr[off];
        return new NumVal(m_rarr[off], m_iarr?[off] ?? 0, m_dim);
    }

    public double get_real(int i, int j)
    {
        Check(i, j);
        int off = Index(i, j);
        return m_parr != null && m_parr[off] is NumVal n ? n.Real : m_rarr[off];
    }

    public void set_1(int i, object x)
    {
        if (Rows == 0) throw new IndexOutOfRangeException("The matrix is empty");
        set_2(i % Rows, i / Rows, x);
    }

    public void set_2(int i, int j, object x)
    {
        Check(i, j);
        int off = Index(i, j);
        if (x is NumVal n && Dim.Eq_b(n.m_dim, m_dim))
        {
            m_parr?.Remove(off);
            m_rarr[off] = n.Real;
            if (n.Imag != 0) (m_iarr ??= new double[m_rarr.Length])[off] = n.Imag;
            else if (m_iarr != null) m_iarr[off] = 0;
        }
        else
        {
            (m_parr ??= new Hashtable())[off] = x;
            m_rarr[off] = PtrNan;
            if (m_iarr != null) m_iarr[off] = 0;
        }
    }

    public void set_1(int i, double r)
    {
        if (Rows == 0) throw new IndexOutOfRangeException("The matrix is empty");
        set_2(i % Rows, i / Rows, r);
    }

    public void set_2(int i, int j, double r)
    {
        Check(i, j);
        int off = Index(i, j);
        m_parr?.Remove(off);
        m_rarr[off] = r;
        if (m_iarr != null) m_iarr[off] = 0;
    }

    private MatVal Like(int r, int c) => new(r, c, HasImag) { m_dim = m_dim };

    private void CopyElement(MatVal from, int fi, int fj, int i, int j)
    {
        int src = from.Index(fi, fj), dst = Index(i, j);
        if (from.m_parr != null && from.m_parr.ContainsKey(src)) { (m_parr ??= new Hashtable())[dst] = from.m_parr[src]; m_rarr[dst] = PtrNan; }
        else
        {
            m_rarr[dst] = from.m_rarr[src];
            if (from.m_iarr != null) m_iarr[dst] = from.m_iarr[src];
        }
    }

    public MatVal Transpose()
    {
        var t = Like(Cols, Rows);
        for (int j = 0; j < Cols; j++)
            for (int i = 0; i < Rows; i++) t.CopyElement(this, i, j, j, i);
        return t;
    }

    public MatVal Column(int col) => SubMatrix(0, (uint)Rows - 1, (uint)col, (uint)col);

    public MatVal Row(int row) => SubMatrix((uint)row, (uint)row, 0, (uint)Cols - 1);

    /// <summary>Rows <paramref name="r1"/>..<paramref name="r2"/> and columns <paramref name="c1"/>..<paramref name="c2"/>, inclusive.</summary>
    public MatVal SubMatrix(uint r1, uint r2, uint c1, uint c2)
    {
        if (r2 < r1 || c2 < c1 || r2 >= Rows || c2 >= Cols)
            throw new IndexOutOfRangeException($"Submatrix ({r1}..{r2}, {c1}..{c2}) is outside a {Rows}×{Cols} matrix");
        var s = Like((int)(r2 - r1 + 1), (int)(c2 - c1 + 1));
        for (int j = 0; j < s.Cols; j++)
            for (int i = 0; i < s.Rows; i++) s.CopyElement(this, (int)r1 + i, (int)c1 + j, i, j);
        return s;
    }

    public override MatVal asVector(string m, uint a) =>
        Cols == 1 ? this : Rows == 1 ? Transpose()
            : throw new InvalidCastException($"{m ?? "This operation"} needs a vector, not a {Rows}×{Cols} matrix");

    public override string ToString() => $"[{Rows}x{Cols}]";
}

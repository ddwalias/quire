#nullable disable
using System.Collections;
using Quire.Mirror;

namespace Quire.Mcd;

/// <summary>The shape of a matrix: <see cref="m_anr"/> is the allocated row count (the column stride).</summary>
[Serializable]
public partial struct mat_val
{
    [MirrorVerified("MatValDifferentialTests")]
    public int m_anr;
    [MirrorVerified("MatValDifferentialTests")]
    public Flags m_flags;
    [MirrorVerified("MatValDifferentialTests")]
    public int m_rows;
    [MirrorVerified("MatValDifferentialTests")]
    public int m_cols;
    [MirrorAnalyzed("Mcd.MatVal")]
    public nint m_rv;
    [MirrorAnalyzed("Mcd.MatVal")]
    public nint m_iv;
}

/// <summary>
/// A matrix, stored column by column in <see cref="m_rarr"/> (and <see cref="m_iarr"/> when it has imaginary
/// parts). Plain numbers share the matrix's dimension <c>m_dim</c>. Any other element (a number with different
/// units, a string, a nested matrix) is a pointer: it lives in <see cref="m_parr"/> under its offset, and its slot
/// in <see cref="m_rarr"/> holds <see cref="PtrNan"/>.
/// </summary>
[Serializable]
public sealed partial class MatVal : DimVal
{
    /// <summary>The NaN pattern that marks a pointer slot.</summary>
    [MirrorVerified("MatValDifferentialTests")]
    public static double PtrNan;

    [MirrorVerified("MatValDifferentialTests")]
    public static uint NanMask;

    [MirrorVerified("MatValDifferentialTests")]
    static MatVal()
    {
        NanMask = 0x7FF80000u;
        PtrNan = ThePtrNan();
    }

    public mat_val Data;
    public int m_offset;
    public double[] m_rarr;
    public double[] m_iarr;
    public Hashtable m_parr;

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal() : this(null, null, 0, 0, 0, null, Flags.MatrixTag, 0) { }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(int r) : this(r, 1, false, r) { }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(int r, int c) : this(r, c, false, r) { }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(int r, int c, bool imag) : this(r, c, imag, r) { }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(int r, int c, bool imag, int anr)
    {
        m_rarr = new double[c * anr];
        m_iarr = imag ? new double[c * anr] : null;
        m_parr = null;
        m_offset = 0;
        Data.m_rows = r;
        Data.m_cols = c;
        Data.m_anr = anr;
        Data.m_flags = Flags.MatrixTag;
    }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr, double[] iarr, int r, int c, int anr, Hashtable ptrs, Flags flags, int offset)
    {
        m_dim = Dim.identity;
        m_rarr = rarr;
        m_iarr = iarr;
        m_parr = ptrs;
        m_offset = offset;
        Data.m_rows = r;
        Data.m_cols = c;
        Data.m_anr = anr;
        Data.m_flags = flags;
    }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr, double[] iarr, int r, int c, int anr, Hashtable ptrs)
        : this(rarr, iarr, r, c, anr, ptrs, Flags.MatrixTag, 0) { }
    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr, double[] iarr, int r, int c, int anr)
        : this(rarr, iarr, r, c, anr, null, Flags.MatrixTag, 0) { }
    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr, double[] iarr, int r, int c)
        : this(rarr, iarr, r, c, r, null, Flags.MatrixTag, 0) { }
    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr, double[] iarr, int r)
        : this(rarr, iarr, r, 1, r, null, Flags.MatrixTag, 0) { }
    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr, double[] iarr) : this(rarr, iarr, rarr.Length, 1, rarr.Length, null, Flags.MatrixTag, 0) { }
    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(double[] rarr) : this(rarr, null, rarr.Length, 1, rarr.Length, null, Flags.MatrixTag, 0) { }

    [MirrorVerified("MatValDifferentialTests")]
    public int Rows => Data.m_rows;
    [MirrorVerified("MatValDifferentialTests")]
    public int Cols => Data.m_cols;
    [MirrorVerified("MatValDifferentialTests")]
    public int Anr => Data.m_anr;
    [MirrorVerified("MatValDifferentialTests")]
    public int Offset => m_offset;
    [MirrorVerified("MatValDifferentialTests")]
    public bool HasImag
    {
        get
        {
            if (m_iarr == null) return false;
            for (int i = 0; i < m_iarr.Length; i++) if (m_iarr[i] != 0) return true;
            m_iarr = null;
            return false;
        }
    }

    [MirrorVerified("MatValDifferentialTests")]
    public bool Packed => Data.m_rows * Data.m_cols == m_rarr.Length;
    [MirrorVerified("MatValDifferentialTests")]
    public bool Immutable
    {
        get => (Data.m_flags & Flags.CopyOnWrite) != 0;
        set
        {
            if (value)
                Data.m_flags |= Flags.CopyOnWrite;
            else
                Data.m_flags &= ~Flags.CopyOnWrite;
        }
    }
    [MirrorVerified("MatValDifferentialTests")]
    public bool Shared
    {
        get => (Data.m_flags & Flags.Shared) != 0;
        set
        {
            if (value)
                Data.m_flags |= Flags.Shared;
            else
                Data.m_flags &= ~Flags.Shared;
        }
    }
    [MirrorVerified("MatValDifferentialTests")]
    public override int Isize => Rows * Cols;
    [MirrorVerified("MatValDifferentialTests")]
    public override int GetHashCode() => Rows.GetHashCode() * 64 + Cols.GetHashCode();
    [MirrorVerified("MatValDifferentialTests")]
    public static unsafe double ThePtrNan()
    {
        double val;
        int* p = (int*)&val;
        *p = -1;
        p[1] = -1;
        return *(double*)p;
    }
    [MirrorVerified("MatValDifferentialTests")]
    public static unsafe bool IsPtrNan(ref double r)
    {
        fixed (double* p = &r)
        {
            int* ptr = (int*)p;
            return ptr[1] == -1;
        }
    }
    [MirrorTested("Mcd.MatVal")]
    private bool IsPtrNan(int r, int c) => IsPtrNan(ref m_rarr[Index(r, c)]);
    [MirrorVerified("MatValDifferentialTests")]
    public void AllocImaginary() => m_iarr = new double[m_rarr.Length];

    [MirrorVerified("MatValDifferentialTests")]
    public int Index(int r) => r + m_offset;

    [MirrorVerified("MatValDifferentialTests")]
    public int Index(int r, int c) => m_offset + r + Anr * c;

    [MirrorVerified("MatValDifferentialTests")]
    public int RowOf(int idx) => (idx - m_offset) % Anr;

    [MirrorVerified("MatValDifferentialTests")]
    public int ColOf(int idx) => (idx - m_offset) / Anr;

}

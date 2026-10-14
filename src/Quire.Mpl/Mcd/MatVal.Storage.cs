#nullable disable
using System.Collections;
using System.Runtime.InteropServices;
using Quire.Mirror;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal")]
    private unsafe static void Copy(double[] src, int src_offset, double[] dst, int dst_offset, int n)
    {
        if (n <= 0) return;
        fixed (double* destination = &dst[dst_offset]) Marshal.Copy(src, src_offset, (nint)destination, n);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private double[] Pack(double[] arr)
    {
        var result = new double[Rows * Cols];
        if (Anr == Rows) Copy(arr, Offset, result, 0, result.Length);
        else for (int col = 0; col < Cols; col++) Copy(arr, Index(0, col), result, Rows * col, Rows);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private double[] Pack(double[] arr, int c)
    {
        var result = new double[Rows];
        Copy(arr, Index(0, c), result, 0, Rows);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private double[] Pack(double[] arr, int r1, int r2, int c1, int c2)
    {
        int rows = r2 - r1;
        var result = new double[rows * (c2 - c1)];
        for (int col = 0; col < c2 - c1; col++) Copy(arr, Index(r1, col), result, rows * col, rows);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackZero(double[] arr)
    {
        foreach (double value in arr) if (value != 0) return arr;
        return null;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackReal() => Pack(m_rarr);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackReal(int c) => Pack(m_rarr, c);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackReal(int r1, int r2, int c1, int c2) => Pack(m_rarr, r1, r2, c1, c2);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackImag() => m_iarr == null ? null : PackZero(Pack(m_iarr));
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackImag(int c) => m_iarr == null ? null : PackZero(Pack(m_iarr, c));
    [MirrorAnalyzed("Mcd.MatVal")]
    public double[] PackImag(int r1, int r2, int c1, int c2) => m_iarr == null ? null : PackZero(Pack(m_iarr, r1, r2, c1, c2));
    [MirrorAnalyzed("Mcd.MatVal")]
    private Hashtable PackPtrs(int c) => PackPtrs(0, Rows, c, c + 1);
    [MirrorAnalyzed("Mcd.MatVal")]
    private Hashtable PackPtrs(int r1, int r2, int c1, int c2)
    {
        if (m_parr == null) return null;
        var result = new Hashtable();
        for (int col = c1; col < c2; col++)
        for (int row = r1; row < r2; row++)
            if (IsPtrNan(row, col)) result[row - r1 + (col - c1) * (r2 - r1)] = m_parr[Index(row, col)];
        return result.Count == 0 ? null : result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal(MatVal m) : this(m.PackReal(), m.PackImag(), m.Rows, m.Cols)
    {
        m_dim = m.m_dim;
        if (m.RepHasPtrs) InitPtrs(m);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void InitPtrs(MatVal m)
    {
        if (m.Packed && (Cols == 1 || Anr == m.Anr)) m_parr = (Hashtable)m.m_parr.Clone();
        else { m_parr = new Hashtable(); CopyPtrs(m); }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void IteratePtrs(PtrDelegate ProcessPtr)
    {
        if (m_parr == null) return;
        foreach (DictionaryEntry entry in m_parr)
        {
            int index = (int)entry.Key;
            if (IsPtrNan(ref m_rarr[index])) ProcessPtr(RowOf(index), ColOf(index), entry.Value);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void TransformPtrs(MatVal m, PtrDelegate op)
    {
        if (!m.RepHasPtrs) return;
        m_parr ??= new Hashtable();
        m.IteratePtrs(op);
        if (m_parr.Count == 0) m_parr = null;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void CopyPtrs(MatVal m) => TransformPtrs(m, set_ptrs_2);
}

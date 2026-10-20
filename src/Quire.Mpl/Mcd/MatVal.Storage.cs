#nullable disable
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Quire.Mirror;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private unsafe static void Copy(double[] src, int src_offset, double[] dst, int dst_offset, int n)
    {
        if (n > 0)
        {
            fixed (double* ptr = &dst[dst_offset])
            {
                Marshal.Copy(src, src_offset, (IntPtr)ptr, n);
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private double[] Pack(double[] arr)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        int offset = Offset;
        double[] array = new double[rows * cols];
        if (anr == rows)
        {
            Copy(arr, offset, array, 0, rows * cols);
        }
        else
        {
            for (int i = 0; i < cols; i++)
            {
                Copy(arr, offset + anr * i, array, rows * i, rows);
            }
        }
        return array;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private double[] Pack(double[] arr, int r1, int r2, int c1, int c2)
    {
        _ = Rows;
        _ = Cols;
        int anr = Anr;
        int subOffset = Offset + r1;
        int subRows = r2 - r1;
        int subCols = c2 - c1;
        double[] array = new double[subRows * subCols];
        for (int i = 0; i < subCols; i++)
        {
            Copy(arr, subOffset + anr * i, array, subRows * i, subRows);
        }
        return array;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private double[] Pack(double[] arr, int c)
    {
        int rows = Rows;
        double[] array = new double[rows];
        Copy(arr, Offset + Anr * c, array, 0, rows);
        return array;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackZero(double[] arr)
    {
        double[] array = arr;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != 0.0)
            {
                return arr;
            }
        }
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackReal()
    {
        return Pack(m_rarr);
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackReal(int c)
    {
        return Pack(m_rarr, c);
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackReal(int r1, int r2, int c1, int c2)
    {
        return Pack(m_rarr, r1, r2, c1, c2);
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackImag()
    {
        if (m_iarr != null)
        {
            return PackZero(Pack(m_iarr));
        }
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackImag(int c)
    {
        if (m_iarr != null)
        {
            return PackZero(Pack(m_iarr, c));
        }
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public double[] PackImag(int r1, int r2, int c1, int c2)
    {
        if (m_iarr != null)
        {
            return PackZero(Pack(m_iarr, r1, r2, c1, c2));
        }
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private Hashtable PackPtrs(int c)
    {
        return PackPtrs(0, Rows, c, c + 1);
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private Hashtable PackPtrs(int r1, int r2, int c1, int c2)
    {
        if (m_parr != null)
        {
            int subRows = r2 - r1;
            bool flag = false;
            Hashtable hashtable = new Hashtable();
            for (int i = c1; i < c2; i++)
            {
                for (int j = r1; j < r2; j++)
                {
                    if (IsPtrNan(j, i))
                    {
                        hashtable[j - r1 + (i - c1) * subRows] = m_parr[Index(j, i)];
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                return null;
            }
            return hashtable;
        }
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public MatVal(MatVal m)
        : this(m.PackReal(), m.PackImag(), m.Rows, m.Cols, m.Rows, null, Flags.ComplexSnan, 0)
    {
        m_dim = m.m_dim;
        if (m.RepHasPtrs)
        {
            InitPtrs(m);
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    private void InitPtrs(MatVal m)
    {
        if (m.Packed && (Cols == 1 || Anr == m.Anr))
        {
            m_parr = (Hashtable)m.m_parr.Clone();
            return;
        }
        m_parr = new Hashtable();
        CopyPtrs(m);
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public void IteratePtrs(PtrDelegate ProcessPtr)
    {
        if (m_parr != null)
        {
            foreach (DictionaryEntry item in m_parr)
            {
                int index = (int)item.Key;
                if (IsPtrNan(ref m_rarr[index]))
                {
                    int r = RowOf(index);
                    int c = ColOf(index);
                    ProcessPtr(r, c, item.Value);
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public void TransformPtrs(MatVal m, PtrDelegate op)
    {
        if (m.RepHasPtrs)
        {
            if (m_parr == null)
            {
                m_parr = new Hashtable();
            }
            m.IteratePtrs(op);
            if (m_parr.Count == 0)
            {
                m_parr = null;
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public void CopyPtrs(MatVal m)
    {
        TransformPtrs(m, set_ptrs_2);
    }
}

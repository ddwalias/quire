#nullable disable
using System.Collections;
using System.Runtime.InteropServices;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal")]
    public override MatVal IsMatrix => this;
    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsVector => Data.m_cols == 1;
    [MirrorAnalyzed("Mcd.MatVal")]
    public bool HasPtrs => m_parr != null && !IsUniformScalar;
    [MirrorAnalyzed("Mcd.MatVal")]
    public bool RepHasPtrs
    {
        get
        {
            if (m_parr == null) return false;
            foreach (int index in m_parr.Keys) if (IsPtrNan(ref m_rarr[index])) return true;
            m_parr.Clear();
            m_parr = null;
            return false;
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public bool IsUniformScalar => m_parr == null || IsUniformScalarPtrs();
    [MirrorAnalyzed("Mcd.MatVal")]
    public bool IsUniform => m_parr == null || IsUniformPtrs();
    [MirrorAnalyzed("Mcd.MatVal")]
    public override bool IsDimless => IsUniformScalar && Dim.Id_b(m_dim);
    [MirrorAnalyzed("Mcd.MatVal")]
    public override Dim PhysicalDimension => UniformDimension;
    [MirrorAnalyzed("Mcd.MatVal")]
    public override Dim UniformDimension => IsUniform ? m_dim : Dim.mixed_matrix_dim;
    [MirrorAnalyzed("Mcd.MatVal")]
    public override MatDim MatrixDimension => MatDim.make(this);
    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsRealMatrix => IsUniformScalar & !HasImag;
    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsRealVector
    {
        get
        {
            if (Data.m_cols == 1) return IsRealMatrix;
            return false;
        }
    }
    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsScalarMatrix
    {
        get
        {
            if (m_parr == null) return true;
            foreach (DictionaryEntry entry in m_parr)
            {
                if (IsPtrNan(ref m_rarr[(int)entry.Key]) && !(entry.Value is NumVal))
                    return false;
            }
            return true;
        }
    }
    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsRealMatrixMixed
    {
        get
        {
            if (HasImag) return false;
            if (m_parr == null) return true;
            foreach (DictionaryEntry entry in m_parr)
            {
                if (IsPtrNan(ref m_rarr[(int)entry.Key]))
                {
                    NumVal numVal = entry.Value as NumVal;
                    if (numVal == null || numVal.Data.ip != 0.0)
                        return false;
                }
            }
            return true;
        }
    }
    [MirrorVerified("MatValRangeIntegralDifferentialTests")]
    public bool IsRange
    {
        get => (Data.m_flags & Flags.JoeyBit) != 0 && IsRealVector;
        set
        {
            if (value)
                Data.m_flags |= Flags.JoeyBit;
            else
                Data.m_flags &= ~Flags.JoeyBit;
        }
    }
    [MirrorVerified("MatValRangeIntegralDifferentialTests")]
    public bool IsIntegral
    {
        get => (Data.m_flags & Flags.Integral) != 0 && IsRealVector;
        set
        {
            if (value)
                Data.m_flags |= Flags.Integral;
            else
                Data.m_flags &= ~Flags.Integral;
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public override bool IsUniformDimOf(Dim d)
    {
        if (m_parr != null)
        {
            int live = 0;
            foreach (DictionaryEntry entry in m_parr)
            {
                if (!IsPtrNan(ref m_rarr[(int)entry.Key])) continue;
                if (!DimVal.IsDimOf(entry.Value, m_dim)) return false;
                live++;
            }
            if (live == Rows * Cols) { m_dim = d; return true; }
        }
        return Dim.Eq_b(m_dim, d);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_1(int i) => get(i + m_offset);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_2(int i, int j) => get(i + m_offset + j * Data.m_anr);
    [MirrorAnalyzed("Mcd.MatVal")]
    private object get(int offset)
    {
        double r = m_rarr[offset];
        if (IsPtrNan(ref r))
        {
            if (m_parr != null)
            {
                object obj = m_parr[offset];
                if (obj != null)
                {
                    return obj;
                }
            }
            r = (m_rarr[offset] = double.NaN);
        }
        return new NumVal(r, m_iarr == null ? 0.0 : m_iarr[offset], m_dim);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public Dim get_1_dim(int i) => get_dim(i + m_offset);
    [MirrorAnalyzed("Mcd.MatVal")]
    public Dim get_2_dim(int i, int j) => get_dim(i + m_offset + j * Data.m_anr);
    [MirrorAnalyzed("Mcd.MatVal")]
    private Dim get_dim(int offset)
    {
        if (!IsPtrNan(ref m_rarr[offset]))
        {
            return m_dim;
        }
        return DimVal.DimOf(m_parr[offset]);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatDim get_1_matdim(int i) => get_matdim(i + m_offset);
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatDim get_2_matdim(int i, int j) => get_matdim(i + m_offset + j * Data.m_anr);
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatDim get_matdim(int offset)
    {
        if (!IsPtrNan(ref m_rarr[offset]))
        {
            return MatDim.make(m_dim);
        }
        return DimVal.MatDimOf(m_parr[offset]);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_1_ptr(int i) => get_ptr(i + m_offset);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_2_ptr(int i, int j) => get_ptr(i + m_offset + j * Data.m_anr);
    [MirrorAnalyzed("Mcd.MatVal")]
    private object get_ptr(int offset)
    {
        double r = m_rarr[offset];
        if (!IsPtrNan(ref r) || m_parr == null)
        {
            return null;
        }
        return m_parr[offset];
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_1_na(NumVal n, int i) => get_na(n, i + m_offset);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_2_na(NumVal n, int i, int j) => get_na(n, i + m_offset + j * Data.m_anr);
    [MirrorAnalyzed("Mcd.MatVal")]
    private object get_na(NumVal n, int idx)
    {
        double r = m_rarr[idx];
        if (IsPtrNan(ref r))
        {
            return m_parr[idx];
        }
        n.Data.rp = r;
        n.Data.ip = m_iarr == null ? 0.0 : m_iarr[idx];
        return n.dv(m_dim);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real(int i) => m_rarr[i + m_offset];
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real(int i, int j) => m_rarr[i + m_offset + j * Data.m_anr];
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_imag(int i) => m_iarr[i + m_offset];
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_imag(int i, int j) => m_iarr[i + m_offset + j * Data.m_anr];
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real_raw(int idx) => m_rarr[idx];
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_imag_raw(int idx) => m_iarr[idx];
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real_mixed(int i) => get_real_mixed_raw(i + m_offset, 0u);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real_mixed(int i, int j) => get_real_mixed_raw(Index(i, j), 0u);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real_mixed_handle(int i, uint handle) => get_real_mixed_raw(i + m_offset, handle);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real_mixed_handle(int i, int j, uint handle) => get_real_mixed_raw(Index(i, j), handle);
    [MirrorAnalyzed("Mcd.MatVal")]
    public double get_real_mixed_raw(int i, uint handle)
    {
        double r = m_rarr[i];
        if (IsPtrNan(ref r))
        {
            NumVal obj = m_parr[i] as NumVal;
            if (obj == null)
            {
                AppException.Raise("non number value found in real argument", 0u, handle);
            }
            return obj.Data.rp;
        }
        return r;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public Dim[] ColDimArray(int c)
    {
        var result = new Dim[Rows];
        int start = Offset + c * Anr;
        int end = start + Rows;
        int item = 0;
        for (int index = start; index < end; index++)
        {
            if (IsPtrNan(ref m_rarr[index]))
            {
                Dim value = DimVal.DimOf(m_parr[index]);
                if (Dim.NonScalar_b(value)) return null;
                result[item++] = value;
            }
            else
            {
                result[item++] = m_dim;
            }
        }
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public Dim[] RowDimArray(int r)
    {
        var result = new Dim[Cols];
        int start = Offset + r;
        int stride = Anr;
        int end = start + stride * Cols;
        int item = 0;
        for (int index = start; index < end; index += stride)
        {
            if (IsPtrNan(ref m_rarr[index]))
            {
                Dim value = DimVal.DimOf(m_parr[index]);
                if (Dim.NonScalar_b(value)) return null;
                result[item++] = value;
            }
            else
            {
                result[item++] = m_dim;
            }
        }
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_1(int i, object x) => set(i + m_offset, x);
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_2(int i, int j, object x) => set(Index(i, j), x);
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set(int offset, object x)
    {
        if (x is NumVal x2)
        {
            set(offset, x2);
        }
        else
        {
            set_ptrs(offset, x);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void setNaN(int offset, object x)
    {
        if (x is NumVal numVal)
        {
            if (numVal.m_dim == Dim.mcd_zero && double.IsNaN(numVal.Data.rp))
            {
                set(offset, numVal.Data.rp, numVal.m_dim);
            }
            else
            {
                set(offset, numVal);
            }
        }
        else
        {
            set_ptrs(offset, x);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void setspec(int offset, object x)
    {
        NumVal numVal = x as NumVal;
        if (numVal != null && numVal.m_dim != null)
        {
            if (numVal.m_dim != m_dim)
            {
                if (m_dim == null)
                {
                    m_dim = numVal.m_dim;
                }
                else
                {
                    set_ptrs_at(offset, numVal);
                    return;
                }
            }
            if (m_iarr == null)
            {
                if (numVal.Data.ip == 0.0)
                {
                    set_at(offset, numVal.Data.rp);
                    return;
                }
                AllocImaginary();
            }
            set_at(offset, numVal.Data.rp, numVal.Data.ip);
        }
        else
        {
            set_ptrs(offset, x);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_1(int i, double re, Dim d) => set(i + m_offset, re, d);
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_2(int i, int j, double re, Dim d) => set(Index(i, j), re, d);
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_1(int i, double re, double im, Dim d) => set(i + m_offset, re, im, d);
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_2(int i, int j, double re, double im, Dim d) => set(Index(i, j), re, im, d);
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set(int offset, double re, Dim d)
    {
        if (d != m_dim && !Dim.IsSpecial(d))
        {
            if (m_dim == null)
            {
                m_dim = d;
            }
            else if (d != null)
            {
                set_ptrs_at(offset, new NumVal(re, 0.0, d));
                return;
            }
        }
        if (d == Dim.mcd_zero && (re == 0.0 || double.IsNaN(re)))
        {
            if (m_parr != null && !IsUniformPtrsNoChange())
            {
                if (m_dim == Dim.identity)
                {
                    set(offset, re);
                }
                else
                {
                    set_ptrs_at(offset, new NumVal(re, 0.0, Dim.identity));
                }
            }
            else
            {
                set_ptrs_at(offset, new NumVal(re, 0.0, d));
            }
        }
        else
        {
            set(offset, re);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set(int offset, double re)
    {
        if (m_iarr == null)
        {
            set_at(offset, re);
        }
        else
        {
            set_at(offset, re, 0.0);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set(int offset, double re, double im, Dim d)
    {
        if (d != m_dim && !Dim.IsSpecial(d))
        {
            if (m_dim == null)
            {
                m_dim = d;
            }
            else if (d != null)
            {
                set_ptrs_at(offset, new NumVal(re, im, d));
                return;
            }
        }
        set(offset, re, im);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set(int offset, double re, double im)
    {
        if (m_iarr == null)
        {
            if (im == 0.0)
            {
                set_at(offset, re);
                return;
            }
            AllocImaginary();
            set_at(offset, re, im);
        }
        else
        {
            set_at(offset, re, im);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set(int offset, NumVal x)
    {
        if (x.m_dim != m_dim && !Dim.IsSpecial(x.m_dim))
        {
            if (m_dim == null)
            {
                m_dim = x.m_dim;
            }
            else
            {
                set_ptrs_at(offset, x);
                return;
            }
        }
        if (m_iarr == null)
        {
            if (x.Data.ip == 0.0)
            {
                set_at(offset, x.Data.rp);
                return;
            }
            AllocImaginary();
        }
        set_at(offset, x.Data.rp, x.Data.ip);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_ptrs_1(int i, object p) => set_ptrs(i + m_offset, p);
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_ptrs_2(int i, int j, object p) => set_ptrs(Index(i, j), p);
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set_ptrs(int offset, object p)
    {
        if (p is string text) p = new StrVal(text);
        if (p is MatVal matrix)
        {
            if (matrix == this || matrix.contains(this)) p = matrix = new MatVal(matrix);
            matrix.Immutable = true;
        }
        set_ptrs_at(offset, p);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public bool contains(object p)
    {
        if (m_parr != null)
            foreach (DictionaryEntry entry in m_parr)
                if (ReferenceEquals(entry.Value, p) || entry.Value is MatVal nested && nested.contains(p)) return true;
        return false;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set_ptrs_raw(int off, object p)
    {
        if (m_parr == null) m_parr = new Hashtable();
        m_parr[off] = NumVal.NotNoValue(p);
        m_rarr[off] = PtrNan;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_2i(int i, int j, double imag)
    {
        if (m_iarr == null) AllocImaginary();
        m_iarr[Index(i, j)] = imag;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_1(int i, double r) => set_at(i + m_offset, r);
    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_2(int i, int j, double r) => set_at(Index(i, j), r);
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set_at(int offset, double re) => m_rarr[offset] = re;
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set_at(int offset, double re, double im) { m_rarr[offset] = re; m_iarr[offset] = im; }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void set_ptrs_at(int off, object p)
    {
        if (m_parr == null) m_parr = new Hashtable();
        m_parr[off] = p;
        m_rarr[off] = PtrNan;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public void AllocImaginary(double[] iarr) => m_iarr = iarr;

    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal Canonicalize()
    {
        if (m_parr != null) CanonicalizePtrs();
        return this;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private bool IsUniformScalarPtrs()
    {
        int count = 0;
        Dim common = null;
        foreach (DictionaryEntry entry in m_parr)
        {
            if (!IsPtrNan(ref m_rarr[(int)entry.Key])) continue;
            if (entry.Value is not NumVal n) return false;
            if (Dim.Eq_b(n.m_dim, common)) common = Dim.Eq(n.m_dim, common);
            else if (n.m_dim != Dim.mcd_zero) return false;
            count++;
        }
        return TidyPtrs(count, 0, common, false);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private bool DimActual()
    {
        for (int col = 0; col < Cols; col++)
        for (int row = 0; row < Rows; row++) if (!IsPtrNan(row, col)) return true;
        return false;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private bool CanonicalizePtrs()
    {
        if (m_dim == Dim.string_dim) m_dim = Dim.identity;
        Dim common = DimActual() ? m_dim : null;
        int count = 0, zeros = 0;
        bool other = false;
        foreach (DictionaryEntry entry in m_parr)
        {
            if (!IsPtrNan(ref m_rarr[(int)entry.Key])) continue;
            if (entry.Value is not NumVal n) { other = true; continue; }
            if (Dim.Eq_b(n.m_dim, common)) common = Dim.Eq(n.m_dim, common);
            else if (n.m_dim != Dim.mcd_zero) { other = true; continue; }
            count++;
            if (n.m_dim == Dim.mcd_zero) zeros++;
        }
        return TidyPtrs(count, zeros, common, other);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private bool TidyPtrs(int n, int nz, Dim d, bool hasOtherPntr)
    {
        if (n == 0)
        {
            if (hasOtherPntr) return false;
            m_parr = null;
            return true;
        }
        if (n == Rows * Cols)
        {
            if (d != null) m_dim = d;
            if (n > nz) ReinsertPtrs(); else ReinsertPtrs1();
            return true;
        }
        bool compatible = Dim.Eq_b(m_dim, d);
        if (compatible)
        {
            m_dim = Dim.Eq(m_dim, d);
            if (hasOtherPntr) ReinsertPtrs1(); else ReinsertPtrs();
        }
        return compatible;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void ReinsertPtrs()
    {
        var table = m_parr;
        var oldReal = m_rarr;
        int oldOffset = Offset, oldStride = Anr;
        bool shared = Shared;
        if (shared)
        {
            m_rarr = PackReal();
            m_iarr = PackImag();
            m_offset = 0;
            Data.m_anr = Rows;
            Shared = false;
        }
        m_parr = null;
        foreach (DictionaryEntry entry in table)
        {
            int oldIndex = (int)entry.Key;
            if (!IsPtrNan(ref oldReal[oldIndex])) continue;
            int index = shared ? (oldIndex - oldOffset) % oldStride + (oldIndex - oldOffset) / oldStride * Rows : oldIndex;
            set(index, entry.Value);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private void ReinsertPtrs1()
    {
        var table = m_parr;
        var oldReal = m_rarr;
        int oldOffset = Offset, oldStride = Anr;
        bool shared = Shared;
        if (shared)
        {
            m_rarr = PackReal();
            m_iarr = PackImag();
            m_offset = 0;
            Data.m_anr = Rows;
            Shared = false;
        }
        m_parr = null;
        foreach (DictionaryEntry entry in table)
        {
            int oldIndex = (int)entry.Key;
            if (!IsPtrNan(ref oldReal[oldIndex])) continue;
            if (entry.Value is NumVal n && n.m_dim == Dim.mcd_zero) n.m_dim = Dim.identity;
            int index = shared ? (oldIndex - oldOffset) % oldStride + (oldIndex - oldOffset) / oldStride * Rows : oldIndex;
            set(index, entry.Value);
        }
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private bool IsUniformPtrs()
    {
        int num = 0;
        Dim dim = null;
        foreach (DictionaryEntry item in m_parr)
        {
            int num2 = (int)item.Key;
            if (!IsPtrNan(ref m_rarr[num2]))
            {
                continue;
            }
            Dim dim2 = DimVal.DimOf(item.Value);
            if (Dim.Eq_b(dim2, dim))
            {
                dim = Dim.Eq(dim2, dim);
                num++;
                continue;
            }
            if (dim2 == Dim.mcd_zero)
            {
                num++;
                continue;
            }
            return false;
        }
        return TidyPtrs(num, 0, dim, hasOtherPntr: false);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private bool IsUniformPtrsNoChange()
    {
        if (m_parr == null)
        {
            return true;
        }
        foreach (DictionaryEntry item in m_parr)
        {
            int num = (int)item.Key;
            if (IsPtrNan(ref m_rarr[num]))
            {
                Dim dim = DimVal.DimOf(item.Value);
                if (Dim.NonScalar_b(dim))
                {
                    return false;
                }
                if (dim != m_dim && !Dim.IsSpecial(dim))
                {
                    return false;
                }
            }
        }
        return true;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    private MatVal dvm(Dim d) { m_dim = d; return Canonicalize(); }
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asVector(string m, uint a)
    {
        if (!IsVector) AppException.Raise(m, a);
        return this;
    }
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asMatrix(string m, uint a) => this;
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asRealVector(string m, uint a)
    {
        if (!IsRealVector) AppException.Raise(m, a);
        return this;
    }
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asRealMatrix(string m, uint a)
    {
        if (!IsRealMatrix) AppException.Raise(m, a);
        return this;
    }
    [MirrorVerified("MatValAsRealVectorDifferentialTests")]
    public static MatVal asRealVector(object mm) => mm is MatVal matrix && matrix.IsRealVector ? matrix : null;
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public void assertRealVector() => assertRealVector(0);
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public void assertRealVector(uint param) { if (!IsRealVector) AppException.Raise("must be real vector", param); }
    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public void assertRealMatrix(uint param) { if (!IsRealMatrix) AppException.Raise("must be real", param); }
}

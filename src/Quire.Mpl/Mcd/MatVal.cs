#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Quire.Mirror;
using Quire.Mpl.Run;

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

    public mat_val Data;

    public int m_offset;

    public double[] m_rarr;

    public double[] m_iarr;

    public Hashtable m_parr;

    [MirrorVerified("MatValDifferentialTests")]
    public int Rows => Data.m_rows;

    [MirrorVerified("MatValDifferentialTests")]
    public int Cols => Data.m_cols;

    [MirrorVerified("MatValDifferentialTests")]
    public int Anr => Data.m_anr;

    [MirrorVerified("MatValDifferentialTests")]
    public int Offset => m_offset;

    [MirrorVerified("MatValDifferentialTests")]
    public bool Packed => Data.m_rows * Data.m_cols == m_rarr.Length;

    [MirrorAnalyzed("Mcd.MatVal")]
    public bool HasPtrs => m_parr != null && !IsUniformScalar;

    [MirrorAnalyzed("Mcd.MatVal")]
    public bool RepHasPtrs
    {
        get
        {
            if (m_parr == null)
            {
                return false;
            }
            System.Collections.IEnumerator enumerator = m_parr.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int num = (int)enumerator.Current;
                if (IsPtrNan(ref m_rarr[num]))
                {
                    return true;
                }
            }
            m_parr.Clear();
            m_parr = null;
            return false;
        }
    }

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

    [MirrorAnalyzed("Mcd.MatVal")]
    public override bool IsUniformDimOf(Dim d)
    {
        if (m_parr != null)
        {
            int num = 0;
            foreach (DictionaryEntry item in m_parr)
            {
                int num2 = (int)item.Key;
                if (IsPtrNan(ref m_rarr[num2]))
                {
                    if (DimVal.IsDimOf(item.Value, m_dim))
                    {
                        num++;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (num == Rows * Cols)
            {
                m_dim = d;
                return true;
            }
        }
        return Dim.Eq_b(m_dim, d);
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
    public override bool IsDimless => IsUniformScalar && Dim.Id_b(m_dim);

    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal Canonicalize()
    {
        if (m_parr != null) CanonicalizePtrs();
        return this;
    }

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

    [MirrorAnalyzed("Mcd.MatVal")]
    public bool IsUniformScalar => m_parr == null || IsUniformScalarPtrs();

    [MirrorAnalyzed("Mcd.MatVal")]
    private bool IsUniformScalarPtrs()
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
            NumVal numVal = item.Value as NumVal;
            if (numVal == null)
            {
                return false;
            }
            Dim dim2 = numVal.m_dim;
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
    private bool DimActual()
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        for (int i = 0; i < cols; i++)
        {
            int num = i * anr + m_offset;
            int num2 = num + rows;
            for (int j = num; j < num2; j++)
            {
                if (!IsPtrNan(ref m_rarr[j]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private bool CanonicalizePtrs()
    {
        if (m_dim == Dim.string_dim)
        {
            m_dim = Dim.identity;
        }
        int num = 0;
        int num2 = 0;
        Dim dim = (DimActual() ? m_dim : null);
        bool hasOtherPntr = false;
        foreach (DictionaryEntry item in m_parr)
        {
            int num3 = (int)item.Key;
            if (!IsPtrNan(ref m_rarr[num3]))
            {
                continue;
            }
            NumVal numVal = item.Value as NumVal;
            if (numVal == null)
            {
                hasOtherPntr = true;
            }
            else
            {
                Dim dim2 = numVal.m_dim;
                if (Dim.Eq_b(dim2, dim))
                {
                    dim = Dim.Eq(dim2, dim);
                    num++;
                    if (dim2 == Dim.mcd_zero)
                    {
                        num2++;
                    }
                }
                else if (dim2 == Dim.mcd_zero)
                {
                    num2++;
                    num++;
                }
                else
                {
                    hasOtherPntr = true;
                }
            }
        }
        return TidyPtrs(num, num2, dim, hasOtherPntr);
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
        if (Shared)
        {
            double[] rarr = m_rarr;
            m_rarr = PackReal();
            m_iarr = PackImag();
            int anr = Anr;
            int offset = m_offset;
            m_offset = 0;
            Data.m_anr = Rows;
            Shared = false;
            Hashtable parr = m_parr;
            m_parr = null;
            foreach (DictionaryEntry item in parr)
            {
                int num = (int)item.Key;
                if (IsPtrNan(ref rarr[num]))
                {
                    num -= offset;
                    set(num % anr + num / anr * Rows, item.Value);
                }
            }
        }
        else
        {
            Hashtable parr2 = m_parr;
            m_parr = null;
            foreach (DictionaryEntry item2 in parr2)
            {
                int num2 = (int)item2.Key;
                if (IsPtrNan(ref m_rarr[num2]))
                {
                    set(num2, item2.Value);
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private void ReinsertPtrs1()
    {
        if (Shared)
        {
            double[] rarr = m_rarr;
            m_rarr = PackReal();
            m_iarr = PackImag();
            int anr = Anr;
            int offset = m_offset;
            m_offset = 0;
            Data.m_anr = Rows;
            Shared = false;
            Hashtable parr = m_parr;
            m_parr = null;
            foreach (DictionaryEntry item in parr)
            {
                int num = (int)item.Key;
                if (IsPtrNan(ref rarr[num]))
                {
                    num -= offset;
                    if (item.Value is NumVal numVal && numVal.m_dim == Dim.mcd_zero)
                    {
                        numVal.m_dim = Dim.identity;
                        set(num % anr + num / anr * Rows, numVal);
                    }
                    else
                    {
                        set(num % anr + num / anr * Rows, item.Value);
                    }
                }
            }
        }
        else
        {
            Hashtable parr2 = m_parr;
            m_parr = null;
            foreach (DictionaryEntry item2 in parr2)
            {
                int num2 = (int)item2.Key;
                if (IsPtrNan(ref m_rarr[num2]))
                {
                    if (item2.Value is NumVal numVal2 && numVal2.m_dim == Dim.mcd_zero)
                    {
                        numVal2.m_dim = Dim.identity;
                        set(num2, numVal2);
                    }
                    else
                    {
                        set(num2, item2.Value);
                    }
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private MatVal dvm(Dim d) { m_dim = d; return Canonicalize(); }

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

    [MirrorAnalyzed("Mcd.MatVal")]
    public override Dim PhysicalDimension => UniformDimension;

    [MirrorAnalyzed("Mcd.MatVal")]
    public override Dim UniformDimension => IsUniform ? m_dim : Dim.mixed_matrix_dim;

    [MirrorAnalyzed("Mcd.MatVal")]
    public override MatDim MatrixDimension => MatDim.make(this);

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

    [MirrorAnalyzed("Mcd.MatVal")]
    public bool IsUniform => m_parr == null || IsUniformPtrs();

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
    public override MatVal IsMatrix => this;

    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsVector => Data.m_cols == 1;

    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsRealMatrix => IsUniformScalar & !HasImag;

    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsScalarMatrix
    {
        get
        {
            if (m_parr == null)
            {
                return true;
            }
            foreach (DictionaryEntry item in m_parr)
            {
                int num = (int)item.Key;
                if (IsPtrNan(ref m_rarr[num]) && !(item.Value is NumVal))
                {
                    return false;
                }
            }
            return true;
        }
    }

    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsRealMatrixMixed
    {
        get
        {
            if (HasImag)
            {
                return false;
            }
            if (m_parr == null)
            {
                return true;
            }
            foreach (DictionaryEntry item in m_parr)
            {
                int num = (int)item.Key;
                if (IsPtrNan(ref m_rarr[num]))
                {
                    if (!(item.Value is NumVal numVal))
                    {
                        return false;
                    }
                    if (numVal.Data.ip != 0.0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [MirrorVerified("MatValRealClassifiersDifferentialTests")]
    public bool IsRealVector
    {
        get
        {
            if (Data.m_cols == 1) return IsRealMatrix;
            return false;
        }
    }

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

    [MirrorVerified("MatValAsRealVectorDifferentialTests")]
    public static MatVal asRealVector(object mm) => mm is MatVal matrix && matrix.IsRealVector ? matrix : null;

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asVector(string m, uint a)
    {
        if (!IsVector) AppException.Raise(m, a);
        return this;
    }

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asRealVector(string m, uint a)
    {
        if (!IsRealVector) AppException.Raise(m, a);
        return this;
    }

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asMatrix(string m, uint a) => this;

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public override MatVal asRealMatrix(string m, uint a)
    {
        if (!IsRealMatrix) AppException.Raise(m, a);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override object[] asTuple(uint rows, uint cols, uint handle)
    {
        if (rows > Rows || cols > Cols) AppException.Raise("incompatible shape", handle);
        return get_elements(rows, cols);
    }

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public void assertRealVector() => assertRealVector(0);

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public void assertRealVector(uint param) { if (!IsRealVector) AppException.Raise("must be real vector", param); }

    [MirrorVerified("MatValAssertionsAndCastsDifferentialTests")]
    public void assertRealMatrix(uint param) { if (!IsRealMatrix) AppException.Raise("must be real", param); }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override string ToString()
    {
        string result = "MatVal[" + Rows + "," + Cols + "]";
        int maxRows = Math.Min(Rows, 10);
        for (int row = 0; row < maxRows; row++)
        {
            switch (Cols)
            {
                case 0:
                    break;
                case 1:
                    result = string.Concat(result, "\n{ ", get_2(row, 0), " }");
                    break;
                case 2:
                    result = string.Concat(new object[] { result, "\n{ ", get_2(row, 0), ", ", get_2(row, 1), " }" });
                    break;
                case 3:
                    result = string.Concat(new object[] { result, "\n{ ", get_2(row, 0), ", ", get_2(row, 1), ", ", get_2(row, 2), " }" });
                    break;
                default:
                    result = string.Concat(new object[] { result, "\n{ ", get_2(row, 0), ", ", get_2(row, 1), ", ", get_2(row, 2), ", ... }" });
                    break;
            }
        }
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override bool Equals(object value)
    {
        if (value == null || GetType() != value.GetType()) return false;
        var matrix = (MatVal)value;
        int rows = Data.m_rows;
        if (rows != matrix.Data.m_rows) return false;
        int cols = Data.m_cols;
        if (cols != matrix.Data.m_cols) return false;
        var first = new NumVal();
        var second = new NumVal();
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            if (!get_2_na(first, row, col).Equals(matrix.get_2_na(second, row, col))) return false;
        return true;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override bool nearly_equal(DimVal value, double fuzz)
    {
        var matrix = value as MatVal;
        if (matrix == null) return false;
        int rows = Data.m_rows;
        if (rows != matrix.Data.m_rows) return false;
        int cols = Data.m_cols;
        if (cols != matrix.Data.m_cols) return false;
        var first = new NumVal();
        var second = new NumVal();
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            if (!DimVal.NearlyEqual(get_2_na(first, row, col), matrix.get_2_na(second, row, col), fuzz)) return false;
        return true;
    }

    [MirrorVerified("MatValDifferentialTests")]
    public override int GetHashCode() => Rows.GetHashCode() * 64 + Cols.GetHashCode();

    [MirrorVerified("MatValDifferentialTests")]
    static MatVal()
    {
        NanMask = 0x7FF80000u;
        PtrNan = ThePtrNan();
    }

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
    public MatVal(int r) : this(r, 1, false, r) { }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(int r, int c) : this(r, c, false, r) { }

    [MirrorVerified("MatValDifferentialTests")]
    public MatVal(int r, int c, bool imag) : this(r, c, imag, r) { }

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
    public MatVal() : this(null, null, 0, 0, 0, null, Flags.MatrixTag, 0) { }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal(RngVal r) : this(r.Rsize)
    {
        IsRange = true;
        IsIntegral = r.Integral;
        m_dim = r.m_dim;
        for (int i = 0; i < r.Rsize; ++i) m_rarr[i] = r.get(i);
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal(DimVal first, DimVal last) : this(RngVal.mkRange2(first, last)) { }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal(DimVal first, DimVal next, DimVal last) : this(RngVal.mkRange3(first, next, last)) { }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal map(F<object, object> f, DimVal collection)
    {
        int isize = collection.Isize;
        MatVal matVal = reserve(isize);
        for (int i = 0; i < isize; i++)
        {
            matVal.Push(f(collection.iterator_get(i)));
        }
        if (collection is MatVal matVal2)
        {
            matVal = matVal.Reshape(matVal2.Rows, matVal2.Cols);
        }
        return matVal;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal map(Function func, DimVal collection) => map(func.AsDelegate_1(), collection);

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal map(F<object, object, object> f, DimVal rvals, DimVal cvals)
    {
        int rows = rvals.Isize, cols = cvals.Isize;
        var result = reserve(rows * cols);
        for (int c = 0; c < cols; ++c)
        {
            var columnValue = cvals.iterator_get(c);
            for (int r = 0; r < rows; ++r) result.Push(f(rvals.iterator_get(r), columnValue));
        }
        return result.Reshape(rows, cols);
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal map(Function func, DimVal rvals, DimVal cvals) => map(func.AsDelegate_2(), rvals, cvals);

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static object[] mapv(F<object, object> f, DimVal collection)
    {
        int length = collection.Isize;
        var result = new object[length];
        for (int i = 0; i < length; ++i) result[i] = f(collection.iterator_get(i));
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object[] as_object_vector(DimVal collection)
    {
        int length = collection.Isize;
        var result = new object[length];
        for (int i = 0; i < length; ++i) result[i] = collection.iterator_get(i);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object mkRange2(object first, object last) => new MatVal(first as DimVal, last as DimVal);

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object mkRange3(object first, object next, object last) => new MatVal(first as DimVal, next as DimVal, last as DimVal);

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

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    private MatVal(MatVal source, Dim dimension)
        : this(source.PackReal(), source.PackImag(), source.Rows, source.Cols, source.Rows, null, Flags.ComplexSnan, 0)
    {
        m_dim = dimension;
        if (source.RepHasPtrs) CastPtrs(source, dimension);
    }

    [MirrorAnalyzed("Mcd.MatVal.Dimensions")]
    private void CastPtrs(MatVal m, Dim d)
    {
        TransformPtrs(m, (int r, int c, object val) =>
        {
            NumVal numVal = val as NumVal;
            int offset = Index(r, c);
            if (numVal != null)
            {
                set(offset, numVal.Data.rp, numVal.Data.ip, d);
            }
            else
            {
                set_ptrs(offset, DimVal.DimCast(val, d));
            }
        });
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private MatVal(MatVal source, int rows, int cols)
        : this(source.CopyResize(source.m_rarr, rows, cols), source.m_iarr == null ? null : source.CopyResize(source.m_iarr, rows, cols), Math.Max(rows, source.Rows), Math.Max(cols, source.Cols))
    {
        m_dim = source.m_dim;
        if (source.RepHasPtrs) InitPtrs(source);
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal realloc(object o, int i, int j)
    {
        MatVal matVal = o as MatVal;
        i = Math.Max(0, i);
        j = Math.Max(0, j);
        if (matVal == null)
        {
            return new MatVal(0, j + 1, false, i + 1);
        }
        MatVal result = new MatVal(matVal, i + 1, j + 1);
        int cols = result.Data.m_cols;
        int rows = result.Data.m_rows;
        result.Data.m_rows = matVal.Data.m_rows;
        result.Data.m_cols = matVal.Data.m_cols;
        result.TidyZeros(matVal.Data.m_rows, matVal.Data.m_cols, rows, cols);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private MatVal(MatVal source, int column)
        : this(source.m_rarr, source.m_iarr, source.Rows, 1, source.Anr, null, Flags.StringTag, source.Index(0, column))
    {
        m_dim = source.m_dim;
        source.Shared = true;
        Shared = true;
        if (source.m_parr != null) TrimPtrs(source.m_parr);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void TrimPtrs(Hashtable pointers)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        for (int i = 0; i < cols; i++)
        {
            int start = i * anr + m_offset;
            int limit = start + rows;
            for (int j = start; j < limit; j++)
            {
                if (IsPtrNan(ref m_rarr[j]))
                {
                    if (m_parr == null) m_parr = new Hashtable();
                    m_parr[j] = pointers[j];
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_sym_column(object value, int column)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array");
        if (column < 0 || column >= matrix.Cols) AppException.Raise("index out of bounds");
        return matrix.Column(column);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_column(object value, int column)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array", 1u);
        if (column < 0 || column >= matrix.Cols) AppException.Raise("index out of bounds", 2u);
        return matrix.Column(column);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Column(int column) => IsUniformScalar & Immutable
        ? new MatVal(this, column)
        : new MatVal(PackReal(column), PackImag(column), Rows, 1, Rows, PackPtrs(column)).dvm(m_dim);

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public double[] real_column(int column)
    {
        if (IsUniformScalar) return PackReal(column);
        var result = new double[Rows];
        for (int row = 0; row < Rows; row++) result[row] = get_real_mixed(row, column);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_sym_row(object value, int row)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array");
        if (row < 0 || row >= matrix.Rows) AppException.Raise("index out of bounds");
        return matrix.Row(row);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object mcd_row(object value, int row)
    {
        var matrix = value as MatVal;
        if (matrix == null) AppException.Raise("must be array", 1u);
        if (row < 0 || row >= matrix.Rows) AppException.Raise("index out of bounds", 2u);
        return matrix.Row(row);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Row(int row) => SubMatrix((uint)row, (uint)(row + 1), 0, (uint)Cols);

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private MatVal(MatVal source, int rowStart, int rowEnd, int colStart, int colEnd)
        : this(source.m_rarr, source.m_iarr, rowEnd - rowStart, colEnd - colStart, source.Anr, null,
            Flags.StringTag | Flags.Shared, source.Index(rowStart, colStart))
    {
        m_dim = source.m_dim;
        if (source.m_parr != null) SubMatrixPtrs(source, rowStart, rowEnd, colStart, colEnd);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal SubMatrix(uint rowStart, uint rowEnd, uint colStart, uint colEnd)
    {
        if (rowStart < rowEnd && colStart < colEnd && rowEnd <= Rows && colEnd <= Cols)
        {
            Immutable = true;
            return new MatVal(this, (int)rowStart, (int)rowEnd, (int)colStart, (int)colEnd);
        }
        AppException.Raise("index out of bounds", 1u);
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal(object[] vals) : this(vals.Length, 1)
    {
        int num = vals.Length;
        Data.m_rows = 0;
        for (int i = 0; i < num; i++)
        {
            PushRaw(vals[i]);
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal MakeVector(object[] vals) => new(vals);

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal MakeVector0(object[] vals)
    {
        int num = vals.Length;
        MatVal matVal = reserve(num);
        for (int i = 0; i < num; i++)
        {
            object obj = vals[i];
            matVal.PushRaw(obj == NumVal.Zero ? NumVal.ZeroId : obj);
        }
        return matVal;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal reserve(int n) { var result = new MatVal(n); result.Data.m_rows = 0; return result; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal reserve(int r, int c) { var result = new MatVal(r, c); result.Data.m_rows = 0; return result; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal Singleton(object o) { var result = new MatVal(1); result.set(0, o); return result; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal Pair(object o1, object o2) { var result = reserve(2); result.PushRaw(o1); result.PushRaw(o2); return result; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static MatVal Triple(object o1, object o2, object o3) { var result = reserve(3); result.PushRaw(o1); result.PushRaw(o2); result.PushRaw(o3); return result; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal Push(object o) => mcd_set_1(Data.m_rows, o);

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushRaw1(object o) { set(Data.m_rows++, o); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushRaw(object o) { setNaN(Data.m_rows++, o); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushRaw(double re) { set(Data.m_rows++, re, Dim.identity); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushRaw(int re) { set(Data.m_rows++, re, Dim.identity); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushReal(double re) { set(Data.m_rows++, re); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal PushZero() { set(Data.m_rows++, 0.0, Dim.mcd_zero); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public int PushRawIndex(object o) { set(Data.m_rows++, o); return Data.m_rows; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal PushVectorArg(object o)
    {
        Cons cons = o as Cons;
        for (;;)
        {
            if (cons == null) AppException.Raise("must be vector", 1u);
            setspec(Data.m_rows++, cons.Field1);
            if (cons.Field2 == null)
            {
                break;
            }
            cons = cons.Field2 as Cons;
        }
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal PromoteDim() { m_dim ??= Dim.identity; return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal PromoteDim(Dim d) { m_dim ??= Dim.intern(d); return this; }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public static MatVal Replicate(object o, int n) => new MatVal(n).FillThis(o, 0, 1, n).PromoteDim();

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal Reshape(int r, int c) { Data.m_rows = r; Data.m_anr = r; Data.m_cols = c; return PromoteDim(); }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal SetShape() { Data.m_rows = Data.m_anr; if (m_parr != null) Canonicalize(); return PromoteDim(); }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public MatVal SetShape1() { Data.m_rows = Data.m_anr; return this; }

    [MirrorAnalyzed("Mcd.MatVal")]
    public override object DynCall(object b)
    {
        return mcd_get_1((uint)(b as DimVal).asInt(1u));
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public override object DynCall(object b, object c)
    {
        return mcd_get_2((uint)(b as DimVal).asInt(1u), (uint)(c as DimVal).asInt(2u));
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public static object mcd_get_1(object m, object i)
    {
        MatVal obj = m as MatVal;
        NumVal numVal = i as NumVal;
        if (obj == null)
        {
            AppException.Raise("must be vector", 1u);
        }
        if (numVal == null)
        {
            AppException.Raise("must be integer", 2u);
        }
        return obj.mcd_get_1((uint)numVal.asInt("must be integer", 2u));
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public override object matrix_get_1(int i)
    {
        return mcd_get_1((uint)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public override object sym_matrix_get_1(int i)
    {
        return mcd_sym_get_1((uint)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public override object matrix_get_2(int i, int j)
    {
        return mcd_get_2((uint)i, (uint)j);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public override object sym_matrix_get_2(int i, int j)
    {
        return mcd_sym_get_2((uint)i, (uint)j);
    }

    [MirrorVerified("MatValDifferentialTests")]
    public override int Isize => Rows * Cols;

    [MirrorAnalyzed("Mcd.MatVal")]
    public override DimVal iterator_get(int n)
    {
        return (DimVal)mcd_get_2((uint)(n % Rows), (uint)(n / Rows));
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public static object mcd_getm_1(object m, int i)
    {
        MatVal obj = m as MatVal;
        if (obj == null)
        {
            AppException.Raise("must be vector", 1u);
        }
        return obj.mcd_get_1((uint)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_1(uint i)
    {
        if (Data.m_cols != 1)
        {
            AppException.Raise("must be vector", 1u);
        }
        if (i >= Data.m_rows)
        {
            AppException.Raise("index out of bounds", 2u);
        }
        return get_1((int)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_1(uint i)
    {
        if (Data.m_cols != 1)
        {
            AppException.Raise("must be vector");
        }
        if (i >= Data.m_rows)
        {
            AppException.Raise("index out of bounds");
        }
        return get_1((int)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public static object mcd_getm_2(object m, int i, int j)
    {
        MatVal obj = m as MatVal;
        if (obj == null)
        {
            AppException.Raise("must be matrix", 1u);
        }
        return obj.mcd_get_2((uint)i, (uint)j);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_2(uint i, uint j)
    {
        if (i >= Data.m_rows)
        {
            AppException.Raise("index out of bounds", 2u);
        }
        if (j >= Data.m_cols)
        {
            AppException.Raise("index out of bounds", 2u);
        }
        return get((int)i + m_offset + (int)j * Data.m_anr);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_2(uint i, uint j)
    {
        if (i >= Data.m_rows)
        {
            AppException.Raise("index out of bounds");
        }
        if (j >= Data.m_cols)
        {
            AppException.Raise("index out of bounds");
        }
        return get((int)i + m_offset + (int)j * Data.m_anr);
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public object[] get_elements(uint rows, uint cols)
    {
        int index = 0;
        var result = new object[rows * cols];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++) result[index++] = get_2(row, col);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public object[] get_elements() => get_elements((uint)Rows, (uint)Cols);

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_1(int i)
    {
        return mcd_get_1((uint)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_1(int i)
    {
        return mcd_sym_get_1((uint)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_2(int i, int j)
    {
        return mcd_get_2((uint)i, (uint)j);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_2(int i, int j)
    {
        return mcd_sym_get_2((uint)i, (uint)j);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object aref_1(int i)
    {
        return mcd_get_1((uint)i);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object aref_2(int i, int j)
    {
        return mcd_get_2((uint)i, (uint)j);
    }

    [MirrorVerified("MatValDifferentialTests")]
    public int Index(int r) => r + m_offset;

    [MirrorVerified("MatValDifferentialTests")]
    public int Index(int r, int c) => m_offset + r + Anr * c;

    [MirrorVerified("MatValDifferentialTests")]
    public int RowOf(int idx) => (idx - m_offset) % Anr;

    [MirrorVerified("MatValDifferentialTests")]
    public int ColOf(int idx) => (idx - m_offset) / Anr;

    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_1(int i) => get(i + m_offset);

    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_2(int i, int j) => get(i + m_offset + j * Data.m_anr);

    [MirrorAnalyzed("Mcd.MatVal")]
    public Dim get_1_dim(int i) => get_dim(i + m_offset);

    [MirrorAnalyzed("Mcd.MatVal")]
    public Dim get_2_dim(int i, int j) => get_dim(i + m_offset + j * Data.m_anr);

    [MirrorAnalyzed("Mcd.MatVal")]
    public MatDim get_1_matdim(int i) => get_matdim(i + m_offset);

    [MirrorAnalyzed("Mcd.MatVal")]
    public MatDim get_2_matdim(int i, int j) => get_matdim(i + m_offset + j * Data.m_anr);

    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_1_ptr(int i) => get_ptr(i + m_offset);

    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_2_ptr(int i, int j) => get_ptr(i + m_offset + j * Data.m_anr);

    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_1_na(NumVal n, int i) => get_na(n, i + m_offset);

    [MirrorAnalyzed("Mcd.MatVal")]
    public object get_2_na(NumVal n, int i, int j) => get_na(n, i + m_offset + j * Data.m_anr);

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
    public ElementType get_type_2(int i, int j, double complex_thresh)
    {
        return get_type(i + m_offset + j * Data.m_anr, complex_thresh);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private IEnumerable<int> offsets(MatAxis a, int n)
    {
        if (a == MatAxis.Row)
        {
            for (int j = 0; j < Data.m_cols; j++)
            {
                yield return n + m_offset + j * Data.m_anr;
            }
        }
        else
        {
            for (int j = 0; j < Data.m_rows; j++)
            {
                yield return j + m_offset + n * Data.m_anr;
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private ElementType get_type(int offset, double complex_thresh)
    {
        double r = m_rarr[offset];
        if (IsPtrNan(ref r))
        {
            if (m_parr != null)
            {
                object obj = m_parr[offset];
                if (obj is StrVal)
                {
                    return ElementType.String;
                }
                if (obj is NumVal numVal)
                {
                    if (double.IsNaN(numVal.Data.rp) || double.IsNaN(numVal.Data.ip))
                    {
                        return ElementType.NaN;
                    }
                    bool flag;
                    if (double.IsNaN(complex_thresh))
                    {
                        flag = (numVal.Data.ip == 0.0);
                    }
                    else
                    {
                        flag = (Math.Abs(numVal.Data.ip) < complex_thresh);
                    }
                    if (!flag)
                    {
                        return ElementType.Complex;
                    }
                    if (!Dim.Id_b(numVal.m_dim))
                    {
                        return ElementType.UnitedDouble;
                    }
                    return ElementType.Double;
                }
                return ElementType.Other;
            }
            r = m_rarr[offset] = double.NaN;
            return ElementType.NaN;
        }
        if (double.IsNaN(r))
        {
            return ElementType.NaN;
        }
        bool isReal;
        if (m_iarr == null)
        {
            isReal = true;
        }
        else if (double.IsNaN(complex_thresh))
        {
            isReal = (m_iarr[offset] == 0.0);
        }
        else
        {
            isReal = (Math.Abs(m_iarr[offset]) < complex_thresh);
        }
        if (!isReal)
        {
            return ElementType.Complex;
        }
        if (!Dim.Id_b(m_dim))
        {
            return ElementType.UnitedDouble;
        }
        return ElementType.Double;
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<ElementType> element_types(MatAxis a, int n, double complex_thresh)
    {
        return from offset in offsets(a, n)
            select get_type(offset, complex_thresh);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<object> get_ptrs(MatAxis a, int n)
    {
        return offsets(a, n).Select(get_ptr);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private double get_double(int offset, double complex_thresh, double scale)
    {
        if (m_iarr == null)
        {
            return m_rarr[offset] / scale;
        }
        if (double.IsNaN(complex_thresh) && m_iarr[offset] == 0.0)
        {
            return m_rarr[offset] / scale;
        }
        if (Math.Abs(m_iarr[offset]) < complex_thresh)
        {
            return m_rarr[offset] / scale;
        }
        return double.NaN;
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<double> get_doubles(MatAxis a, int n, double complex_thresh, double scale)
    {
        return from offset in offsets(a, n)
            select get_double(offset, complex_thresh, scale);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private object get_data(int offset, double complex_thresh, double scale)
    {
        double r = m_rarr[offset];
        if (IsPtrNan(ref r))
        {
            if (m_parr != null)
            {
                return m_parr[offset];
            }
            r = m_rarr[offset] = double.NaN;
            return r;
        }
        return get_double(offset, complex_thresh, scale);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<object> get_data(MatAxis a, int n, double complex_thresh, double scale)
    {
        return from offset in offsets(a, n)
            select get_data(offset, complex_thresh, scale);
    }

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
    public MatDim get_matdim(int offset)
    {
        if (!IsPtrNan(ref m_rarr[offset]))
        {
            return MatDim.make(m_dim);
        }
        return DimVal.MatDimOf(m_parr[offset]);
    }

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

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public override MatVal extend_1(int i)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (Data.m_cols > 1) AppException.Raise("must be vector", 1u);
        if (Immutable) return ReAllocate(i + 1);
        Stretch(i + 1);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setm_1(object o, int i, object v) => v is DimVal ? (o as DimVal).matrix_set_1(i, v) : (o as DimVal).matrix_set_1_ob(i, v);

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_set_1(int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (Data.m_cols > 1) AppException.Raise("must be vector", 1u);
        if (Immutable) { var copy = ReAllocate(i + 1); copy.set_1(i, v); return copy; }
        Stretch(i + 1);
        set(i + m_offset, v);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_1(int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        Stretch(i + 1);
        set(i + m_offset, v);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_1(int i, double re)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        Stretch(i + 1);
        set(i + m_offset, re, Dim.identity);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setm_2(object o, int i, int j, object v) => v is DimVal ? (o as DimVal).matrix_set_2(i, j, v) : (o as DimVal).matrix_set_2_ob(i, j, v);

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public override MatVal extend_2(int i, int j)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        if (Immutable) return ReAllocate(i + 1, j + 1);
        Stretch(i + 1, j + 1);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_set_2(int i, int j, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        if (Immutable) { var copy = ReAllocate(i + 1, j + 1); copy.set_2(i, j, v); return copy; }
        Stretch(i + 1, j + 1);
        set(Index(i, j), v);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_2(int i, int j, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        Stretch(i + 1, j + 1);
        set(Index(i, j), v);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal mcd_upd_2(int i, int j, double re)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        if (j < 0) AppException.Raise("index out of bounds", 3u);
        Stretch(i + 1, j + 1);
        set(Index(i, j), re, Dim.identity);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setcolumn(object o, int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        MatVal matVal = o as MatVal;
        if (matVal == null)
        {
            MatVal matVal2 = v as MatVal;
            if (matVal2 == null)
            {
                matVal2 = Singleton(v);
            }
            int rows = matVal2.Rows;
            MatVal matVal3 = new MatVal(rows, i + 1);
            matVal3.setcolumn(i, matVal2);
            return matVal3;
        }
        if (matVal.Immutable)
        {
            MatVal matVal4 = v as MatVal;
            if (matVal4 == null)
            {
                matVal4 = Singleton(v);
            }
            int rows2 = matVal4.Rows;
            MatVal matVal5 = matVal.ReAllocate(rows2, i + 1);
            matVal5.setcolumn(i, matVal4);
            return matVal5;
        }
        matVal.setcolumn(i, v);
        return matVal;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setcolumn(int i, object o)
    {
        MatVal matVal = o as MatVal;
        if (matVal == null)
        {
            matVal = Singleton(o);
        }
        int rows = matVal.Rows;
        if (!matVal.IsVector)
        {
            AppException.Raise("must be vector", 2u);
        }
        Stretch(rows, i + 1);
        setcol(i, matVal);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public void setcol(int i, MatVal v)
    {
        if (v.m_dim != m_dim)
        {
            if (m_dim == null)
            {
                m_dim = v.m_dim;
            }
            else
            {
                setmatrix_ptrs(0, i, v);
                return;
            }
        }
        int rows = v.Rows;
        int destOffset = m_offset + i * Data.m_anr;
        Copy(v.m_rarr, v.m_offset, m_rarr, destOffset, rows);
        if (v.m_iarr == null)
        {
            if (m_iarr != null)
            {
                FillArr(m_iarr, 0.0, destOffset, 1, rows);
            }
        }
        else
        {
            if (m_iarr == null)
            {
                AllocImaginary();
            }
            Copy(v.m_iarr, v.m_offset, m_iarr, destOffset, rows);
        }
        if (v.RepHasPtrs)
        {
            if (m_parr == null)
            {
                m_parr = new Hashtable();
            }
            for (int j = 0; j < rows; j++)
            {
                if (IsPtrNan(ref m_rarr[j + destOffset]))
                {
                    m_parr[destOffset + j] = v.m_parr[j + v.m_offset];
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal mcd_setrow(object o, int i, object v)
    {
        if (i < 0) AppException.Raise("index out of bounds", 2u);
        MatVal matVal = o as MatVal;
        if (matVal == null)
        {
            MatVal matVal2 = v as MatVal;
            if (matVal2 == null)
            {
                matVal2 = Singleton(v);
            }
            int cols = matVal2.Cols;
            MatVal matVal3 = new MatVal(i + 1, cols);
            matVal3.setrow(i, matVal2);
            return matVal3;
        }
        if (matVal.Immutable)
        {
            MatVal matVal4 = v as MatVal;
            if (matVal4 == null)
            {
                matVal4 = Singleton(v);
            }
            int cols2 = matVal4.Cols;
            MatVal matVal5 = matVal.ReAllocate(i + 1, cols2);
            matVal5.setrow(i, matVal4);
            return matVal5;
        }
        matVal.setrow(i, v);
        return matVal;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setrow(int i, object o) => setblock(i, 0, o);

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal setblock(int i, int j, object v)
    {
        MatVal matVal = v as MatVal;
        if (matVal == null)
        {
            Stretch(i + 1, j + 1);
            set_2(i, j, v);
        }
        else
        {
            Stretch(i + matVal.Rows, j + matVal.Cols);
            setmatrix(i, j, matVal);
        }
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void setmatrix(int i, int j, MatVal v)
    {
        if (v.m_dim != m_dim)
        {
            if (m_dim == null)
            {
                m_dim = v.m_dim;
            }
            else
            {
                setmatrix_ptrs(i, j, v);
                return;
            }
        }
        int rows = v.Rows;
        int cols = v.Cols;
        int anr = Anr;
        int anr2 = v.Anr;
        int destOffset = m_offset + i + j * anr;
        int srcOffset = v.m_offset;
        if (v.m_iarr != null && m_iarr == null)
        {
            AllocImaginary();
        }
        for (int k = 0; k < cols; k++)
        {
            Copy(v.m_rarr, srcOffset, m_rarr, destOffset, rows);
            if (v.m_iarr == null)
            {
                if (m_iarr != null)
                {
                    FillArr(m_iarr, 0.0, destOffset, 1, rows);
                }
            }
            else
            {
                Copy(v.m_iarr, srcOffset, m_iarr, destOffset, rows);
            }
            srcOffset += anr2;
            destOffset += anr;
        }
        if (v.RepHasPtrs)
        {
            AssignPtrs(v, i, j);
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void setmatrix_ptrs(int ii, int jj, MatVal v)
    {
        int rows = v.Rows;
        int cols = v.Cols;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                set_2(i + ii, j + jj, v.get_2(i, j));
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal ReAlloc(object o, int r)
    {
        MatVal matVal = o as MatVal;
        if (matVal != null)
        {
            return matVal.ReAllocate(r);
        }
        return new MatVal(r);
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public static MatVal ReAlloc(object o, int r, int c)
    {
        MatVal matVal = o as MatVal;
        if (matVal != null)
        {
            return matVal.ReAllocate(r, c);
        }
        return new MatVal(r, c);
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal ReAllocate(int r) => new(this, Math.Max(r, Rows), 1);

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public MatVal ReAllocate(int r, int c) => new(this, Math.Max(r, Rows), Math.Max(c, Cols));

    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_1(int i, object x) => set(i + m_offset, x);

    [MirrorAnalyzed("Mcd.MatVal")]
    public void set_2(int i, int j, object x) => set(Index(i, j), x);

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
        {
            foreach (DictionaryEntry item in m_parr)
            {
                if (item.Value == p)
                {
                    return true;
                }
                if (item.Value is MatVal matVal && matVal.contains(p))
                {
                    return true;
                }
            }
        }
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

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    [MirrorVerified("MatValCollectionDifferentialTests")]
    public static object vectorize(Function f, object[] args)
    {
        int argc = args.Length;
        arg_desc[] descriptors = null;
        int mat_count = 0;
        int mat_rows = 0;
        int mat_cols = 0;
        bool all_polymorphic = true;
        f.AssertFullApp(argc);
        for (int i = 0; i < argc; i++)
        {
            if (args[i] is MatVal matVal)
            {
                if (mat_count == 0)
                {
                    descriptors = new arg_desc[argc - i];
                    mat_rows = matVal.Rows;
                    mat_cols = matVal.Cols;
                }
                else if (mat_rows != matVal.Rows || mat_cols != matVal.Cols) AppException.Raise("unequal matrix dimensions");
                descriptors[mat_count].m_mat = matVal;
                descriptors[mat_count].m_pos = i;
                mat_count++;
                if (!matVal.IsPolymorphic)
                {
                    all_polymorphic = false;
                }
            }
        }
        if (mat_count == 0)
        {
            return f.Apply(args);
        }
        MatVal result = new MatVal(mat_rows, mat_cols);
        for (int j = 0; j < mat_cols; j++)
        {
            for (int k = 0; k < mat_rows; k++)
            {
                for (int l = 0; l < mat_count; l++)
                {
                    descriptors[l].FetchInto(args, k, j);
                }
                result.set_2(k, j, f.Apply(args));
            }
        }
        if (result.IsPolymorphic && result.m_dim == null && !all_polymorphic)
        {
            result.m_dim = Dim.identity;
        }
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Transpose()
    {
        var result = new MatVal(TransposeReal(), TransposeImag(), Cols, Rows) { m_dim = m_dim };
        result.TransposePtrs(this);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    public MatVal Fill(object v, int init, int incr, int n)
    {
        int extent = (incr > 0 ? init + incr * (n - 1) : init) + 1;
        if (Immutable) return ReAllocate(extent).FillThis(v, init, incr, n);
        Stretch(extent);
        return FillThis(v, init, incr, n);
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal FillThis(object o, int init, int incr, int n)
    {
        if (o is NumVal x)
        {
            return FillThis(x, init, incr, n);
        }
        return FillThisPtrs(o, init, incr, n);
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal FillThis(NumVal x, int init, int incr, int n)
    {
        int start = init + m_offset;
        if (m_iarr == null)
        {
            if (x.Data.ip == 0) { FillArr(m_rarr, x.Data.rp, start, incr, n); return this; }
            AllocImaginary();
        }
        FillArr(m_rarr, x.Data.rp, start, incr, n);
        FillArr(m_iarr, x.Data.ip, start, incr, n);
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private MatVal FillThisPtrs(object p, int init, int incr, int n)
    {
        int num = init + m_offset;
        for (int i = 0; i < n; i++)
        {
            set_ptrs_at(num, p);
            num += incr;
        }
        return this;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override DimVal asImmutable() { Immutable = true; return this; }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public MatVal Fix() { Immutable = true; return this; }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static MatVal Fix(object value)
    {
        var matVal = value as MatVal;
        if (matVal != null) return matVal.Fix();
        return null;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public override DimVal asResult()
    {
        var result = Packed ? this : new MatVal(this);
        result = result.Canonicalize();
        if (result.m_dim == null) result.m_dim = Dim.FixDimension(result.m_dim);
        return result.Fix();
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public static object FixForExport(object value)
    {
        if (value is not DimVal dimensional) return value is string text ? new StrVal(text) : value;
        if (dimensional.m_dim == null) dimensional.m_dim = Dim.FixDimension(dimensional.m_dim);
        var matVal = value as MatVal;
        if (matVal != null) return matVal.Fix();
        return dimensional.asResult();
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

    [MirrorVerified("MatValDifferentialTests")]
    public static unsafe double ThePtrNan()
    {
        double val;
        double* bits = &val;
        int* p = (int*)bits;
        *p = -1;
        p[1] = -1;
        return *bits;
    }

    [MirrorTested("Mcd.MatVal")]
    private bool IsPtrNan(int r, int c) => IsPtrNan(ref m_rarr[Index(r, c)]);

    [MirrorVerified("MatValDifferentialTests")]
    public void AllocImaginary() => m_iarr = new double[m_rarr.Length];

    [MirrorAnalyzed("Mcd.MatVal")]
    public void AllocImaginary(double[] iarr) => m_iarr = iarr;

    [MirrorAnalyzed("Mcd.MatVal.Collections")]
    private static void FillArr(double[] arr, double v, int init, int incr, int n)
    {
        int num;
        int i;
        int num2;
        int j;
        if (incr == 1)
        {
            num = init + n;
            for (i = init; i < num; i++)
            {
                arr[i] = v;
            }
            return;
        }
        num2 = init;
        for (j = 0; j < n; j++)
        {
            arr[num2] = v;
            num2 += incr;
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private double[] TransposeArr(double[] array)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        int offset = Offset;
        var result = new double[rows * cols];
        for (int col = 0; col < cols; col++)
        {
            int dstIndex = col;
            int srcIndex = offset + col * anr;
            for (int row = 0; row < rows; row++)
            {
                result[dstIndex] = array[srcIndex++];
                dstIndex += cols;
            }
        }
        return result;
    }

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

    [MirrorAnalyzed("Mcd.MatVal")]
    public object[] PackObjects(int offset)
    {
        int rows = Rows;
        int cols = Cols;
        object[] array = new object[rows * cols + offset];
        int num = offset;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                array[num++] = get_2(j, i);
            }
        }
        return array;
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public object[] PackObjects()
    {
        return PackObjects(0);
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void Stretch(int n)
    {
        if (n > Data.m_rows)
        {
            if (n > Data.m_anr)
            {
                int newAnr = Math.Max(n, 2 * Data.m_anr);
                m_rarr = Resize(m_rarr, newAnr);
                m_iarr = (m_iarr == null) ? null : Resize(m_iarr, newAnr);
                if (RepHasPtrs)
                {
                    int offset = m_offset;
                    int limit = offset + n;
                    for (int i = offset; i < limit; i++)
                    {
                        if (i - offset >= Data.m_rows && !IsPtrNan(ref m_rarr[i]) && m_rarr[i] == 0.0)
                        {
                            if (m_parr == null)
                            {
                                m_parr = new Hashtable();
                            }
                            m_rarr[i] = ThePtrNan();
                            m_parr[i] = NumVal.ZeroId;
                        }
                    }
                }
                Data.m_anr = newAnr;
            }
            Data.m_rows = n;
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void TidyZeros(int oldr, int oldc, int newr, int newc)
    {
        if (RepHasPtrs)
        {
            for (int i = 0; i < newc; i++)
            {
                int start = i * Data.m_anr + m_offset;
                int limit = start + newr;
                for (int j = start; j < limit; j++)
                {
                    if ((i >= oldc || j - start >= oldr) && !IsPtrNan(ref m_rarr[j]) && m_rarr[j] == 0.0)
                    {
                        if (m_dim != Dim.identity && m_dim != null)
                        {
                            if (m_parr == null)
                            {
                                m_parr = new Hashtable();
                            }
                            m_rarr[j] = ThePtrNan();
                            m_parr[j] = NumVal.ZeroId;
                        }
                        else if (m_dim == null)
                        {
                            m_dim = Dim.identity;
                        }
                    }
                }
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void Stretch(int r, int c)
    {
        int oldCols = Data.m_cols;
        int oldRows = Data.m_rows;
        if (c > Data.m_cols)
        {
            if (m_rarr == null || c * Data.m_anr <= m_rarr.Length)
            {
                Data.m_cols = c;
            }
            else
            {
                int capacity = Math.Max(r, Rows);
                if (m_parr != null && capacity != Data.m_anr)
                {
                    m_parr = ResizePtrs(capacity, m_offset);
                }
                m_rarr = Resize(m_rarr, capacity, c);
                m_iarr = (m_iarr == null) ? null : Resize(m_iarr, capacity, c);
                Data.m_rows = capacity;
                Data.m_cols = c;
                Data.m_anr = capacity;
                TidyZeros(oldRows, oldCols, Data.m_rows, Data.m_cols);
                return;
            }
        }
        if (r > Data.m_rows)
        {
            if (r > Data.m_anr)
            {
                int newCols = Math.Max(c, Cols);
                int capacity = Math.Max(r, Math.Min(2 * Data.m_anr, 16777216 / (newCols == 0 ? 1 : newCols)));
                if (m_parr != null)
                {
                    m_parr = ResizePtrs(capacity, m_offset);
                }
                m_rarr = Resize(m_rarr, capacity, newCols);
                m_iarr = (m_iarr == null) ? null : Resize(m_iarr, capacity, newCols);
                Data.m_cols = newCols;
                Data.m_anr = capacity;
                TidyZeros(oldRows, oldCols, r, Data.m_cols);
            }
            Data.m_rows = r;
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    public void StretchWithColumnFill(int r, int c, DimVal fill)
    {
        int oldCols = Cols;
        int rows = Rows;
        Stretch(r, c);
        for (int i = oldCols; i < c; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                set_2(j, i, fill);
            }
        }
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private double[] Resize(double[] arr, int nr)
    {
        var result = new double[nr];
        Copy(arr, Offset, result, 0, Rows);
        return result;
    }

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private double[] CopyResize(double[] arr, int nr, int nc) => Resize(arr, Math.Max(nr, Rows), Math.Max(nc, Cols));

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private double[] Resize(double[] arr, int nr, int nc)
    {
        int rows = Rows;
        int cols = Cols;
        int anr = Anr;
        int offset = Offset;
        double[] array = new double[nr * nc];
        if (nr == anr)
        {
            Copy(arr, offset, array, 0, nr * cols);
        }
        else
        {
            for (int i = 0; i < cols; i++)
            {
                Copy(arr, offset + anr * i, array, nr * i, rows);
            }
        }
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

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public double[] TransposeReal() => TransposeArr(m_rarr);

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public double[] TransposeImag() => m_iarr == null ? null : TransposeArr(m_iarr);

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

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void SubMatrixPtrs(MatVal source, int rowStart, int rowEnd, int colStart, int colEnd) =>
        TransformPtrs(source, new TrimSubMatrixPtrs(this, rowStart, rowEnd, colStart, colEnd).set_submatrix_ptrs);

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private void AssignPtrs(MatVal m, int r, int c) => TransformPtrs(m, new AssignMatrixPtrs(this, r, c).assign_matrix_ptrs);

    [MirrorAnalyzed("Mcd.MatVal.Growth")]
    private Hashtable ResizePtrs(int anr, int off)
    {
        Hashtable hashtable = new Hashtable();
        ResizeMatrixPtrs resizeMatrixPtrs = new ResizeMatrixPtrs(hashtable, anr, off);
        IteratePtrs(new PtrDelegate(resizeMatrixPtrs.resize_matrix_ptrs));
        if (hashtable.Count <= 0)
        {
            return null;
        }
        return hashtable;
    }

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void set_ptrs_2_transpose(int row, int col, object value) => set_ptrs_2(col, row, value);

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public void TransposePtrs(MatVal source) => TransformPtrs(source, set_ptrs_2_transpose);

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    private void set_ptrs_2_reverse(int row, int col, object value) => set_ptrs_2(Rows - row, col, value);

    [MirrorAnalyzed("Mcd.MatVal.Views")]
    public void ReversePtrs(MatVal source) => TransformPtrs(source, set_ptrs_2_reverse);

    [MirrorAnalyzed("Mcd.MatVal.Storage")]
    public void CopyPtrs(MatVal m)
    {
        TransformPtrs(m, set_ptrs_2);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    public static MatVal operator /(MatVal matrix, double scalar)
    {
        if (matrix.Packed && !matrix.Immutable)
        {
            return matrix.Scale(scalar);
        }
        return new MatVal(matrix).Scale(scalar);
    }

    [MirrorAnalyzed("Mcd.MatVal")]
    private unsafe MatVal Scale(double scalar)
    {
        if (!IsUniformScalar)
        {
            fixed (double* ptr = &m_rarr[0])
            {
                int num = m_rarr.Length;
                while (--num >= 0)
                {
                    if (IsPtrNan(ref ptr[num]))
                    {
                        m_parr[num] = Utilities.Divide(m_parr[num], scalar);
                    }
                    else
                    {
                        ptr[num] /= scalar;
                    }
                }
            }
        }
        else
        {
            Utilities.ScaleVector(m_rarr, scalar);
        }
        if (m_iarr != null)
        {
            Utilities.ScaleVector(m_iarr, scalar);
        }
        return this;
    }

    [MirrorOf("Mcd.MatVal+arg_desc, mplrun")]
    private partial struct arg_desc
    {
        [MirrorAnalyzed("Mcd.MatVal.Collections")] public int m_pos;
        [MirrorAnalyzed("Mcd.MatVal.Collections")] public MatVal m_mat;
        [MirrorAnalyzed("Mcd.MatVal.Collections")]
        public void FetchInto(object[] args, int i, int j) => args[m_pos] = m_mat.get_2(i, j);
    }

    [MirrorOf("Mcd.MatVal+TrimSubMatrixPtrs, mplrun")]
    private partial class TrimSubMatrixPtrs
    {
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private MatVal m_m;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_r1;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_r2;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_c1;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        private int m_c2;
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        public TrimSubMatrixPtrs(MatVal matrix, int rowStart, int rowEnd, int colStart, int colEnd)
        { m_m = matrix; m_r1 = rowStart; m_r2 = rowEnd; m_c1 = colStart; m_c2 = colEnd; }
        [MirrorAnalyzed("Mcd.MatVal.Views")]
        public void set_submatrix_ptrs(int row, int col, object value)
        {
            if (m_r1 <= row && row < m_r2 && m_c1 <= col && col < m_c2)
                m_m.set_ptrs_2(row - m_r1, col - m_c1, value);
        }
    }

    [MirrorOf("Mcd.MatVal+AssignMatrixPtrs, mplrun")]
    private partial class AssignMatrixPtrs
    {
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private MatVal m_m;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_r;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_c;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public AssignMatrixPtrs(MatVal m, int r, int c) { m_m = m; m_r = r; m_c = c; }
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public void assign_matrix_ptrs(int i, int j, object v) => m_m.set_ptrs_2(i + m_r, j + m_c, v);
    }

    [MirrorOf("Mcd.MatVal+ResizeMatrixPtrs, mplrun")]
    private partial class ResizeMatrixPtrs
    {
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private Hashtable m_parr;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_anr;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        private int m_off;
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public ResizeMatrixPtrs(Hashtable parr, int anr, int off) { m_parr = parr; m_anr = anr; m_off = off; }
        [MirrorAnalyzed("Mcd.MatVal.Growth")]
        public void resize_matrix_ptrs(int i, int j, object v) => m_parr[m_off + i + j * m_anr] = v;
    }
}

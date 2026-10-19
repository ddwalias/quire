#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public sealed partial class MatVal
{
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
    public ElementType get_type_2(int i, int j, double complex_thresh)
    {
        return get_type(i + m_offset + j * Data.m_anr, complex_thresh);
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
                NumVal numVal = obj as NumVal;
                if (numVal == null)
                {
                    return ElementType.Other;
                }
                if (double.IsNaN(numVal.Data.rp) || double.IsNaN(numVal.Data.ip))
                {
                    return ElementType.NaN;
                }
                bool flag = double.IsNaN(complex_thresh) ? (numVal.Data.ip == 0.0) : (Math.Abs(numVal.Data.ip) < complex_thresh);
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
            m_rarr[offset] = (r = double.NaN);
            return ElementType.NaN;
        }
        if (double.IsNaN(r))
        {
            return ElementType.NaN;
        }
        bool flag2;
        if (m_iarr == null)
        {
            flag2 = true;
        }
        else if (double.IsNaN(complex_thresh))
        {
            flag2 = (m_iarr[offset] == 0.0);
        }
        else
        {
            flag2 = (Math.Abs(m_iarr[offset]) < complex_thresh);
        }
        if (!flag2)
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
            double nan = m_rarr[offset] = double.NaN;
            return nan;
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
    public object[] PackObjects()
    {
        return PackObjects(0);
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quire.Mcd;

namespace Quire.Automation;

public partial class MatrixValueProxy : ValueProxy, IMatrixValueProxy, IValueProxy
{
    private partial class MatrixValueData : IMatrixValueDataAccessor
    {
        private MatrixValueProxy m_parent;

        private PlainDataConversionOptions m_options;

        private double m_scale;

        private PlainDataTypes non_plain_mask = PlainDataTypes.NonPlainType | PlainDataTypes.NonUniformlyUnitedDouble;

        private PlainDataTypes double_mask = PlainDataTypes.NaN | PlainDataTypes.Double | PlainDataTypes.UniformlyUnitedDouble;

        private PlainDataTypes string_mask = PlainDataTypes.NaN | PlainDataTypes.String;

        private PlainDataTypes elem_to_plain(ElementType e)
        {
            if (e == ElementType.Double)
            {
                return PlainDataTypes.Double;
            }
            if (e == ElementType.String)
            {
                return PlainDataTypes.String;
            }
            if (e == ElementType.Complex || e == ElementType.NaN)
            {
                return PlainDataTypes.NaN;
            }
            if (e == ElementType.UnitedDouble)
            {
                if (!m_parent.m_value.IsUniform)
                {
                    return PlainDataTypes.NonUniformlyUnitedDouble;
                }
                return PlainDataTypes.UniformlyUnitedDouble;
            }
            return PlainDataTypes.NonPlainType;
        }

        private PlainDataTypes fold_plain(IEnumerable<PlainDataTypes> e)
        {
            return e.Aggregate(PlainDataTypes.Uninitialized, (PlainDataTypes accum, PlainDataTypes x) => accum | x);
        }

        private PlainDataTypes plain_type(IEnumerable<ElementType> e)
        {
            return fold_plain(e.Select(elem_to_plain));
        }

        private IEnumerable<int> indices(MatAxis a)
        {
            int count = ((a == MatAxis.Row) ? m_parent.m_value.Rows : m_parent.m_value.Cols);
            return Enumerable.Range(0, count);
        }

        private IEnumerable<ElementType> element_types(MatAxis a, int n)
        {
            return m_parent.m_value.element_types(a, n, m_options.ImaginaryThreshold);
        }

        private PlainDataTypes plain_type(MatAxis a, int n)
        {
            return plain_type(element_types(a, n));
        }

        private IEnumerable<PlainDataTypes> plain_types(MatAxis a)
        {
            return from n in indices(a)
                select plain_type(a, n);
        }

        private IEnumerable<int> mask(MatAxis a, PlainDataTypes mask)
        {
            return (from n in indices(a)
                where (plain_type(a, n) & mask) != 0
                select n).DefaultIfEmpty(-1);
        }

        private IEnumerable<double> double_data(MatAxis a, int n)
        {
            MatVal value = m_parent.m_value;
            double imaginaryThreshold = m_options.ImaginaryThreshold;
            return value.get_doubles(a, n, imaginaryThreshold, m_scale);
        }

        private IEnumerable<string> string_data(MatAxis a, int n)
        {
            return from x in m_parent.m_value.get_ptrs(a, n)
                select (x == null) ? null : x.ToString();
        }

        private IEnumerable<object> get_data(MatAxis a, int n)
        {
            MatVal value = m_parent.m_value;
            double imaginaryThreshold = m_options.ImaginaryThreshold;
            return from x in value.get_data(a, n, imaginaryThreshold, m_scale)
                select (x is StrVal) ? x.ToString() : x;
        }

        private bool not_nan(double d)
        {
            return !double.IsNaN(d);
        }

        private bool obj_not_nan(object o)
        {
            if (o is double)
            {
                return not_nan(Convert.ToDouble(o));
            }
            return true;
        }

        private IList plain_data(MatAxis a, int n)
        {
            PlainDataTypes plainDataTypes = plain_type(a, n);
            List<double> list;
            List<string> stringData;
            List<object> objectData;
            if ((plainDataTypes & non_plain_mask) == 0)
            {
                if ((plainDataTypes & ~double_mask) == 0)
                {
                    list = new List<double>(double_data(a, n));
                    if ((plainDataTypes & PlainDataTypes.NaN) != 0)
                    {
                        return list.GetRange(0, list.FindLastIndex(not_nan) + 1);
                    }
                    return list;
                }
                if ((plainDataTypes & ~string_mask) == 0)
                {
                    stringData = new List<string>(string_data(a, n));
                    if ((plainDataTypes & PlainDataTypes.NaN) != 0)
                    {
                        return stringData.GetRange(0, stringData.FindLastIndex((string s) => s != null) + 1);
                    }
                    return stringData;
                }
                objectData = new List<object>(get_data(a, n));
                if ((plainDataTypes & PlainDataTypes.NaN) != 0)
                {
                    return objectData.GetRange(0, objectData.FindLastIndex(obj_not_nan) + 1);
                }
                return objectData;
            }
            return null;
        }

        private List<IList> plain_data(MatAxis a, int startIndex, int endIndex)
        {
            int count = (endIndex == -1)
                ? (int)((a == MatAxis.Row) ? m_parent.Rows : m_parent.Cols)
                : (endIndex - startIndex + 1);
            return new List<IList>(from n in Enumerable.Range(startIndex, count)
                select plain_data(a, n));
        }

        public MatrixValueData(MatrixValueProxy p, PlainDataConversionOptions o, double scale)
        {
            m_parent = p;
            m_options = o;
            m_scale = scale;
        }

        public List<PlainDataTypes> GetRowsPlainDataTypes()
        {
            return new List<PlainDataTypes>(plain_types(MatAxis.Row));
        }

        public int GetFirstNonPlainDataRowIndex()
        {
            return mask(MatAxis.Row, non_plain_mask).First();
        }

        public List<IList> GetRowsPlainDataValues(int startIndex, int endIndex)
        {
            return plain_data(MatAxis.Row, startIndex, endIndex);
        }

        public List<PlainDataTypes> GetColumnsPlainDataTypes()
        {
            return new List<PlainDataTypes>(plain_types(MatAxis.Col));
        }

        public int GetFirstNonPlainDataColumnIndex()
        {
            return mask(MatAxis.Col, non_plain_mask).First();
        }

        public List<IList> GetColumnsPlainDataValues(int startIndex, int endIndex)
        {
            return plain_data(MatAxis.Col, startIndex, endIndex);
        }
    }

    protected MatVal m_value;

    protected MatVal Mat => m_value;

    public override object Core => m_value;

    public override IMatrixValueProxy AsMatrix => this;

    public uint Rows => (uint)Mat.Rows;

    public uint Cols => (uint)Mat.Cols;

    public bool IsRange => Mat.IsRange;

    public bool IsUniform => Mat.IsUniform;

    public bool IsUniformScalar => Mat.IsUniformScalar;

    public bool IsDimless => Mat.IsDimless;

    public bool IsVector => Mat.IsVector;

    public bool IsRealMatrix => Mat.IsRealMatrix;

    public bool IsScalarMatrix => Mat.IsScalarMatrix;

    public bool IsRealMatrixMixed => Mat.IsRealMatrixMixed;

    public MatrixValueProxy(MatVal v, bool bSimplify, RuntimeOptions o)
        : base(bSimplify, o)
    {
        m_value = v;
    }

    public IMatrixValueProxy SubMatrix(uint r1, uint r2, uint c1, uint c2)
    {
        return new MatrixValueProxy(Mat.SubMatrix(r1, r2, c1, c2), Simplify, m_options);
    }

    public IValueProxy Get(uint i, uint j)
    {
        return ValueProxy.MakeProxy(Mat.mcd_get_2(i, j), Simplify, m_options);
    }

    public IMatrixValueDataAccessor GetPlainData(PlainDataConversionOptions options)
    {
        return new MatrixValueData(this, options, Unit.magnitude);
    }
}

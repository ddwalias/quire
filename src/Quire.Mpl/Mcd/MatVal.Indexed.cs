#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public sealed partial class MatVal
{
    [MirrorAnalyzed("Mcd.MatVal")]
    public override object DynCall(object b) => mcd_get_1((uint)(b as DimVal).asInt(1));
    [MirrorAnalyzed("Mcd.MatVal")]
    public override object DynCall(object b, object c) => mcd_get_2((uint)(b as DimVal).asInt(1), (uint)(c as DimVal).asInt(2));
    [MirrorAnalyzed("Mcd.MatVal")]
    public static object mcd_get_1(object m, object i)
    {
        var matrix = m as MatVal;
        var index = i as NumVal;
        if (matrix == null) AppException.Raise("must be vector", 1u);
        if (index == null) AppException.Raise("must be integer", 2u);
        return matrix.mcd_get_1((uint)index.asInt("must be integer", 2));
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public override object matrix_get_1(int i) => mcd_get_1((uint)i);
    [MirrorAnalyzed("Mcd.MatVal")]
    public override object matrix_get_2(int i, int j) => mcd_get_2((uint)i, (uint)j);
    [MirrorAnalyzed("Mcd.MatVal")]
    public override object sym_matrix_get_1(int i) => mcd_sym_get_1((uint)i);
    [MirrorAnalyzed("Mcd.MatVal")]
    public override object sym_matrix_get_2(int i, int j) => mcd_sym_get_2((uint)i, (uint)j);
    [MirrorAnalyzed("Mcd.MatVal")]
    public override DimVal iterator_get(int n) => (DimVal)mcd_get_2((uint)(n % Rows), (uint)(n / Rows));
    [MirrorAnalyzed("Mcd.MatVal")]
    public static object mcd_getm_1(object m, int i)
    {
        var matrix = m as MatVal;
        if (matrix == null) AppException.Raise("must be vector", 1u);
        return matrix.mcd_get_1((uint)i);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public static object mcd_getm_2(object m, int i, int j)
    {
        var matrix = m as MatVal;
        if (matrix == null) AppException.Raise("must be matrix", 1u);
        return matrix.mcd_get_2((uint)i, (uint)j);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_1(uint i)
    {
        if (Cols != 1) AppException.Raise("must be vector", 1u);
        if (i >= Rows) AppException.Raise("index out of bounds", 2u);
        return get_1((int)i);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_1(uint i)
    {
        if (Cols != 1) AppException.Raise("must be vector");
        if (i >= Rows) AppException.Raise("index out of bounds");
        return get_1((int)i);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_2(uint i, uint j)
    {
        if (i >= Rows || j >= Cols) AppException.Raise("index out of bounds", 2u);
        return get_2((int)i, (int)j);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_2(uint i, uint j)
    {
        if (i >= Rows || j >= Cols) AppException.Raise("index out of bounds");
        return get_2((int)i, (int)j);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_1(int i) => mcd_get_1((uint)i);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_get_2(int i, int j) => mcd_get_2((uint)i, (uint)j);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_1(int i) => mcd_sym_get_1((uint)i);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object mcd_sym_get_2(int i, int j) => mcd_sym_get_2((uint)i, (uint)j);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object aref_1(int i) => mcd_get_1((uint)i);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object aref_2(int i, int j) => mcd_get_2((uint)i, (uint)j);

    [MirrorAnalyzed("Mcd.MatVal")]
    private IEnumerable<int> offsets(MatAxis a, int n)
    {
        if (a == MatAxis.Row)
            for (int col = 0; col < Cols; col++) yield return Index(n, col);
        else for (int row = 0; row < Rows; row++) yield return Index(row, n);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public ElementType get_type_2(int i, int j, double complex_thresh) => get_type(Index(i, j), complex_thresh);
    [MirrorAnalyzed("Mcd.MatVal")]
    private ElementType get_type(int offset, double complex_thresh)
    {
        double real = m_rarr[offset];
        if (IsPtrNan(ref real))
        {
            if (m_parr != null)
            {
                var pointer = m_parr[offset];
                if (pointer is StrVal) return ElementType.String;
                if (pointer is not NumVal number) return ElementType.Other;
                if (double.IsNaN(number.Real) || double.IsNaN(number.Imag)) return ElementType.NaN;
                bool realNumber = double.IsNaN(complex_thresh) ? number.Imag == 0 : Math.Abs(number.Imag) < complex_thresh;
                if (!realNumber) return ElementType.Complex;
                return Dim.Id_b(number.m_dim) ? ElementType.Double : ElementType.UnitedDouble;
            }
            m_rarr[offset] = double.NaN;
            return ElementType.NaN;
        }
        if (double.IsNaN(real)) return ElementType.NaN;
        if (m_iarr != null)
        {
            bool realNumber = double.IsNaN(complex_thresh) ? m_iarr[offset] == 0 : Math.Abs(m_iarr[offset]) < complex_thresh;
            if (!realNumber) return ElementType.Complex;
        }
        return Dim.Id_b(m_dim) ? ElementType.Double : ElementType.UnitedDouble;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<ElementType> element_types(MatAxis a, int n, double complex_thresh) => offsets(a, n).Select(i => get_type(i, complex_thresh));
    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<object> get_ptrs(MatAxis a, int n) => offsets(a, n).Select(get_ptr);
    [MirrorAnalyzed("Mcd.MatVal")]
    private double get_double(int offset, double complex_thresh, double scale)
    {
        if (m_iarr == null || double.IsNaN(complex_thresh) && m_iarr[offset] == 0 || Math.Abs(m_iarr[offset]) < complex_thresh)
            return m_rarr[offset] / scale;
        return double.NaN;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<double> get_doubles(MatAxis a, int n, double complex_thresh, double scale) => offsets(a, n).Select(i => get_double(i, complex_thresh, scale));
    [MirrorAnalyzed("Mcd.MatVal")]
    private object get_data(int offset, double complex_thresh, double scale)
    {
        if (!IsPtrNan(ref m_rarr[offset])) return get_double(offset, complex_thresh, scale);
        return m_parr != null ? m_parr[offset] : m_rarr[offset] = double.NaN;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public IEnumerable<object> get_data(MatAxis a, int n, double complex_thresh, double scale) => offsets(a, n).Select(i => get_data(i, complex_thresh, scale));

    [MirrorAnalyzed("Mcd.MatVal")]
    public object[] PackObjects() => PackObjects(0);
    [MirrorAnalyzed("Mcd.MatVal")]
    public object[] PackObjects(int offset)
    {
        var result = new object[Rows * Cols + offset];
        int index = offset;
        for (int c = 0; c < Cols; c++) for (int r = 0; r < Rows; r++) result[index++] = get_2(r, c);
        return result;
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public static MatVal operator /(MatVal matrix, double scalar) => (matrix.Packed && !matrix.Immutable ? matrix : new MatVal(matrix)).Scale(scalar);
    [MirrorAnalyzed("Mcd.MatVal")]
    private MatVal Scale(double scalar)
    {
        if (IsUniformScalar) Utilities.ScaleVector(m_rarr, scalar);
        else
        {
            _ = m_rarr[0];
            for (int i = m_rarr.Length - 1; i >= 0; i--)
                if (IsPtrNan(ref m_rarr[i])) m_parr[i] = Utilities.Divide(m_parr[i], scalar);
                else m_rarr[i] /= scalar;
        }
        if (m_iarr != null) Utilities.ScaleVector(m_iarr, scalar);
        return this;
    }
}

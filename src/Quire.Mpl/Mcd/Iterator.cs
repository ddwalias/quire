#nullable disable
using Quire.Mirror;

namespace Quire.Mcd;

/// <summary>
/// A cursor over the values of a for loop: <see cref="first"/> is the current value and <see cref="next"/> advances,
/// returning null when done. Behaviour follows mplrun 12.0.1.0 (docs/mirror/evidence/Mcd.RngVal.md).
/// </summary>
public abstract partial class Iterator
{
    /// <summary>The iterators one after another.</summary>
    public partial class SeqIterator : Iterator
    {
        [MirrorVerified("RngValDifferentialTests")]
        private Iterator[] m_its;

        [MirrorVerified("RngValDifferentialTests")]
        private int m_curr;

        [MirrorVerified("RngValDifferentialTests")]
        public SeqIterator(Iterator[] its)
        {
            m_curr = 0;
            m_its = its;
        }

        [MirrorVerified("RngValDifferentialTests")]
        public override object first() => m_its[m_curr].first();

        /// <summary>Moves to the next iterator when the current one ends (null, not an empty iterator, is
        /// expected for an empty part).</summary>
        [MirrorVerified("RngValDifferentialTests")]
        public override Iterator next()
        {
            if (m_its[m_curr].next() == null) m_curr++;
            if (m_curr >= m_its.Length) return null;
            return this;
        }
    }

    /// <summary>One value.</summary>
    public partial class ScalarIterator : Iterator
    {
        [MirrorVerified("RngValDifferentialTests")]
        private object m_init;

        [MirrorVerified("RngValDifferentialTests")]
        public ScalarIterator(object init) { m_init = init; }

        [MirrorVerified("RngValDifferentialTests")]
        public override object first() => m_init;

        [MirrorVerified("RngValDifferentialTests")]
        public override Iterator next() => null;
    }

    /// <summary>The elements of a matrix in column order.</summary>
    public partial class MatrixIterator : Iterator
    {
        [MirrorVerified("MatrixIteratorDifferentialTests")]
        private MatVal m_mat;

        [MirrorVerified("MatrixIteratorDifferentialTests")]
        private int m_row;

        [MirrorVerified("MatrixIteratorDifferentialTests")]
        private int m_col;

        [MirrorVerified("MatrixIteratorDifferentialTests")]
        public MatrixIterator(MatVal mat)
        {
            m_mat = mat;
            m_row = 0;
            m_col = 0;
        }

        [MirrorVerified("MatrixIteratorDifferentialTests")]
        public override object first() => m_mat.get_2(m_row, m_col);

        [MirrorVerified("MatrixIteratorDifferentialTests")]
        public override Iterator next()
        {
            if (++m_row >= m_mat.Data.m_rows)
            {
                if (++m_col >= m_mat.Data.m_cols) return null;
                m_row = 0;
            }
            return this;
        }
    }

    public abstract object first();

    public abstract Iterator next();

    /// <summary>The current value, then advances.</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public static object it_incr(object ignore, Iterator it)
    {
        object value = it.first();
        it.next();
        return value;
    }

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forRange(double init, double incr, int count) => forRange(RngVal.IncrRange(init, incr, count, Dim.identity));

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forRange(double init, double end) => forRange(RngVal.Range(init, end));

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forRange(double init, double next, double end) => forRange(RngVal.Range(init, next, end, Dim.identity));

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forRange(DimVal init, DimVal next, DimVal end) => forRange(RngVal.mkRange3(init, next, end));

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forRange(DimVal init, DimVal end) => forRange(RngVal.mkRange2(init, end));

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forRange(RngVal r) => new RangeIterator(r);

    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forValues(Iterator[] its) => new SeqIterator(its);

    /// <summary>A matrix iterates its elements (null when it is empty), a range its values, anything else itself.</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public static Iterator forValue(object o)
    {
        if (o is not MatVal mat)
        {
            if (o is not RngVal rng) return new ScalarIterator(o);
            return forRange(rng);
        }
        if (mat.Rows <= 0 || mat.Cols <= 0) return null;
        return new MatrixIterator(mat);
    }

    [MirrorVerified("RngValDifferentialTests")]
    protected Iterator() { }
}

/// <summary>The values of a range; the count is fixed when the iterator is made.</summary>
public partial class RangeIterator : Iterator
{
    [MirrorVerified("RngValDifferentialTests")]
    private RngVal m_rng;

    [MirrorVerified("RngValDifferentialTests")]
    private int m_it;

    [MirrorVerified("RngValDifferentialTests")]
    private int m_n;

    [MirrorVerified("RngValDifferentialTests")]
    public RangeIterator(RngVal r)
    {
        m_rng = r;
        m_it = 0;
        m_n = m_rng.Rsize;
    }

    [MirrorVerified("RngValDifferentialTests")]
    public override object first() => ri_first();

    [MirrorVerified("RngValDifferentialTests")]
    public object ri_first() => m_rng.range_get(m_it);

    [MirrorVerified("RngValDifferentialTests")]
    public override Iterator next() => ri_next();

    [MirrorVerified("RngValDifferentialTests")]
    public Iterator ri_next()
    {
        if (++m_it >= m_n) return null;
        return this;
    }
}

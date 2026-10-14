using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

public class MatValCollectionTests
{
    public MatValCollectionTests() => RuntimeThread.Ensure();
    [Fact]
    public void Reserve_tracks_capacity_and_raw_push_increments_before_failure()
    {
        var m = MatVal.reserve(2, 3);
        Assert.Equal(0, m.Rows);
        Assert.Equal(2, m.Anr);
        Assert.Equal(3, m.Cols);
        Assert.Equal(6, m.m_rarr.Length);
        Assert.Same(m, m.PushRaw(4));
        Assert.Equal(1, m.Rows);
        Assert.Equal(2, m.PushRawIndex(new NumVal(5)));
        Assert.Equal(new double[] { 4, 5, 0, 0, 0, 0 }, m.m_rarr);
        var empty = MatVal.reserve(0);
        Assert.Throws<IndexOutOfRangeException>(() => empty.PushRaw(1));
        Assert.Equal(1, empty.Rows);
    }
    [Fact]
    public void Shape_helpers_preserve_storage_and_control_null_dimension_promotion()
    {
        var m = MatVal.reserve(4);
        var storage = m.m_rarr;
        Assert.Null(m.m_dim);
        Assert.Same(m, m.SetShape1());
        Assert.Null(m.m_dim);
        Assert.Equal(4, m.Rows);
        Assert.Same(m, m.Reshape(2, 7));
        Assert.Same(Dim.identity, m.m_dim);
        Assert.Same(storage, m.m_rarr);
        Assert.Equal(2, m.Anr);
        Assert.Equal(7, m.Cols);
        Assert.Equal(4, m.m_rarr.Length);
    }
    [Fact]
    public void Replicate_numeric_value_ignores_units_and_pointer_value_keeps_identity()
    {
        var length = Dim.Parse("V{Length 60000}");
        var numeric = MatVal.Replicate(new NumVal(3, 4, length), 3);
        Assert.Same(Dim.identity, numeric.m_dim);
        Assert.Equal(new double[] { 3, 3, 3 }, numeric.m_rarr);
        Assert.Equal(new double[] { 4, 4, 4 }, numeric.m_iarr);
        var pointer = new object();
        var pointers = MatVal.Replicate(pointer, 2);
        Assert.Same(pointer, pointers.m_parr[0]);
        Assert.Same(pointer, pointers.m_parr[1]);
        Assert.True(MatVal.IsPtrNan(ref pointers.m_rarr[0]));
    }
    [Fact]
    public void Fill_writes_strides_clears_imaginary_and_retains_stale_pointer_entries()
    {
        var m = new MatVal(5, 1, true);
        var pointer = new object();
        m.Fill(pointer, 0, 2, 3);
        m.Fill(new NumVal(7, 2, Dim.identity), 4, -2, 3);
        Assert.Equal(new double[] { 7, 0, 7, 0, 7 }, m.m_rarr);
        Assert.Equal(new double[] { 2, 0, 2, 0, 2 }, m.m_iarr);
        Assert.Same(pointer, m.m_parr[2]);
        m.Fill(new NumVal(1), 0, 2, 3);
        Assert.Equal(new double[5], m.m_iarr);
    }
    [Fact]
    public void Mapping_uses_column_major_iteration_and_restores_shape()
    {
        new RuntimeOptions().SyncToThread();
        var source = new MatVal(new double[] { 1, 2, 3, 4 }, null, 2, 2);
        var seen = new List<double>();
        var mapped = MatVal.map((F<object, object>)(o => { var value = ((NumVal)o).Real; seen.Add(value); return new NumVal(value * 10); }), source);
        Assert.Equal(new double[] { 1, 2, 3, 4 }, seen);
        Assert.Equal(new double[] { 10, 20, 30, 40 }, mapped.m_rarr);
        Assert.Equal(2, mapped.Rows);
        Assert.Equal(2, mapped.Cols);
        var product = MatVal.map((F<object, object, object>)((r, c) => new NumVal(((NumVal)r).Real + ((NumVal)c).Real)), new MatVal(new double[] { 1, 2 }), new MatVal(new double[] { 10, 20, 30 }));
        Assert.Equal(new double[] { 11, 12, 21, 22, 31, 32 }, product.m_rarr);
        Assert.Equal(2, product.Rows);
        Assert.Equal(3, product.Cols);
    }
    [Fact]
    public void Range_construction_copies_flags_dimension_and_iteration_values()
    {
        var range = RngVal.mkRange3(new NumVal(2), new NumVal(4), new NumVal(8));
        var matrix = new MatVal(range);
        Assert.True(matrix.IsRange);
        Assert.Equal(range.Integral, matrix.IsIntegral);
        Assert.Same(range.m_dim, matrix.m_dim);
        Assert.Equal(new double[] { 2, 4, 6, 8 }, matrix.m_rarr);
        var objects = MatVal.as_object_vector(matrix);
        Assert.Equal(new double[] { 2, 4, 6, 8 }, objects.Cast<NumVal>().Select(n => n.Real));
        var mapped = MatVal.mapv((F<object, object>)(o => ((NumVal)o).Real), matrix);
        Assert.Equal(new object[] { 2.0, 4.0, 6.0, 8.0 }, mapped);
        Assert.Equal(new double[] { 2, 3, 4 }, Assert.IsType<MatVal>(MatVal.mkRange2(new NumVal(2), new NumVal(4))).m_rarr);
    }
    private sealed class AddFunction : Function
    {
        public AddFunction() : base(2, 0) { }
        public override void invVec<R>(out R result, object[] args) => result = (R)(object)new NumVal(((NumVal)args[0]).Real + ((NumVal)args[1]).Real);
        public override object Apply(object[] args) => new NumVal(((NumVal)args[0]).Real + ((NumVal)args[1]).Real);
    }
    [Fact]
    public void Vectorize_mutates_arguments_to_last_elements_and_rejects_shape_mismatch()
    {
        var first = new MatVal(new double[] { 1, 2, 3, 4 }, null, 2, 2);
        var second = new MatVal(new double[] { 10, 20, 30, 40 }, null, 2, 2);
        object[] args = [first, second];
        var result = Assert.IsType<MatVal>(MatVal.vectorize(new AddFunction(), args));
        Assert.Equal(new double[] { 11, 22, 33, 44 }, result.m_rarr);
        Assert.Equal(4, Assert.IsType<NumVal>(args[0]).Real);
        Assert.Equal(40, Assert.IsType<NumVal>(args[1]).Real);
        var error = Assert.Throws<AppException>(() => MatVal.vectorize(new AddFunction(), [first, new MatVal(1)]));
        Assert.Equal("unequal matrix dimensions", error.Message);
        Assert.Equal(3, Assert.IsType<NumVal>(MatVal.vectorize(new AddFunction(), [new NumVal(1), new NumVal(2)])).Real);
    }
    [Fact]
    public void Collection_constructor_preserves_polymorphism_and_raw_nan()
    {
        var m = new MatVal(new object[] { NumVal.Zero, NumVal.mcNaN });
        Assert.Null(m.m_dim);
        Assert.Equal(2, m.Rows);
        Assert.Equal(0, m.m_rarr[0]);
        Assert.True(double.IsNaN(m.m_rarr[1]));
        var promoted = MatVal.MakeVector0(new object[] { NumVal.Zero });
        Assert.Same(Dim.identity, promoted.m_dim);
        Assert.Throws<AppException>(() => MatVal.reserve(2).PushVectorArg(new object()));
    }
}

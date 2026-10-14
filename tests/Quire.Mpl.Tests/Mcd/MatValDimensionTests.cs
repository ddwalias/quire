using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Mpl.Tests.Mcd;

public class MatValDimensionTests
{
    public MatValDimensionTests() => RuntimeThread.Ensure();
    private static Dim Length => Dim.Parse("V{Length 60000}");
    private static Dim Time => Dim.Parse("V{Time 60000}");

    [Fact]
    public void Cast_copies_pack_storage_and_replace_numeric_pointer_units()
    {
        var source = new MatVal(new double[] { 99, 1, 2, 99, 3, 4, 99 }, null!, 2, 2, 3, null!, Flags.MatrixTag, 1);
        source.m_dim = Length;
        source.set_2(1, 1, new NumVal(5, 6, Time));
        var stripped = MatVal.StripDimension(source);
        Assert.NotSame(source.m_rarr, stripped.m_rarr);
        Assert.Equal(0, stripped.Offset);
        Assert.Equal(2, stripped.Anr);
        Assert.Equal(Flags.ComplexSnan, stripped.Data.m_flags);
        Assert.Null(stripped.m_parr);
        Assert.Same(Dim.identity, stripped.m_dim);
        Assert.Equal(5, stripped.get_real(1, 1));
        Assert.Equal(6, stripped.get_imag(1, 1));
        Assert.Same(Time, source.get_2_dim(1, 1));
        Assert.Same(Length, source.m_dim);
        Assert.Same(Time, source.SetDim(Time).m_dim);
        Assert.Same(Length, source.m_dim);
    }

    [Fact]
    public void Uniform_and_polymorphic_conversions_preserve_alias_only_when_allowed()
    {
        var uniform = new MatVal(new[] { 2.0 });
        Assert.Same(uniform, MatVal.AsUniform(uniform));
        Assert.Same(uniform, MatVal.AsDimless(uniform));
        Assert.NotSame(uniform, MatVal.AsPolymorphic(uniform));
        var poly = new MatVal(1);
        Assert.Same(poly, MatVal.AsPolymorphic(poly));
        var mixed = new MatVal(new[] { 1.0, 2.0 }) { m_dim = Length };
        mixed.set_1(1, new NumVal(3, 0, Time));
        var converted = MatVal.AsUniform(mixed);
        Assert.NotSame(mixed, converted);
        Assert.Same(Length, converted.m_dim);
        Assert.Null(converted.m_parr);
        Assert.Equal(3, converted.get_real(1));
        Assert.Same(Time, mixed.get_1_dim(1));
    }

    [Fact]
    public void Uniform_application_changes_default_and_retains_pointer_dimensions()
    {
        var matrix = new MatVal(new[] { 1.0, 2.0 });
        matrix.set_1(1, new NumVal(4, 0, Time));
        var table = matrix.m_parr;
        Assert.Same(matrix, matrix.ApplyDimension(MatDim.make(Length)));
        Assert.Same(table, matrix.m_parr);
        Assert.Same(Length, matrix.m_dim);
        Assert.Same(Time, matrix.get_1_dim(1));
        Assert.Same(matrix, matrix.ApplyRowDimensions(Length, Length));
        Assert.Same(matrix, matrix.ApplyColDimensions(Length, Length));
    }

    [Fact]
    public void Row_and_column_application_preserve_complex_parts_and_can_replace_live_pointer_with_nan()
    {
        var rows = new MatVal(new[] { 1.0, 2.0, 3.0, 4.0 }, new[] { 5.0, 6.0, 7.0, 8.0 }, 2, 2);
        Assert.Same(rows, rows.ApplyRowDimensions(Dim.identity, Time));
        Assert.Same(Dim.identity, rows.m_dim);
        Assert.Same(Time, rows.get_2_dim(1, 1));
        var number = Assert.IsType<NumVal>(rows.get_2(1, 1));
        Assert.Equal(4, number.Real);
        Assert.Equal(8, number.Imag);
        rows.ApplyColDimensions(Length, Dim.identity);
        Assert.Same(Length, rows.get_2_dim(1, 0));
        Assert.True(double.IsNaN(Assert.IsType<NumVal>(rows.get_2(1, 0)).Real));
        Assert.Same(Time, rows.get_2_dim(1, 1));
        Assert.Throws<NullReferenceException>(() => rows.ApplyLinearDimension(null!));
    }

    [Fact]
    public void Division_returns_divisor_and_changes_nested_pointer_to_dimension()
    {
        new RuntimeOptions().SyncToThread();
        var nested = new MatVal(new[] { 2.0 }) { m_dim = Length };
        var matrix = new MatVal(new[] { 3.0, 4.0, 5.0 }) { m_dim = Length };
        matrix.set_1(1, new NumVal(4, 2, Time));
        matrix.set_ptrs_1(2, nested);
        Assert.Same(Length, matrix.DivideByDefaultDimension());
        Assert.Same(Dim.identity, matrix.m_dim);
        Assert.Same(Dim.Div(Time, Length), matrix.get_1_dim(1));
        Assert.Same(Length, matrix.get_1(2));
        Assert.Same(Length, nested.m_dim);
        var table = matrix.m_parr;
        Assert.Same(Dim.identity, matrix.DivideByDimension(Dim.identity));
        Assert.Same(table, matrix.m_parr);
    }

    [Fact]
    public void Matrix_dimension_cast_and_create_apply_order_derived_units()
    {
        new RuntimeOptions().SyncToThread();
        object other = new object();
        Assert.Same(other, MatVal.MDimCast(other, MatDim.make(Time)));
        var matrix = new MatVal(new[] { 1.0, 2.0, 3.0 }) { m_dim = Length };
        Assert.Same(matrix, MatVal.MDimCast(matrix, null!));
        var cast = Assert.IsType<MatVal>(MatVal.MDimCast(matrix, MatDim.make(Time)));
        Assert.Same(Time, cast.m_dim);
        Assert.Same(Length, matrix.m_dim);
        var orders = new MatVal(new[] { 0.0, 0.0, 2.0 });
        var created = Assert.IsType<MatVal>(MatVal.MatCreateDim(orders, new NumVal(2, 0, Time), matrix));
        Assert.Same(Length, created.get_1_dim(0));
        Assert.Same(Dim.Div(Length, Time), created.get_1_dim(1));
        Assert.Same(Length, created.get_1_dim(2));
        Assert.Same(matrix, MatVal.MatCreateDim(orders, new NumVal(2), matrix));
        Assert.Same(matrix, MatVal.MatCreateDim(other, new NumVal(2, 0, Time), matrix));
    }
}

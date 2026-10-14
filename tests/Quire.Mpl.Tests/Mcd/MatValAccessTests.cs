using System.Collections;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

public class MatValAccessTests
{
    public MatValAccessTests() => RuntimeThread.Ensure();
    private static Dim Length => Dim.Parse("V{Length 60000}");
    private static Dim Time => Dim.Parse("V{Time 60000}");
    private static MatVal View() => new(new double[8], new double[8], 2, 2, 3, new Hashtable(), Flags.MatrixTag, 1);

    [Fact]
    public void Access_uses_allocated_stride_and_raw_one_dimensional_offsets()
    {
        var m = View();
        m.set_2(0, 1, 7.0);
        Assert.Equal(7, m.m_rarr[4]);
        Assert.Equal(7, ((NumVal)m.get_1(3)).Real);
        Assert.Equal(0, ((NumVal)m.get_1(2)).Real);
        Assert.Equal(0, ((NumVal)m.get_1(-1)).Real);
        Assert.Throws<IndexOutOfRangeException>(() => m.get_1(-2));
        m.set_2i(0, 1, 9);
        m.set_2(0, 1, 11.0);
        Assert.Equal(9, m.get_imag(0, 1));
        Assert.Equal(11, m.get_real(0, 1));
    }
    [Fact]
    public void Numeric_setters_adopt_dimensions_and_retain_incompatible_pointers()
    {
        new RuntimeOptions().SyncToThread();
        var m = new MatVal(2);
        m.set_1(0, new NumVal(1, 2, Length));
        Assert.Same(Length, m.m_dim);
        var pointer = new NumVal(3, 4, Time);
        m.set_1(1, pointer);
        Assert.Same(pointer, m.get_1(1));
        Assert.True(MatVal.IsPtrNan(ref m.m_rarr[1]));
        Assert.Same(Time, m.get_1_dim(1));
        Assert.Equal(3, m.get_real_mixed(1));
        Assert.False(m.IsUniform);
        m.set_1(1, 7.0);
        Assert.Equal(7, ((NumVal)m.get_1(1)).Real);
        Assert.Same(pointer, m.m_parr[1]);
        Assert.False(m.RepHasPtrs);
        Assert.Null(m.m_parr);
    }
    [Fact]
    public void Orphan_pointer_get_repairs_slot_and_non_number_mixed_access_reports_handle()
    {
        var m = new MatVal(new[] { MatVal.PtrNan });
        Assert.True(double.IsNaN(((NumVal)m.get_1(0)).Real));
        Assert.False(MatVal.IsPtrNan(ref m.m_rarr[0]));
        m.set_1(0, "hello");
        Assert.IsType<StrVal>(m.get_1(0));
        var error = Assert.Throws<AppException>(() => m.get_real_mixed_handle(0, 19));
        Assert.Equal("non number value found in real argument", error.Message);
        Assert.Equal(19u, error.Handle);
    }
    [Fact]
    public void Canonicalization_packs_shared_view_without_overwriting_original_arrays()
    {
        new RuntimeOptions().SyncToThread();
        var m = View();
        for (int c = 0; c < 2; c++) for (int r = 0; r < 2; r++) m.set_ptrs_2(r, c, new NumVal(r + c, 0, Length));
        var real = m.m_rarr;
        m.Shared = true;
        Assert.True(m.IsUniformScalar);
        Assert.Same(Length, m.m_dim);
        Assert.Null(m.m_parr);
        Assert.NotSame(real, m.m_rarr);
        Assert.Equal(0, m.Offset);
        Assert.Equal(2, m.Anr);
        Assert.False(m.Shared);
        Assert.Equal(new double[] { 0, 1, 1, 2 }, m.m_rarr);
        Assert.True(MatVal.IsPtrNan(ref real[1]));
    }
    [Fact]
    public void Nested_pointer_assignment_copies_self_and_marks_child_immutable()
    {
        var m = new MatVal(2);
        m.set_1(0, "hello");
        var text = m.get_1(0);
        m.set_1(1, m);
        var child = Assert.IsType<MatVal>(m.get_1(1));
        Assert.NotSame(m, child);
        Assert.True(child.Immutable);
        Assert.Same(text, child.get_1(0));
        Assert.True(m.contains(text));
        Assert.False(m.contains(m));
        Assert.False(m.IsScalarMatrix);
    }
    [Fact]
    public void Copies_and_packing_preserve_dimensions_and_pointer_objects()
    {
        new RuntimeOptions().SyncToThread();
        var m = View();
        m.m_dim = Length;
        m.set_2(0, 0, 2.0);
        m.set_2(1, 1, "text");
        var copy = new MatVal(m);
        Assert.Equal(2, copy.Anr);
        Assert.Equal(0, copy.Offset);
        Assert.Same(Length, copy.m_dim);
        Assert.NotSame(m.m_rarr, copy.m_rarr);
        Assert.NotSame(m.m_parr, copy.m_parr);
        Assert.Same(m.get_2(1, 1), copy.get_2(1, 1));
        Assert.Null(copy.m_iarr);
        Assert.Equal(Flags.MatrixTag, copy.Data.m_flags);
        Assert.Equal(new double[] { 2, 0 }, m.PackReal(0, 2, 1, 2));
    }
    [Fact]
    public void Reusable_numeric_getter_mutates_scratch_only_for_numeric_slots()
    {
        var m = new MatVal(new[] { 2.0, 3.0 });
        var scratch = new NumVal(7, 8);
        Assert.Same(scratch, m.get_1_na(scratch, 1));
        Assert.Equal(3, scratch.Real);
        Assert.Equal(0, scratch.Imag);
        m.set_1(0, "text");
        Assert.IsType<StrVal>(m.get_1_na(scratch, 0));
        Assert.Equal(3, scratch.Real);
    }
    [Fact]
    public void Vector_queries_use_column_shape_and_conversions_keep_error_key()
    {
        var row = new MatVal(1, 2);
        Assert.False(row.IsVector);
        Assert.Throws<AppException>(() => row.asVector("custom error", 7));
        var vector = new MatVal(2);
        vector.IsRange = true;
        vector.IsIntegral = true;
        Assert.True(vector.IsRange);
        Assert.True(vector.IsIntegral);
        vector.set_2i(0, 0, 1);
        Assert.False(vector.IsRange);
        Assert.False(vector.IsIntegral);
    }
}

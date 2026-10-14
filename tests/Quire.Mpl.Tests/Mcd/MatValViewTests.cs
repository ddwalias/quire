using System.Reflection;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

public class MatValViewTests
{
    public MatValViewTests() => RuntimeThread.Ensure();
    [Fact]
    public void Mutable_columns_copy_and_immutable_uniform_columns_alias_with_shared_flags()
    {
        var matrix = new MatVal(new[] { 1.0, 2.0, 3.0, 4.0 }, null!, 2, 2);
        var copy = matrix.Column(1);
        Assert.NotSame(matrix.m_rarr, copy.m_rarr);
        Assert.Equal(new[] { 3.0, 4.0 }, copy.m_rarr);
        Assert.False(matrix.Shared);
        matrix.Fix();
        var view = matrix.Column(1);
        Assert.Same(matrix.m_rarr, view.m_rarr);
        Assert.True(matrix.Shared);
        Assert.True(view.Shared);
        Assert.Equal(Flags.StringTag | Flags.Shared, view.Data.m_flags);
        Assert.Equal(2, view.Offset);
        Assert.Same(matrix, matrix.asImmutable());
        Assert.Same(matrix, MatVal.Fix(matrix));
        Assert.Null(MatVal.Fix("other"));
    }

    [Fact]
    public void Submatrix_bounds_are_half_open_and_pointer_tables_are_trimmed_to_physical_keys()
    {
        var matrix = new MatVal(new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 }, null!, 2, 3);
        matrix.set_ptrs_2(1, 1, "chosen");
        matrix.set_ptrs_2(0, 0, "outside");
        var view = matrix.SubMatrix(1, 2, 1, 3);
        Assert.Equal(1, view.Rows);
        Assert.Equal(2, view.Cols);
        Assert.Equal(3, view.Offset);
        Assert.Equal(2, view.Anr);
        Assert.Same(matrix.m_rarr, view.m_rarr);
        Assert.True(matrix.Immutable);
        Assert.False(matrix.Shared);
        Assert.True(view.Shared);
        Assert.Single(view.m_parr);
        Assert.True(view.m_parr.ContainsKey(3));
        Assert.Equal("chosen", Assert.IsType<StrVal>(view.get_2(0, 0)).Field1);
        Assert.Equal(6, Assert.IsType<NumVal>(view.get_2(0, 1)).Real);
        Assert.Throws<AppException>(() => matrix.SubMatrix(0, 0, 0, 1));
        Assert.Throws<AppException>(() => matrix.SubMatrix(0, 3, 0, 1));
        var row = matrix.Row(1);
        Assert.Equal(1, row.Rows);
        Assert.Equal(3, row.Cols);
    }

    [Fact]
    public void Transpose_honors_offset_stride_and_retains_zero_imaginary_storage()
    {
        var matrix = new MatVal(new[] { 99.0, 1.0, 2.0, 99.0, 3.0, 4.0, 99.0 }, new double[7], 2, 2, 3, null!, Flags.MatrixTag, 1);
        matrix.set_ptrs_2(1, 0, "value");
        var transpose = matrix.Transpose();
        Assert.Equal(2, transpose.Rows);
        Assert.Equal(0, transpose.Offset);
        Assert.NotNull(transpose.m_iarr);
        Assert.Equal(4, transpose.m_iarr.Length);
        Assert.Equal(3, transpose.get_real(1, 0));
        Assert.Equal("value", Assert.IsType<StrVal>(transpose.get_2(0, 1)).Field1);
        Assert.Null(new MatVal(new[] { 1.0 }).TransposeImag());
        Assert.Equal(new[] { 0.0, 0.0, 0.0, 0.0 }, matrix.TransposeImag());
    }

    [Fact]
    public void Reverse_pointer_callback_preserves_original_rows_minus_row_index()
    {
        var source = new MatVal(new[] { 1.0, 2.0 });
        source.set_ptrs_1(0, "first");
        var padded = new MatVal(2, 1, false, 3);
        padded.ReversePtrs(source);
        Assert.True(padded.m_parr.ContainsKey(2));
        Assert.Equal("first", Assert.IsType<StrVal>(padded.m_parr[2]).Field1);
        Assert.Throws<IndexOutOfRangeException>(() => new MatVal(2).ReversePtrs(source));
        var trim = typeof(MatVal).GetMethod("TrimPtrs", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var view = new MatVal(source.m_rarr, null!, 1, 1, 2);
        trim.Invoke(view, new object[] { source.m_parr });
        Assert.Single(view.m_parr);
    }

    [Fact]
    public void Tuples_equality_and_text_use_logical_column_major_elements()
    {
        new RuntimeOptions().SyncToThread();
        var matrix = new MatVal(new[] { 1.0, 2.0, 3.0, 4.0 }, null!, 2, 2);
        Assert.Equal(new[] { 1.0, 2.0, 3.0, 4.0 }, matrix.get_elements().Cast<NumVal>().Select(n => n.Real));
        Assert.Equal(new[] { 1.0, 3.0 }, matrix.asTuple(1, 2, 7).Cast<NumVal>().Select(n => n.Real));
        Assert.Empty(matrix.asTuple(0, 2, 7));
        Assert.Equal(7u, Assert.Throws<AppException>(() => matrix.asTuple(3, 1, 7)).Argument);
        Assert.True(matrix.Equals(new MatVal(matrix)));
        Assert.False(matrix.Equals(null));
        Assert.False(matrix.Equals(new MatVal(1)));
        var nearby = new MatVal(new[] { 1.0001, 2.0, 3.0, 4.0 }, null!, 2, 2);
        Assert.True(matrix.nearly_equal(nearby, 0.001));
        Assert.False(matrix.nearly_equal(nearby, 0.000001));
        Assert.False(matrix.nearly_equal(new NumVal(1), 1));
        Assert.Equal($"MatVal[2,2]\n{{ {matrix.get_2(0, 0)}, {matrix.get_2(0, 1)} }}\n{{ {matrix.get_2(1, 0)}, {matrix.get_2(1, 1)} }}", matrix.ToString());
        Assert.Equal("MatVal[12,0]", new MatVal(12, 0).ToString());
        Assert.Equal(11, new MatVal(12, 4).ToString().Split('\n').Length);
        Assert.Contains(", ... }", new MatVal(1, 4).ToString());
    }

    [Fact]
    public void Result_packs_views_but_export_only_fixes_input_in_place()
    {
        var matrix = new MatVal(2, 2, false, 3);
        var exported = MatVal.FixForExport(matrix);
        Assert.Same(matrix, exported);
        Assert.False(matrix.Packed);
        Assert.True(matrix.Immutable);
        Assert.Same(Dim.identity, matrix.m_dim);
        var result = Assert.IsType<MatVal>(matrix.asResult());
        Assert.NotSame(matrix, result);
        Assert.True(result.Packed);
        Assert.True(result.Immutable);
        Assert.Same(result, result.asResult());
        Assert.IsType<StrVal>(MatVal.FixForExport("text"));
        object other = new object();
        Assert.Same(other, MatVal.FixForExport(other));
        var scalar = new NumVal(2);
        Assert.IsType<NumVal>(MatVal.FixForExport(scalar));
        Assert.Same(Dim.identity, scalar.m_dim);
    }

    [Fact]
    public void Row_and_column_wrappers_keep_original_error_positions_and_real_column_reads_mixed_numbers()
    {
        var matrix = new MatVal(new[] { 1.0, 2.0 });
        matrix.set_1(1, new NumVal(3, 0, Dim.Parse("V{Time 60000}")));
        Assert.Equal(new[] { 1.0, 3.0 }, matrix.real_column(0));
        Assert.IsType<MatVal>(MatVal.mcd_column(matrix, 0));
        Assert.IsType<MatVal>(MatVal.mcd_sym_column(matrix, 0));
        Assert.IsType<MatVal>(MatVal.mcd_row(matrix, 1));
        Assert.IsType<MatVal>(MatVal.mcd_sym_row(matrix, 1));
        Assert.Equal(1u, Assert.Throws<AppException>(() => MatVal.mcd_column("text", 0)).Argument);
        Assert.Equal(uint.MaxValue, Assert.Throws<AppException>(() => MatVal.mcd_sym_column(matrix, -1)).Argument);
        Assert.Equal(2u, Assert.Throws<AppException>(() => MatVal.mcd_row(matrix, 2)).Argument);
        Assert.Equal(uint.MaxValue, Assert.Throws<AppException>(() => MatVal.mcd_sym_row("text", 0)).Argument);
    }
}

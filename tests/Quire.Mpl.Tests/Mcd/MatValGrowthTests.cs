using System.Collections;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

public class MatValGrowthTests
{
    public MatValGrowthTests() => RuntimeThread.Ensure();
    [Fact]
    public void Vector_growth_doubles_capacity_and_set_preserves_copy_on_write()
    {
        var matrix = new MatVal(new double[] { 1, 2 });
        Assert.Same(matrix, matrix.mcd_set_1(2, new NumVal(3)));
        Assert.Equal(3, matrix.Rows);
        Assert.Equal(4, matrix.Anr);
        Assert.Equal(new double[] { 1, 2, 3, 0 }, matrix.m_rarr);
        matrix.Immutable = true;
        matrix.Shared = true;
        var copy = matrix.mcd_set_1(4, new NumVal(5));
        Assert.NotSame(matrix, copy);
        Assert.Equal(3, matrix.Rows);
        Assert.Equal(5, copy.Rows);
        Assert.False(copy.Immutable);
        Assert.False(copy.Shared);
        Assert.Equal(new double[] { 1, 2, 3, 0, 5 }, copy.PackReal());
        Assert.Same(matrix, matrix.mcd_upd_1(0, 9.0));
        Assert.Equal(9, matrix.m_rarr[0]);
        Assert.True(matrix.Immutable);
    }

    [Fact]
    public void Matrix_growth_remaps_live_pointers_and_treats_column_and_row_capacity_differently()
    {
        var matrix = new MatVal(2, 2);
        matrix.set_ptrs_2(1, 1, "pointer");
        matrix.mcd_set_2(2, 1, new NumVal(7));
        Assert.Equal(4, matrix.Anr);
        Assert.Equal(3, matrix.Rows);
        Assert.IsType<StrVal>(matrix.get_2(1, 1));
        Assert.True(matrix.m_parr.ContainsKey(5));
        Assert.False(matrix.m_parr.ContainsKey(3));
        matrix.mcd_set_2(3, 2, new NumVal(8));
        Assert.Equal(4, matrix.Rows);
        Assert.Equal(4, matrix.Anr);
        Assert.Equal(3, matrix.Cols);
        Assert.IsType<StrVal>(matrix.get_2(1, 1));
        Assert.Equal(8, ((NumVal)matrix.get_2(3, 2)).Real);
        Assert.Equal(7, ((NumVal)matrix.get_2(2, 1)).Real);
    }

    [Fact]
    public void Reallocation_copies_views_and_never_shrinks_logical_shape()
    {
        var matrix = new MatVal(new double[] { -1, 1, 2, -2, 3, 4, -3 }, null, 2, 2, 3, null, Flags.MatrixTag | Flags.CopyOnWrite, 1);
        var copy = matrix.ReAllocate(1, 1);
        Assert.Equal(2, copy.Rows);
        Assert.Equal(2, copy.Cols);
        Assert.Equal(2, copy.Anr);
        Assert.Equal(0, copy.Offset);
        Assert.Equal(new double[] { 1, 2, 3, 4 }, copy.m_rarr);
        Assert.NotSame(matrix.m_rarr, copy.m_rarr);
        Assert.False(copy.Immutable);
        var capacity = MatVal.realloc(copy, 4, 3);
        Assert.Equal(2, capacity.Rows);
        Assert.Equal(2, capacity.Cols);
        Assert.Equal(5, capacity.Anr);
        Assert.Equal(20, capacity.m_rarr.Length);
        var empty = MatVal.realloc(null!, -2, -3);
        Assert.Equal(0, empty.Rows);
        Assert.Equal(1, empty.Cols);
        Assert.Equal(1, empty.Anr);
        Assert.Equal(3, MatVal.ReAlloc(null!, 3).Rows);
        Assert.Equal(2, MatVal.ReAlloc(null!, 3, 2).Cols);
    }

    [Fact]
    public void Pointer_matrices_fill_new_unit_cells_with_identity_zero_and_only_fill_old_rows()
    {
        var length = Dim.Parse("V{Length 60000}");
        var matrix = new MatVal(1, 1) { m_dim = length };
        matrix.set_ptrs_2(0, 0, "pointer");
        matrix.extend_2(1, 1);
        Assert.IsType<StrVal>(matrix.get_2(0, 0));
        Assert.Same(NumVal.ZeroId, matrix.m_parr[matrix.Index(1, 0)]);
        Assert.Same(NumVal.ZeroId, matrix.m_parr[matrix.Index(0, 1)]);
        var fill = new MatVal(new double[] { 2, 3 });
        fill.StretchWithColumnFill(4, 3, new NumVal(9));
        Assert.Equal(new double[] { 2, 3, 0, 0, 9, 9, 0, 0, 9, 9, 0, 0 }, fill.PackReal());
    }

    [Fact]
    public void Block_and_column_assignment_copy_imaginary_storage_and_pointer_values()
    {
        var source = new MatVal(new double[] { 1, 2, 3, 4 }, new double[] { 5, 6, 7, 8 }, 2, 2);
        source.set_ptrs_2(1, 1, "nested");
        var destination = new MatVal(1, 1);
        destination.setblock(1, 1, source);
        Assert.Equal(3, destination.Rows);
        Assert.Equal(3, destination.Cols);
        Assert.Equal(1, ((NumVal)destination.get_2(1, 1)).Real);
        Assert.Equal(5, ((NumVal)destination.get_2(1, 1)).Imag);
        Assert.Same(source.get_2(1, 1), destination.get_2(2, 2));
        destination.setcolumn(1, new MatVal(new double[] { 10, 11 }));
        Assert.Equal(0, destination.m_iarr[destination.Index(1, 1)]);
        Assert.Equal(11, ((NumVal)destination.get_2(1, 1)).Real);
        var created = MatVal.mcd_setrow(null!, 1, source);
        Assert.Equal(3, created.Rows);
        Assert.Equal(2, created.Cols);
        Assert.Same(source.get_2(1, 1), created.get_2(2, 1));
    }

    [Fact]
    public void Updates_ignore_immutability_and_dimension_mismatch_assigns_element_objects()
    {
        var length = Dim.Parse("V{Length 60000}");
        var destination = new MatVal(1, 1) { m_dim = length, Immutable = true };
        destination.mcd_upd_2(1, 1, new NumVal(2, 3, length));
        Assert.Equal(2, destination.Rows);
        Assert.Equal(2, destination.Cols);
        Assert.True(destination.Immutable);
        destination.mcd_upd_2(1, 1, 4.0);
        Assert.Same(Dim.identity, ((NumVal)destination.get_2(1, 1)).m_dim);
        destination.setblock(0, 0, new MatVal(new double[] { 7, 8 }));
        Assert.Equal(7, ((NumVal)destination.get_2(0, 0)).Real);
        Assert.Same(Dim.identity, ((NumVal)destination.get_2(0, 0)).m_dim);
        destination.setblock(0, 1, new NumVal(9));
        Assert.Equal(9, ((NumVal)destination.get_2(0, 1)).Real);
        var rowCopy = MatVal.mcd_setrow(destination, 2, new NumVal(6));
        Assert.NotSame(destination, rowCopy);
        Assert.Equal(3, rowCopy.Rows);
        Assert.Equal(2, destination.Rows);
        var vector = new MatVal(1) { Immutable = true };
        Assert.NotSame(vector, vector.extend_1(2));
        Assert.NotSame(destination, destination.extend_2(3, 4));
        vector.mcd_upd_1(1, new NumVal(3));
        Assert.Equal(3, ((NumVal)vector.get_1(1)).Real);
    }

    [Fact]
    public void Column_wrappers_copy_immutable_destination_and_validate_vector_inputs()
    {
        var destination = new MatVal(1, 1) { Immutable = true };
        var result = MatVal.mcd_setcolumn(destination, 2, new NumVal(6));
        Assert.NotSame(destination, result);
        Assert.Equal(1, destination.Cols);
        Assert.Equal(3, result.Cols);
        Assert.Equal(6, ((NumVal)result.get_2(0, 2)).Real);
        var created = MatVal.mcd_setcolumn(null!, 1, new MatVal(new double[] { 3, 4 }));
        Assert.Equal(new double[] { 0, 0, 3, 4 }, created.PackReal());
        Assert.Throws<AppException>(() => new MatVal(2, 2).extend_1(0));
        Assert.Throws<AppException>(() => destination.mcd_set_1(-1, new NumVal(0)));
        Assert.Throws<AppException>(() => destination.mcd_set_2(0, -1, new NumVal(0)));
        Assert.Throws<AppException>(() => destination.setcolumn(0, new MatVal(1, 2)));
        Assert.Throws<AppException>(() => MatVal.mcd_setrow(destination, -1, new NumVal(0)));
    }
}

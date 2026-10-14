using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Engine.Tests;

public class MatrixAndRangeTests : EngineTestBase
{
    [Fact]
    public void Matrix_literals_are_row_major()
    {
        var m = Assert.IsType<MatVal>(Last("(eval (matrix 2 2 1 2 3 4))").Value);
        Assert.Equal(2, m.At(0, 1).Real);
        Assert.Equal(3, m.At(1, 0).Real);
    }

    [Fact]
    public void Matrix_product_and_inverse()
    {
        var inv = Assert.IsType<MatVal>(Last("(define M (matrix 2 2 4 7 2 6))", "(eval (op_power M -1))").Value);
        Assert.Equal(0.6, inv.At(0, 0).Real, 12);
        Assert.Equal(-0.7, inv.At(0, 1).Real, 12);
        var id = Assert.IsType<MatVal>(Last("(eval (op_mult M (op_power M -1)))").Value);
        Assert.Equal(1, id.At(1, 1).Real, 12);
        Assert.Equal(0, id.At(0, 1).Real, 12);
    }

    [Fact]
    public void Vector_times_vector_is_dot_product() =>
        AssertNum(Last("(eval (op_mult (matrix 3 1 1 2 3) (matrix 3 1 4 5 6)))"), 32);

    [Fact]
    public void Determinant_and_lsolve()
    {
        AssertNum(Last("(eval (op_abs (matrix 2 2 1 2 3 4)))"), -2);
        var x = Assert.IsType<MatVal>(Last("(eval (op_apply lsolve (matrix 2 2 2 1 1 3) (matrix 2 1 3 5)))").Value);
        Assert.Equal(0.8, x.At(0).Real, 12);
        Assert.Equal(1.4, x.At(1).Real, 12);
    }

    [Fact]
    public void Indexing_respects_ORIGIN()
    {
        AssertNum(Last("(define v (matrix 3 1 10 20 30))", "(eval (op_index v 0))"), 10);
        AssertNum(Last("(define ORIGIN 1)", "(eval (op_index v 1))"), 10);
    }

    [Fact]
    public void Index_out_of_range_is_an_error() =>
        AssertError(Last("(define v (matrix 2 1 1 2))", "(eval (op_index v 5))"), "out of range");

    [Fact]
    public void Range_evaluation_produces_a_table()
    {
        var r = Last("(define x (op_range2 1 4))", "(eval (op_power x 2))");
        Assert.True(r.IsRangeTable);
        var m = Assert.IsType<MatVal>(r.Value);
        Assert.Equal([1.0, 4, 9, 16], m.Elements().Select(e => e.Real));
    }

    [Fact]
    public void Stepped_range_with_units()
    {
        var r = Last("(define t (op_range3 (op_mult 0 s) (op_mult 0.5 s) (op_mult 2 s)))", "(eval t)");
        var m = Assert.IsType<MatVal>(r.Value);
        Assert.Equal(5, m.Length());
        Assert.Equal(Dim.Parse(BuiltinUnits.Time), m.At(4).m_dim);
    }

    [Fact]
    public void Range_with_units_needs_a_step() =>
        AssertError(Last("(define t (op_range2 (op_mult 0 s) (op_mult 2 s)))"), "needs a step");

    [Fact]
    public void Indexed_definition_over_a_range_builds_a_vector()
    {
        var m = Assert.IsType<MatVal>(Last("(define i (op_range2 0 4))", "(define (op_index v i) (op_power i 2))", "(eval v)").Value);
        Assert.Equal([0.0, 1, 4, 9, 16], m.Elements().Select(e => e.Real));
    }

    [Fact]
    public void Two_range_indices_build_a_matrix()
    {
        var m = Assert.IsType<MatVal>(Last(
            "(define i (op_range2 0 1))", "(define j (op_range2 0 2))",
            "(define (op_index M i j) (op_add (op_mult 10 i) j))", "(eval M)").Value);
        Assert.Equal(2, m.Rows);
        Assert.Equal(3, m.Cols);
        Assert.Equal(12, m.At(1, 2).Real);
    }

    [Fact]
    public void Aggregates_accept_range_variables_without_iterating()
    {
        AssertNum(Last("(define x (op_range2 1 10))", "(eval (op_apply mean x))"), 5.5);
        AssertNum(Last("(eval (op_apply sum x))"), 55);
    }

    [Fact]
    public void Statistics()
    {
        AssertNum(Last("(define d (matrix 4 1 2 4 4 6))", "(eval (op_apply stdev d))"), Math.Sqrt(2));
        AssertNum(Last("(eval (op_apply Stdev d))"), Math.Sqrt(8.0 / 3));
        AssertNum(Last("(eval (op_apply median d))"), 4);
    }

    [Fact]
    public void Interpolation()
    {
        AssertNum(Last("(define vx (matrix 3 1 0 1 2))", "(define vy (matrix 3 1 0 10 40))", "(eval (op_apply linterp vx vy 1.5))"), 25);
        AssertNum(Last("(eval (op_apply spline vx vy 1))"), 10);
    }
}

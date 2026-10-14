using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Mpl.Tests.Mcd;

public class MatDimTests
{
    public MatDimTests() => RuntimeThread.Ensure();
    private sealed class Grid : MatDim
    {
        private readonly Dim[,] values;
        public Grid(Dim[,] values) : base(values.GetLength(0), values.GetLength(1)) => this.values = values;
        public override Dim dim(int i, int j) => values[i, j];
        public override MatDim matdim(int i, int j) => this;
        protected override bool eq_b(MatDim m2) => ReferenceEquals(this, m2);
    }
    private static Dim Length => Dim.Parse("V{Length 60000}");
    private static Dim Time => Dim.Parse("V{Time 60000}");

    [Fact]
    public void Shape_and_base_queries_extract_each_coordinate()
    {
        var g = new Grid(new[,] { { Length, Time }, { Time, Dim.identity } });
        Assert.Equal(2, g.Rows);
        Assert.Equal(2, g.Cols);
        Assert.Null(g.UniformDim);
        Assert.Null(g.RowDimensions);
        Assert.Null(g.ColDimensions);
        Assert.Null(g.IsLinear);
        Assert.Equal(new[] { Length, Time }, g.RowDimensionArray(0));
        Assert.Equal(new[] { Time, Dim.identity }, g.ColDimensionArray(1));
        Assert.Throws<IndexOutOfRangeException>(() => g.RowDimensionArray(2));
        Assert.Empty(new Grid(new Dim[1, 0]).RowDimensionArray(0));
        Assert.Null(MatDim.CMult(null!, null!));
    }
    [Fact]
    public void Uniformity_and_equality_distinguish_reference_and_dimension_rules()
    {
        Assert.Same(Length, MatDim.Uniform([Length, Length]));
        Assert.Null(MatDim.Uniform([Length, Time]));
        Assert.Null(MatDim.Uniform(null!));
        Assert.Null(MatDim.Uniform([null!, null!]));
        Assert.Throws<IndexOutOfRangeException>(() => MatDim.Uniform([]));
        Assert.True(MatDim.Eq_b(Array.Empty<Dim>(), Array.Empty<Dim>()));
        Assert.True(MatDim.Eq_b(new[] { Length }, new[] { Length }));
        Assert.False(MatDim.Eq_b(new[] { Length }, new[] { Time }));
        Assert.False(MatDim.Eq_b(new[] { Length }, Array.Empty<Dim>()));
    }
    [Fact]
    public void Array_arithmetic_preserves_inputs_and_checks_constant_factors()
    {
        new RuntimeOptions().SyncToThread();
        var dims = new[] { Length, Time };
        var scaled = MatDim.Mult(Length, dims);
        Assert.Same(Length, dims[0]);
        Assert.Same(Dim.Pwr(Length, 2), scaled[0]);
        Assert.Same(Length, MatDim.IsScalarMultiple(dims, scaled));
        Assert.Null(MatDim.IsScalarMultiple(dims, new[] { Time, Length }));
        var inverse = MatDim.Invert(dims);
        Assert.Same(Dim.identity, MatDim.Mult(dims, inverse));
        Assert.Null(MatDim.Mult(dims, dims));
        Assert.Same(Dim.Mult(Length, Time), MatDim.Product(Dim.identity, dims));
        Assert.Same(Time, MatDim.Product(Time, []));
        Assert.Equal(new[] { Dim.identity, Length, Time }, MatDim.Push(Dim.identity, dims));
        Assert.Empty(MatDim.Invert([]));
        Assert.Empty(MatDim.Mult(Length, []));
        Assert.Throws<NullReferenceException>(() => MatDim.Mult(Length, null!));
        Assert.Throws<NullReferenceException>(() => MatDim.Invert(null!));
        Assert.Throws<IndexOutOfRangeException>(() => MatDim.Mult(Array.Empty<Dim>(), Array.Empty<Dim>()));
        Assert.Throws<IndexOutOfRangeException>(() => MatDim.IsScalarMultiple([], []));
        Assert.Same(Dim.identity, MatDim.Mult(new[] { Length }, new[] { inverse[0], Time }));
    }

    [Fact]
    public void Factories_preserve_original_uniform_shape_rule_and_axis_arrays()
    {
        new RuntimeOptions().SyncToThread();
        var uniform=MatDim.make(MatDim.RC.ROW,new[]{Length,Length},7);
        Assert.Equal(2,uniform.Rows);
        Assert.Equal(2,uniform.Cols);
        Assert.Same(uniform,MatDim.Transpose(uniform));
        Assert.Same(Length,uniform.dim(-1,99));
        var dimensions=new[]{Length,Time};
        var rows=MatDim.make(MatDim.RC.ROW,dimensions,3);
        Assert.Same(dimensions,rows.RowDimensions);
        Assert.Same(dimensions,rows.ColDimensionArray(99));
        Assert.Equal(3,rows.Cols);
        var transposed=MatDim.Transpose(rows);
        Assert.Equal(3,transposed.Rows);
        Assert.Same(dimensions,transposed.ColDimensions);
        Assert.Same(Dim.Mult(Length,Time),MatDim.Det(rows,42));
        Assert.Null(MatDim.Pwr(rows,2));
        Assert.Null(MatDim.Pwr(rows,0));
        Assert.Same(Dim.Pwr(Length,-1),MatDim.Pwr(uniform,-1).UniformDim);
        Assert.Same(Dim.identity,MatDim.Pwr(uniform,0).UniformDim);
        Assert.True(MatDim.Eq_b(MatDim.make(Length),uniform));
    }
    [Fact]
    public void Default_linear_algebra_dispatch_returns_null_for_unhandled_dimension_kind()
    {
        var grid=new Grid(new[,]{{Length}});
        Assert.Null(MatDim.MMult(grid,grid));
        Assert.Null(MatDim.DMult(grid,grid));
        Assert.Null(MatDim.Transpose(grid));
        Assert.Null(MatDim.Det(grid,1));
        Assert.Null(MatDim.Pwr(grid,-1));
        Assert.False(MatDim.Eq_b(grid,Length));
    }
}

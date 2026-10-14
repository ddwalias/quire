using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Run;

public class FunctionMatrixTests
{
    private class Probe : Function
    {
        public object[] Last = null!;
        public Probe(int purity, int n = 0, int? min = null, bool varargs = false) : base(purity, n)
        { if (min.HasValue) m_minargs = min.Value; m_varargs = varargs; }
        public override void invVec<R>(out R result, object[] args) { Last = args; result = (R)(object)args.Length; }
    }
    private sealed class OverrideProbe : Probe
    {
        public OverrideProbe() : base(1) { }
        public override void inv<R, A>(out R result, A value) => result = (R)(object)"inv";
        public override void app<R, A>(out R result, A value) => result = (R)(object)"app";
    }
    [Fact]
    public void Constructor_and_full_application_preserve_original_purity_contract()
    {
        var f = new Probe(5, 2);
        Assert.Equal("<anonymous>", f.Name);
        Assert.Equal("Function: <anonymous>", f.ToString());
        f.Name = "sum";
        Assert.Equal("Function: sum", f.ToString());
        Assert.Equal(3, f.Purity);
        Assert.Equal(3, f.MinArgs);
        Assert.Same(Dim.function_dim, f.m_dim);
        Assert.True(f.IsFullyApplied(3));
        Assert.False(f.IsFullyApplied(2));
        var error = Assert.Throws<AppException>(() => f.AssertFullApp(2, 19));
        Assert.Equal("too_few_arguments\t3\t2", error.Message);
        Assert.Equal(19u, error.Argument);
        Assert.Equal(0u, error.Handle);
        Assert.True(new Probe(0, varargs: true).IsFullyApplied(-7));
    }
    [Fact]
    public void AppVec_dispatches_all_arities_and_preserves_varargs_array_identity()
    {
        for (int count = 0; count <= 19; ++count)
        {
            var f = new Probe(count);
            object[] args = Enumerable.Range(0, count).Cast<object>().ToArray();
            Assert.Equal(count, f.Apply(args));
            Assert.Equal(args, f.Last);
            if (count == 0 || count > 16) Assert.Same(args, f.Last);
            else Assert.NotSame(args, f.Last);
        }
        var variadic = new Probe(1, varargs: true);
        object[] input = [1, 2, 3];
        Assert.Equal(3, variadic.Apply(input));
        Assert.Same(input, variadic.Last);
    }
    [Fact]
    public void Optional_padding_and_arity_sixteen_boundary_match_original()
    {
        var optional = new Probe(3, min: 1);
        Assert.Equal(3, optional.Apply([1]));
        Assert.Equal(new object?[] { 1, null, null }, optional.Last);
        var boundary = new Probe(17, min: 16);
        Assert.Null(boundary.Apply(new object[16]));
        Assert.Null(boundary.Last);
        var wider = new Probe(19, min: 17);
        Assert.Equal(19, wider.Apply(new object[17]));
    }
    [Fact]
    public void Delegates_use_inv_overrides_while_apply_uses_app_overrides()
    {
        var f = new OverrideProbe();
        Assert.Equal("inv", f.AsDelegate_1()(1));
        Assert.Equal("app", f.Apply([1]));
        var optional = new Probe(2, min: 1);
        Assert.Equal(1, optional.AsDelegate_1()(42));
        Assert.Single(optional.Last);
        Assert.Equal(2, optional.Apply([42]));
        var error = Assert.Throws<AppException>(() => new Probe(2).AsDelegate_1(7));
        Assert.Equal(7u, error.Argument);
        Assert.Equal(0u, error.Handle);
    }
    [Fact]
    public void Partial_application_captures_values_and_array_helpers_preserve_alias_rules()
    {
        var f = new Probe(3);
        var closure = Assert.IsAssignableFrom<Function>(f.Apply([1]));
        Assert.Equal(2, closure.Purity);
        Assert.Equal("<anonymous>[1]", closure.Name);
        Assert.Equal(3, closure.Apply([2, 3]));
        Assert.Equal(new object[] { 1, 2, 3 }, f.Last);
        object[] array = [1, 2];
        Assert.Same(array, FunctionUtilities.take(2, array));
        Assert.Same(array, FunctionUtilities.take(3, array));
        Assert.Equal(new object[] { 1 }, FunctionUtilities.take(1, array));
        Assert.Equal(new object[] { 2 }, FunctionUtilities.but(1, array));
        Assert.Empty(FunctionUtilities.but(2, array));
        Assert.Equal(new object[] { 0, 1, 2 }, FunctionUtilities.PushFront(0, array));
        Assert.Throws<AppException>(() => new Probe(0).app(out object _));
    }
    private sealed class NumberProbe : Function
    {
        public NumberProbe(int arity) : base(arity, 0) { }
        public override void invVec<R>(out R result, object[] args) => result = (R)(object)new NumVal(args.Length);
    }
    [Fact]
    public void Function_backed_matrix_maps_invoke_real_dependencies()
    {
        new RuntimeOptions().SyncToThread();
        var source = new MatVal(new double[] { 2, 4 });
        var unary = MatVal.map(new NumberProbe(1), source);
        Assert.Equal(2, unary.Rows);
        Assert.Equal(new double[] { 1, 1 }, unary.m_rarr);
        var binary = MatVal.map(new NumberProbe(2), source, source);
        Assert.Equal(new double[] { 2, 2, 2, 2 }, binary.m_rarr);
        Assert.Equal(2, binary.Cols);
    }
}

using System;
using System.Linq;
using System.Reflection;
using Quire.Mcd;
using Quire.Mpl.Run;
using Xunit;

namespace Quire.Mpl.Tests.Run;

public class FunctionTests
{
    private class SimpleProbe : Function
    {
        public SimpleProbe(int purity, int n = 0) : base(purity, n) { }
        public override void invVec<R>(out R result, object[] args)
        {
            result = (R)(object)string.Join(":", args);
        }
    }

    [Fact]
    public void FunctionUtilities_array_helpers_behave_as_expected()
    {
        object[] a = [1, 2];
        object[] b = [3, 4, 5];

        var concatenated = FunctionUtilities.concat(a, b);
        Assert.Equal([1, 2, 3, 4, 5], concatenated);

        int[] nums = [10, 20, 30, 40];
        Assert.Equal([10, 20], FunctionUtilities.take(2, nums));
        Assert.Same(nums, FunctionUtilities.take(5, nums));

        Assert.Equal([30, 40], FunctionUtilities.but(2, nums));
        Assert.Empty(FunctionUtilities.but(5, nums));

        Assert.Equal([0, 1, 2], FunctionUtilities.PushFront(0, a));
        Assert.Equal([-1, 0, 1, 2], FunctionUtilities.PushFront(-1, 0, a));

        Assert.Equal([1, 2, 99], FunctionUtilities.Push(a, 99));
        Assert.Equal([1, 2, 98, 99], FunctionUtilities.Push(a, 98, 99));

        Assert.Equal(16, FunctionUtilities.maxargs);
        Assert.Empty(FunctionUtilities.empty_vec);
    }

    [Fact]
    public void FunctionUtilities_reflection_caches_are_initialized()
    {
        Assert.NotNull(FunctionUtilities.FcnCtor);
        Assert.NotNull(FunctionUtilities.CloCtorGeneric);
        Assert.NotNull(FunctionUtilities.MethCtor);
        Assert.NotNull(FunctionUtilities.ObjMethCtor);
        Assert.NotNull(FunctionUtilities.CallMethod);
        Assert.Equal(17, FunctionUtilities.CallMethod.Length);

        var cloCtor = FunctionUtilities.CloCtor(typeof(int));
        Assert.NotNull(cloCtor);
        Assert.True(cloCtor.IsGenericMethod);

        var callMethod = FunctionUtilities.GetCallMethod(typeof(string), [typeof(int), typeof(double)]);
        Assert.NotNull(callMethod);
        Assert.Equal("Call_2", callMethod.Name);
    }

    [Fact]
    public void Delegates_projections_and_invocations_work()
    {
        var p21 = Delegates.project21<int, string>();
        var p22 = Delegates.project22<int, string>();
        Assert.Equal(42, p21(42, "hello"));
        Assert.Equal("hello", p22(42, "hello"));

        var pi1 = Delegates.pi_21<int, int>();
        var pi2 = Delegates.pi_22<int, int>();
        Assert.Equal(10, pi1.Apply([10, 20]));
        Assert.Equal(20, pi2.Apply([10, 20]));

        F<int, int> inc = x => x + 1;
        F<int, int, int> add = (x, y) => x + y;
        F<int, int, int, int> add3 = (x, y, z) => x + y + z;
        F<int, int, int, int, int> add4 = (x, y, z, w) => x + y + z + w;

        Assert.Equal(11, Delegates.Invoke1(inc, 10));
        Assert.Equal(15, Delegates.Invoke2(add, 10, 5));
        Assert.Equal(24, Delegates.Invoke3(add3, 10, 5, 9));
        Assert.Equal(30, Delegates.Invoke4(add4, 10, 5, 9, 6));

        var wrapped1 = Delegates.Wrap1(inc);
        Assert.Equal(8, wrapped1(7));

        F<object, int, object> addObj = (x, y) => x + (int)y;
        var clo1 = Delegates.DelegateClosure_1(addObj, 100);
        Assert.Equal(105, clo1(5));
    }

    [Fact]
    public void Function_factories_and_composition()
    {
        var c = Function.ConstantFunction("pi");
        Assert.Equal("pi", c.Apply([]));
        Assert.Equal("pi", c.Apply([1, 2, 3]));

        F<object, object> doubleFn = x => (int)x * 2;
        F<object, object> plus1Fn = x => (int)x + 1;

        var f1 = DelegateFunction.makeF(doubleFn, "double");
        var f2 = DelegateFunction.makeF(plus1Fn, "plus1");

        var composed = Function.Compose(f1, f2); // f1(f2(x)) = (x + 1) * 2
        Assert.Equal(12, composed.Apply([5]));

        // VectorFunction expects a 1-column MatVal of functions
        Assert.Null(Function.VectorFunction(42));
        var mat = new MatVal(2, 1, true);
        mat.set_1(0, Function.ConstantFunction(10));
        mat.set_1(1, Function.ConstantFunction(20));
        var vecFn = Function.VectorFunction(mat);
        Assert.NotNull(vecFn);
        var resultMat = vecFn.Apply([1, 2]) as MatVal;
        Assert.NotNull(resultMat);
        Assert.Equal(2, resultMat.Rows);
        Assert.Equal(1, resultMat.Cols);
        Assert.Equal(10, resultMat.get_1(0));
        Assert.Equal(20, resultMat.get_1(1));
    }

    [Fact]
    public void Function_inverse_and_vectorization_flags()
    {
        var f = new SimpleProbe(1);
        Assert.Throws<AppException>(() => f.InverseFunction);

        var inv = new SimpleProbe(1);
        f.DeclareInverse(inv);
        Assert.Same(inv, f.InverseFunction);
        Assert.Same(f, inv.InverseFunction);

        Assert.Same(f, f.IgnoreVectorization());
        Assert.Same(f, f.IgnoreVectorization(false));

        f.OptFunc(3);
        Assert.Equal(3, f.MinArgs);
    }

    [Fact]
    public void Function_asFunction_assertions_and_conversions()
    {
        var f = new SimpleProbe(2);
        Assert.Same(f, f.asFunction("test", 1));
        Assert.Same(f, f.asFunctionN(2, 1));
        Assert.NotNull(f.IsFunctionN(2));
        Assert.Null(f.IsFunctionN(3));
    }

    [Fact]
    public void Function_dispatch_calls_from_zero_to_four_arguments()
    {
        var f = new SimpleProbe(4);
        Assert.Equal("1:2:3:4", f.Call(1, 2, 3, 4));
        Assert.Equal("1:2:3:4", f.DynCall(1, 2, 3, 4));
        Assert.Equal("1:2:3:4", f.Call_4<string, int, int, int, int>(1, 2, 3, 4));
        Assert.Equal("1:2:3:4", f.Call_vec<string>([1, 2, 3, 4]));
        Assert.Equal("1:2:3:4", f.DynApply([1, 2, 3, 4]));
        Assert.Equal("1:2:3:4", f.VecApply([1, 2, 3, 4]));
    }

    [Fact]
    public void DelegateFunction_higher_arity_factories()
    {
        F<object, object, object, object> ternary = (a, b, c) => $"{a}-{b}-{c}";
        F<object, object, object, object, object> quartnary = (a, b, c, d) => $"{a}-{b}-{c}-{d}";

        var f3 = DelegateFunction.makeF(ternary, "ternary");
        var f4 = DelegateFunction.makeF(quartnary, "quartnary");

        Assert.Equal("1-2-3", f3.Apply([1, 2, 3]));
        Assert.Equal("1-2-3-4", f4.Apply([1, 2, 3, 4]));
    }
}

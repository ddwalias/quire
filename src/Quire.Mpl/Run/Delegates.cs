#nullable disable
using System;
using System.Reflection;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Delegates
{
    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type[] gtypes;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type[] vtypes;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static MethodInfo[] imethods;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function PI_1;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function PI_2;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    private static MethodInfo[] clo_method;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    static Delegates()
    {
        clo_method = [null, typeof(Delegates).GetMethod("Closure_1")];
        PI_1 = pi_21<object, object>();
        PI_2 = pi_22<object, object>();
        gtypes =
        [
            null, typeof(F<>), typeof(F<,>), typeof(F<,,>), typeof(F<,,,>), typeof(F<,,,,>), typeof(F<,,,,,>),
            typeof(F<,,,,,,>), typeof(F<,,,,,,,>), typeof(F<,,,,,,,,>), typeof(F<,,,,,,,,,>), typeof(F<,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,>), typeof(F<,,,,,,,,,,,,>), typeof(F<,,,,,,,,,,,,,>), typeof(F<,,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,,>), typeof(F<,,,,,,,,,,,,,,,,>), typeof(F<,,,,,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,,,,,>),
        ];
        vtypes =
        [
            typeof(Io), typeof(FV<>), typeof(FV<,>), typeof(FV<,,>), typeof(FV<,,,>), typeof(FV<,,,,>),
            typeof(FV<,,,,,>), typeof(FV<,,,,,,>), typeof(FV<,,,,,,,>), typeof(FV<,,,,,,,,>), typeof(FV<,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,>), typeof(FV<,,,,,,,,,,,>), typeof(FV<,,,,,,,,,,,,>), typeof(FV<,,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,,>), typeof(FV<,,,,,,,,,,,,,,,>), typeof(FV<,,,,,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,,,,,>),
        ];
        imethods =
        [
            null, typeof(Delegates).GetMethod("Invoke1"), typeof(Delegates).GetMethod("Invoke2"),
            typeof(Delegates).GetMethod("Invoke3"), typeof(Delegates).GetMethod("Invoke4"),
        ];
    }

    [MirrorVerified("DelegatesDifferentialTests")]
    public Delegates() { }

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type[] clo_params(Type[] ftys)
    {
        Type[] array = new Type[ftys.Length - 1];
        Array.Copy(ftys, 2, array, 1, ftys.Length - 2);
        array[0] = ftys[0];
        return array;
    }

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type[] fparams(Type r, Type[] args)
    {
        Type[] array = new Type[1 + args.Length];
        Array.Copy(args, 0, array, 1, args.Length);
        array[0] = r;
        return array;
    }

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type[] fparams(MethodInfo m)
    {
        ParameterInfo[] parameters = m.GetParameters();
        Type[] array = new Type[parameters.Length + 1];
        array[0] = m.ReturnType;
        int num = 1;
        foreach (ParameterInfo parameterInfo in parameters)
        {
            array[num++] = parameterInfo.ParameterType;
        }
        return array;
    }

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type[] fparams(Delegate d) => fparams(d.GetType().GetMethod("Invoke"));

    [MirrorVerified("DelegatesDifferentialTests")]
    public static object Invoke1<R, A>(F<R, A> f, object x) => f((A)x);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static object Invoke2<R, A, B>(F<R, A, B> f, object x, object y) => f((A)x, (B)y);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static object Invoke3<R, A, B, C>(F<R, A, B, C> f, object x, object y, object z) => f((A)x, (B)y, (C)z);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static object Invoke4<R, A, B, C, D>(F<R, A, B, C, D> f, object x, object y, object z, object w) => f((A)x, (B)y, (C)z, (D)w);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<R, A> Premium_1<R, A>(F<R, A> f) => (A a) =>
    {
        Function.CheckPremium();
        return f(a);
    };

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<R, A, B> Premium_2<R, A, B>(F<R, A, B> f) => (A a, B b) =>
    {
        Function.CheckPremium();
        return f(a, b);
    };

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<R, A, B, C> Premium_3<R, A, B, C>(F<R, A, B, C> f) => (A a, B b, C c) =>
    {
        Function.CheckPremium();
        return f(a, b, c);
    };

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<R, A, B, C, D> Premium_4<R, A, B, C, D>(F<R, A, B, C, D> f) => (A a, B b, C c, D d) =>
    {
        Function.CheckPremium();
        return f(a, b, c, d);
    };

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    private static F<R, A> Closure_1<R, CLO, A>(F<R, CLO, A> f, CLO clo) => (A a) => f(clo, a);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    private static F<R, A, B> Closure_2<R, CLO, A, B>(F<R, CLO, A, B> f, CLO clo) => (A a, B b) => f(clo, a, b);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    private static F<R, A, B, C> Closure_3<R, CLO, A, B, C>(F<R, CLO, A, B, C> f, CLO clo) => (A a, B b, C c) => f(clo, a, b, c);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    private static F<R, A, B, C, D> Closure_4<R, CLO, A, B, C, D>(F<R, CLO, A, B, C, D> f, CLO clo) => (A a, B b, C c, D d) => f(clo, a, b, c, d);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object> DelegateClosure_1<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object> f) return Closure_1(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_1<CLO>(Delegate d, CLO clo)
    {
        var f = DelegateClosure_1(d, clo);
        if (f == null) return new Fn_clo<CLO>(d, clo, 2);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object> DelegateClosure_2<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object, object> f) return Closure_2(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_2<CLO>(Delegate d, CLO clo)
    {
        var f = DelegateClosure_2(d, clo);
        if (f == null) return new Fn_clo<CLO>(d, clo, 3);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object, object> DelegateClosure_3<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object, object, object> f) return Closure_3(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_3<CLO>(Delegate d, CLO clo)
    {
        var f = DelegateClosure_3(d, clo);
        if (f == null) return new Fn_clo<CLO>(d, clo, 4);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object, object, object> DelegateClosure_4<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object, object, object, object> f) return Closure_4(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_4<CLO>(Delegate d, CLO clo)
    {
        var f = DelegateClosure_4(d, clo);
        if (f == null) return new Fn_clo<CLO>(d, clo, 5);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Delegate DelegateClosure<CLO>(Delegate d, CLO clo, int purity)
    {
        switch (purity)
        {
            case 2:
                return DelegateClosure_1(d, clo);
            case 3:
                return DelegateClosure_2(d, clo);
            case 4:
                return DelegateClosure_3(d, clo);
            case 5:
                return DelegateClosure_4(d, clo);
            default:
                return null;
        }
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Delegate toFDelegate(Type F, Delegate d)
    {
        Type type = d.GetType();
        if (type.Equals(F)) return d;
        MethodInfo method = type.GetMethod("Invoke");
        Type[] array = fparams(method);
        if (array[0].IsClass)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i].Equals(typeof(object))) continue;
                for (int j = i; j < array.Length; j++)
                {
                    if (!array[j].IsClass) return null;
                }
                Type targetType = FType(array);
                if (!d.GetType().Equals(targetType))
                    d = Delegate.CreateDelegate(targetType, d, method);
                return Delegate.CreateDelegate(F, d, imethods[array.Length - 1].MakeGenericMethod(array));
            }
            return Delegate.CreateDelegate(F, d, method);
        }
        return null;
    }

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type FType(Type[] tys) => gtypes[tys.Length].MakeGenericType(tys);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type VType(Type[] tys)
    {
        Type type = vtypes[tys.Length];
        if (tys.Length == 0) return type;
        return type.MakeGenericType(tys);
    }

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Delegate MethodToDelegate(MethodInfo mi) => Delegate.CreateDelegate(FType(fparams(mi)), mi);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type MethodToDelegateType(MethodInfo mi) => FType(fparams(mi));

    [MirrorVerified("DelegatesDifferentialTests")]
    public static ConstructorInfo MethodToDelegateCtor(MethodInfo mi) => MethodToDelegateType(mi).GetConstructor([typeof(object), typeof(uint)]);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Type ParameterTypesToDelegateType(Type r, Type[] args) => r == typeof(void) ? VType(args) : FType(fparams(r, args));

    [MirrorVerified("DelegatesDifferentialTests")]
    public static ConstructorInfo ParameterTypesToDelegateCtor(Type r, Type[] args) => ParameterTypesToDelegateType(r, args).GetConstructor([typeof(object), typeof(nint)]);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<object, object> Wrap1<A, B>(F<A, B> f) => (object x) => f((B)x);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<object, object, object> Wrap2<A, B, C>(F<A, B, C> f) => (object x, object y) => f((B)x, (C)y);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<object, object, object, object> Wrap3<A, B, C, D>(F<A, B, C, D> f) => (object x, object y, object z) => f((B)x, (C)y, (D)z);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<A, A, B> project21<A, B>() => (A a, B b) => a;

    [MirrorVerified("DelegatesDifferentialTests")]
    public static F<B, A, B> project22<A, B>() => (A a, B b) => b;

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Function pi_21<A, B>() => DelegateFunction.make(project21<A, B>(), 2);

    [MirrorVerified("DelegatesDifferentialTests")]
    public static Function pi_22<A, B>() => DelegateFunction.make(project22<A, B>(), 2);
}

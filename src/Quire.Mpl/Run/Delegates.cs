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

    static Delegates()
    {
        clo_method = new MethodInfo[2]
        {
            null,
            typeof(Delegates).GetMethod("Closure_1", BindingFlags.NonPublic | BindingFlags.Static)
        };
        PI_1 = pi_21<object, object>();
        PI_2 = pi_22<object, object>();
        gtypes = new Type[20]
        {
            null,
            typeof(F<>),
            typeof(F<,>),
            typeof(F<,,>),
            typeof(F<,,,>),
            typeof(F<,,,,>),
            typeof(F<,,,,,>),
            typeof(F<,,,,,,>),
            typeof(F<,,,,,,,>),
            typeof(F<,,,,,,,,>),
            typeof(F<,,,,,,,,,>),
            typeof(F<,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,,,,>),
            typeof(F<,,,,,,,,,,,,,,,,,,>)
        };
        vtypes = new Type[19]
        {
            typeof(Io),
            typeof(FV<>),
            typeof(FV<,>),
            typeof(FV<,,>),
            typeof(FV<,,,>),
            typeof(FV<,,,,>),
            typeof(FV<,,,,,>),
            typeof(FV<,,,,,,>),
            typeof(FV<,,,,,,,>),
            typeof(FV<,,,,,,,,>),
            typeof(FV<,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,,,,>),
            typeof(FV<,,,,,,,,,,,,,,,,,>)
        };
        imethods = new MethodInfo[5]
        {
            null,
            typeof(Delegates).GetMethod("Invoke1"),
            typeof(Delegates).GetMethod("Invoke2"),
            typeof(Delegates).GetMethod("Invoke3"),
            typeof(Delegates).GetMethod("Invoke4")
        };
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public Delegates() { }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type[] clo_params(Type[] ftys)
    {
        Type[] array = new Type[ftys.Length - 1];
        Array.Copy(ftys, 2, array, 1, ftys.Length - 2);
        array[0] = ftys[0];
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type[] fparams(Type r, Type[] args)
    {
        Type[] array = new Type[1 + args.Length];
        Array.Copy(args, 0, array, 1, args.Length);
        array[0] = r;
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type[] fparams(MethodInfo m)
    {
        ParameterInfo[] parameters = m.GetParameters();
        Type[] array = new Type[parameters.Length + 1];
        array[0] = m.ReturnType;
        for (int i = 0; i < parameters.Length; i++)
            array[i + 1] = parameters[i].ParameterType;
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type[] fparams(Delegate d) => fparams(d.GetType().GetMethod("Invoke"));

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static object Invoke1<R, A>(F<R, A> f, object x) => f((A)x);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static object Invoke2<R, A, B>(F<R, A, B> f, object x, object y) => f((A)x, (B)y);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static object Invoke3<R, A, B, C>(F<R, A, B, C> f, object x, object y, object z) => f((A)x, (B)y, (C)z);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static object Invoke4<R, A, B, C, D>(F<R, A, B, C, D> f, object x, object y, object z, object w) => f((A)x, (B)y, (C)z, (D)w);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<R, A> Premium_1<R, A>(F<R, A> f) => (A a) =>
    {
        Function.CheckPremium();
        return f(a);
    };

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<R, A, B> Premium_2<R, A, B>(F<R, A, B> f) => (A a, B b) =>
    {
        Function.CheckPremium();
        return f(a, b);
    };

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<R, A, B, C> Premium_3<R, A, B, C>(F<R, A, B, C> f) => (A a, B b, C c) =>
    {
        Function.CheckPremium();
        return f(a, b, c);
    };

    [MirrorAnalyzed("Mpl.Run.Delegates")]
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
        if (d is F<object, CLO, object> f)
            return Closure_1(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_1<CLO>(Delegate d, CLO clo)
    {
        F<object, object> f = DelegateClosure_1(d, clo);
        if (f == null)
            return new Fn_clo<CLO>(d, clo, 2);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object> DelegateClosure_2<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object, object> f)
            return Closure_2(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_2<CLO>(Delegate d, CLO clo)
    {
        F<object, object, object> f = DelegateClosure_2(d, clo);
        if (f == null)
            return new Fn_clo<CLO>(d, clo, 3);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object, object> DelegateClosure_3<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object, object, object> f)
            return Closure_3(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_3<CLO>(Delegate d, CLO clo)
    {
        F<object, object, object, object> f = DelegateClosure_3(d, clo);
        if (f == null)
            return new Fn_clo<CLO>(d, clo, 4);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object, object, object> DelegateClosure_4<CLO>(Delegate d, CLO clo)
    {
        if (d is F<object, CLO, object, object, object, object> f)
            return Closure_4(f, clo);
        return null;
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function FunctionClosure_4<CLO>(Delegate d, CLO clo)
    {
        F<object, object, object, object, object> f = DelegateClosure_4(d, clo);
        if (f == null)
            return new Fn_clo<CLO>(d, clo, 5);
        return DelegateFunction.makeF(f, null);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Delegate DelegateClosure<CLO>(Delegate d, CLO clo, int purity) => purity switch
    {
        2 => (Delegate)DelegateClosure_1(d, clo),
        3 => DelegateClosure_2(d, clo),
        4 => DelegateClosure_3(d, clo),
        5 => DelegateClosure_4(d, clo),
        _ => null,
    };

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

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type FType(Type[] tys) => gtypes[tys.Length].MakeGenericType(tys);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type VType(Type[] tys)
    {
        Type type = vtypes[tys.Length];
        if (tys.Length == 0) return type;
        return type.MakeGenericType(tys);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Delegate MethodToDelegate(MethodInfo mi) => Delegate.CreateDelegate(FType(fparams(mi)), mi);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type MethodToDelegateType(MethodInfo mi) => FType(fparams(mi));

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static ConstructorInfo MethodToDelegateCtor(MethodInfo mi) => MethodToDelegateType(mi).GetConstructor(new Type[2]
    {
        typeof(object),
        typeof(uint)
    });

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Type ParameterTypesToDelegateType(Type r, Type[] args)
    {
        if (r != typeof(void))
            return FType(fparams(r, args));
        return VType(args);
    }

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static ConstructorInfo ParameterTypesToDelegateCtor(Type r, Type[] args) => ParameterTypesToDelegateType(r, args).GetConstructor(new Type[2]
    {
        typeof(object),
        typeof(nint)
    });

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object> Wrap1<A, B>(F<A, B> f) => (object x) => f((B)x);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object> Wrap2<A, B, C>(F<A, B, C> f) => (object x, object y) => f((B)x, (C)y);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<object, object, object, object> Wrap3<A, B, C, D>(F<A, B, C, D> f) => (object x, object y, object z) => f((B)x, (C)y, (D)z);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<A, A, B> project21<A, B>() => (A a, B b) => a;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static F<B, A, B> project22<A, B>() => (A a, B b) => b;

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function pi_21<A, B>() => DelegateFunction.make(project21<A, B>(), 2);

    [MirrorAnalyzed("Mpl.Run.Delegates")]
    public static Function pi_22<A, B>() => DelegateFunction.make(project22<A, B>(), 2);
}

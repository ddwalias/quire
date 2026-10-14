#nullable disable
using System;
using System.Reflection;
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>Helpers shared by callable values.</summary>
public abstract partial class FunctionUtilities : DimVal
{
    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public const int maxargs = 16;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static object[] empty_vec;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static MethodInfo FcnCtor;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static MethodInfo CloCtorGeneric;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static ConstructorInfo MethCtor;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static ConstructorInfo ObjMethCtor;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static MethodInfo[] CallMethod;

    static FunctionUtilities()
    {
        empty_vec = new object[0];
        FcnCtor = typeof(DelegateFunction).GetMethod("make");
        CloCtorGeneric = typeof(Function).GetMethod("closure");
        MethCtor = typeof(MethFunction).GetConstructor(new System.Type[2]
        {
            typeof(RuntimeMethodHandle),
            typeof(int)
        });
        ObjMethCtor = typeof(MethFunction).GetConstructor(new System.Type[3]
        {
            typeof(RuntimeMethodHandle),
            typeof(int),
            typeof(object)
        });
        CallMethod = new MethodInfo[17]
        {
            null,
            typeof(Function).GetMethod("Call_1"),
            typeof(Function).GetMethod("Call_2"),
            typeof(Function).GetMethod("Call_3"),
            typeof(Function).GetMethod("Call_4"),
            typeof(Function).GetMethod("Call_5"),
            typeof(Function).GetMethod("Call_6"),
            typeof(Function).GetMethod("Call_7"),
            typeof(Function).GetMethod("Call_8"),
            typeof(Function).GetMethod("Call_9"),
            typeof(Function).GetMethod("Call_10"),
            typeof(Function).GetMethod("Call_11"),
            typeof(Function).GetMethod("Call_12"),
            typeof(Function).GetMethod("Call_13"),
            typeof(Function).GetMethod("Call_14"),
            typeof(Function).GetMethod("Call_15"),
            typeof(Function).GetMethod("Call_16")
        };
    }

    protected FunctionUtilities() { }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static MethodInfo CloCtor(System.Type ty) => CloCtorGeneric.MakeGenericMethod(ty);

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static ConstructorInfo InstMethCtor(System.Type ty) => ObjMethCtor;

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static MethodInfo GetCallMethod(System.Type r, System.Type[] args)
    {
        if (args.Length > 16)
            return typeof(Function).GetMethod("Call_vec").MakeGenericMethod(r);
        return CallMethod[args.Length].MakeGenericMethod(Delegates.fparams(r, args));
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static object[] concat(object[] a, object[] b)
    {
        object[] array = new object[a.Length + b.Length];
        Array.Copy(a, array, a.Length);
        Array.Copy(b, 0, array, a.Length, b.Length);
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static T[] but<T>(int i, T[] a)
    {
        int len = a.Length;
        if (i < len)
        {
            T[] array = new T[len - i];
            Array.Copy(a, i, array, 0, len - i);
            return array;
        }
        return new T[0];
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static T[] take<T>(int i, T[] a)
    {
        if (i < a.Length)
        {
            T[] array = new T[i];
            Array.Copy(a, array, i);
            return array;
        }
        return a;
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static object[] PushFront(object b, object[] a)
    {
        object[] array = new object[a.Length + 1];
        a.CopyTo(array, 1);
        array[0] = b;
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static object[] PushFront(object b, object c, object[] a)
    {
        object[] array = new object[a.Length + 2];
        a.CopyTo(array, 2);
        array[0] = b;
        array[1] = c;
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static object[] Push(object[] a, object b)
    {
        object[] array = new object[a.Length + 1];
        a.CopyTo(array, 0);
        array[a.Length] = b;
        return array;
    }

    [MirrorAnalyzed("Mpl.Run.FunctionUtilities")]
    public static object[] Push(object[] a, object b, object c)
    {
        object[] array = new object[a.Length + 2];
        a.CopyTo(array, 0);
        array[a.Length] = b;
        array[a.Length + 1] = c;
        return array;
    }
}

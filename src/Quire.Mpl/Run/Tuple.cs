#nullable disable
using System.Reflection;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// How the runtime represents a tuple of values of given types (docs/mirror/evidence/Mpl.Run.Tuple.md): one value
/// alone, an object array, or one of the generic <c>Tuple</c> classes of 2 to 5 fields.
/// </summary>
public partial class Tuple
{
    /// <summary>The generic tuple class of each arity; slots 0 and 1 are null.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private static System.Type[] tup_types;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private static MethodInfo gf;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private static MethodInfo sf;

    static Tuple()
    {
        gf = typeof(Tuple).GetMethod("GetField");
        sf = typeof(Tuple).GetMethod("SetField");
        tup_types = [null, null, typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>), typeof(Tuple<,,,,>)];
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Tuple() { }

    /// <summary>0 for null, the length of an object array, otherwise 1.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static int tuple_length(object o)
    {
        if (o == null) return 0;
        object[] a = o as object[];
        if (a != null) return a.Length;
        return 1;
    }

    /// <summary>Throws for fewer than 2 types (no class) and past 5 (index out of range).</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private static System.Type IType(System.Type[] tys) => tup_types[tys.Length].MakeGenericType(tys);

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static bool AllObjects(System.Type[] tys)
    {
        foreach (System.Type t in tys)
            if (t.IsValueType) return false;
        return true;
    }

    /// <summary>The generic tuple class for the types; null past 5 types, and throws below 2.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static System.Type TupleType(System.Type[] tys)
    {
        if (tys.Length < tup_types.Length) return IType(tys);
        return null;
    }

    /// <summary>
    /// One type stands for itself; only reference types give an object array; 2 to 5 types with a value type give
    /// the generic class; more give an object array (<see cref="Rep.HeteroVector"/>). No types at all reach
    /// <see cref="IType"/> and throw.
    /// </summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static System.Type TupleRep(out Rep rep, System.Type[] tys)
    {
        if (tys.Length == 1)
        {
            rep = Rep.Singleton;
            return tys[0];
        }
        if (AllObjects(tys))
        {
            rep = Rep.ObjVector;
            return typeof(object[]);
        }
        if (tys.Length < tup_types.Length)
        {
            rep = Rep.TupleType;
            return IType(tys);
        }
        rep = Rep.HeteroVector;
        return typeof(object[]);
    }

    /// <summary>The public field <c>m_</c><paramref name="n"/> of the type, or null.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static FieldInfo GetFieldInfo(System.Type ty, int n)
    {
        if (ty == null) return null;
        return ty.GetField("m_" + n);
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static MethodInfo Getter(System.Type ty) => gf.MakeGenericMethod(ty);

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static MethodInfo Setter(System.Type ty) => sf.MakeGenericMethod(ty);

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static T GetField<T>(object[] gtup, int i) => (T)gtup[i];

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static void SetField<T>(object[] gtup, int i, T v) => gtup[i] = v;
}

public partial class Tuple<A, B>
{
    [MirrorVerified("MplRunHelpersDifferentialTests")] public A m_1;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public B m_2;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Tuple() { }
}

public partial class Tuple<A, B, C>
{
    [MirrorVerified("MplRunHelpersDifferentialTests")] public A m_1;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public B m_2;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public C m_3;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Tuple() { }
}

public partial class Tuple<A, B, C, D>
{
    [MirrorVerified("MplRunHelpersDifferentialTests")] public A m_1;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public B m_2;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public C m_3;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public D m_4;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Tuple() { }
}

public partial class Tuple<A, B, C, D, E>
{
    [MirrorVerified("MplRunHelpersDifferentialTests")] public A m_1;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public B m_2;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public C m_3;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public D m_4;
    [MirrorVerified("MplRunHelpersDifferentialTests")] public E m_5;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Tuple() { }
}

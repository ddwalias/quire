#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeTupleNameMangleDifferentialTests")]
    public static string TupleNameMangle(Type[] types)
    {
        return NameMangle(TuplePrefix, types);
    }

    [MirrorVerified("DynamicTypeTupleNameMangleDifferentialTests")]
    public static string TupleNameMangle(int n)
    {
        Type[] array = new Type[n];
        Type type = typeof(object);
        for (int i = 0; i < n; i++)
        {
            array[i] = type;
        }
        return TupleNameMangle(array);
    }

    [MirrorVerified("DynamicTypeNameMangleDifferentialTests")]
    private static string NameMangle(string prefix, Type[] types)
    {
        string text = prefix;
        foreach (Type ty in types)
        {
            text += TypeToName(ty);
        }
        return text;
    }
}

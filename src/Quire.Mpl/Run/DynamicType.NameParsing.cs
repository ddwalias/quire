#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeDemangleDifferentialTests")]
    private static string RemovePrefix(string tyName, string prefix)
    {
        int num = tyName.IndexOf(prefix);
        if (num < 0)
        {
            AppException.Raise("demangle error");
        }
        return tyName.Substring(num + prefix.Length + 1);
    }

    [MirrorVerified("DynamicTypeDemangleDifferentialTests")]
    private static Type[] NameDemangle(string tyName, string prefix)
    {
        string[] array = Explode(RemovePrefix(tyName, prefix));
        int num = array.Length;
        Type[] types = new Type[num];
        for (int i = 0; i < num; i++)
        {
            types[i] = NameToType(array[i]);
        }
        return types;
    }

    [MirrorVerified("DynamicTypeDemangleDifferentialTests")]
    public static Type[] TupleNameDemangle(string tyName)
    {
        return NameDemangle(tyName, TuplePrefix);
    }

    [MirrorVerified("DynamicTypeExplodeDifferentialTests")]
    private static string[] Explode(string Name)
    {
        return Name.Split(DelimiterString.ToCharArray());
    }
}

#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
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

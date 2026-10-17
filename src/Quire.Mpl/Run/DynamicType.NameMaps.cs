#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeNameMapsDifferentialTests")]
    private static Type NameToType(string tyName)
    {
        Type type = (Type)NameToTypeTable[tyName];
        if (type == null)
        {
            AppException.Raise("demangle error");
        }

        return type;
    }

    [MirrorVerified("DynamicTypeNameMapsDifferentialTests")]
    private static string TypeToName(Type ty)
    {
        string text = (string)TypeToNameTable[ty];
        if (text != null)
        {
            return text;
        }

        return ".ref";
    }
}

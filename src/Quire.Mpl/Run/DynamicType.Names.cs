using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeNameHelpersDifferentialTests")]
    public static string TypeParameterName(int i)
    {
        return TypeParameterName("T", i);
    }

    [MirrorVerified("DynamicTypeNameHelpersDifferentialTests")]
    public static string TypeParameterName(string prefix, int i)
    {
        return string.Concat((object)prefix, i + 1);
    }

    [MirrorVerified("DynamicTypeNameHelpersDifferentialTests")]
    public static string FieldName(int i)
    {
        return string.Concat((object)"Field", i + 1);
    }
}

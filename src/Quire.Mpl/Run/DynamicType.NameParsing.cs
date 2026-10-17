#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeExplodeDifferentialTests")]
    private static string[] Explode(string Name)
    {
        return Name.Split(DelimiterString.ToCharArray());
    }
}

#nullable disable
using Quire.Mirror;

namespace Quire.FunctionalitiesLimitation;

public partial class FunctionalityItem
{
    [MirrorVerified("Release IL")]
    public FunctionalityItem() { }

    [MirrorVerified("Release IL")]
    public EnumFunctionalityKey FunctionalityKey { get; set; }

    [MirrorVerified("Release IL")]
    public FunctionalityItem Parent { get; set; }

    [MirrorVerified("Release IL")]
    public EnumFunctionalityRelationType FunctionalityRelationType { get; set; }
}

#nullable disable
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class ProtectedItemType
{
    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    public ProtectedItemType() { }

    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    private string itemtypeField;

    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    public string itemtype { get => itemtypeField; set => itemtypeField = value; }

    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }

    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    public IDeS11NProvider DeS11NProvider { get; set; }
}

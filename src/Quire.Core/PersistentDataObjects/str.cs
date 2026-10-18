#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class str
{
    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    public str() { }

    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    private string expridField;

    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    public string exprid { get => expridField; set => expridField = value; }

    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    private string spaceField;

    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    public string space { get => spaceField; set => spaceField = value; }

    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectsstrMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}

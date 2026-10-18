#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class userValueType
{
    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    public userValueType() { }

    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    private string nameField;

    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    public string name { get => nameField; set => nameField = value; }

    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    private userValueTypeType typeField;

    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    public userValueTypeType type { get => typeField; set => typeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}

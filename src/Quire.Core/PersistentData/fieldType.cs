#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class fieldType
{
    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    public fieldType() { }

    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    private object textField;

    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    public object text { get => textField; set => textField = value; }

    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    public object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    private ItemChoiceType1 itemElementNameField;

    [MirrorVerified("PtcPersistentDatafieldTypeMechanicalDifferentialTests")]
    public ItemChoiceType1 ItemElementName { get => itemElementNameField; set => itemElementNameField = value; }
}

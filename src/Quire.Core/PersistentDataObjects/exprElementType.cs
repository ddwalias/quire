#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class exprElementType
{
    [MirrorVerified("PtcPersistentDataObjectsexprElementTypeMechanicalDifferentialTests")]
    public exprElementType() { }

    [MirrorVerified("PtcPersistentDataObjectsexprElementTypeMechanicalDifferentialTests")]
    private string expridField;

    [MirrorVerified("PtcPersistentDataObjectsexprElementTypeMechanicalDifferentialTests")]
    public string exprid { get => expridField; set => expridField = value; }
}

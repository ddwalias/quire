#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class real
{
    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    private byte fontField;

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    public byte font { get => fontField; set => fontField = value; }

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    private string expridField;

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    public string exprid { get => expridField; set => expridField = value; }

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    private baseType baseField;

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    public baseType @base { get => baseField; set => baseField = value; }

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectsrealMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}

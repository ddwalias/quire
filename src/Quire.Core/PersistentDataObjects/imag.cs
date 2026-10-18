#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class imag
{
    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    private baseType baseField;

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    public baseType @base { get => baseField; set => baseField = value; }

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    private imagSymbol symbolField;

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    public imagSymbol symbol { get => symbolField; set => symbolField = value; }

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    private byte fontField;

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    public byte font { get => fontField; set => fontField = value; }

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    private string expridField;

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    public string exprid { get => expridField; set => expridField = value; }

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectsimagMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}

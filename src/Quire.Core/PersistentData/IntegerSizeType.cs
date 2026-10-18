#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class IntegerSizeType
{
    [MirrorVerified("PtcPersistentDataIntegerSizeTypeMechanicalDifferentialTests")]
    public IntegerSizeType() { }

    [MirrorVerified("PtcPersistentDataIntegerSizeTypeMechanicalDifferentialTests")]
    private int widthField;

    [MirrorVerified("PtcPersistentDataIntegerSizeTypeMechanicalDifferentialTests")]
    public int width { get => widthField; set => widthField = value; }

    [MirrorVerified("PtcPersistentDataIntegerSizeTypeMechanicalDifferentialTests")]
    private int heightField;

    [MirrorVerified("PtcPersistentDataIntegerSizeTypeMechanicalDifferentialTests")]
    public int height { get => heightField; set => heightField = value; }
}

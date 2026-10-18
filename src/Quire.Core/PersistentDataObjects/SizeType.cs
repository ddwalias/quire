#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class SizeType
{
    [MirrorVerified("PtcPersistentDataObjectsSizeTypeMechanicalDifferentialTests")]
    public SizeType() { }

    [MirrorVerified("PtcPersistentDataObjectsSizeTypeMechanicalDifferentialTests")]
    private double widthField;

    [MirrorVerified("PtcPersistentDataObjectsSizeTypeMechanicalDifferentialTests")]
    public double width { get => widthField; set => widthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsSizeTypeMechanicalDifferentialTests")]
    private double heightField;

    [MirrorVerified("PtcPersistentDataObjectsSizeTypeMechanicalDifferentialTests")]
    public double height { get => heightField; set => heightField = value; }
}

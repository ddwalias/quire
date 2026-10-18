#nullable disable
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentDataObjects;

public partial class basePictureType
{
    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    public basePictureType() { }

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    private double displaywidthField;

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    public double displaywidth { get => displaywidthField; set => displaywidthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    private bool displaywidthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    public bool displaywidthSpecified { get => displaywidthFieldSpecified; set => displaywidthFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    private double displayheightField;

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    public double displayheight { get => displayheightField; set => displayheightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    private bool displayheightFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    public bool displayheightSpecified { get => displayheightFieldSpecified; set => displayheightFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsbasePictureTypeMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }
}

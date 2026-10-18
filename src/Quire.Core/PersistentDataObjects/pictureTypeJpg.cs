#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class pictureTypeJpg
{
    [MirrorVerified("PtcPersistentDataObjectspictureTypeJpgMechanicalDifferentialTests")]
    private string qualityField;

    [MirrorVerified("PtcPersistentDataObjectspictureTypeJpgMechanicalDifferentialTests")]
    public pictureTypeJpg()
    {
        qualityField = "75";
    }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeJpgMechanicalDifferentialTests")]
    public string quality { get => qualityField; set => qualityField = value; }
}

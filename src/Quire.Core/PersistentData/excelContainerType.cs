#nullable disable
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class excelContainerType
{
    [MirrorVerified("PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests")]
    private bool isIconField;

    [MirrorVerified("PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests")]
    public excelContainerType()
    {
        isIconField = false;
    }

    [MirrorVerified("PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests")]
    public bool IsIcon { get => isIconField; set => isIconField = value; }

    [MirrorVerified("PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests")]
    protected override ExternalObjectType ExternalObjectType => ExternalObjectType.Excel;

    [MirrorVerified("PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests")]
    public bool AutoResize => false;

    [MirrorVerified("PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests")]
    public bool HasFakeOleViewportSize => false;
}

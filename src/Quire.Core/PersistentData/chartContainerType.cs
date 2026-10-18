#nullable disable
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class chartContainerType
{
    [MirrorVerified("PtcPersistentDatachartContainerTypeMechanicalDifferentialTests")]
    public chartContainerType() { }

    [MirrorVerified("PtcPersistentDatachartContainerTypeMechanicalDifferentialTests")]
    private IntegerSizeType sizeInPixelsField;

    [MirrorVerified("PtcPersistentDatachartContainerTypeMechanicalDifferentialTests")]
    public IntegerSizeType SizeInPixels { get => sizeInPixelsField; set => sizeInPixelsField = value; }

    [MirrorVerified("PtcPersistentDatachartContainerTypeMechanicalDifferentialTests")]
    protected override ExternalObjectType ExternalObjectType => ExternalObjectType.Chart;

    [MirrorVerified("PtcPersistentDatachartContainerTypeMechanicalDifferentialTests")]
    public bool AutoResize => false;

    [MirrorVerified("PtcPersistentDatachartContainerTypeMechanicalDifferentialTests")]
    public bool HasFakeOleViewportSize => false;
}

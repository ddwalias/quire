#nullable disable
using System.Windows;
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class oleObjectRegionType
{
    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    public oleObjectRegionType() { }

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    private bool autoResizeField;

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    public bool AutoResize { get => autoResizeField; set => autoResizeField = value; }

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    protected override ExternalObjectType ExternalObjectType => ExternalObjectType.Ole;

    [MirrorVerified("PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests")]
    public Nullable<Size> FakeOleViewportSize { get; set; }
}

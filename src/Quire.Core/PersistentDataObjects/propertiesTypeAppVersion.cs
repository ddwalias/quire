#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class propertiesTypeAppVersion
{
    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests")]
    public propertiesTypeAppVersion() { }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests")]
    private string releaseField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests")]
    public string release { get => releaseField; set => releaseField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeAppVersionMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}

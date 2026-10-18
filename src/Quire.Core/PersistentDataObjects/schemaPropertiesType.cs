#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class schemaPropertiesType
{
    [MirrorVerified("PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests")]
    public schemaPropertiesType() { }

    [MirrorVerified("PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests")]
    private string nameField;

    [MirrorVerified("PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests")]
    public string name { get => nameField; set => nameField = value; }

    [MirrorVerified("PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests")]
    private string versionField;

    [MirrorVerified("PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests")]
    public string version { get => versionField; set => versionField = value; }
}

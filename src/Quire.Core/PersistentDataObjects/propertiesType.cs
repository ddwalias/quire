#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class propertiesType
{
    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private string applicationField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public propertiesType()
    {
        applicationField = "MathcadPrime";
    }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public string application { get => applicationField; set => applicationField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private propertiesTypeAppVersion appVersionField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public propertiesTypeAppVersion appVersion { get => appVersionField; set => appVersionField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private propertiesTypeSerializationVersion serializationVersionField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public propertiesTypeSerializationVersion serializationVersion { get => serializationVersionField; set => serializationVersionField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private string engineVersionField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public string engineVersion { get => engineVersionField; set => engineVersionField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private string buildField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public string build { get => buildField; set => buildField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private schemaPropertiesType[] schemaPropertiesListField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public schemaPropertiesType[] schemaPropertiesList { get => schemaPropertiesListField; set => schemaPropertiesListField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    private propertiesTypeNewData newDataField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests")]
    public propertiesTypeNewData NewData { get => newDataField; set => newDataField = value; }
}

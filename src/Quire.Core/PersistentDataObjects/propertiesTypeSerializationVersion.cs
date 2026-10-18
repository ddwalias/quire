#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class propertiesTypeSerializationVersion
{
    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    public propertiesTypeSerializationVersion() { }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    private propertiesTypeSerializationVersionArchitecture architectureField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    public propertiesTypeSerializationVersionArchitecture architecture { get => architectureField; set => architectureField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    private bool architectureFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    public bool architectureSpecified { get => architectureFieldSpecified; set => architectureFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    private string cultureField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    public string Culture { get => cultureField; set => cultureField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    private string uiCultureField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    public string UiCulture { get => uiCultureField; set => uiCultureField = value; }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeSerializationVersionMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}

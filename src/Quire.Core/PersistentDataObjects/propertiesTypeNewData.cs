#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class propertiesTypeNewData
{
    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeNewDataMechanicalDifferentialTests")]
    public propertiesTypeNewData() { }

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeNewDataMechanicalDifferentialTests")]
    private XmlElement[] anyField;

    [MirrorVerified("PtcPersistentDataObjectspropertiesTypeNewDataMechanicalDifferentialTests")]
    public XmlElement[] Any { get => anyField; set => anyField = value; }
}

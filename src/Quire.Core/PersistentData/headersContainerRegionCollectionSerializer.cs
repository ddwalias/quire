#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class headersContainerRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDataheadersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public headersContainerRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDataheadersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public string RootNodeName => "headers";

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => ContainerWrapper.Regions; set => ContainerWrapper.Regions = value; }

    [MirrorVerified("Release IL")]
    private System.Xml.Serialization.XmlSerializer GetXmlSerializer(bool useOverrides)
    {
        if (useOverrides)
        {
            return _xmlSerializerWithOverrides ?? (_xmlSerializerWithOverrides = worksheetRegionType.CreateSerializerWithOverrides(typeof(cachedWorksheetRegionType.headersContainer), RootNodeName));
        }
        return _xmlSerializerDefault ?? (_xmlSerializerDefault = worksheetRegionType.CreateDefaultSerializer(typeof(cachedWorksheetRegionType.headersContainer), RootNodeName));
    }
}

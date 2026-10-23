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

    [MirrorVerified("Release IL")]
    public void Deserialize(System.Xml.XmlElement regionCollectionXml, bool useOverrides)
    {
        using (var reader = new System.Xml.XmlNodeReader(regionCollectionXml))
        {
            var container = GetXmlSerializer(useOverrides).Deserialize(reader) as cachedWorksheetRegionType.headersContainer;
            if (container != null)
            {
                _containerWrapper = new headersContainerWrapper();
                if (container.header != null)
                {
                    var header = new header();
                    header.regions = container.header.regions;
                    _containerWrapper.Header = header;
                }
                if (container.firstPageHeader != null)
                {
                    var header = new header();
                    header.regions = container.firstPageHeader.regions;
                    _containerWrapper.FirstPageHeader = header;
                }
            }
        }
    }
}

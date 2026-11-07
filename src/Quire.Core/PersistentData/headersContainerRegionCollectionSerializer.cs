#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class headersContainerRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDataheadersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public string RootNodeName => "headers";

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => ContainerWrapper.Regions; set => ContainerWrapper.Regions = value; }

    [MirrorVerified("Release IL")]
    public System.Xml.XmlElement Serialize(System.Xml.XmlDocument xmlContentDocument, bool useOverrides)
    {
        // Release IL null-tests the callback item through the IHasPreSerialize base interface (dup/brtrue shape).
        ContainerWrapper.Header.Regions?.Each((IRegionType item) => ((IHasPreSerialize)item)?.PreSerialize());
        ContainerWrapper.FirstPageHeader.Regions?.Each((IRegionType item) => ((IHasPreSerialize)item)?.PreSerialize());
        using (Quire.Xml.XmlNodeWriter xmlWriter = new Quire.Xml.XmlNodeWriter(xmlContentDocument))
        {
            GetXmlSerializer(useOverrides).Serialize(xmlWriter, ContainerWrapper.Container);
        }
        if (xmlContentDocument.ChildNodes.Count == 1)
        {
            return xmlContentDocument.ChildNodes[0] as System.Xml.XmlElement;
        }
        return null;
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

    [MirrorVerified("Release IL")]
    private System.Xml.Serialization.XmlSerializer GetXmlSerializer(bool useOverrides)
    {
        if (useOverrides)
        {
            return _xmlSerializerWithOverrides ?? (_xmlSerializerWithOverrides = worksheetRegionType.CreateSerializerWithOverrides(typeof(cachedWorksheetRegionType.headersContainer), RootNodeName));
        }
        return _xmlSerializerDefault ?? (_xmlSerializerDefault = worksheetRegionType.CreateDefaultSerializer(typeof(cachedWorksheetRegionType.headersContainer), RootNodeName));
    }

    [MirrorVerified("PtcPersistentDataheadersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public headersContainerRegionCollectionSerializer() { }
}

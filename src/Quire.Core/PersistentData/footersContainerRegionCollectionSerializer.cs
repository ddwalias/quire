#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class footersContainerRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDatafootersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public footersContainerRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDatafootersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public string RootNodeName => "footers";

    public void Deserialize(System.Xml.XmlElement regionCollectionXml, bool useOverrides)
    {
        using System.Xml.XmlNodeReader xmlReader = new System.Xml.XmlNodeReader(regionCollectionXml);
        if (GetXmlSerializer(useOverrides).Deserialize(xmlReader) is cachedWorksheetRegionType.footersContainer footersContainer)
        {
            _containerWrapper = new footersContainerWrapper();
            if (footersContainer.footer != null)
            {
                footer footer2 = new footer();
                footer2.regions = footersContainer.footer.regions;
                _containerWrapper.Footer = footer2;
            }

            if (footersContainer.firstPageFooter != null)
            {
                footer footer3 = new footer();
                footer3.regions = footersContainer.firstPageFooter.regions;
                _containerWrapper.FirstPageFooter = footer3;
            }
        }
    }

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => ContainerWrapper.Regions; set => ContainerWrapper.Regions = value; }

    [MirrorVerified("Release IL")]
    private System.Xml.Serialization.XmlSerializer GetXmlSerializer(bool useOverrides)
    {
        if (useOverrides)
        {
            return _xmlSerializerWithOverrides ?? (_xmlSerializerWithOverrides = worksheetRegionType.CreateSerializerWithOverrides(typeof(cachedWorksheetRegionType.footersContainer), RootNodeName));
        }
        return _xmlSerializerDefault ?? (_xmlSerializerDefault = worksheetRegionType.CreateDefaultSerializer(typeof(cachedWorksheetRegionType.footersContainer), RootNodeName));
    }
}

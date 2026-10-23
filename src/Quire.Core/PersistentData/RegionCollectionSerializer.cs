#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class RegionCollectionSerializer<TCollection>
{
    public System.Xml.XmlElement Serialize(System.Xml.XmlDocument xmlContentDocument, bool useOverrides)
    {
        PreSerialize();
        Regions = global::Quire.EnumerableExtensions.CollectAsList<IRegionType, IRegionType>(Regions, AddProtectionIfNeeded);
        using (global::Quire.Xml.XmlNodeWriter xmlWriter = new global::Quire.Xml.XmlNodeWriter(xmlContentDocument))
        {
            GetXmlSerializer(useOverrides).Serialize(xmlWriter, RegionCollection);
        }
        if (xmlContentDocument.ChildNodes.Count == 1)
        {
            return xmlContentDocument.ChildNodes[0] as System.Xml.XmlElement;
        }
        return null;
    }

    private IRegionType AddProtectionIfNeeded(IRegionType item)
    {
        if (item.Item is IProtectableDataType { Protect: not false } protectableDataType)
        {
            item.Item = new ProtectedItemType
            {
                DataToProtect = protectableDataType,
                itemtype = protectableDataType.XmlRootElementName
            };
            item.PreSerialize();
        }
        return item;
    }

    public void Deserialize(System.Xml.XmlElement regionCollectionXml, bool useOverrides)
    {
        using System.Xml.XmlNodeReader xmlReader = new System.Xml.XmlNodeReader(regionCollectionXml);
        RegionCollection = GetXmlSerializer(useOverrides).Deserialize(xmlReader) as TCollection;
    }

    private System.Xml.Serialization.XmlSerializer GetXmlSerializer(bool useOverrides)
    {
        if (useOverrides)
        {
            return _worksheetXmlSerializerWithOverrides ?? (_worksheetXmlSerializerWithOverrides = worksheetRegionType.CreateSerializerWithOverrides(_typeOfCollection, RootNodeName));
        }
        return _worksheetXmlSerializerDefault ?? (_worksheetXmlSerializerDefault = worksheetRegionType.CreateDefaultSerializer(_typeOfCollection, RootNodeName));
    }

    public abstract string RootNodeName { get; }
    public abstract TCollection RegionCollection { get; protected set; }

    [MirrorVerified("PtcPersistentDataRegionCollectionSerializer1MechanicalDifferentialTests")]
    protected RegionCollectionSerializer() { }

    [MirrorVerified("Release IL")]
    public override IEnumerable<IRegionType> Regions { get => RegionCollection.Regions; set => RegionCollection.Regions = value; }
}

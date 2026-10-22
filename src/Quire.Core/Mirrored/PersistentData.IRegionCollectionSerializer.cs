#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial interface IRegionCollectionSerializer
{
    string RootNodeName { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.PersistentDataObjects.IRegionType> Regions { get; set; }
    global::System.Xml.XmlElement Serialize(global::System.Xml.XmlDocument xmlContentDocument, bool useOverrides) ;
    void Deserialize(global::System.Xml.XmlElement regionCollectionXml, bool useOverrides) ;
}

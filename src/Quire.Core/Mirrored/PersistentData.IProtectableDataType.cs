#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial interface IProtectableDataType
{
    bool Protect { get; }
    string XmlRootElementName { get; }
    global::System.Xml.Serialization.XmlSerializer GetXmlSerializer(bool useOverrides) ;
}

#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class headersContainerRegionCollectionSerializer
{
    private headersContainerWrapper _containerWrapper;
    private static global::System.Xml.Serialization.XmlSerializer _xmlSerializerWithOverrides;
    private static global::System.Xml.Serialization.XmlSerializer _xmlSerializerDefault;

    public global::Quire.PersistentData.headersContainerWrapper ContainerWrapper { get => _containerWrapper ?? (_containerWrapper = new global::Quire.PersistentData.headersContainerWrapper()); set => _containerWrapper = value; }
}

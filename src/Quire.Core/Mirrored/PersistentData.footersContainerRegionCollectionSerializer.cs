#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class footersContainerRegionCollectionSerializer
{
    private footersContainerWrapper _containerWrapper;
    private static global::System.Xml.Serialization.XmlSerializer _xmlSerializerWithOverrides;
    private static global::System.Xml.Serialization.XmlSerializer _xmlSerializerDefault;

    public global::Quire.PersistentData.footersContainerWrapper ContainerWrapper { get => _containerWrapper ?? (_containerWrapper = new global::Quire.PersistentData.footersContainerWrapper()); set => _containerWrapper = value; }
}

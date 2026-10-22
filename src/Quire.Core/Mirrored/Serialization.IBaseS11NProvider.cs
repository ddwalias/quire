#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IBaseS11NProvider
{
    global::System.IO.Packaging.PackagePart PackagePart { get; }
    global::Quire.Serialization.ISerializationHelper Helper { get; }
    global::System.Xml.XmlNamespaceManager NameSpaceManager { get; }
    global::System.Xml.XmlDocument XmlContentDocument { get; }
    bool UseOverrides { get; }
}

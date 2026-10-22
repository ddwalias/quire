#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial class matrix
{
    public static global::System.Xml.Serialization.XmlSerializer XmlSerializer { get => _xmlSerializer ?? (_xmlSerializer = CreateSerializer(_typeOfThis, "matrix")); }
    public static global::System.Xml.Serialization.XmlSerializer CreateSerializer(global::System.Type typeOfRoot, string rootNodeName)
    {
        global::System.Xml.Serialization.XmlSerializer serializer;
        using (new global::Quire.Xml.XmlSerializerCreationWrapper())
        {
            serializer = new global::System.Xml.Serialization.XmlSerializer(typeOfRoot, new global::System.Xml.Serialization.XmlRootAttribute(rootNodeName) { Namespace = "http://schemas.mathsoft.com/math50" });
        }
        return serializer;
    }
}

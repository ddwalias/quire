#nullable disable
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class ProtectedItemType
{
    [MirrorVerified("Release IL")]
    public string dataidref
    {
        get
        {
            if (dataidrefField == null)
            {
                IS11NProvider s11NProvider = S11NProvider;
                using (global::System.IO.Stream stream = new global::Quire.MindTouch.IO.ChunkedMemoryStream())
                {
                    global::System.Xml.Serialization.XmlSerializer xmlSerializer = DataToProtect.GetXmlSerializer(s11NProvider.UseOverrides);
                    DebugTools.DevAssert(ProtectableTypesRegistry.GetXmlSerializer(itemtype, s11NProvider.UseOverrides) == xmlSerializer, null);
                    using (global::System.Xml.XmlWriter xmlWriter = global::System.Xml.XmlWriter.Create(stream, new global::System.Xml.XmlWriterSettings()))
                    {
                        xmlSerializer.Serialize(xmlWriter, DataToProtect);
                    }
                    dataidrefField = s11NProvider.PackProtectedPart(stream, null, itemtype);
                }
            }
            return dataidrefField;
        }
        set => dataidrefField = value;
    }

    [MirrorVerified("PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests")]
    public ProtectedItemType() { }
}

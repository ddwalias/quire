#nullable disable
using System.ComponentModel;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class PropertySerializableToXmlAttribute
{
    [MirrorVerified("PtcControlsCoreSerializationPropertySerializableToXmlAttributeMechanicalDifferentialTests")]
    public readonly SerializationInfo _serializationInfo;

    [MirrorVerified("PtcControlsCoreSerializationPropertySerializableToXmlAttributeMechanicalDifferentialTests")]
    public PropertySerializableToXmlAttribute(object o, string propertyName, string attributeName, SerializationInfo serializationInfo) : base(o, propertyName, attributeName)
    {
        _serializationInfo = serializationInfo;
    }

    [MirrorVerified("PtcControlsCoreSerializationPropertySerializableToXmlAttributeMechanicalDifferentialTests")]
    public PropertySerializableToXmlAttribute(object o, string propertyName, string attributeName, TypeConverter typeConverter) : this(o, propertyName, attributeName, typeConverter, null) { }

    [MirrorVerified("PtcControlsCoreSerializationPropertySerializableToXmlAttributeMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();
}

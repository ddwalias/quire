#nullable disable
using System.ComponentModel;
using System.Xml;
using Quire.Mirror;

namespace Quire.Serialization;

internal partial class XmlConverter
{
    private XmlNode ConvertStringToXmlNode(string nodeString)
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(nodeString);
        return xmlDocument.DocumentElement;
    }

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override object CreateInstance(ITypeDescriptorContext context, global::System.Collections.IDictionary propertyValues) => base.CreateInstance(context, propertyValues);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override bool Equals(object obj) => base.Equals(obj);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => base.GetCreateInstanceSupported(context);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override int GetHashCode() => base.GetHashCode();

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) => base.GetProperties(context, value, attributes);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override bool GetPropertiesSupported(ITypeDescriptorContext context) => base.GetPropertiesSupported(context);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => base.GetStandardValues(context);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => base.GetStandardValuesExclusive(context);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => base.GetStandardValuesSupported(context);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override bool IsValid(ITypeDescriptorContext context, object value) => base.IsValid(context, value);

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public override string ToString() => base.ToString();

    [MirrorVerified("PtcSerializationXmlConverterMechanicalDifferentialTests")]
    public XmlConverter() { }
}

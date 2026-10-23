#nullable disable
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Quire.Mirror;

namespace Quire.Serialization;

internal partial class XmlConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        bool result;
        if (typeof(string) == sourceType)
        {
            result = true;
        }
        else
        {
            result = base.CanConvertTo(context, sourceType);
        }
        return result;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        bool result;
        if (typeof(string) == destinationType)
        {
            result = true;
        }
        else
        {
            result = base.CanConvertTo(context, destinationType);
        }
        return result;
    }

    private XmlNode ConvertStringToXmlNode(string nodeString)
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(nodeString);
        return xmlDocument.DocumentElement;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        object result;
        if (value is string)
        {
            string text = value as string;
            if (text.StartsWith("<array>"))
            {
                result = ConvertStringToXmlNode(text).ChildNodes.Cast<XmlNode>().ToArray();
            }
            else
            {
                result = ConvertStringToXmlNode(text);
            }
        }
        else
        {
            result = base.ConvertFrom(context, culture, value);
        }
        return result;
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        object result;
        if (typeof(string) == destinationType)
        {
            if (value is XmlNode)
            {
                result = (value as XmlNode).OuterXml;
            }
            else if (value is XmlNode[])
            {
                StringBuilder builder = new StringBuilder("<array>");
                (value as XmlNode[]).Each((XmlNode node) =>
                {
                    builder.Append(node.OuterXml);
                });
                builder.Append("</array>");
                result = builder.ToString();
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }
        }
        else
        {
            result = base.ConvertTo(context, culture, value, destinationType);
        }
        return result;
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

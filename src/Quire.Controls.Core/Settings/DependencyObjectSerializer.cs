#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Quire.Controls.Core.Settings;

internal partial class DependencyObjectSerializer : IXmlSerializable
{
    private readonly Dictionary<string, DependencyProperty> property;

    protected DependencyObject dependencyObject;

    private readonly string localName;

    public DependencyObjectSerializer(Dictionary<string, DependencyProperty> property, DependencyObject dependencyObject, string localName)
    {
        this.property = property;
        this.dependencyObject = dependencyObject;
        this.localName = localName;
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(localName);
        foreach (KeyValuePair<string, DependencyProperty> item in property)
        {
            writer.WriteStartAttribute(item.Key);
            writer.WriteValue(dependencyObject.GetValue(item.Value));
            writer.WriteEndAttribute();
        }
        writer.WriteEndElement();
    }
}

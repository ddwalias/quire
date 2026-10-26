#nullable disable
using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial class PropertySerializableToXmlAttribute : PropertyToAttribute, IXmlSerializable, IXElementSerializable, IXmlElementSerializable
{
    public readonly SerializationInfo _serializationInfo;

    private bool ShouldSerializeDefaultValue
    {
        get
        {
            if (_serializationInfo != null)
            {
                return (_serializationInfo.Flags & SerializationFlags.DONT_SERIALIZE_DEFAULT_VALUE) == 0;
            }
            return true;
        }
    }

    private object DefaultValue
    {
        get
        {
            object defVal = null;
            if (_serializationInfo != null)
            {
                if ((_serializationInfo.Flags & SerializationFlags.DEFAULT_VALUE_FROM_PROPERTY) != 0)
                {
                    if (!AttemptToGetDefaultValue(out defVal))
                    {
                        DebugTools.DevAssert(false, null);
                    }
                }
                else
                {
                    defVal = _serializationInfo.DefaultValue;
                }
            }
            return defVal;
        }
    }

    private bool CanWriteXML
    {
        get
        {
            if (component == null)
            {
                DebugTools.DevAssert(false, null);
                return false;
            }
            if (!ShouldSerializeDefaultValue)
            {
                return !object.Equals(DefaultValue, PropertyValue);
            }
            return true;
        }
    }

    public PropertySerializableToXmlAttribute(object o, string propertyName, string attributeName)
        : this(o, propertyName, attributeName, (SerializationInfo)null)
    {
    }

    public PropertySerializableToXmlAttribute(object o, string propertyName, string attributeName, SerializationInfo serializationInfo)
        : base(o, propertyName, attributeName)
    {
        _serializationInfo = serializationInfo;
    }

    public PropertySerializableToXmlAttribute(object o, string propertyName, string attributeName, TypeConverter typeConverter)
        : this(o, propertyName, attributeName, typeConverter, null)
    {
    }

    public PropertySerializableToXmlAttribute(object o, string propertyName, string attributeName, TypeConverter typeConverter, SerializationInfo serializationInfo)
        : base(o, propertyName, attributeName, typeConverter)
    {
        _serializationInfo = serializationInfo;
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        if (component != null)
        {
            StringPropertyValue = reader.GetAttribute(attributeName);
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        if (CanWriteXML)
        {
            writer.WriteAttributeString(attributeName, StringPropertyValue);
        }
    }

    public void ReadXml(XElement xElement)
    {
        if (component != null)
        {
            XAttribute xAttribute = xElement.Attribute(attributeName);
            if (xAttribute != null)
            {
                StringPropertyValue = xAttribute.Value;
            }
            else if (_serializationInfo?.DefaultValue != null)
            {
                StringPropertyValue = _serializationInfo.DefaultValue.ToString();
            }
            else
            {
                ResetValue();
            }
        }
    }

    public void WriteXml(XElement xElement)
    {
        if (CanWriteXML)
        {
            xElement.SetAttributeValue(attributeName, StringPropertyValue);
        }
    }

    public void ReadXml(XmlElement xElement)
    {
        if (component != null)
        {
            XmlAttribute xmlAttribute = xElement.Attributes[attributeName];
            if (xmlAttribute != null)
            {
                StringPropertyValue = xmlAttribute.Value;
            }
            else
            {
                ResetValue();
            }
        }
    }

    public void WriteXml(XmlElement xElement)
    {
        if (CanWriteXML)
        {
            xElement.SetAttribute(attributeName, StringPropertyValue);
        }
    }
}

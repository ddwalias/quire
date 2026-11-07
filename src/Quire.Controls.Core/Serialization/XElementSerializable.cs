#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Quire;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class XElementSerializable
{
    private readonly XName _name;

    public object[] Children { get; set; }

    protected XName Name => _name;

    protected XNamespace Namespace => _name.Namespace;

    private bool HasChildren
    {
        get
        {
            if (Children != null)
            {
                return Children.Length != 0;
            }
            return false;
        }
    }

    public XElementSerializable(XName name, params object[] children)
    {
        _name = name;
        Children = children;
    }

    protected virtual void CreateChildren(XElement thisElement = null)
    {
        DebugTools.DevAssert(false, null);
    }

    protected virtual XElement GetThisElement(XElement xElement)
    {
        XElement result;
        if (xElement.Name == _name)
        {
            result = xElement;
        }
        else
        {
            IEnumerable<XElement> source = xElement.Elements(_name);
            int num = source.Count();
            if (num > 1)
            {
                throw new ConstraintException(string.Format("1 {0} element was expected and {1} found", _name, num));
            }
            if (num > 0)
            {
                result = source.First();
            }
            else
            {
                result = null;
            }
        }
        return result;
    }

    private XElement GetThisElement()
    {
        XElement xElement = new XElement(_name);
        WriteElementContents(xElement);
        return xElement;
    }

    public virtual void ReadXml(XElement xElement)
    {
        XElement thisElement = GetThisElement(xElement);
        ReadElementContents(thisElement);
    }

    public virtual void WriteXml(XElement xElement)
    {
        XElement thisElement = GetThisElement();
        xElement.Add(thisElement);
    }

    public virtual void WriteXml(XDocument xDocument)
    {
        XElement thisElement = GetThisElement();
        xDocument.Add(thisElement);
    }

    public virtual void WriteElementContents(XElement thisElement)
    {
        if (!HasChildren)
        {
            CreateChildren(thisElement);
        }
        if (thisElement != null && Children != null)
        {
            object[] children = Children;
            foreach (object obj in children)
            {
                if (obj is IXElementSerializable)
                {
                    (obj as IXElementSerializable).WriteXml(thisElement);
                }
                else if (obj is IXmlSerializable)
                {
                    using (XmlWriter writer = thisElement.CreateWriter())
                    {
                        (obj as IXmlSerializable).WriteXml(writer);
                    }
                }
            }
        }
    }

    public virtual void ReadElementContents(XElement thisElement)
    {
        if (!HasChildren)
        {
            CreateChildren(thisElement);
        }
        if (thisElement != null && Children != null)
        {
            object[] children = Children;
            foreach (object obj in children)
            {
                if (obj is IXElementSerializable)
                {
                    (obj as IXElementSerializable).ReadXml(thisElement);
                }
                else if (obj is IXmlSerializable)
                {
                    using (XmlReader reader = thisElement.CreateReader())
                    {
                        (obj as IXmlSerializable).ReadXml(reader);
                    }
                }
            }
        }
    }

    public virtual XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        XElement xElement = XElement.Load(reader);
        ReadXml(xElement);
    }

    public void WriteXml(XmlWriter writer)
    {
        GetThisElement().Save(writer);
    }

    protected static string GetXmlAttributeName(PropertyInfo propertyInfo)
    {
        if (DebugTools.AssertNotNull(propertyInfo, null).Failed)
        {
            return null;
        }
        string text;
        try
        {
            text = (Attribute.GetCustomAttribute(propertyInfo, typeof(XmlAttributeAttribute)) as XmlAttributeAttribute)?.AttributeName;
        }
        catch (Exception ex)
        {
            text = null;
            DebugTools.FailAssertion(ex);
        }
        if (!text.IsNotEmpty())
        {
            return propertyInfo.Name;
        }
        return text;
    }

    protected static bool GetXmlAttributeDefaultValue(PropertyInfo propertyInfo, out object defaultVal)
    {
        defaultVal = null;
        if (DebugTools.AssertNotNull(propertyInfo, null).Failed)
        {
            return false;
        }
        try
        {
            DefaultValueAttribute defaultValueAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
            if (defaultValueAttribute == null)
            {
                return false;
            }
            defaultVal = defaultValueAttribute.Value;
            return true;
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
            return false;
        }
    }
}

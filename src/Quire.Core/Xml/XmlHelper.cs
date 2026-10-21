#nullable disable
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class XmlHelper
{
    [MirrorVerified("Release IL")]
    public static XmlAttribute AddAttribute(XmlElement node, string name, string value)
    {
        XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
        attribute.Value = value;
        node.Attributes.Append(attribute);
        return attribute;
    }

    [MirrorVerified("Release IL")]
    public static XmlElement AddChild(XmlElement node, string name, string innerText, bool nonPrintableExpected)
    {
        XmlElement element = node.OwnerDocument.CreateElement(name);
        element.InnerText = ReplaceNonPrintableCharacters(innerText);
        if (!nonPrintableExpected)
        {
            DebugTools.DevAssert(innerText.Length == element.InnerText.Length, null);
        }
        node.AppendChild(element);
        return element;
    }

    [MirrorVerified("Release IL")]
    public static XmlElement GetXmlElementIfLocalNameIsSuitable(XmlNode element, string expectedLocalName)
    {
        if (element == null || !(element.LocalName == expectedLocalName))
        {
            return null;
        }
        return element as XmlElement;
    }

    [MirrorVerified("Release IL")]
    public static XElement XmlNodeToXElement(XmlNode source)
    {
        XDocument document = new XDocument();
        using (XmlWriter writer = document.CreateWriter())
        {
            source.WriteTo(writer);
        }
        return document.Root;
    }

    [MirrorVerified("Release IL")]
    public static XmlNode XElementToXmlNode(XElement source)
    {
        using XmlReader reader = source.CreateReader();
        XmlDocument document = new XmlDocument(reader.NameTable);
        document.Load(reader);
        return document;
    }

    [MirrorVerified("Release IL")]
    public static void UpdateToLowerCase(IEnumerable<XElement> elements, IEnumerable<string> names)
    {
        foreach (XElement element in elements)
        {
            foreach (string name in names)
            {
                XAttribute attribute = element.Attribute(XName.Get(name, ""));
                if (attribute != null)
                {
                    attribute.Value = attribute.Value.ToLowerInvariant();
                }
            }
        }
    }

    [MirrorVerified("Release IL")]
    public static void UpdateNameSpace(IEnumerable<XElement> elements, string newNameSpaceName)
    {
        if (elements == null)
        {
            return;
        }
        foreach (XElement element in elements)
        {
            if (string.IsNullOrEmpty(element.Name.Namespace.NamespaceName))
            {
                element.Name = XName.Get(element.Name.LocalName, newNameSpaceName);
                if (element.HasElements)
                {
                    UpdateNameSpace(element.Elements(), newNameSpaceName);
                }
            }
        }
    }

    [MirrorVerified("Release IL")]
    public static void SearchForNodesInXml(XmlNode root, string nodeName, global::Quire.Delegate1<XmlNode> action)
    {
        foreach (XmlNode childNode in root.ChildNodes)
        {
            if (childNode.Name.Equals(nodeName))
            {
                action(childNode);
            }
            else if (childNode.HasChildNodes)
            {
                SearchForNodesInXml(childNode, nodeName, action);
            }
        }
    }

    [MirrorVerified("Release IL")]
    public static StreamWriter CreateStreamWriterForXamlCopy(Stream stream, bool leaveOpen) => new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen);
}

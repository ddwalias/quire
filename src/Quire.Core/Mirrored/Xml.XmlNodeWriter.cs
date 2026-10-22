#nullable disable
#pragma warning disable

namespace Quire.Xml;

partial class XmlNodeWriter
{
    private readonly global::System.Xml.XmlNode _xmlNode;
    public XmlNodeWriter(global::System.Xml.XmlNode node)
    {
        _xmlNode = node;
        if (node is global::System.Xml.XmlDocument)
        {
            _xmlDocument = node as global::System.Xml.XmlDocument;
        }
        else
        {
            _xmlDocument = node.OwnerDocument;
        }

        _writeState = global::System.Xml.WriteState.Start;
        _currentNode = node;
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            ns = _xmlNode.NamespaceURI;
        }

        _currentNode = _currentNode.AppendChild(_xmlDocument.CreateElement(prefix, localName, ns));
    }
}

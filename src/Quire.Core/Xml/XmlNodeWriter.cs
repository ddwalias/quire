#nullable disable
using System.Xml;
using System.Xml;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Xml;

public partial class XmlNodeWriter
{
    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteStartDocument() { }

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteStartDocument(bool standalone) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteEndDocument() { }

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteDocType(string name, string pubid, string sysid, string subset) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteCData(string text) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteComment(string text) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteProcessingInstruction(string name, string text) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteEntityRef(string name) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteCharEntity(char ch) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteWhitespace(string ws) { }

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteSurrogateCharEntity(char lowChar, char highChar) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteChars(char[] buffer, int index, int count) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void WriteRaw(char[] buffer, int index, int count) => throw new NotImplementedException();

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override void Flush() { }

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override string LookupPrefix(string ns) => ns;

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    private WriteState _writeState;

    [MirrorVerified("PtcXmlXmlNodeWriterMechanicalDifferentialTests")]
    public override WriteState WriteState => _writeState;

    [MirrorVerified("Release IL")]
    private XmlNode _currentNode;

    [MirrorVerified("Release IL")]
    public override void WriteEndElement() => _currentNode = _currentNode.ParentNode;

    [MirrorVerified("Release IL")]
    public override void WriteFullEndElement() => _currentNode = _currentNode.ParentNode;

    [MirrorVerified("Release IL")]
    private readonly XmlDocument _xmlDocument;

    [MirrorVerified("Release IL")]
    public override void WriteStartAttribute(string prefix, string localName, string ns) => _currentNode = _currentNode.Attributes.Append(_xmlDocument.CreateAttribute(prefix, localName, ns));

    [MirrorVerified("Release IL")]
    public override void WriteEndAttribute() => _currentNode = ((XmlAttribute)_currentNode).OwnerElement;

    [MirrorVerified("Release IL")]
    public override void WriteString(string text) => _currentNode.AppendChild(_xmlDocument.CreateTextNode(text));

    [MirrorVerified("Release IL")]
    public override void WriteRaw(string data) => _currentNode.AppendChild(_xmlDocument.CreateTextNode(data));

    [MirrorVerified("Release IL")]
    public override void WriteBase64(byte[] buffer, int index, int count) => _currentNode.AppendChild(_xmlDocument.CreateTextNode(Convert.ToBase64String(buffer, index, count, (Base64FormattingOptions)0)));

    [MirrorVerified("Release IL")]
    public override void Close() => _currentNode = null;
}

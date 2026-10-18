#nullable disable
using System.Xml;
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
}

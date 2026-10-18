#nullable disable
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Xml;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls;

public partial class DitaWriter
{
    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private void WriteId(string value) => base.WriteAttributeString("id", value);

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public override void WriteEndElement() => base.WriteFullEndElement();

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public string PackImage(ImagePackagingInformation pictureInfo) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public string PackExternalObject(string sourceFileName, string targetPackagePartName, ExternalObjectType objectType) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public string PackIncludedWorksheet(string fullFilePath, DateTime lastSavedDate, string uniqueId, Func<Package, string, PackagePart> writePackagePart) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public string PackFlowDocument(FlowDocument flowDocument, string regionId) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public string PackProtectedPart(Stream content, string uniqueId, string regionType) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler { get; }

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private readonly string _filePath;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private string DirectoryName => Path.Combine(_filePath, "graphics");

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public PackagePart PackagePart => null;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private readonly ISerializationHelper _helper;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public ISerializationHelper Helper => _helper;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public XmlDocument XmlContentDocument { get; set; }

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public bool UseOverrides => true;

    partial class Element
    {
        [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
        private readonly XmlWriter _writer;

        [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
        public virtual void Dispose() => _writer.WriteEndElement();
    }
}

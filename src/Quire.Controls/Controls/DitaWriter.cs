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

    public partial class Element
    {
        [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
        private readonly XmlWriter _writer;

        public Element(XmlWriter writer, string localName)
        {
            _writer = writer;
            writer.WriteStartElement(localName);
        }

        public Element(XmlWriter writer, string localName, string nameSpace)
        {
            _writer = writer;
            writer.WriteStartElement(localName, nameSpace);
        }

        [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
        public virtual void Dispose() => _writer.WriteEndElement();
    }

    public partial class Worksheet : Element
    {
        private readonly Element _regions;
        private const string ElementName = "worksheet";

        public Worksheet(DitaWriter writer)
            : base(writer, "worksheet", "http://schemas.mathsoft.com/worksheet50")
        {
            _regions = new Element(writer, "regions");
            writer.ZeroCurrentId();
        }

        public override void Dispose()
        {
            _regions.Dispose();
            base.Dispose();
        }
    }

    private string CurrentIdString => string.Format("ml-{0}", _currentId);

    public void ZeroCurrentId()
    {
        _currentId = 0;
    }

    private string ImageFileName(string extension)
    {
        return string.Format("{0}\\{1}{2}.{3}", "graphics", _fileName, _currentId, extension);
    }

    private void EnsureDirectoryExists()
    {
        string directoryName = DirectoryName;
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
    }
}

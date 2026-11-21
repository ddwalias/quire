#nullable disable
using System;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Worksheet;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls;

public partial class DitaWriter
{
    public const string RootElement = "pubsTopic";
    private const double EquationscalingFactor = 2.0;
    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler { get; }
    private readonly string _fileName;
    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private readonly string _filePath;
    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private readonly ISerializationHelper _helper;
    private const string P = "p";
    private const string Fig = "fig";
    private int _currentId;
    private const string GraphicsFolderName = "graphics";
    public const string PubsBody = "pubsBody";

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private string DirectoryName => Path.Combine(_filePath, "graphics");

    private string CurrentIdString => string.Format("ml-{0}", _currentId);

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public PackagePart PackagePart => null;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    private void WriteId(string value) => base.WriteAttributeString("id", value);

    public DitaWriter(string fileName, ISerializationHelper documentHelper)
        : base(fileName, Encoding.UTF8)
    {
        _fileName = Path.GetFileNameWithoutExtension(fileName);
        _filePath = Path.GetDirectoryName(fileName);
        _helper = documentHelper;
        WriteHeader();
        WriteStartElement("pubsBody");
        _currentId = 0;
    }

    private void WriteHeader()
    {
        WriteStartDocument();
        WriteComment("Arbortext, Inc., 1988-2008, v.4002");
        WriteDocType("pubsTopic", "-//PTC//DTD PUBS DITA Topic//EN", null, " ");
        WriteProcessingInstruction("Pub", "Inc");
        WriteStartElement("pubsTopic");
        WriteId(string.Format("{0}-{1:yyyyMMdd}", _fileName, DateTime.Today));
        WriteElementString("title", _fileName);
    }

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

    private FileStream WriteImageElementCreateFile(string extension)
    {
        string text = ImageFileName(extension);
        using (new Element(this, "fig"))
        {
            using (new Element(this, xmlWorksheetNodeNames.Image))
            {
                WriteId(CurrentIdString);
                WriteAttributeString("href", text);
            }
        }
        EnsureDirectoryExists();
        return File.Create(Path.Combine(_filePath, text));
    }

    private void WriteContentControl(FrameworkElement element, Stream fileStream) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void WriteContentControl(ContentControl contentControl)
    {
        using FileStream fileStream = WriteImageElementCreateFile(xmlWorksheetNodeNames.defaultImageExtention);
        WriteContentControl((contentControl.Content as FrameworkElement) ?? contentControl, fileStream);
    }

    public void WriteRichTextBox(RichTextBox richTextBox)
    {
        FlowDocument document = richTextBox.Document;
        WriteElementString("p", new System.Windows.Documents.TextRange(document.ContentStart, document.ContentEnd).Text);
    }

    public void WriteImageControl(ImageControl imageControl)
    {
        BitmapSource source = imageControl.Source as BitmapSource;
        Uri originalImageUri = imageControl.OriginalImageUri;
        string imageExtension = ImageControl.GetImageExtension(originalImageUri);
        using (Stream stream = WriteImageElementCreateFile(imageExtension))
        {
            if (!(originalImageUri == null ? false : ImageUtils.CopyImageToStream(originalImageUri, stream, imageControl.OriginalStreamHash)))
            {
                ImageUtils.EncodeImage(source, imageExtension, stream);
            }
        }
        _currentId++;
    }

    public void WriteRegionML(UIElement region)
    {
        DebugTools.DevAssert(region is FrameworkElement, "region is expected to be a framework element, otherwise it's width and height will not be saved.");
        XmlContentDocument = new XmlDocument();
        if (RegionFactory.Instance.ConvertToIRegionType(() => new worksheetRegionType(), this, region) is worksheetRegionType worksheetRegionType2)
        {
            worksheetRegionType2.regionid = CurrentIdString;
            worksheetRegionType2.Location = new Point(WorksheetControl.GetWorksheetLeft(region), WorksheetControl.GetWorksheetTop(region));
            worksheetRegionType2.Serialize().WriteTo(this);
        }
        _currentId++;
    }

    public void WriteImageControlML(ImageControl imageControl)
    {
        _currentId++;
    }

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
}

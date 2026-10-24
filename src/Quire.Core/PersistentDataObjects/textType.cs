#nullable disable
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using System.Xml;
using Quire.Core;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class textType
{
    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    private bool istextblockField;

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public textType()
    {
        istextblockField = false;
    }

    private void SetDocument(FlowDocument document)
    {
        DebugTools.DevAssert(_document == null && document != null, null);
        _document = document;
        if (_saveInline)
        {
            DocumentWithTextFormat = document;
        }
        else
        {
            DocumentWithTextFormat = new FlowDocument
            {
                Language = Document.Language
            };
            TextFormat.CopyTextFormat(Document, DocumentWithTextFormat);
        }
        SetXaml();
    }

    public void AddFontName(string name)
    {
        if (FontNames == null)
        {
            FontNames = new List<string> { name };
        }
        else if (!FontNames.Contains(name))
        {
            FontNames.Add(name);
        }
    }

    private void SetXaml()
    {
        if (Document != null && S11NProvider != null)
        {
            SerializeFlowDocument(S11NProvider, null);
        }
        else
        {
            Any = null;
        }
    }

    private void SerializeFlowDocument(global::Quire.Serialization.IS11NProvider s11NProvider, string regionId)
    {
        itemidref = _saveInline ? null : s11NProvider.PackFlowDocument(Document, regionId);
        Any = CreateXamlFromFlowDocument(DocumentWithTextFormat);
    }

    private static XmlElement CreateXamlFromFlowDocument(FlowDocument flowDocument)
    {
        XmlDocument xmlDocument = new XmlDocument();
        using (global::Quire.MindTouch.IO.ChunkedMemoryStream chunkedMemoryStream = new global::Quire.MindTouch.IO.ChunkedMemoryStream())
        {
            using (global::System.IO.StreamWriter streamWriter = global::Quire.Xml.XmlHelper.CreateStreamWriterForXamlCopy(chunkedMemoryStream, leaveOpen: true))
            {
                global::System.Windows.Markup.XamlWriter.Save(flowDocument, streamWriter);
                streamWriter.Flush();
            }
            chunkedMemoryStream.Position = 0L;
            xmlDocument.Load(chunkedMemoryStream);
        }
        XmlElement documentElement = xmlDocument.DocumentElement;
        if (documentElement != null)
        {
            xmlDocument.RemoveChild(documentElement);
        }
        return documentElement;
    }

    private FlowDocument LoadFlowDocumentFromInnerXml()
    {
        XmlNode any = Any;
        XmlNamespaceManager nsManager = global::Quire.Serialization.TextRegionSerializationHelper.CreateNamespaceManagerForTextRegions(any);
        RemovePageGapContainers(any, nsManager);
        global::Quire.Serialization.TextRegionSerializationHelper.CopyImagesFromPackageIntoFlowDocument(any, _deS11NProvider, nsManager);
        object obj;
        using (XmlNodeReader reader = new XmlNodeReader(any))
        {
            obj = global::System.Windows.Markup.XamlReader.Load(reader);
        }
        return obj as FlowDocument;
    }

    private void RemovePageGapContainers(XmlNode xmlNode, XmlNamespaceManager nsManager)
    {
        if (_deS11NProvider.Helper.FormatVersion == null || _deS11NProvider.Helper.FormatVersion <= _lastVerToContainPageGaps)
        {
            string xPath = string.Format("//{0}:{1}", TextRegionSerializationHelper.PTC_CONTROLS_TEXT_NAMESPACE[0], TextRegionSerializationHelper.PAGE_GAP_CONTAINER_NODE_NAME);
            foreach (XmlNode item in global::Quire.Serialization.TextRegionSerializationHelper.SelectNodes(xmlNode, xPath, nsManager))
            {
                XmlNode parentNode = item.ParentNode;
                DebugTools.DevAssert(parentNode != null, null);
                parentNode.RemoveChild(item);
            }
        }
    }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public void AddAnnotation(MC14ConversionAnnotationType annotation) => IMC14AnnotationProviderExtensions.AddMC14ConversionLossAnnotations(this, annotation);

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    private XmlElement anyField;

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public XmlElement Any { get => anyField; set => anyField = value; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    private textEmbeddedRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public textEmbeddedRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public bool istextblock { get => istextblockField; set => istextblockField = value; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    private string itemidrefField;

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public string itemidref { get => itemidrefField; set => itemidrefField = value; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public int VisualNumberOfLines { get; set; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public List<string> FontNames { get; set; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    private global::System.Windows.Documents.FlowDocument _document;

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public global::System.Windows.Documents.FlowDocument Document { get => _document; set => SetDocument(value); }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    private global::System.Windows.Documents.FlowDocument DocumentWithTextFormat { get; set; }

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public IEnumerable<string> MC14ConversionLossAnnotations => MC14ConversionLossAnnotationsLocal;

    [MirrorVerified("PtcPersistentDataObjectstextTypeMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }

    [MirrorVerified("Release IL")]
    public void ApplyTextFormatPropertyValueToControl(DependencyProperty property, object value, Block block = default) => TextFormat.SetTextFormat(property, value, DocumentWithTextFormat);

    [MirrorVerified("Release IL")]
    public static TextPointer GetPositionAtOffset(FlowDocument document, int offset) => document.ContentStart.GetPositionAtOffset(offset);

    [MirrorVerified("Release IL")]
    public static int GetOffsetToPosition(FlowDocument document, TextPointer position) => document.ContentStart.GetOffsetToPosition(position);
}

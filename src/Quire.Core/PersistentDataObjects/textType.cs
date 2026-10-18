#nullable disable
using System.Xml;
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
}

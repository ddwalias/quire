#nullable disable
using System.Xml;
using System.Xml;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class mathType
{
    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private resultFormatType resultFormatField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public resultFormatType resultFormat { get => resultFormatField; set => resultFormatField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private mathFormattingType formattingOverrideField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public mathFormattingType formattingOverride { get => formattingOverrideField; set => formattingOverrideField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private bool optimizeField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool optimize { get => optimizeField; set => optimizeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private bool disablecalcField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool disablecalc { get => disablecalcField; set => disablecalcField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private string errorField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public string error { get => errorField; set => errorField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private bool pushdownField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool pushdown { get => pushdownField; set => pushdownField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private int resultRefField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public int resultRef { get => resultRefField; set => resultRefField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private bool resultRefFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool resultRefSpecified { get => resultRefFieldSpecified; set => resultRefFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private bool showLhsOfDefinitionField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool showLhsOfDefinition { get => showLhsOfDefinitionField; set => showLhsOfDefinitionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private LhsOfDefinitionVisibility visibilityLhsOfDefinitionField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public LhsOfDefinitionVisibility visibilityLhsOfDefinition { get => visibilityLhsOfDefinitionField; set => visibilityLhsOfDefinitionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    private bool symbolicEvalKeywordsVisibilityStateField;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool symbolicEvalKeywordsVisibilityState { get => symbolicEvalKeywordsVisibilityStateField; set => symbolicEvalKeywordsVisibilityStateField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public XmlNode[] InnerXml { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public XmlNode[] InnerXmlForXamlSerialization { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public IEnumerable<string> MC14ConversionLossAnnotations => MC14ConversionLossAnnotationsLocal;

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public string[] EquationSExpression { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsmathTypeMechanicalDifferentialTests")]
    public bool IsCalculationDisabled => disablecalc;

    [MirrorVerified("Release IL")]
    public mathType()
    {
        optimizeField = false;
        disablecalcField = false;
        pushdownField = false;
        showLhsOfDefinitionField = true;
        visibilityLhsOfDefinitionField = LhsOfDefinitionVisibility.Visible;
        symbolicEvalKeywordsVisibilityStateField = true;
    }
}

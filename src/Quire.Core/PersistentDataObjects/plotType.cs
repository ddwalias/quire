#nullable disable
using System.Xml.Serialization;
using System.Xml.Serialization;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class plotType
{
    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private static void OverridePropertySerializationToRawXmlNode(XmlAttributeOverrides overiddes, Type propertyOwnerType, string oldPropertyName, string newPropertyName, string elementName, int order) => OverridePropertySerializationToRawXmlNode(overiddes, propertyOwnerType, oldPropertyName, newPropertyName, elementName, "http://schemas.mathsoft.com/worksheet50", order);

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private textType titleField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public textType title { get => titleField; set => titleField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private bool disablecalcField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public bool disablecalc { get => disablecalcField; set => disablecalcField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private BackgroundType backgroundtypeField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public BackgroundType backgroundtype { get => backgroundtypeField; set => backgroundtypeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private bool originpositioningField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public bool originpositioning { get => originpositioningField; set => originpositioningField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private bool showaxisexpressionsField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public bool showaxisexpressions { get => showaxisexpressionsField; set => showaxisexpressionsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private double widthField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public double width { get => widthField; set => widthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private bool widthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public bool widthSpecified { get => widthFieldSpecified; set => widthFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private double heightField;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public double height { get => heightField; set => heightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    private bool heightFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public bool heightSpecified { get => heightFieldSpecified; set => heightFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests")]
    public bool IsCalculationDisabled => disablecalc;

    [MirrorVerified("Release IL")]
    public plotType()
    {
        disablecalcField = false;
        backgroundtypeField = BackgroundType.paper;
        originpositioningField = false;
        showaxisexpressionsField = true;
    }

    public static void GetOverrides(XmlAttributeOverrides overrides)
    {
        int order = 100;
        OverridePropertySerializationToRawXmlNode(overrides, typeof(axes3Type), "xAxis", "xAxisRaw", "xAxis", order++);
        OverridePropertySerializationToRawXmlNode(overrides, typeof(axes3Type), "yAxis", "yAxisRaw", "yAxis", order++);
        OverridePropertySerializationToRawXmlNode(overrides, typeof(axes3Type), "zAxis", "zAxisRaw", "zAxis", order++);
        OverridePropertySerializationToRawXmlNode(overrides, typeof(contourPlotType), "plotEquation", "plotEquationRaw", "plotEquation", order);
    }

    private static void OverridePropertySerializationToRawXmlNode(XmlAttributeOverrides overiddes, Type propertyOwnerType, string oldPropertyName, string newPropertyName, string elementName, string namespaceUri, int order)
    {
        XmlAttributes attributes = new XmlAttributes
        {
            XmlIgnore = true
        };
        overiddes.Add(propertyOwnerType, oldPropertyName, attributes);
        XmlAttributes xmlAttributes = new XmlAttributes
        {
            XmlIgnore = false
        };
        XmlAnyElementAttribute attribute = new XmlAnyElementAttribute(elementName, namespaceUri)
        {
            Order = order
        };
        xmlAttributes.XmlAnyElements.Add(attribute);
        overiddes.Add(propertyOwnerType, newPropertyName, xmlAttributes);
    }
}

#nullable disable
using System.Data;
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Controls.Core.Serialization;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Settings;

public partial class CalculationSettings : XElementSerializable, IXDocumentRepairer, IShouldValidate
{
    protected readonly string _nameSpaceUri;

    private const string LocalName = "calculation";

    public bool ShouldValidateOnWrite => true;

    public bool ShouldValidateOnRead => true;

    public CalculationSettings(IEngineeringDocument document, string nameSpaceUri)
        : base(XName.Get("calculation", nameSpaceUri), new BuiltInVariables(document.DocCalculationVariables, nameSpaceUri), new CalculationBehavior(document.DocCalculationOptions, nameSpaceUri), new XElementSerializable(XName.Get("units", nameSpaceUri), new XElementSerializable(XName.Get("customUnitSystem", nameSpaceUri), new PropertySerializableToXmlAttribute(document, DocumentAttachedProperties.CustomBaseUnitSystemProperty.Name, "base-unit-system", new UnitSystemsConverter()), new UnitSystemCustomized(document, "baseUnits", nameSpaceUri), new UnitSystemCustomized(document, "derivedUnits", nameSpaceUri)), new XElementSerializable(XName.Get("currentUnitSystem", nameSpaceUri), new PropertySerializableToXmlAttribute(document, DocumentAttachedProperties.DefaultUnitSystemProperty.Name, "name", new UnitSystemsConverter()))), new PropertySerializableToXmlAttribute(document, "CacheWorksheet", "cache-worksheet", new XmlBooleanConverter()))
    {
        _nameSpaceUri = nameSpaceUri;
    }

    public CalculationSettings(CalculationVariables calcVars, CalculationOptions calcOptions, string nameSpaceUri)
        : base(XName.Get("calculation", nameSpaceUri), new BuiltInVariables(calcVars, nameSpaceUri), new CalculationBehavior(calcOptions, nameSpaceUri), new XElementSerializable(XName.Get("units", nameSpaceUri), new XElementSerializable(XName.Get("currentUnitSystem", nameSpaceUri), new PropertySerializableToXmlAttribute(null, DocumentAttachedProperties.DefaultUnitSystemProperty.Name, "name", new UnitSystemsConverter()))))
    {
        _nameSpaceUri = nameSpaceUri;
    }

    public override XmlSchema GetSchema()
    {
        if (_nameSpaceUri == "http://schemas.ptc.com/mathcad/settings/calculation10")
        {
            return SchemasManager.Calculation10.Schema;
        }
        throw new ConstraintException(string.Format("_nameSpaceUri should be {0}, current value is {1}", "http://schemas.ptc.com/mathcad/settings/calculation10", _nameSpaceUri));
    }

    protected static void OnSchemaValidation(object sender, ValidationEventArgs e)
    {
    }

    public void RepairXDocument(XDocument xDocument)
    {
        Calculation10Repairer.RepairXDocument(xDocument);
    }
}

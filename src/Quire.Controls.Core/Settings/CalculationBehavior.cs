#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

internal partial class CalculationBehavior : XElementSerializable
{
    public const string LocalName = "calculationBehavior";

    private static readonly XmlBooleanConverter BoolConverter = new XmlBooleanConverter();

    public CalculationBehavior(object o, string nameSpaceUri)
        : base(XName.Get("calculationBehavior", nameSpaceUri), new PropertySerializableToXmlAttribute(o, "IsAutoCalcOn", "automatic-recalculation", BoolConverter), new PropertySerializableToXmlAttribute(o, "StringsIndexedFromOrigin", "strings-use-origin", BoolConverter), new PropertySerializableToXmlAttribute(o, "StrictSingularityChecking", "matrix-strict-singularity-check", BoolConverter), new PropertySerializableToXmlAttribute(o, "StrictComparisions", "exact-boolean", BoolConverter), new PropertySerializableToXmlAttribute(o, "UnitsAndConstantsInSymbolics", "units-in-symbolics", BoolConverter), new PropertySerializableToXmlAttribute(o, "UnitsAndConstantsInSymbolics", "constants-in-symbolics", BoolConverter), new PropertySerializableToXmlAttribute(o, "SymbolicEngineType", "symbolic-engine"), new PropertySerializableToXmlAttribute(o, "BinOctHexMode", "bin-oct-hex-mode", BoolConverter))
    {
    }
}

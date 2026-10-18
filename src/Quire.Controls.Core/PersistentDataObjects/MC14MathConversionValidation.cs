#nullable disable
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public static partial class MC14MathConversionValidation
{
    [MirrorVerified("PtcPersistentDataObjectsMC14MathConversionValidationMechanicalDifferentialTests")]
    private static bool ContainsSymbolicEvaluationAsEquals(XContainer document) => ContainsAttributeWithValue("symEval", "style", "equals", document);

    [MirrorVerified("PtcPersistentDataObjectsMC14MathConversionValidationMechanicalDifferentialTests")]
    private static bool ContainsBooleanEqualityAsEquals(XContainer document) => ContainsAttributeWithValue("equal", "style", "equal", document);

    [MirrorVerified("PtcPersistentDataObjectsMC14MathConversionValidationMechanicalDifferentialTests")]
    private static bool ContainsLocalDefinitionAsEquals(XContainer document) => ContainsAttributeWithValue("localDefine", "style", "equal", document);

    [MirrorVerified("PtcPersistentDataObjectsMC14MathConversionValidationMechanicalDifferentialTests")]
    public static XName GetMc14Math30XName(string localName) => XName.Get(localName, "http://schemas.mathsoft.com/math30");
}

#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class XmcdToIResultNodeTransform
{
    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private readonly IResultNodeFactory _resultNodeFactory;

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    public XmcdToIResultNodeTransform(IResultNodeFactory resultNodeFactory)
    {
        _resultNodeFactory = resultNodeFactory;
    }

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private static int GetUnitPowerDenominator(XmlNode node) => GetIntegerFromAttribute(node, "power-denominator", 1);

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private static int GetUnitPowerNumerator(XmlNode node) => GetIntegerFromAttribute(node, "power-numerator", 1);
}

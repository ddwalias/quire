#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ResultNodeToXmcdTransform
{
    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IEmptyResultNode empty) => WritePlaceholderXml();

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IDisplayMatrixResultNode displayMatrix) => WriteDisplayMatrixXml(displayMatrix);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IEagerMatrixResultNode eagerMatrix) => WriteMatrixXml(eagerMatrix);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public XmlWriter Writer { get; private set; }
}

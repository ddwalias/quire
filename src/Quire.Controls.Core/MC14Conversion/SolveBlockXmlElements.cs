#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.Controls.Core.MC14Conversion;

public partial class SolveBlockXmlElements
{
    [MirrorVerified("PtcControlsCoreMC14ConversionSolveBlockXmlElementsMechanicalDifferentialTests")]
    private static string MlDescendant(string localName) => string.Format(".//{0}:{1}", "ml", localName);

    [MirrorVerified("PtcControlsCoreMC14ConversionSolveBlockXmlElementsMechanicalDifferentialTests")]
    private readonly global::System.Collections.IEnumerator _enumerator;

    [MirrorVerified("PtcControlsCoreMC14ConversionSolveBlockXmlElementsMechanicalDifferentialTests")]
    public SolveBlockXmlElements(global::System.Collections.IEnumerator enumerator)
    {
        _enumerator = enumerator;
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionSolveBlockXmlElementsMechanicalDifferentialTests")]
    private static readonly string[] SolverFunctions;

    [MirrorVerified("PtcControlsCoreMC14ConversionSolveBlockXmlElementsMechanicalDifferentialTests")]
    public static bool ContainsSolverFunction(XmlNode xmlNode) => ContainNodes(xmlNode, SolverFunctions);
}

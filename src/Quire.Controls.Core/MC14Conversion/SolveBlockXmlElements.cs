#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Quire.Mirror;
using Quire.Xml;

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
    private static readonly string[] SolverFunctions = new string[6]
    {
        MlDescendant("Find"),
        MlDescendant("Maximize"),
        MlDescendant("Minerr"),
        MlDescendant("Minimize"),
        MlDescendant("Odesolve"),
        MlDescendant("Pdesolve")
    };

    [MirrorVerified("PtcControlsCoreMC14ConversionSolveBlockXmlElementsMechanicalDifferentialTests")]
    public static bool ContainsSolverFunction(XmlNode xmlNode) => ContainNodes(xmlNode, SolverFunctions);

    private static bool ContainNode(XmlNode xmlElement, string xPath)
    {
        return xmlElement.SelectSingleNode(xPath, XmlWorksheetData.Mathcad14NameSpaceManager) != null;
    }

    private static bool ContainNodes(XmlNode xmlElement, IEnumerable<string> xPaths)
    {
        foreach (string current in xPaths)
        {
            if (ContainNode(xmlElement, current))
            {
                return true;
            }
        }
        return false;
    }

    public static bool ContainsSolverFunctionWithFilter(XmlNode xmlNode)
    {
        string[] second = new string[1] { MlDescendant("Find") };
        return ContainNodes(xmlNode, SolverFunctions.Except(second));
    }

    public static bool ContainsMCDXMinMaxSolverFunction(XmlNode xmlNode)
    {
        return xmlNode.SelectSingleNode(".//ml:id[. = \"Maximize\" or . = \"Minimize\"]", XmlWorksheetData.McdxNameSpaceManager) != null;
    }
}

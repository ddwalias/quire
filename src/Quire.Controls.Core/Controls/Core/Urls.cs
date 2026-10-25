#nullable disable
using Quire.Xml;

namespace Quire.Controls.Core;

public static partial class Urls
{
    public static readonly string DefaultMathcadPrimeWebServerHelpPathDir = "https://support.ptc.com/help/mathcad/r" + XmlWorksheetData.CurrentAppVersion.ToString(2) + "/";

    public static readonly string DefaultEngNbWebServerHelpPathDir = "https://support.ptc.com/help/engineering_notebook/r" + XmlWorksheetData.CurrentAppVersion.ToString(2) + "/";

    public static readonly string DefaultUrlParam = "?utm_source=mathcad";

    public const string MathcadExpressUpgrade = "https://www.ptc.com/go/mathcadexpressupgrade";

    public const string MathcadCommunity = "https://community.ptc.com/mathcad-6";

    public const string MathcadEngineeringResources = "https://community.ptc.com/mathcad-7/ptc-mathcad-prime-worksheets-directory-173941";

    public const string MathcadSupportCenter = "https://support.ptc.com/support/mathcad_supportCenter.htm";

    public const string MathcadWebsiteAddress = "www.ptc.com/products/Mathcad";

    public const string MathcadLearningConnector = "https://learningconnector.ptc.com/search/all?product=Mathcad%20Prime";

    public const string MathcadNewUsers = "https://learningconnector.ptc.com/playlist/PL1638953274949/get-started-with-mathcad";

    public const string MathcadStartPageAddress = "www.ptc.com/{0}/mathcad/start-page";
}

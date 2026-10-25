#nullable disable
using Quire.Xml;

namespace Quire.Controls.Core;

public static partial class Urls
{
    public static readonly string DefaultMathcadPrimeWebServerHelpPathDir = "https://support.ptc.com/help/mathcad/r" + XmlWorksheetData.CurrentAppVersion.ToString(2) + "/";

    public static readonly string DefaultEngNbWebServerHelpPathDir = "https://support.ptc.com/help/engineering_notebook/r" + XmlWorksheetData.CurrentAppVersion.ToString(2) + "/";

    public static readonly string DefaultUrlParam = "?utm_source=mathcad";
}

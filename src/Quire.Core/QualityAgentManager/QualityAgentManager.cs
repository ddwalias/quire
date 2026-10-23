#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.QualityAgentManager;

public partial class QualityAgentManager
{
    private static string GetMathcadPrimeBitness()
    {
        string text = BuildArchitectureToBitnessConverter.Instance.Convert(global::Quire.Xml.XmlWorksheetData.Architecture, typeof(string), null, null).ToString();
        if (!string.IsNullOrEmpty(text))
        {
            return text;
        }
        return string.Empty;
    }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    private static string GetMathcadPrimeInstallationDate() => MsiHelper.GetProductInfo(MsiHelper.MathcadCurrentReleaseProductCode, MsiHelper.InstallProperty.InstallDate);

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    public bool IsReportSendingEnabled { private get; set; }

    [MirrorVerified("Release IL")]
    private static string GetMathcadPrimeVersion() => string.Format("{0}", WpfUtils.ApplicationName).Replace(" ", "_");
}

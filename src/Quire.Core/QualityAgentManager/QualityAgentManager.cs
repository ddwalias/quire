#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.QualityAgentManager;

public partial class QualityAgentManager
{
    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    private static string GetMathcadPrimeInstallationDate() => MsiHelper.GetProductInfo(MsiHelper.MathcadCurrentReleaseProductCode, MsiHelper.InstallProperty.InstallDate);

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    public bool IsReportSendingEnabled { private get; set; }

    [MirrorVerified("Release IL")]
    private static string GetMathcadPrimeVersion() => string.Format("{0}", WpfUtils.ApplicationName).Replace(" ", "_");
}

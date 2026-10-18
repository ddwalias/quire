#nullable disable
using Quire.Mirror;

namespace Quire.QualityAgentManager;

public partial struct QualityAgentReportData
{
    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string HostID { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string DomainName { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string InstallationDate { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string Scn { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string Checksum { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string MathcadVersion { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    private readonly Version _mathcadReleaseVersion;

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string MathcadReleaseVersion => _mathcadReleaseVersion.ToString(2);

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string InstallSource { get; private set; }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests")]
    public string Bitness { get; private set; }
}

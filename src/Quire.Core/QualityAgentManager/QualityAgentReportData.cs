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

    [MirrorVerified("Release IL")]
    public string StartTime => _startTime.ToString(DateTimeReportFormat);

    [MirrorVerified("Release IL")]
    public string EndTime => _endTime.ToString(DateTimeReportFormat);

    [MirrorVerified("Release IL")]
    public string TimeZone
    {
        get
        {
            if (_timeZone == null)
            {
                return string.Empty;
            }
            return _timeZone.ToString();
        }
    }

    [MirrorVerified("Release IL")]
    public string CurrentCulture
    {
        get
        {
            if (_currentCulture == null)
            {
                return string.Empty;
            }
            return _currentCulture.ToString();
        }
    }

    [MirrorVerified("Release IL")]
    public string CurrentUICulture
    {
        get
        {
            if (_currentUiCulture == null)
            {
                return string.Empty;
            }
            return _currentUiCulture.ToString();
        }
    }
}

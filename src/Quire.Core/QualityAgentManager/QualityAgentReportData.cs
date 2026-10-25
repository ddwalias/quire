#nullable disable
using Quire.Mirror;

namespace Quire.QualityAgentManager;

public partial struct QualityAgentReportData
{
    private readonly global::Quire.License.LicenseType _licenseType;

    public QualityAgentReportData(
        string hostId,
        string domainName,
        string installationDate,
        global::System.DateTime? licenseIssueDate,
        global::Quire.License.LicenseType licenseType,
        string scn,
        global::System.DateTime startTime,
        global::System.DateTime endTime,
        global::System.TimeZoneInfo timeZone,
        string md5Checksum,
        string mathcadVersion,
        global::System.Version mathcadReleaseVersion,
        global::System.Globalization.CultureInfo currentCulture,
        global::System.Globalization.CultureInfo currentUiCulture,
        string installSource,
        string bitness)
    {
        this = default;
        HostID = hostId;
        DomainName = domainName;
        InstallationDate = installationDate;
        _licenseIssueDate = licenseIssueDate;
        _licenseType = licenseType;
        Scn = scn;
        _startTime = startTime;
        _endTime = endTime;
        _timeZone = timeZone;
        Checksum = md5Checksum;
        MathcadVersion = mathcadVersion;
        _mathcadReleaseVersion = mathcadReleaseVersion;
        _currentCulture = currentCulture;
        _currentUiCulture = currentUiCulture;
        InstallSource = installSource;
        Bitness = bitness;
    }

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

    [MirrorVerified("Release IL")]
    public string LicenseType => _licenseType.ToString();

    [MirrorVerified("Release IL")]
    public string LicenseDate
    {
        get
        {
            if ((object)_licenseIssueDate is DateTime)
            {
                goto HasLicenseIssueDate;
            }
            return string.Empty;

        HasLicenseIssueDate:
            DateTime licenseIssueDate = _licenseIssueDate.Value;
            return licenseIssueDate.ToString(DateTimeReportFormat);
        }
    }
}

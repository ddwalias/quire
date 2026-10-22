#nullable disable
#pragma warning disable

namespace Quire.QualityAgentManager;

partial interface IQualityAgentReportData
{
    string HostID { get; }
    string DomainName { get; }
    string InstallationDate { get; }
    string LicenseDate { get; }
    string LicenseType { get; }
    string Scn { get; }
    string StartTime { get; }
    string EndTime { get; }
    string TimeZone { get; }
    string Checksum { get; }
    string MathcadVersion { get; }
    string MathcadReleaseVersion { get; }
    string CurrentCulture { get; }
    string CurrentUICulture { get; }
    string InstallSource { get; }
    string Bitness { get; }
}

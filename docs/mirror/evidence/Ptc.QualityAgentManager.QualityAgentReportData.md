# Ptc.QualityAgentManager.QualityAgentReportData

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `HostID get` | 0x0600084a | MemberGen: `public string HostID { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HostID set` | 0x06000849 | MemberGen: `public string HostID { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DomainName get` | 0x0600084c | MemberGen: `public string DomainName { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DomainName set` | 0x0600084b | MemberGen: `public string DomainName { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstallationDate get` | 0x0600084e | MemberGen: `public string InstallationDate { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstallationDate set` | 0x0600084d | MemberGen: `public string InstallationDate { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Scn get` | 0x06000852 | MemberGen: `public string Scn { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Scn set` | 0x06000851 | MemberGen: `public string Scn { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Checksum get` | 0x06000857 | MemberGen: `public string Checksum { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Checksum set` | 0x06000856 | MemberGen: `public string Checksum { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MathcadVersion get` | 0x06000859 | MemberGen: `public string MathcadVersion { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MathcadVersion set` | 0x06000858 | MemberGen: `public string MathcadVersion { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MathcadReleaseVersion get` | 0x0600085a | MemberGen: `_mathcadReleaseVersion.ToString(2)` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstallSource get` | 0x0600085e | MemberGen: `public string InstallSource { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstallSource set` | 0x0600085d | MemberGen: `public string InstallSource { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Bitness get` | 0x06000860 | MemberGen: `public string Bitness { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Bitness set` | 0x0600085f | MemberGen: `public string Bitness { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcQualityAgentManagerQualityAgentReportDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

# Ptc.IO.PackageOperationsProvider

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcIOPackageOperationsProviderMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetPackageForWrite(string)` | 0x06000fd4 | MemberGen: `Package.Open(packageName, (FileMode)2)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetPackageForWrite(FileStream)` | 0x06000fd5 | MemberGen: `Package.Open(stream, (FileMode)2)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetPackageForRead(string)` | 0x06000fd6 | MemberGen: `Package.Open(packageName, (FileMode)3, (FileAccess)1, (FileShare)1)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetMainPartFromPackage(Package)` | 0x06000fd7 | MemberGen: `package.GetPart(_worksheet)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetHeaderPartFromPackage(Package)` | 0x06000fd8 | MemberGen: `GetPartIfExists(package, _header)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFooterPartFromPackage(Package)` | 0x06000fd9 | MemberGen: `GetPartIfExists(package, _footer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetHeadersContainerPartFromPackage(Package)` | 0x06000fda | MemberGen: `GetPartIfExists(package, _headersContainer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFootersContainerPartFromPackage(Package)` | 0x06000fdb | MemberGen: `GetPartIfExists(package, _footersContainer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetCoreMetadataPartFromPackage(Package)` | 0x06000fdd | MemberGen: `GetSinglePartByRelationshipType(package, "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties")` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddMainPartToPackage(Package)` | 0x06000fe8 | MemberGen: `AddWorksheetPartToPackage(package, _worksheet)` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddHeaderPartToPackage(Package)` | 0x06000fe9 | MemberGen: `AddWorksheetPartToPackage(package, _header)` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddFooterPartToPackage(Package)` | 0x06000fea | MemberGen: `AddWorksheetPartToPackage(package, _footer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddHeadersContainerPartToPackage(Package)` | 0x06000feb | MemberGen: `AddWorksheetPartToPackage(package, _headersContainer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddFootersContainerPartToPackage(Package)` | 0x06000fec | MemberGen: `AddWorksheetPartToPackage(package, _footersContainer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `OpenFileStreamForReadWithFullShare(string)` | 0x06000ff0 | MemberGen: `File.Open(path, (FileMode)3, (FileAccess)1, (FileShare)3)` | Mechanical; Release IL equals Mathcad's | Verified |
| `CompressionLevel get` | 0x06000fe6 | MemberGen: `public CompressionOption CompressionLevel { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CompressionLevel set` | 0x06000fe7 | MemberGen: `public CompressionOption CompressionLevel { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcIOPackageOperationsProviderMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

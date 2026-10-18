# Ptc.Controls.Core.Serialization.SerializationHelper

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ApplicationName get` | 0x06000bdd | MemberGen: `public string ApplicationName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ApplicationName set` | 0x06000bde | MemberGen: `public string ApplicationName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AppVersion get` | 0x06000bdf | MemberGen: `public Version AppVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AppVersion set` | 0x06000be0 | MemberGen: `public Version AppVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Release get` | 0x06000be1 | MemberGen: `public string Release { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Release set` | 0x06000be2 | MemberGen: `public string Release { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatVersion get` | 0x06000be3 | MemberGen: `public Version FormatVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatVersion set` | 0x06000be4 | MemberGen: `public Version FormatVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EngineVersion get` | 0x06000be5 | MemberGen: `public string EngineVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EngineVersion set` | 0x06000be6 | MemberGen: `public string EngineVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14TextStyles get` | 0x06000bf4 | MemberGen: `_textStyles` | Mechanical; Release IL equals Mathcad's | Verified |
| `MainRegions get` | 0x06000bf5 | MemberGen: `public IList<IRegionPersistentData> MainRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MainRegions set` | 0x06000bf6 | MemberGen: `public IList<IRegionPersistentData> MainRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeaderRegions get` | 0x06000bf7 | MemberGen: `public IList<IRegionPersistentData> HeaderRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeaderRegions set` | 0x06000bf8 | MemberGen: `public IList<IRegionPersistentData> HeaderRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FooterRegions get` | 0x06000bf9 | MemberGen: `public IList<IRegionPersistentData> FooterRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FooterRegions set` | 0x06000bfa | MemberGen: `public IList<IRegionPersistentData> FooterRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FirstPageHeaderRegions get` | 0x06000bfb | MemberGen: `public IList<IRegionPersistentData> FirstPageHeaderRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FirstPageHeaderRegions set` | 0x06000bfc | MemberGen: `public IList<IRegionPersistentData> FirstPageHeaderRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FirstPageFooterRegions get` | 0x06000bfd | MemberGen: `public IList<IRegionPersistentData> FirstPageFooterRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FirstPageFooterRegions set` | 0x06000bfe | MemberGen: `public IList<IRegionPersistentData> FirstPageFooterRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BytesComparer: .ctor()` | 0x060010c7 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

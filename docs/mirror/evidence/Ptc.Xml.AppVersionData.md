# Ptc.Xml.AppVersionData

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcXmlAppVersionDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060003ac | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentAppVersion get` | 0x060003a6 | MemberGen: `public string CurrentAppVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentAppVersion set` | 0x060003a7 | MemberGen: `public string CurrentAppVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentFormatVersion get` | 0x060003a8 | MemberGen: `public string CurrentFormatVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentFormatVersion set` | 0x060003a9 | MemberGen: `public string CurrentFormatVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentEngNbVersion get` | 0x060003aa | MemberGen: `public string CurrentEngNbVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentEngNbVersion set` | 0x060003ab | MemberGen: `public string CurrentEngNbVersion { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcXmlAppVersionDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

# Ptc.UserSettingsProvider

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcUserSettingsProviderMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetPreviousVersion(SettingsContext, SettingsProperty)` | 0x06000371 | MemberGen: `throw new NotImplementedException()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Upgrade(SettingsContext, SettingsPropertyCollection)` | 0x06000373 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000379 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ConfigFolderName get` | 0x06000361 | MemberGen: `public static string ConfigFolderName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ConfigFolderName set` | 0x06000362 | MemberGen: `public static string ConfigFolderName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConfigCopiedForMOR get` | 0x06000363 | MemberGen: `public static bool IsConfigCopiedForMOR { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConfigCopiedForMOR set` | 0x06000364 | MemberGen: `public static bool IsConfigCopiedForMOR { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ApplicationName get` | 0x06000367 | MemberGen: `public override string ApplicationName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ApplicationName set` | 0x06000368 | MemberGen: `public override string ApplicationName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcUserSettingsProviderMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

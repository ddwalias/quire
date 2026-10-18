# Ptc.Controls.Core.Settings.CalculationSettings

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `OnSchemaValidation(object, ValidationEventArgs)` | 0x060009ff | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RepairXDocument(XDocument)` | 0x06000a00 | MemberGen: `Calculation10Repairer.RepairXDocument(xDocument)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShouldValidateOnWrite get` | 0x06000a01 | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShouldValidateOnRead get` | 0x06000a02 | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

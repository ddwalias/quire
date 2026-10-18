# Ptc.Controls.Core.Settings.TextFormatSerializer

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSettingsTextFormatSerializerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `TextDecorationSerializer get` | 0x06000a55 | MemberGen: `public TextDecorationsToAttributesSerializer TextDecorationSerializer { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextDecorationSerializer set` | 0x06000a56 | MemberGen: `public TextDecorationsToAttributesSerializer TextDecorationSerializer { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleKey get` | 0x06000a57 | MemberGen: `public string StyleKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleKey set` | 0x06000a58 | MemberGen: `public string StyleKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSettingsTextFormatSerializerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

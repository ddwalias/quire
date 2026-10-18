# Ptc.Controls.Core.Serialization.ConversionAnnotationMessage

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationConversionAnnotationMessageMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ToString()` | 0x06000a86 | MemberGen: `Text` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000a87 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CategoryKey get` | 0x06000a82 | MemberGen: `public string CategoryKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CategoryKey set` | 0x06000a83 | MemberGen: `public string CategoryKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Text get` | 0x06000a84 | MemberGen: `public string Text { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Text set` | 0x06000a85 | MemberGen: `public string Text { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationConversionAnnotationMessageMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

# Ptc.Controls.Core.MC14Conversion.Mathcad14TextStyle

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Dictionary)` | 0x06000dee | MemberGen: `_stylesDictionary = stylesDictionary;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x06000dd8 | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x06000dd9 | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseStyleName get` | 0x06000dda | MemberGen: `public string BaseStyleName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseStyleName set` | 0x06000ddb | MemberGen: `public string BaseStyleName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

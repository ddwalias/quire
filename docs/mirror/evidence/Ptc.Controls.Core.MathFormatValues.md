# Ptc.Controls.Core.MathFormatValues

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreMathFormatValuesMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060003f6 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontFamilyName get` | 0x060003e8 | MemberGen: `public string FontFamilyName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontFamilyName set` | 0x060003e9 | MemberGen: `public string FontFamilyName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontSize get` | 0x060003ea | MemberGen: `public Nullable<double> FontSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontSize set` | 0x060003eb | MemberGen: `public Nullable<double> FontSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBold get` | 0x060003ec | MemberGen: `public Nullable<bool> IsBold { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBold set` | 0x060003ed | MemberGen: `public Nullable<bool> IsBold { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsItalic get` | 0x060003ee | MemberGen: `public Nullable<bool> IsItalic { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsItalic set` | 0x060003ef | MemberGen: `public Nullable<bool> IsItalic { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsUnderline get` | 0x060003f0 | MemberGen: `public Nullable<bool> IsUnderline { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsUnderline set` | 0x060003f1 | MemberGen: `public Nullable<bool> IsUnderline { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontColor get` | 0x060003f2 | MemberGen: `public Nullable<Color> FontColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontColor set` | 0x060003f3 | MemberGen: `public Nullable<Color> FontColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HighlightColor get` | 0x060003f4 | MemberGen: `public Nullable<Color> HighlightColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HighlightColor set` | 0x060003f5 | MemberGen: `public Nullable<Color> HighlightColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreMathFormatValuesMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

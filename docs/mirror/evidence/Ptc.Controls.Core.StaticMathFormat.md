# Ptc.Controls.Core.StaticMathFormat

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreStaticMathFormatMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `FontFamilyName get` | 0x060003d9 | MemberGen: `public string FontFamilyName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontFamilyName set` | 0x060003da | MemberGen: `public string FontFamilyName { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontSize get` | 0x060003db | MemberGen: `public Nullable<double> FontSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontSize set` | 0x060003dc | MemberGen: `public Nullable<double> FontSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBold get` | 0x060003dd | MemberGen: `public Nullable<bool> IsBold { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBold set` | 0x060003de | MemberGen: `public Nullable<bool> IsBold { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsItalic get` | 0x060003df | MemberGen: `public Nullable<bool> IsItalic { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsItalic set` | 0x060003e0 | MemberGen: `public Nullable<bool> IsItalic { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsUnderline get` | 0x060003e1 | MemberGen: `public Nullable<bool> IsUnderline { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsUnderline set` | 0x060003e2 | MemberGen: `public Nullable<bool> IsUnderline { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontColor get` | 0x060003e3 | MemberGen: `public Nullable<Color> FontColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontColor set` | 0x060003e4 | MemberGen: `public Nullable<Color> FontColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HighlightColor get` | 0x060003e5 | MemberGen: `public Nullable<Color> HighlightColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HighlightColor set` | 0x060003e6 | MemberGen: `public Nullable<Color> HighlightColor { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreStaticMathFormatMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

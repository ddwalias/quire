# Ptc.Controls.Core.FontManager

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreFontManagerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IsConnected get` | 0x06000261 | MemberGen: `public bool IsConnected { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConnected set` | 0x06000262 | MemberGen: `public bool IsConnected { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: .ctor(string, double, bool, bool, bool, Color)` | 0x06000f9e | MemberGen: `: base(fontFamilyName, fontPointSize, isItalic, isBold, isUnderline, fontColor) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: FontFamilyName get` | 0x06000f9f | MemberGen: `base.Item1` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: FontPointSize get` | 0x06000fa0 | MemberGen: `base.Item2` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: IsItalic get` | 0x06000fa1 | MemberGen: `base.Item3` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: IsBold get` | 0x06000fa2 | MemberGen: `base.Item4` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: IsUnderline get` | 0x06000fa3 | MemberGen: `base.Item5` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontKey: FontColor get` | 0x06000fa4 | MemberGen: `base.Item6` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreFontManagerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

# Ptc.TextStyleData

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcTextStyleDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ToString()` | 0x06000341 | MemberGen: `StyleName` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetStyle(DependencyObject, Style)` | 0x06000344 | MemberGen: `element.SetValue(StyleProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetStyleName(DependencyObject, string)` | 0x06000346 | MemberGen: `element.SetValue(StyleNameProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `OldStyle get` | 0x06000342 | MemberGen: `_oldStyle` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleName get` | 0x0600034a | MemberGen: `GetStyleName(this); set => SetStyleName(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleName set` | 0x0600034b | MemberGen: `GetStyleName(this); set => SetStyleName(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleKey get` | 0x0600034c | MemberGen: `public string StyleKey { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleKey set` | 0x0600034d | MemberGen: `public string StyleKey { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseOnKey get` | 0x0600034e | MemberGen: `public string BaseOnKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseOnKey set` | 0x0600034f | MemberGen: `public string BaseOnKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Style get` | 0x06000350 | MemberGen: `GetStyle(this); set => SetStyle(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Style set` | 0x06000351 | MemberGen: `GetStyle(this); set => SetStyle(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleChanged add` | 0x0600033e | MemberGen: `public static event Delegate1<TextStyleData> StyleChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleChanged remove` | 0x0600033f | MemberGen: `public static event Delegate1<TextStyleData> StyleChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcTextStyleDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

# Ptc.Controls.Core.MathFormat

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreMathFormatMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SetValue(DependencyProperty, object)` | 0x06000428 | MemberGen: `_owner.SetValue(dependencyProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClearValue(DependencyProperty)` | 0x06000429 | MemberGen: `_owner.ClearValue(dependencyProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsPropertyReadonly(DependencyProperty)` | 0x06000439 | MemberGen: `_readonlyProperties.Contains(dependencyProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFontFamilyNameSet get` | 0x0600042d | MemberGen: `IsPropertySet(MathFormatAttachedProperties.FontFamilyNameProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFontSizeSet get` | 0x0600042e | MemberGen: `IsPropertySet(MathFormatAttachedProperties.FontSizeProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsItalicSet get` | 0x0600042f | MemberGen: `IsPropertySet(MathFormatAttachedProperties.ItalicProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBoldSet get` | 0x06000430 | MemberGen: `IsPropertySet(MathFormatAttachedProperties.BoldProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsUnderlineSet get` | 0x06000431 | MemberGen: `IsPropertySet(MathFormatAttachedProperties.UnderlineProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFontColorSet get` | 0x06000432 | MemberGen: `IsPropertySet(MathFormatAttachedProperties.FontColorProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsHighlightColorSet get` | 0x06000433 | MemberGen: `IsPropertySet(MathFormatAttachedProperties.HighlightColorProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x0600040b | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x0600040c | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreMathFormatMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

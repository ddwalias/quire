# Ptc.Controls.Core.NumericUpDown

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreNumericUpDownMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `UpButton_Click(object, RoutedEventArgs)` | 0x060004f3 | MemberGen: `IncrementValue()` | Mechanical; Release IL equals Mathcad's | Verified |
| `DownButton_Click(object, RoutedEventArgs)` | 0x060004f4 | MemberGen: `DecrementValue()` | Mechanical; Release IL equals Mathcad's | Verified |
| `NumberChangedAction get` | 0x060004e6 | MemberGen: `public Action<int> NumberChangedAction { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NumberChangedAction set` | 0x060004e7 | MemberGen: `public Action<int> NumberChangedAction { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Minimum get` | 0x060004ec | MemberGen: `public int Minimum { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Minimum set` | 0x060004ed | MemberGen: `public int Minimum { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Maximum get` | 0x060004ee | MemberGen: `public int Maximum { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Maximum set` | 0x060004ef | MemberGen: `public int Maximum { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreNumericUpDownMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

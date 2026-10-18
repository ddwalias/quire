# Ptc.Undo.SetPropertyCommand

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcUndoSetPropertyCommandMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ClearValue()` | 0x060006f1 | MemberGen: `_dependencyObject.ClearValue(_dependencyProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Redo()` | 0x060006f2 | MemberGen: `Value = _newValue` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x060006ed | MemberGen: `_dependencyObject.GetValue(_dependencyProperty); set => _dependencyObject.SetValue(_dependencyProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x060006ee | MemberGen: `_dependencyObject.GetValue(_dependencyProperty); set => _dependencyObject.SetValue(_dependencyProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcUndoSetPropertyCommandMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

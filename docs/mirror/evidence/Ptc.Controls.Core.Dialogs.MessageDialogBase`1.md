# Ptc.Controls.Core.Dialogs.MessageDialogBase`1

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreDialogsMessageDialogBase1MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CancelButton get` | 0x06000ec4 | MemberGen: `private Button CancelButton { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CancelButton set` | 0x06000ec5 | MemberGen: `private Button CancelButton { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Result get` | 0x06000ec6 | MemberGen: `protected Nullable<T> Result { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Result set` | 0x06000ec7 | MemberGen: `protected Nullable<T> Result { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreDialogsMessageDialogBase1MechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

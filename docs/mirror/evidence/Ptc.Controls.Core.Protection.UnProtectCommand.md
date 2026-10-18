# Ptc.Controls.Core.Protection.UnProtectCommand

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(IProtectableModel, Action)` | 0x06000c2d | MemberGen: `: base(model, showWarningDialogRequest) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Undo()` | 0x06000c2f | MemberGen: `Protect()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Redo()` | 0x06000c30 | MemberGen: `Unprotect()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x06000c31 | MemberGen: `"Remove Control Protection"` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

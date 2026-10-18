# Ptc.Undo.UndoableCommand

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcUndoUndoableCommandMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Do()` | 0x060006fc | MemberGen: `_do.Invoke()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Undo()` | 0x060006fd | MemberGen: `_undo.Invoke()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Redo()` | 0x060006fe | MemberGen: `_redo.Invoke()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x060006ff | MemberGen: `_name` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcUndoUndoableCommandMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

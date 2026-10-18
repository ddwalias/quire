# Ptc.Undo.UndoableCommandWithSideEffects

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcUndoUndoableCommandWithSideEffectsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(string, DelegateFunction0, Delegate0)` | 0x06000700 | MemberGen: `: base(name, doMethod, undoMethod) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(string, DelegateFunction0, Delegate0, Delegate0)` | 0x06000701 | MemberGen: `: base(name, doMethod, undoMethod, redoMethod) { }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcUndoUndoableCommandWithSideEffectsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

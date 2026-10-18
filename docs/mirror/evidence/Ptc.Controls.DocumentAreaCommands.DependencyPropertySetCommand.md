# Ptc.Controls.DocumentAreaCommands.DependencyPropertySetCommand

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Undo()` | 0x060002ce | MemberGen: `_target.SetValue(_property, _old_value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Redo()` | 0x060002cf | MemberGen: `Do()` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsDocumentAreaCommandsDependencyPropertySetCommandMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

# Mpl.Run.InstructionSeq

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunInstructionSeqMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Instruction[])` | 0x06000350 | MemberGen: `m_ops = ops;` | Mechanical; Release IL equals Mathcad's | Verified |
| `EmitCode(ILGenerator)` | 0x06000351 | Indexed loop dispatches each `m_ops[i].EmitCode(il)` | Directly compared using recording generators; exact Release IL | Verified |

## Test cases

**Observed** (`MplRunInstructionSeqMechanicalDifferentialTests`): the constructor on generated inputs (Mechanical: zero,
null and distinct values per parameter and field), comparing the result and target fields.

**Observed** (`InstructionEmitCodeDifferentialTests`): compares a five-instruction sequence containing each operand
form using test recording subclasses of Quire and Mathcad `ILGenerator`, including emitted order and argument state.

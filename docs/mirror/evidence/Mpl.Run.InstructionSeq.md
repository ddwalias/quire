# Mpl.Run.InstructionSeq

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; `EmitCode` reconstructed from `Mpl.Run.InstructionSeq` IL
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunInstructionSeqMechanicalDifferentialTests`, `Quire.Differential.Tests.MplRunInstructionEmitDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Instruction[])` | 0x06000350 | MemberGen: `m_ops = ops;` | Mechanical; Release IL equals Mathcad's | Verified |
| `EmitCode(ILGenerator)` | 0x06000351 | Loops through `m_ops` in index order and calls each instruction's `EmitCode(il)`. | High; directly compared on five recorded instructions using `nop`, `ldc.i4`, `box`, `ldfld`, and `newobj`; other sequences and null arrays are not established. | Verified |

## Test cases

**Observed** (`MplRunInstructionSeqMechanicalDifferentialTests`): the constructor on generated inputs (Mechanical: zero, null and distinct
values per parameter and field), comparing the result, the arguments and target fields with Mathcad's.

**Observed** (`MplRunInstructionEmitDifferentialTests`): `EmitCode` is invoked directly on a five-element sequence on both artifacts;
captured opcodes and operands are compared in order. Other sequences are not established.

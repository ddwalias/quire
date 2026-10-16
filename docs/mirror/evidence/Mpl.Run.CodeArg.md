# Mpl.Run.CodeArg

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunCodeArgMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `BegArg(ILGenerator, int)` | 0x0600034e | MemberGen: `m_beg.EmitCode(il)` | Mechanical; Release IL equals Mathcad's | Verified |
| `EndArg(ILGenerator, int)` | 0x0600034f | MemberGen: `m_end.EmitCode(il)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(Code, Code)` | 0x0600034c | Calls `Code()` then stores `beg` and `end` | Directly compared with distinct references | Verified |
| `.ctor(Code)` | 0x0600034d | Calls `Code()`, stores `beg`, then stores `Code.NullCode` | Directly compared after the test fixture seeds Quire's stub `NullCode`; no claim about the initializer | Verified |

## Test cases

**Observed** (`MplRunCodeArgMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`CodeArgDifferentialTests`): both constructors with distinct references, plus the one-argument overload with nonnull and null `beg`. The test fixture
seeds Quire's `Code.NullCode` with a `Code` object so the constructor's field read can be directly compared; the `Code` static initializer remains outside this batch.

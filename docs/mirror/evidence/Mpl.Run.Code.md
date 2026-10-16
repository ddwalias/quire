# Mpl.Run.Code

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunCodeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `EmitCode(ILGenerator)` | 0x06000344 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BegArg(ILGenerator, int)` | 0x06000345 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EndArg(ILGenerator, int)` | 0x06000346 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000347 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`MplRunCodeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

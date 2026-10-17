# Mpl.Run.Code

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen bodies and `tools/reference/reference.py Mpl.Run.Code --il`; initializer token `0x06000343`
- **Quire:** `src/Quire.Mpl/Run/Code.cs`; tests: `Quire.Differential.Tests.MplRunCodeMechanicalDifferentialTests`, `Quire.Differential.Tests.CodeNullCodeDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor()` | 0x06000343 | Creates one default `Code` instance and stores it in static `NullCode`. | High; exact IL is `newobj Code::.ctor`, `stsfld NullCode`, `ret` | Verified |
| `NullCode` | — | Static reference set by `.cctor()` to a default `Code` instance. | High; direct comparison observes the initialized default state; fresh allocation is established by the exact `newobj; stsfld; ret` IL | Verified |
| `EmitCode(ILGenerator)` | 0x06000344 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BegArg(ILGenerator, int)` | 0x06000345 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EndArg(ILGenerator, int)` | 0x06000346 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000347 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`MplRunCodeMechanicalDifferentialTests`): each method on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`CodeNullCodeDifferentialTests`): directly invokes the type initializer on Quire and Mathcad, comparing the initialized static value. Both values are the default `Code` instance; the initializer's Release IL is exact.

# Mpl.Run.CodeFunction

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** `EmitCode` was rebuilt by MemberGen; the constructor and argument walks were reconstructed from Mathcad IL and their direct comparisons. Release IL is checked against Mathcad's.
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.CodeFunctionDifferentialTests` and
  `Quire.Differential.Tests.MplRunCodeFunctionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `m_args` | — | `Code[]` argument array, assigned by the constructor and indexed by both walks | Observed; tests check the original array reference | Verified |
| `.ctor(Code, Code[])` | 0x0600034b | `m_call = call; m_args = args` | Observed; fields preserved by reference | Verified |
| `BegArg(ILGenerator, int)` | 0x06000348 | `m_args[i].BegArg(il, i)` | Observed; forwards the selected argument and index | Verified |
| `EmitCode(ILGenerator)` | 0x06000349 | MemberGen: `m_call.EmitCode(il)` | Mechanical; Release IL equals Mathcad's | Verified |
| `EndArg(ILGenerator, int)` | 0x0600034a | `m_args[i].EndArg(il, i)` | Observed; forwards the selected argument and index | Verified |

## Test cases

**Observed** (`CodeFunctionDifferentialTests`): constructor checks that both fields retain their input references for empty and
populated arrays; `BegArg` and `EndArg` at indices 0 and 1, using distinct argument slots and a nested function to distinguish
selection and forwarded indices by exception outcomes.
**Mechanical** (`MplRunCodeFunctionMechanicalDifferentialTests`): `EmitCode` on generated inputs (zero, null and distinct
values per parameter and field), comparing the result, arguments and target fields with Mathcad's.

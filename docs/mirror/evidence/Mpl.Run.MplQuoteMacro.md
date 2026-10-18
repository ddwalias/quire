# Mpl.Run.MplQuoteMacro

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

MPL quote macro that expands calls `(quote x)` to quoted expressions `'x` (`Qt(x)`).

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `singleton` | 0x0400005f | — | Public static singleton instance. | High | Verified |
| `.ctor()` | 0x06000133 | `6256ad1a` | Private ctor calling `base(new Sym("quote", null))`. | High | Verified |
| `.cctor()` | 0x06000134 | `3de95c1d` | Initializes `singleton = new MplQuoteMacro()`. | High | Verified |
| `ExpandCall(MplEnv, Sexpr)` | 0x06000135 | `bac33f95` | If `f` is not `SCons` with length 2, calls `IllegalMacroCall(sCons)`; otherwise returns `new Qt(sCons.second)`. | High | Verified |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons of `.ctor()`, `.cctor()`, `singleton` identity and `ExpandCall` across valid length-2 lists (`(quote x)` returning `Qt(x)`), invalid length-3 lists (`(quote x y)` throwing `AppException("illegal macro call")`), invalid length-1 lists (`(quote)` throwing `AppException("illegal macro call")`), non-SCons tokens throwing `NullReferenceException`, and null input throwing `NullReferenceException`.
- **Inferred** (`EnvTests`): Unit tests covering `singleton` initialization, `ExpandCall` return type `Qt`, argument preservation, exception messages, and handle propagation.

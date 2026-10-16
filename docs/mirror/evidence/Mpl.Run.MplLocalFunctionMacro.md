# Mpl.Run.MplLocalFunctionMacro

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Differential.Tests.EnvDifferentialTests`

Local function macro data is retained, but expansion is rejected in the current compilation unit.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_env` | 0x04000069 | — | Stores the environment passed to the constructor. | High | Verified |
| `m_handle` | 0x0400006a | — | Stores the definition handle. | High | Verified |
| `m_lambda` | 0x0400006b | — | Stores the lambda expression. | High | Verified |
| `.ctor(MplEnv, Sym, uint, Sexpr)` | 0x06000146 | `dc32699b` | Calls the base constructor with `s`, then stores `env`, `handle`, and `lambda` in that order. | High | Verified |
| `ExpandCall(MplEnv, Sexpr)` | 0x06000147 | `0e7ccad9` | Calls `CanNotExpand(form.handle)`; explicit null return follows the throwing helper. Null `form` fails while reading `handle`. | High | Tested |
| `ExpandRef(MplEnv, Tok)` | 0x06000148 | `0e7ccad9` | Calls `CanNotExpand(s.handle)`; explicit null return follows the throwing helper. Null `s` fails while reading `handle`. | High | Tested |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct constructor comparison and calls to `ExpandCall` and `ExpandRef` with non-null forms, comparing the `AppException` message and handle.

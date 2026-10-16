# Mpl.Run.MplForeignMacro

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Differential.Tests.EnvDifferentialTests`

Foreign macro wrapper that lazily resolves a macro from its captured environment and delegates expansion.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_env` | 0x04000061 | — | Environment used for lazy signature resolution. | High | Verified |
| `m_sig` | 0x04000062 | — | Foreign macro signature passed to `LoadForeignMacro`. | High | Verified |
| `m_macro` | 0x04000063 | — | Cached resolved macro; constructor explicitly sets it to null. | High | Verified |
| `.ctor(MplEnv, Sym, string)` | 0x0600013b | `1bea31de` | Calls the base constructor with `s`, then stores `env`, `sig`, and null cache. | High | Verified |
| `Macro` | 0x0600013c | `3f2098c0` | Returns a cached macro when non-null; otherwise loads from `m_env`, stores the result, and returns it. A throwing load leaves the cache null. | High | Tested |
| `ExpandCall(MplEnv, Sexpr)` | 0x0600013d | `0886a559` | Calls `Macro.ExpandCall(e, form)`. | High | Verified |
| `ExpandRef(MplEnv, Tok)` | 0x0600013e | `de6a60d6` | Calls `Macro.ExpandRef(e, s)`. | High | Verified |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct constructor comparison; lazy lookup and expansion failure through a root `LocalEnv`; getter cache path and delegation to a cached `MplSymbolMacro` for both call and reference expansion.

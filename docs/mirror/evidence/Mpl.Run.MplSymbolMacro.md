# Mpl.Run.MplSymbolMacro

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

MPL symbol macro that expands references to a fixed replacement expression, and expands calls by replacing the head with the replacement and keeping the call tail.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_replacement` | 0x040000af | — | Private replacement `Sexpr`. | High | Verified |
| `.ctor(Sym, Sexpr)` | 0x0600012e | `5b28f587` | Calls `base(s)` and sets `m_replacement = rep`. | High | Verified |
| `ExpandRef(MplEnv, Tok)` | 0x0600012f | `3ecf0fe3` | Returns `m_replacement`. | High | Verified |
| `ExpandCall(MplEnv, Sexpr)` | 0x06000130 | `ee1bca87` | Returns `new SCons(m_replacement, ((SCons)f).tail)`. | High | Verified |
| `make(Atom, int)` | 0x06000131 | `4ea091e4` | Returns `new MplSymbolMacro(s as Sym, int_constant.NewTok(n, 0u))`. | High | Verified |
| `ToString()` | 0x06000132 | `39e627fd` | Returns `"MplSymbolMacro: " + m_name + " " + m_replacement`. | High | Stub |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons of `.ctor`, `make`, `ExpandRef`, and `ExpandCall` across atoms, symbols, lists, non-lists, and null inputs.
- **Inferred** (`EnvTests`): Behavioural tests covering replacement substitution and call tail preservation.

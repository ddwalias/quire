# Mpl.Run.MplBackquoteMacro

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Env.cs`; tests: `Quire.Mpl.Tests.Run.EnvTests`, `Quire.Differential.Tests.EnvDifferentialTests`

MPL backquote macro that expands quasiquoted expressions via the `MplMacro.backquote` foreign macro.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `backquote` | 0x04000064 | — | Static readonly `Sym("backquote", "MplMacro")` token. | High | Verified |
| `singleton` | 0x04000065 | — | Public static singleton instance. | High | Verified |
| `.ctor()` | 0x06000140 | `c8818874` | Private ctor calling `base(backquote)`. | High | Verified |
| `.cctor()` | 0x0600013f | — | Initializes `backquote = new Sym("backquote", "MplMacro")` and `singleton = new MplBackquoteMacro()`. | High | Verified |
| `ExpandCall(MplEnv, Sexpr)` | 0x06000141 | `5540cb12` | If `m_macro == null`, loads foreign macro via `e.LoadForeignMacro("MplMacro.backquote")`; delegates call to `m_macro.ExpandCall(e, form)`. In mplrun, base `MplEnv.LoadForeignMacro` unconditionally throws `AppException("Can not resolve signature")`. | High | Tested |

## Test cases

- **Observed** (`EnvDifferentialTests`): Direct comparisons of `.ctor()`, `.cctor()`, `singleton` and `backquote` identities, `ExpandCall` with null environment (throwing `NullReferenceException` on `m_macro == null`), `ExpandCall` with `LocalEnv` (throwing `NullReferenceException` from `m_parent`), and `ExpandCall` with cached `m_macro` across valid and invalid macro forms.
- **Inferred** (`EnvTests`): Unit tests covering `singleton` initialization, `backquote` symbol name and qualifier, `ExpandCall` exception behavior with null/empty environments, and delegating call expansion to cached foreign macro.

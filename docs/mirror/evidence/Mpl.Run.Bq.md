# Mpl.Run.Bq

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

A backquoted expression, printed `` `x ``.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x04000016 | — | Public mutable. | High | Verified |
| `.ctor(object), .ctor(Sexpr)` | 0x06000058, 0x06000059 | `a3648f2c` | Tag 4; the object overload converts with `as Sexpr`. | High | Verified |
| `ToString()` | 0x0600005a | `920cadee` | `` "`" + Field1 ``. | High | Verified |
| `walk_expression(MplEnv)` | 0x0600005b | `d086f52e` | `MplBackquoteMacro.singleton.ExpandCall(env, this)`, walked as an expression. The Quire body retains Mathcad's exact 19-byte IL. | High for dispatch; successful foreign expansion requires the host macro integration | Verified |

## Test cases

- **Observed** (`BqDifferentialTests`): Direct `MethodComparison.Compare` cases cover the object constructor with an expression and unrelated object, the `Sexpr` constructor with an expression and null, `ToString()` on an expression, and `walk_expression(null)` with the macro singleton caches empty. The walk reaches the macro entry point and both original and Quire throw `NullReferenceException` when the environment is dereferenced.
- **Limit:** The installed `mplrun.dll` base `MplEnv.LoadForeignMacro` throws `AppException("Can not resolve signature")`, so successful backquote expansion is unavailable without the external host integration. The walk comparison covers the observable hostless failure path, not successful expansion.

## Open

- Direct successful expansion through the external `MplMacro.backquote` host integration remains unobserved.

# Mpl.Run.Nul

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

The empty list. Its debugging text is `Nul`.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `singleton` | 0x0400000e | — | Public static mutable field, set once by the static constructor. `SList.ToString` tests against this instance, so other `Nul` objects don't count as the end. | High | Verified |
| `token` | 0x0400000f | — | `Sym.NewTok("Nul", "MplMacro", 0)`: the qualifier is dropped (see `Sym`). | High | Verified |
| `.ctor()` | 0x0600002f | `54b9255c` | Tag 0. | High | Verified |
| `scons` | 0x06000030 | `ce2965e0` | null. | High | Verified |
| `quote` | 0x06000031 | `b83d494f` | `token` itself. | High | Verified |
| `ToString()` | 0x06000032 | `78f0eaf1` | `"Nul"`. | High | Verified |
| `FormatTail()` | 0x06000033 | `5dd7a3e4` | `""`, which closes a proper list. | High | Verified |

## Test cases

As in Mpl.Run.Sexpr.md; `Nul.singleton.quote` is checked to be `Nul.token` on both sides.

# Mpl.Run.SAppend

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

An append form, printed `(SAPPEND a b …)`.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x0400001c, 0x0400001d | — | Public mutable. | High | Verified |
| `.ctor(object, object)` | 0x0600006f | `08a680fa` | `as Sexpr` on both, then the other constructor. | High | Verified |
| `.ctor(Sexpr, Sexpr)` | 0x06000070 | `e2065e56` | Tag 7. | High | Verified |
| `FormatAppend()` | 0x06000071 | `5c84f12a` | `" " + Field1 + Field2.FormatAppend()`; other S-expressions append `" " + ToString()`, so a trailing `Nul` prints as ` Nul`. | High | Verified |
| `ToString()` | 0x06000072 | `ef269e8c` | `"(SAPPEND " + Field1 + Field2.FormatAppend() + ")"`. | High | Verified |

## Test cases

As in Mpl.Run.Sexpr.md.

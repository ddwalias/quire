# Mpl.Run.Cm

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

An unquote inside a backquote, printed `,x`.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x04000017 | — | Public mutable. | High | Verified |
| `.ctor(object), .ctor(Sexpr)` | 0x0600005c, 0x0600005d | `5a005615` | Tag 5; the object overload converts with `as Sexpr`. | High | Verified |
| `ToString()` | 0x0600005e | `d663aaca` | `"," + Field1`. | High | Verified |

## Test cases

As in Mpl.Run.Sexpr.md.

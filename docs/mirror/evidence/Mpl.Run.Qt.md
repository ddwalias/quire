# Mpl.Run.Qt

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

A quoted expression, printed `'x`.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x04000014 | — | Public mutable. | High | Verified |
| `Qt_constructor` | 0x04000015 | — | Private static `new Sym("Qt", "MplMacro")` (qualifier dropped). | High | Verified |
| `.cctor` | 0x06000051 | — | Initialises Qt_constructor. | High | Verified |
| `.ctor(object), .ctor(Sexpr)` | 0x06000052, 0x06000053 | `461fd68e` | Tag 3; the object overload converts with `as Sexpr`. | High | Verified |
| `quotation` | 0x06000054 | `c70b14a1` | `Field1.quote`. | High | Verified |
| `quote` | 0x06000055 | `633f25d6` | `list2(new Tok(Qt_constructor, m_tag), Field1.quote)`: the token's handle is the tag, 3. | High | Verified |
| `ToString()` | 0x06000056 | `7b4deb10` | `"'" + Field1`. | High | Verified |
| `walk_expression(MplEnv)` | 0x06000057 | `339a830e` | this (same IL as `SCons.scons`). | High | Verified |

## Test cases

As in Mpl.Run.Sexpr.md.

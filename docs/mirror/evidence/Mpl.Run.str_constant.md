# Mpl.Run.str_constant

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.AtomTests`, `Quire.Differential.Tests.SexprDifferentialTests`

A string atom.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x04000052 | — | Public mutable; null is kept (pre-§3c Quire stored ""). | High | Verified |
| `token` | 0x04000053 | — | Private static `Sym.NewTok("StrCon", "MplMacro", 0)`. | High | Verified |
| `.ctor(string)` | 0x06000100 | `9aac9231` | Tag 4. | High | Verified |
| `NewTok(string, uint)` | 0x06000101 | `749e31f1` | A token of the new constant. | High | Verified |
| `StrCon` | 0x06000102 | `0aedc00d` | `Field1` (same IL as `ToString`). | High | Verified |
| `quote_atom` | 0x06000103 | `f057118f` | `(StrCon s)`. | High | Verified |
| `.cctor` | 0x060000ff | — | Initialises token. | High | Verified |
| `ToString()` | 0x06000104 | `0aedc00d` | The raw text: no quotes, no escapes (pre-§3c Quire quoted and escaped). | High | Verified |

## Test cases

As in Mpl.Run.int_constant.md.

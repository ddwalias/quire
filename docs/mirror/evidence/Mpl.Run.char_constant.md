# Mpl.Run.char_constant

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.AtomTests`, `Quire.Differential.Tests.SexprDifferentialTests`

A char atom.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x04000054 | — | Public mutable. | High | Verified |
| `.ctor(char)` | 0x06000105 | `7f9c8ddf` | Tag 3. | High | Verified |
| `NewTok(char, uint)` | 0x06000106 | `cd0e8a7b` | A token of the new constant. | High | Verified |
| `CharCon` | 0x06000107 | `c7278d81` | `Field1`. | High | Verified |
| `ToString()` | 0x06000108 | `bb4e5edd` | The char as a one-character string. No `quote_atom` override, so quoting throws. | High | Verified |

## Test cases

As in Mpl.Run.int_constant.md.

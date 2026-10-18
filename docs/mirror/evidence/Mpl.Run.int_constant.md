# Mpl.Run.int_constant

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.AtomTests`, `Quire.Differential.Tests.SexprDifferentialTests`

An int atom.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x0400004d | — | Public mutable. | High | Verified |
| `token` | 0x0400004e | — | Private static `Sym.NewTok("IntCon", "MplMacro", 0)` (qualifier dropped). | High | Verified |
| `zero` | 0x0400004f | — | Static `NewTok(0, 0)`. | High | Verified |
| `.ctor(string)` | 0x060000f0 | `f1191103` | Tag 1; `int.TryParse` with the current culture, 0 when it fails (null, overflow, "1e3", "0x10" …). | High; only the test culture compared | Verified |
| `.ctor(int)` | 0x060000f1 | `ddf358fb` | Tag 1. | High | Verified |
| `NewTok(int, uint), NewTok(string, uint)` | 0x060000f2, 0x060000f3 | `7cd82ae1` | A token of the new constant. | High | Verified |
| `IntCon` | 0x060000f4 | `d322a4b8` | `Field1`. | High | Verified |
| `quote_atom` | 0x060000f5 | `fffea6cc` | `(IntCon n)`. | High | Verified |
| `.cctor` | 0x060000ef | — | Initialises token and zero. | High | Verified |
| `ToString()` | 0x060000f6 | `8321ec47` | `Field1.ToString()`, current culture. | High | Verified |

## Test cases

Observed in `SexprDifferentialTests.Constructors_and_factories` and `Atom_members`; inferred in `AtomTests`.

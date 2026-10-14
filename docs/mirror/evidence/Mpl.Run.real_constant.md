# Mpl.Run.real_constant

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Atom.cs`; tests: `Quire.Mpl.Tests.Run.AtomTests`, `Quire.Differential.Tests.SexprDifferentialTests`

A double atom.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1` | 0x04000050 | — | Public mutable. | High | Verified |
| `token` | 0x04000051 | — | Private static `Sym.NewTok("RealCon", "MplMacro", 0)`. | High | Verified |
| `.ctor(string)` | 0x060000f8 | `c20334d2` | **Tag 1 (`IntTag`)**, not 2: Mathcad's quirk, kept. `double.TryParse` with the current culture; 0 when it fails. | High; only the test culture compared | Verified |
| `.ctor(double)` | 0x060000f9 | `caf18503` | Tag 2. | High | Verified |
| `NewTok(double, uint), NewTok(string, uint)` | 0x060000fa, 0x060000fb | `7fc3318a` | A token of the new constant. | High | Verified |
| `RealCon` | 0x060000fc | `915d02c4` | `Field1`. | High | Verified |
| `quote_atom` | 0x060000fd | `ee05ae03` | `(RealCon x)`. | High | Verified |
| `ToString()` | 0x060000fe | `c0fd06e5` | `Field1.ToString()`: current culture, shortest round-trip (pre-§3c Quire used invariant "R"). | High | Verified |

## Test cases

Observed in `SexprDifferentialTests` (NaN, ±∞, −0, 1e400, "∞", subnormals); inferred in `AtomTests`.

# Mpl.Run.True

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Cons.cs`; tests: `Quire.Mpl.Tests.Run.MplRunHelpersTests`, `Quire.Differential.Tests.MplRunHelpersDifferentialTests`

The runtime truth value.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `singleton` | 0x040000b9 | — | Static mutable, set by the static constructor. | High | Verified |
| `.cctor()` | 0x060003e8 | — | Sets singleton to a new True instance. | High | Verified |
| `.ctor()` | 0x060003ea | `41ad57d1` | No state. | High | Verified |
| `ToString()` | 0x060003e9 | `c5de8e74` | `"True"`. | High | Verified |

## Test cases

Observed in `MplRunHelpersDifferentialTests.True_members`.

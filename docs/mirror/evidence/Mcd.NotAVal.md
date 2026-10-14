# Mcd.NotAVal

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/NumVal.cs`; tests: `Quire.Differential.Tests.NumValDifferentialTests`

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.cctor`0()->[System.Runtime]System.Void|True` | 0x060006cf | IL | .cctor`0()->[System.Runtime]System.Void|True contract | High | Verified |
| `.ctor`0()->[System.Runtime]System.Void|False` | 0x060006d1 | IL | .ctor`0()->[System.Runtime]System.Void|False contract | High | Verified |
| `ToString`0()->[System.Runtime]System.String|False` | 0x060006d0 | IL | ToString`0()->[System.Runtime]System.String|False contract | High | Verified |

## Test cases

**Observed** (`NumValDifferentialTests`, against the installed `mplrun.dll`):
- Statics and constructors
- Setter and conversion operations
- Exact and fuzzy comparisons (nearly_equal, NearlyEqual)
- Strict comparisons
- Formatting in invariant and custom cultures

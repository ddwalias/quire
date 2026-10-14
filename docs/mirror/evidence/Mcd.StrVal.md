# Mcd.StrVal

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **IL:** il/mplrun/Mcd.StrVal.il
- **Quire:** `src/Quire.Mpl/Mcd/NumVal.cs`; tests: `Quire.Differential.Tests.NumValDifferentialTests`

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.ctor`0([System.Runtime]System.String)->[System.Runtime]System.Void|False` | 0x060006d5 | IL | .ctor`0([System.Runtime]System.String)->[System.Runtime]System.Void|False contract | High | Verified |
| `get_IsString`0()->[System.Runtime]System.String|False` | 0x060006d2 | IL | get_IsString`0()->[System.Runtime]System.String|False contract | High | Verified |
| `asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.String|False` | 0x060006d3 | IL | asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.String|False contract | High | Verified |
| `get_m_string`0()->[System.Runtime]System.String|False` | 0x060006d4 | IL | get_m_string`0()->[System.Runtime]System.String|False contract | High | Verified |
| `ToString`0()->[System.Runtime]System.String|False` | 0x060006d6 | IL | ToString`0()->[System.Runtime]System.String|False contract | High | Verified |
| `Equals`0([System.Runtime]System.Object)->[System.Runtime]System.Boolean|False` | 0x060006d7 | IL | Equals`0([System.Runtime]System.Object)->[System.Runtime]System.Boolean|False contract | High | Verified |
| `GetHashCode`0()->[System.Runtime]System.Int32|False` | 0x060006d8 | IL | GetHashCode`0()->[System.Runtime]System.Int32|False contract | High | Verified |

## Test cases

**Observed** (`NumValDifferentialTests`, against the installed `mplrun.dll`):
- Constructor with strings, null, empty
- Properties `IsString`, `m_string`
- Methods `ToString`, `asString`, `Equals`, `GetHashCode`

# Ptc.Calculation.Core.Trace2DResults.TraceDataPoints

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** Mathcad behavior and IL extracted with `python tools\reference\reference.py Ptc.Calculation.Core.Trace2DResults.TraceDataPoints --assembly Ptc.Calculation.Core --il`
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(IVectorListResultNode, IVectorListResultNode)` | 0x060002b0 | Throw parameterless `ArgumentNullException` if either input is null; store range then result vectors | Direct Mathcad comparisons cover both stored proxy references, null range, and null result; Release IL exact | Verified |
| `.ctor()` | 0x060002b1 | Call `object` constructor | Mechanical direct comparison; Release IL exact | Verified |
| `RangePoints get` | 0x060002b2 | Return `RangePoints` backing field | Mechanical direct comparison; Release IL exact | Verified |
| `RangePoints set` | 0x060002b3 | Store `RangePoints` backing field | Mechanical direct comparison; Release IL exact | Verified |
| `ResultPoints get` | 0x060002b4 | Return `ResultPoints` backing field | Mechanical direct comparison; Release IL exact | Verified |
| `ResultPoints set` | 0x060002b5 | Store `ResultPoints` backing field | Mechanical direct comparison; Release IL exact | Verified |
| `IsEmpty get` | 0x060002b6 | Return true only when both `ResultPoints` and `RangePoints` are null | Direct Mathcad comparisons cover both-null, range-only, result-only, and both-present states; Release IL exact | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests`): the two-argument constructor is compared with non-null proxy inputs and each null-input branch; the `IsEmpty` getter is compared for all four null/non-null reference combinations. Existing no-argument constructor and property comparisons remain in the same test class.

Release MethodAudit inspected all 7 declarations in this type; each reports structure_matches=true and exact_match=true. Nine ancillary assembly inspection errors are outside this type. The current Release Mathcad test class passed 7/7; the normal gate is pending.

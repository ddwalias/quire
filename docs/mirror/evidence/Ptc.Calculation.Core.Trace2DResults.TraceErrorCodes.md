# Ptc.Calculation.Core.Trace2DResults.TraceErrorCodes

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** Mathcad behavior and IL extracted with `python tools\reference\reference.py Ptc.Calculation.Core.Trace2DResults.TraceErrorCodes --assembly Ptc.Calculation.Core --il`
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CreateTooManyPointsInResultMessage(int)` | 0x0600027f | Format `ErrorTooManyPointsInResult` and the maximum point count with `"{0}\t\t\t{1:N0}"` using `CultureInfo.CurrentUICulture` | Direct Mathcad comparisons cover three UI cultures and six values including negative, zero, grouping boundary and million; Release IL exact | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests`): compares the output directly against Mathcad for `en-US`, `de-DE`, and `fr-FR`, with limits -1, 0, 1, 999, 1000, and 1000000. The original UI culture is restored after the test.

# Ptc.Calculation.Core.Trace2DResults.TraceTypeDefinitions

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen for `GetMinRangeColumns`; `tools/reference/reference.py Ptc.Calculation.Core.Trace2DResults.TraceTypeDefinitions --assembly Ptc.Calculation.Core --il` for `.cctor()` (IL output remains in ignored `.mirror-cache`)
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsMechanicalDifferentialTests`, `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsCctorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetMinRangeColumns(TraceType)` | 0x060002c2 | MemberGen: `1` | Mechanical; Release IL equals Mathcad's | Verified |
| `.cctor()` | 0x060002bf | Creates the eight-entry `TraceType` to `TraceTypeRange` map | Directly compares every enum key and range boundary; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsMechanicalDifferentialTests`): compares `GetMinRangeColumns` on generated inputs.

**Observed** (`PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsCctorMechanicalDifferentialTests`): invokes `.cctor()` on both artifacts and compares all dictionary keys with their `Start`/`End` values.
